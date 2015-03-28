using System.Diagnostics;
using System.IO;
using System.Linq;

namespace CoyneSolutions.SpeeDiff
{
    public static class TortoiseSvnHelper
    {
        private const string TortoiseProcPath = @"C:\Program Files\TortoiseSVN\bin\TortoiseProc.exe";

        public static bool Exists
        {
            get { return File.Exists(TortoiseProcPath); }
        }

        public static string GetUrlFromRepoBrowser()
        {
            var tempfile = Path.GetTempFileName();
            var process = Process.Start(TortoiseProcPath, string.Format("/command:repobrowser /outfile:\"{0}\"", tempfile));
            process.WaitForExit();
            if (File.Exists(tempfile))
            {
                var lines = File.ReadAllLines(tempfile);
                if (lines.Length > 0) // Zero lines if they cancelled
                {
                    return lines[0].Trim(); // Second line is the revision
                }
            }
            return null;
        }
    }
}
