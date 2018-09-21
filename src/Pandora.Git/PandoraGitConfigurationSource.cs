using Elders.Pandora;
using Microsoft.Extensions.Configuration;
using System;

namespace Pandora.Git
{
    public class PandoraGitConfigurationSource : IConfigurationSource
    {
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            string applicationName = Environment.GetEnvironmentVariable("APPLICATION_NAME");
            IPandoraContext context = new ClusterContext(applicationName);
            IPandoraGitSettings gitSettings = new GitSettingsFromEnvironmentVariables();
            IPandoraFactory pandoraFactory = new PandoraGitFactory(context, gitSettings);
            Elders.Pandora.Pandora pandora = new Elders.Pandora.Pandora(pandoraFactory);

            return new PandoraConfigurationProvider(pandora);
        }
    }
}
