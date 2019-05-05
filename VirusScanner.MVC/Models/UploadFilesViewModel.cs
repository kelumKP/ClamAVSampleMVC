using System;
using System.Collections.Generic;

namespace VirusScanner.MVC.Models
{
    public class UploadFilesViewModel
    {
        public IList<ScanResult> Results { get; set; }
    }

    public class ScanResult
    {
        public string FileName { get; set; }
        public string Result { get; set; }
        public string Message { get; set; }
        public string RawResult { get; set; }
    }
}
