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
// DONE // Add a keyboard shortcut to jump to the file text box
// Enable viewing in an external diff viewer of your choice
// Make listview scroll when you Alt-Left or Alt-Right offscreen
// Maybe change the textbox to a dropdown with recent values
// Add real icons
using System;
using System.Windows.Forms;

namespace CoyneSolutions.SpeeDiff
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
