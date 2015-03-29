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

        public static void ViewDiff(string leftTitle, string leftFile, string rightTitle, string rightFile)
        {
            var tortoiseMergePath = Path.Combine(Path.GetDirectoryName(Config.TortoiseProcPath), "TortoiseMerge.exe");
            var arguments = string.Format("/base:\"{0}\" /basename:\"{1}\" /mine:\"{2}\" /minename:\"{3}\" /readonly", leftFile, leftTitle, rightFile, rightTitle);
            Process.Start(tortoiseMergePath, arguments);
        }
    }
}
