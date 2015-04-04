using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LibGit2Sharp;

namespace CoyneSolutions.SpeeDiff
{
    public class GitRevisionProvider : RevisionProvider
    {
        internal GitRevisionProvider(string path)
        {
            Path = path;
        }

        public override string RevisionPrefix
        {
            get { return string.Empty; }
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
            get { return false; }
        }

        private void LoadRevisions(string path)
        {
            Revisions = new List<Revision>();

            string repositoryPath, relativePath;
            if (!TryGetGitRepositoryPath(path, out repositoryPath, out relativePath))
            {
                throw new Exception("Not a git repository");
            }
            using (var repository = new Repository(repositoryPath))
            {
                var commits = repository.Commits.Where(c =>
                    c.Tree[relativePath] != null
                    && (
                        !c.Parents.Any()
                        ||
                        c.Parents.Count() == 1
                        && (
                            c.Parents.Single().Tree[relativePath] == null
                            || c.Tree[relativePath].Target.Id != c.Parents.Single().Tree[relativePath].Target.Id
                            )
                        )
                    );

                foreach (var commit in commits)
                {
                    var revision = new Revision
                    {
                        Author = string.Format("{0} <{1}>", commit.Author.Name, commit.Author.Email),
                        Message = commit.Message,
                        RevisionId = commit.Sha,
                        RevisionTime = commit.Author.When.LocalDateTime,
                    };

                    var blob = commit.Tree[relativePath].Target as Blob;
                    if (blob == null)
                    {
                        throw new Exception("Target is not a blob for " + relativePath);
                    }
                    using (TextReader reader = new StreamReader(blob.GetContentStream()))
                    {
                        revision.Content = reader.ReadToEnd();
                    }

                    Revisions.Add(revision);
                }
            }
        }

        public static bool IsInGitRepository(string path)
        {
            string ignore;
            return TryGetGitRepositoryPath(path, out ignore, out ignore);
        }

        private static bool TryGetGitRepositoryPath(string path, out string repositoryPath, out string relativePath)
        {
            repositoryPath = null;
            relativePath = null;

            if (path.Contains("://")) return false;

            var startingDirectory = new DirectoryInfo(path); // The path needs to be the exact case that it is on disk
            if (!startingDirectory.Exists && (startingDirectory.Parent == null || !startingDirectory.Parent.Exists)) // startingDirectory should be a file, not a directory, so it won't exist, but its parent will.
            {
                return false;
            }

            DirectoryInfo currentDirectory;
            if ((File.GetAttributes(startingDirectory.FullName) & FileAttributes.Directory) == FileAttributes.Directory)
            {
                currentDirectory = startingDirectory;
            }
            else
            {
                currentDirectory = startingDirectory.Parent;
            }

            if (currentDirectory == null || !currentDirectory.Exists) return false;

            while (currentDirectory != null)
            {
                if (Directory.Exists(System.IO.Path.Combine(currentDirectory.FullName, ".git")))
                {
                    repositoryPath = currentDirectory.FullName;
                    relativePath = startingDirectory.FullName.Substring(repositoryPath.Length + 1);
                    relativePath = relativePath.Replace("\\", "/");
                    return true;
                }
                currentDirectory = currentDirectory.Parent;
            }
            return false;
        }

        protected override string SourceControlTypeId
        {
            get { return "git"; }
        }
    }
}