using System.Diagnostics;
using System.IO;

namespace CoyneSolutions.SpeeDiff
{
    public static class TortoiseSvnHelper
    {
        public static bool Exists
        {
            get { return File.Exists(Config.TortoiseProcPath); }
        }

        public static string GetUrlFromRepoBrowser()
        {
            var tempfile = Path.GetTempFileName();
            var process = Process.Start(Config.TortoiseProcPath, string.Format("/command:repobrowser /outfile:\"{0}\"", tempfile));
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

        public static string DiffProgram
        {
            get { return Path.Combine(Path.GetDirectoryName(Config.TortoiseProcPath), "TortoiseMerge.exe"); }
        }

        public static string DiffParameters
        {
            get { return "/base:\"{left}\" /basename:\"{lefttitle}\" /mine:\"{right}\" /minename:\"{righttitle}\" /readonly"; }
        }
    }
}
