using System;

namespace CoyneSolutions.SpeeDiff
{
    public class RevisionLoadedEventArgs : EventArgs
    {
        public Revision Revision { get; private set; }

        public RevisionLoadedEventArgs(Revision revision)
        {
            Revision = revision;
        }
    }
}
