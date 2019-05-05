using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.Exceptions;
using nClam;
using VirusScanner.MVC.Domain.Entities;
using VirusScanner.MVC.Models;
using VirusScanner.MVC.Persistence;

namespace VirusScanner.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly UploadsDbContext _context;
        private readonly MinioClient _minio;
        private readonly ClamClient _clam;
        private readonly ILogger _logger;

        private const string BUCKET_NAME = "virus-scanned-files";

        public HomeController(UploadsDbContext context, 
                                MinioClient minio, 
                                ClamClient clam, 
                                ILogger<HomeController> logger)
        {
            _context = context;
            _minio = minio;
            _clam = clam;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet("UploadFiles")]
        public IActionResult UploadFiles()
        {
            return View();
        }

        [HttpPost("UploadFiles")]
        public async Task<IActionResult> UploadFiles(List<IFormFile> files)
        {
            var log = new List<ScanResult>();

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    var extension = formFile.FileName.Contains('.')
                        ? formFile.FileName.Substring(formFile.FileName.LastIndexOf('.'), formFile.FileName.Length - formFile.FileName.LastIndexOf('.'))
                        : string.Empty;
                    var file = new File
                    {
                        Name = $"{Guid.NewGuid()}{extension}",
                        Alias = formFile.FileName,
                        Region = "us-east-1",
                        Bucket = BUCKET_NAME,
                        ContentType = formFile.ContentType,
                        Size = formFile.Length,
                        Uploaded = DateTime.UtcNow,
                    };
                    var ping = await _clam.PingAsync();

                    if (ping)
                    {
                        _logger.LogInformation("Successfully pinged the ClamAV server.");
                        var result = await _clam.SendAndScanFileAsync(formFile.OpenReadStream());

                        file.ScanResult = result.Result.ToString();
                        file.Infected = result.Result == ClamScanResults.VirusDetected;
                        file.Scanned = DateTime.UtcNow;
                        if (result.InfectedFiles != null)
                        {
                            foreach (var infectedFile in result.InfectedFiles)
                            {
                                file.Viruses.Add(new Virus
                                {
                                    Name = infectedFile.VirusName
                                });
                            }
                        }
                        var metaData = new Dictionary<string, string>
                        {
                            { "av-status", result.Result.ToString() },
                            { "av-timestamp", DateTime.UtcNow.ToString() },
                            { "alias", file.Alias }
                        };

                        try
                        {
                            var found = await _minio.BucketExistsAsync(BUCKET_NAME);
                            if (!found)
                            {
                                await _minio.MakeBucketAsync(BUCKET_NAME);
                            }
                            await _minio.PutObjectAsync(BUCKET_NAME,
                                                        file.Name,
                                                        formFile.OpenReadStream(),
                                                        formFile.Length,
                                                        formFile.ContentType,
                                                        metaData);
                            await _context.Files.AddAsync(file);
                            await _context.SaveChangesAsync();
                        }
                        catch (MinioException e)
                        {
                            _logger.LogError($"File Upload Error: {e.Message}");
                        }


                        var scanResult = new ScanResult()
                        {
                            FileName = formFile.FileName,
                            Result = result.Result.ToString(),
                            Message = result.InfectedFiles?.FirstOrDefault()?.VirusName,
                            RawResult = result.RawResult
                        };
                        log.Add(scanResult);
                    }
                    else
                    {
                        _logger.LogWarning("Wasn't able to connect to the ClamAV server.");
                    }
                }
            }

            var model = new UploadFilesViewModel
            {
                Results = log
            };

            return View("UploadResults", model);
        }
    }
}
