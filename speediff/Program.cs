// Test with bad path
// Test with UTF8 (need to pass it to streamreader ctor?)
// DONE // Separate line numbers into different (scroll-synced) textboxes, so you can copy and paste
// DONE // Maintain scroll position when switching revisions
// DONE // Save sizes of columns in listview, and window size
// DONE // Maybe make listview sortable...
// DONE // Enable flipping through changes with Ctrl-Up and Ctrl-Down
// Use a faster text box! Or try building RTF and assigning it all at once for speed
// Maybe use a cool text box with syntax highlighting
// Add a loading spinner
// DONE // Add a keyboard shortcut to jump to the file text box
// Enable viewing in an external diff viewer of your choice (maybe right-click a revision)
// Make listview scroll when you Alt-Left or Alt-Right offscreen
// DONE // Maybe change the textbox to a dropdown with recent values
// DONE // Add real icons
// Save splits?
// DONE // Prevent "ding" on Enter in filename box
// DONE // Maybe combine the two Properties save calls
// DONE // Add form icon
// DONE // Shrink line number columns
// Maybe ding when you try to go to the next/previous change when there is none, or the next/previous revision
// Add binary release to github
// Deal with authentication?
// Limit revisions loaded, for speed?
// Fix horizontal scroll sync to refresh properly -- and don't sync it to the line number textboxes
// Maybe add a current-line-compare box at the bottom?
// Right-click to view entire log (do I get that info already?), or to view it in TortoiseSVN
// Maybe jump to the first change, if you're on the first line of the file?
// DONE // Maybe show a count of the changes, like p4merge? Types too. (could go in a status bar)
// DONE // Set focus to left or right textbox after selecting a revision, so you can ctrl-home to the top and such
// DONE // Disable tab to the line number boxes; disable caret if possible (or disable the whole thing, if I can figure out how to keep the background color)
// DONE // Add Browse... option in the combobox
// DONE // Detect TortoiseSVN and offer RepoBrowser option in combobox
// DONE // Show filename in title bar
// Somehow highlight the whole line for diffs that might be off to the right; possibly color the line number box
// Add fallback for SVN servers that don't support FileVersions (codeplex proxy, apparently)?
// Test with bad server certificate
// Add blame-ish "go back to where this line changed" function?
// Show blame info somehow, if I have it already?

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
