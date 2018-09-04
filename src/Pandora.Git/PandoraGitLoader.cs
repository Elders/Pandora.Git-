using Elders.Pandora;
using Elders.Pandora.Box;
using System.IO;

namespace Pandora.Git
{
    public class PandoraGitLoader
    {
        /// <summary>
        /// Clones the repository needed and then loads the needed configurations in the Pandora object
        /// </summary>
        /// <param name="applicationName">The name of the file with jars in it</param>
        /// <param name="gitSettings">The general git settings needed to clone</param>
        /// <param name="options">Options to get environment specific configurations</param>
        public PandoraGitLoader(string applicationName, GitSettings gitSettings, PandoraOptions options)
        {
            string pattern = $"{applicationName}.json";
            Jar = new JarFinder(gitSettings).FindJar(pattern);
            Box box = Box.Mistranslate(Jar);
            var opener = new PandoraBoxOpener(box);
            Directory.SetCurrentDirectory(Path.Combine(Directory.GetCurrentDirectory(), gitSettings.WorkingDir));

            Configuration cfg = opener.Open(options);
            string directoryToDelete = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(Directory.GetParent(directoryToDelete).ToString());
            DeleteDirectory(directoryToDelete);

            var cfgRepo = new GitConfigurationRepo(cfg);
            var appContext = new ApplicationContext(applicationName, options.ClusterName, options.MachineName);
            Pandora = new Elders.Pandora.Pandora(appContext, cfgRepo);
        }

        public Elders.Pandora.Pandora Pandora { get; private set; }

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
    }
}
