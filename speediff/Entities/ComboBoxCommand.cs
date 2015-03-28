using System;

namespace CoyneSolutions.SpeeDiff
{
    public class ComboBoxCommand
    {
        public string Text { get; private set; }
        public Action Action { get; private set; }

        public ComboBoxCommand(string text, Action action)
        {
            Text = text;
            Action = action;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
