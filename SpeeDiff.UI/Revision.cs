using System;

namespace CoyneSolutions.SpeeDiff
{
    public class Revision
    {
        public string RevisionId { get; set; }
        public DateTime RevisionTime { get; set; }
        public string Author { get; set; }
        public string Message { get; set; }
        internal string Content { get; set; }
        public string GetContent()
        {
            return Content;
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} \"{3}\" [{4}]", RevisionId, RevisionTime, Author, Message, Content == null ? -1 : Content.Length);
        }
    }
}
