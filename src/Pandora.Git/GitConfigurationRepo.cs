using Elders.Pandora;
using Elders.Pandora.Box;
using System;
using System.Collections.Generic;

namespace Pandora.Git
{
    public sealed class GitConfigurationRepo : IConfigurationRepository
    {
        private readonly Configuration _cfg;

        public GitConfigurationRepo(Configuration pandoraConfiguration)
        {
            if (pandoraConfiguration is null) throw new ArgumentNullException(nameof(pandoraConfiguration));

            _cfg = pandoraConfiguration;
        }

        public void Delete(string key) { throw new NotSupportedException(); }

        public bool Exists(string key)
        {
            return _cfg.ContainsKey(key);
        }

        public string Get(string key)
        {
            if (_cfg.ContainsKey(key))
                return _cfg[key].ToString();

            return string.Empty;
        }

        public IEnumerable<DeployedSetting> GetAll() { throw new NotSupportedException(); }

        public void Set(string key, string value) { throw new NotSupportedException(); }
    }
}
