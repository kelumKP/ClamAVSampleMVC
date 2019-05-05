using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using nClam;
using VirusScanner.MVC.Models;

namespace VirusScanner.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ClamClient _clam;

        public HomeController(ClamClient clam)
        {
            _clam = clam;
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
                    var ping = await _clam.PingAsync();

                    if(ping)
                    {
                        var result = await _clam.SendAndScanFileAsync(formFile.OpenReadStream());

                        log.Add(new ScanResult()
                        {
                            FileName = formFile.FileName,
                            Result = result.Result.ToString(),
                            Message = result.InfectedFiles?.FirstOrDefault()?.VirusName,
                            RawResult = result.RawResult
                        });
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
