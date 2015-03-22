// Test with bad path
// Test with UTF8 (need to pass it to streamreader ctor?)
// DONE // Separate line numbers into different (scroll-synced) textboxes, so you can copy and paste
// DONE // Maintain scroll position when switching revisions
// Save sizes of columns in listview, and window size
// Maybe make listview sortable...
// DONE // Enable flipping through changes with Ctrl-Up and Ctrl-Down
// Use a faster text box!
// Maybe use a cool text box with syntax highlighting
// Add a loading spinner
// Add a keyboard shortcut to jump to the text box
// Enable viewing in an external diff viewer of your choice
using System;
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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmDiff());
        }
    }
}
