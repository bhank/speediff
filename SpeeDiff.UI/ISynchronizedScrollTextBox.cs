using System.Drawing;

namespace CoyneSolutions.SpeeDiff
{
    public interface ISynchronizedScrollTextBox
    {
        bool ShowScrollBars { get; set; }
        void AppendText(string text, Color color = new Color());
        string Text { get; set; }
        void StopRepaint();
        void StartRepaint();
        void SaveScrollPosition();
        void RestoreScrollPosition();
        void Clear();
    }
}
