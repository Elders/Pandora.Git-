using Elders.Pandora;
using Elders.Pandora.Box;
using System;
using System.IO;

namespace Pandora.Git
{
    public class PandoraGitFactory : IPandoraFactory
    {
        object refreshPandora = new object();

        private readonly IPandoraContext _context;
        private readonly GitSettings _gitSettings;
        private IConfigurationRepository _configurationRepository;

        /// <summary>
        /// Clones the repository needed and then loads the needed configurations in the Pandora object
        /// </summary>
        /// <param name="applicationName">The name of the file with jars in it</param>
        /// <param name="gitSettings">The general git settings needed to clone</param>
        /// <param name="options">Options to get environment specific configurations</param>
        public PandoraGitFactory(IPandoraContext context, GitSettings gitSettings)
        {
            if (context is null) throw new ArgumentNullException(nameof(context));
            if (gitSettings is null) throw new ArgumentNullException(nameof(gitSettings));

            _context = context;
            _gitSettings = gitSettings;

            Refresh();
        }

        public void Refresh()
        {
            lock (refreshPandora)
            {
                string pattern = $"{_context.ApplicationName}.json";
                Jar = new JarFinder(_gitSettings).FindJar(pattern);
                Box box = Box.Mistranslate(Jar);
                var opener = new PandoraBoxOpener(box);
                Directory.SetCurrentDirectory(Path.Combine(Directory.GetCurrentDirectory(), _gitSettings.WorkingDir));

                PandoraOptions options = new PandoraOptions(_context.Cluster, _context.Machine);
                Configuration cfg = opener.Open(options);
                string directoryToDelete = Directory.GetCurrentDirectory();
                Directory.SetCurrentDirectory(Directory.GetParent(directoryToDelete).ToString());
                DeleteDirectory(directoryToDelete);

                _configurationRepository = new GitConfigurationRepo(cfg);
            }
        }

        public Jar Jar { get; private set; }

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

            Directory.Delete(directoryPath, false);
        }

        public IPandoraContext GetContext()
        {
            return _context;
        }

        public IConfigurationRepository GetConfiguration()
        {
            return _configurationRepository;
        }
    }
}
