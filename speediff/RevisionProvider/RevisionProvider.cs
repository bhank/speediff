using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoyneSolutions.SpeeDiff
{
    public abstract class RevisionProvider
    {
        public static RevisionProvider GetRevisionProvider(string path)
        {
            if (GitRevisionProvider.IsInGitRepository(path))
            {
                return new GitRevisionProvider(path);
            }
            else if(SvnRevisionProvider.IsInSvnRepository(path))
            {
                return new SvnRevisionProvider(path);
            }
            throw new ArgumentException("Path is not in a Git or SVN repository", "path");
        }

        public bool MatchesSourceControlTypeId(string typelist)
        {
            const string delim = ",";
            return (delim + typelist + delim).Contains(delim + SourceControlTypeId + delim);
        }

        protected abstract string SourceControlTypeId { get; }

        protected string Path { get; set; }
        public IList<Revision> Revisions { get; protected set; }
        public abstract string RevisionPrefix { get; }
        public abstract Task Initialize();
        public abstract Task<IList<Revision>> LoadRevisions();
        public abstract bool IsRevisionIdNumeric { get; }

    }
}
