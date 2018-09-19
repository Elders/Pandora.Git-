using Elders.Pandora.Box;
using Newtonsoft.Json;
using System.IO;
using System.Linq;

namespace Pandora.Git
{
    public class JarFinder
    {
        private readonly IPandoraGitSettings _gitSettings;
        private readonly string _checkoutDir;

        public JarFinder(IPandoraGitSettings gitSettings)
        {
            this._gitSettings = gitSettings;
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

            _checkoutDir = LibGit2Sharp.Repository.Clone(gitSettings.SourceUrl, gitSettings.WorkingDir, cloneOptions);
        }

        public Jar FindJar(string pattern)
        {
            var dir = new DirectoryInfo(_checkoutDir).Parent;
            var jarFile = dir.GetFiles(pattern, SearchOption.AllDirectories).SingleOrDefault();
            if (jarFile is null)
                throw new FileNotFoundException($"Unable to find file {pattern} within {dir.FullName}.");

            var jar = JsonConvert.DeserializeObject<Jar>(File.ReadAllText(jarFile.FullName));

            return jar;
        }
    }
}
