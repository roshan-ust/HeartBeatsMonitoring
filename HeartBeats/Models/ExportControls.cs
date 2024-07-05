using System;
using System.IO;
using System.Linq;

namespace HeartBeats.Models
{
    public class ExportControls
    {
        public string ExportFilePath { get; set; } = @"\Wolseley\HeartbeatReport.docx";

        public ExportControls()
        {
            var availableDrives = GetAvailableDrivesExcludingC();
            if(availableDrives.Length > 0)
            {
                ExportFilePath = availableDrives[0]+ ExportFilePath.Trim('\\');
            }
        }

        private string[] GetAvailableDrivesExcludingC()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            return Array.FindAll(allDrives, drive => drive.IsReady && drive.DriveType == DriveType.Fixed && drive.Name != "C:\\")
                        .Select(drive => drive.Name)
                        .ToArray();
        }
    }
}
