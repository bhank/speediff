using System;

namespace CoyneSolutions.SpeeDiff
{
    public class FindEventArgs : EventArgs
    {
        public string Text { get; set; }
        public bool UseRegularExpressions { get; set; }
        public bool SearchLeft { get; set; }
        public bool SearchRight { get; set; }
    }
}
