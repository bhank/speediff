// Test with bad path
// Test with UTF8 (need to pass it to streamreader ctor?)
// Separate line numbers into different (scroll-synced) textboxes, so you can copy and paste
// DONE // Maintain scroll position when switching revisions
// Save sizes of columns in listview, and window size
// Maybe make listview sortable...
// Enable flipping through changes with Ctrl-Up and Ctrl-Down
// Use a faster text box!
// Maybe use a cool text box with syntax highlighting
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CoyneSolutions.SpeeDiff;

namespace speediff
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Console.WriteLine("Hey!");
            //Test1();
            //Console.ReadKey();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmDiff());
        }

        private static void Test1()
        {
            var revisionProvider = RevisionProvider.GetRevisionProvider(Environment.GetCommandLineArgs()[1]);
            revisionProvider.RevisionLoaded += (sender, args) => Console.WriteLine(args.Revision);
            Task.WaitAll(revisionProvider.Initialize());

            var currentRevisionIndex = revisionProvider.Revisions.Count - 1;
            while(true)
            {
                CompareWithPreviousRevision(revisionProvider.Revisions, currentRevisionIndex);
                while (true)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.LeftArrow)
                    {
                        if (currentRevisionIndex > 0)
                        {
                            currentRevisionIndex--;
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Already on first revision!");
                        }
                    }
                    if (key.Key == ConsoleKey.RightArrow)
                    {
                        if (currentRevisionIndex < revisionProvider.Revisions.Count - 1)
                        {
                            currentRevisionIndex++;
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Already on last revision!");
                        }
                    }
                    if (key.Key == ConsoleKey.Q) Environment.Exit(0);
                    Console.Write("?");
                }

            }
        }

        private static void CompareWithPreviousRevision(IList<Revision> revisions, int currentRevisionIndex)
        {
            if (currentRevisionIndex == 0 || currentRevisionIndex >= revisions.Count) return;

            var leftFile = WriteTempFile(revisions[currentRevisionIndex - 1].GetContent());
            var rightFile = WriteTempFile(revisions[currentRevisionIndex].GetContent());
            RunDiff(leftFile, rightFile);
            File.Delete(leftFile);
            File.Delete(rightFile);
        }

        private static void RunDiff(string leftFile, string rightFile)
        {
            var process = new Process
            {
                StartInfo =
                    {
                        FileName = "tortoisemerge.exe",
                        Arguments = string.Format(@"""{0}"" ""{1}""", leftFile, rightFile),
                        CreateNoWindow = true
                    }
            };
            process.Start();
            process.WaitForExit();
        }

        private static string WriteTempFile(string content)
        {
            var fileName = Path.GetTempFileName();
            File.WriteAllText(fileName, content);
            return fileName;
        }
    }
}
