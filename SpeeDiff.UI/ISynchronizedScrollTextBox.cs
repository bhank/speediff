using System.Drawing;
using System.Windows.Forms;

namespace CoyneSolutions.SpeeDiff
{
    public interface ISynchronizedScrollTextBox
    {
        bool ShowScrollBars { get; set; }
        bool ReadOnly { get; set; }
        Font Font { get; set; }
        bool WordWrap { get; set; }
        Color BackColor { get; set; }
        Cursor Cursor { get; set; }
        void AppendText(string text, Color color = new Color());
        string Text { get; set; }
        int SelectionStart { get; set; }
        void StopRepaint();
        void StartRepaint();
        void SaveScrollPosition();
        void RestoreScrollPosition();
        void Clear();
    }
}
