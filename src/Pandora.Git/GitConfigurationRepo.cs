using Elders.Pandora;
using Elders.Pandora.Box;
using System;
using System.Collections.Generic;

namespace Pandora.Git
{
    public sealed class GitConfigurationRepo : IConfigurationRepository
    {
        private readonly IPandoraContext context;
        private readonly Configuration _cfg;

        public GitConfigurationRepo(IPandoraContext context, Configuration pandoraConfiguration)
        {
            if (pandoraConfiguration is null) throw new ArgumentNullException(nameof(pandoraConfiguration));

            this.context = context;
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

        public IEnumerable<DeployedSetting> GetAll()
        {
            foreach (var setting in _cfg.AsDictionary())
            {
                Key key = Key.Parse(setting.Key);
                yield return new DeployedSetting(
                        raw: setting.Key,
                        applicationName: context.ApplicationName,
                        cluster: context.Cluster,
                        machine: context.Machine,
                        settingKey: key.SettingKey,
                        value: setting.Value.ToString());
            }
        }

        public void Set(string key, string value) { throw new NotSupportedException(); }
    }
}
