using System.Collections.Generic;

namespace CoyneSolutions.SpeeDiff
{
    public interface IRevisionProvider
    {
        IList<Revision> Revisions { get; }
        //string GetRevisionContent(string revisionId);
    }
}
