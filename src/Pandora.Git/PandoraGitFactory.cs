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
        private readonly IPandoraGitSettings _gitSettings;
        private IConfigurationRepository _configurationRepository;

        /// <summary>
        /// Clones the repository needed and then loads the needed configurations in the Pandora object
        /// </summary>
        /// <param name="applicationName">The name of the file with jars in it</param>
        /// <param name="gitSettings">The general git settings needed to clone</param>
        /// <param name="options">Options to get environment specific configurations</param>
        public PandoraGitFactory(IPandoraContext context, IPandoraGitSettings gitSettings)
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
                using (var jarFinder = new JarFinder(_gitSettings, _context))
                {
                    var jarFindResult = jarFinder.FindJar();

                    Directory.SetCurrentDirectory(jarFindResult.JarLocation.FullName);
                    Box box = Box.Mistranslate(jarFindResult.Jar);
                    var opener = new PandoraBoxOpener(box);
                    PandoraOptions options = new PandoraOptions(_context.Cluster, _context.Machine);
                    Configuration cfg = opener.Open(options);
                    _configurationRepository = new GitConfigurationRepo(_context, cfg);
                }
            }
        }

        public Jar Jar { get; private set; }

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
