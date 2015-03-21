using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoyneSolutions.SpeeDiff
{
    public interface IRevisionProvider
    {
        string Path { get; }
        IList<Revision> Revisions { get; }
        Task Initialize();
        event EventHandler<RevisionLoadedEventArgs> RevisionLoaded;
    }
}
