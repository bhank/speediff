using System;

namespace CoyneSolutions.SpeeDiff
{
    public abstract class RevisionProvider
    {
        public static IRevisionProvider GetRevisionProvider(string path)
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
    }
}
