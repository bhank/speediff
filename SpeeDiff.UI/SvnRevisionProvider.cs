using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using SharpSvn;

namespace CoyneSolutions.SpeeDiff
{
    public class SvnRevisionProvider : IRevisionProvider
    {
        private SvnClient svnClient = new SvnClient();
        public IList<Revision> Revisions { get; private set; }
        public event EventHandler<RevisionLoadedEventArgs> RevisionLoaded;

        async public Task Initialize(string path)
        {
            await Task.Run(() => LoadRevisions(path));
        }

        private void LoadRevisions(string path)
        {
            Revisions = new List<Revision>();
            var target = new SvnUriTarget(path);
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

                Revisions.Add(revision);

                var revisionLoaded = RevisionLoaded;
                if (revisionLoaded != null)
                {
                    revisionLoaded(this, new RevisionLoadedEventArgs(revision));
                }
            });
        }
    }
}