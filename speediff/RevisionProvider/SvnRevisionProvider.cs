using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using SharpSvn;
using SharpSvn.UI;

namespace CoyneSolutions.SpeeDiff
{
    public class SvnRevisionProvider : RevisionProvider
    {
        internal SvnRevisionProvider(string path)
        {
            Path = path;
            SvnUI.Bind(svnClient, new SvnUIBindArgs());
        }

        private SvnClient svnClient = new SvnClient();

        public override string RevisionPrefix
        {
            get { return "r"; }
        }

        /// <summary>
        /// Quickly check whether this might be an SVN repo.
        /// If it's a local path, check it; if it's a URL, assume it's OK to avoid a round-trip.
        /// </summary>
        public static bool IsInSvnRepository(string path)
        {
            return path.Contains("://") || (new SvnClient()).GetUriFromWorkingCopy(path) != null;
        }

        public override async Task Initialize()
        {
            await Task.Run(() => LoadRevisions(Path));
        }

        public override async Task<IList<Revision>> LoadRevisions()
        {
            await Initialize();
            return Revisions;
        }

        public override bool IsRevisionIdNumeric
        {
            get { return true; }
        }

        private async void LoadRevisions(string path)
        {
            Revisions = new List<Revision>();
            SvnTarget target;
            if (path.Contains("://"))
            {
                target = new SvnUriTarget(path);
            }
            else
            {
                target = new SvnPathTarget(path);
            }
            var svnFileVersionsArgs = new SvnFileVersionsArgs {Start = SvnRevision.Zero, End = SvnRevision.Head};
            svnClient.FileVersions(target, svnFileVersionsArgs, (sender, args) =>
            {
                var revision = new Revision
                {
                    Author = args.Author,
                    Message = args.LogMessage,
                    RevisionId = args.Revision.ToString(CultureInfo.InvariantCulture),
                    RevisionTime = args.Time,
                };

                using (TextReader reader = new StreamReader(args.GetContentStream()))
                {
                    revision.Content = reader.ReadToEnd();
                }

                Revisions.Insert(0,revision); // Put them in newest-to-oldest order
            });
        }

        protected override string SourceControlTypeId
        {
            get { return "svn"; }
        }
    }
}