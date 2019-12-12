using Elders.Pandora;
using Elders.Pandora.Box;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;

namespace Pandora.Git
{
    public class JarFinder : IDisposable
    {
        private readonly IPandoraGitSettings _gitSettings;
        private readonly IPandoraContext _context;
        private readonly string _checkoutDir;

        public JarFinder(IPandoraGitSettings gitSettings, IPandoraContext context)
        {
            _gitSettings = gitSettings;
            _context = context;
            var cloneOptions = new LibGit2Sharp.CloneOptions
            {

                IsBare = false,
                Checkout = true,
                CredentialsProvider = new LibGit2Sharp.Handlers.CredentialsHandler((a, b, c) => new LibGit2Sharp.UsernamePasswordCredentials()
                {
                    Username = gitSettings.Username,
                    Password = gitSettings.Password
                })
            };

            try
            {
                _checkoutDir = LibGit2Sharp.Repository.Clone(gitSettings.SourceUrl, gitSettings.WorkingDir, cloneOptions);
            }
            catch (LibGit2Sharp.LibGit2SharpException ex)
            {
                Exception error = new Exception("Unable to checkout repository. Please check the credentials", ex);
                throw error;
            }
        }

        public JarFindResult FindJar()
        {
            string pattern = $"{_context.ApplicationName}.json";

            var workingDirFullPath = Path.Combine(Directory.GetCurrentDirectory(), _gitSettings.WorkingDir, _gitSettings.JarsLocation);
            var jarsDir = new DirectoryInfo(workingDirFullPath);

            var jarFile = jarsDir.GetFiles(pattern, SearchOption.AllDirectories).SingleOrDefault();
            if (jarFile is null)
                throw new FileNotFoundException($"Unable to find file {pattern} within {jarsDir.FullName}.");

            var jar = JsonConvert.DeserializeObject<Jar>(File.ReadAllText(jarFile.FullName));

            return new JarFindResult(jarsDir, jar);
        }

        void Clean()
        {
            var dirToDelete = new DirectoryInfo(_checkoutDir).Parent.Parent;
            string workingDir = dirToDelete.Parent.FullName;
            Directory.SetCurrentDirectory(workingDir);
            DeleteDirectory(dirToDelete.FullName);
        }

        void DeleteDirectory(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                return;
            }

            var files = Directory.GetFiles(directoryPath);
            var directories = Directory.GetDirectories(directoryPath);

            foreach (var file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (var dir in directories)
            {
                DeleteDirectory(dir);
            }

            File.SetAttributes(directoryPath, FileAttributes.Normal);

            try
            {
                Directory.Delete(directoryPath, true);
            }
            catch (IOException)
            {
                Directory.Delete(directoryPath, true);
            }
            catch (UnauthorizedAccessException)
            {
                Directory.Delete(directoryPath, true);
            }
        }

        public void Dispose()
        {
            Clean();
        }

        public class JarFindResult
        {
            public JarFindResult(DirectoryInfo jarLocation, Jar jar)
            {
                JarLocation = jarLocation;
                Jar = jar;
            }

            public DirectoryInfo JarLocation { get; set; }

            public Jar Jar { get; set; }
        }
    }
}
