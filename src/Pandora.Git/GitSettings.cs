using System;
using System.IO;

namespace Pandora.Git
{
    public class GitSettings : IPandoraGitSettings
    {
        internal GitSettings()
        {

        }

        /// <summary>
        /// The constructor for the basic settings to make git work
        /// </summary>
        /// <param name="sourceUrl">The url for the remote repository</param>
        /// <param name="workingDir">The directory in which the repository will be cloned and deleted from. Defaults to .pandora</param>
        /// <param name="username">The username is needed if the repository is private</param>
        /// <param name="password">The password is needed if the repository is private</param>
        public GitSettings(string sourceUrl, string workingDir = ".pandora", string username = "", string password = "")
        {
            if (string.IsNullOrEmpty(sourceUrl)) throw new ArgumentNullException(nameof(sourceUrl));

            SourceUrl = sourceUrl;
            WorkingDir = Path.Combine(workingDir, DateTime.UtcNow.ToString("yyyyMMddhhmmss"));
            Username = username;
            Password = password;
        }

        public string SourceUrl { get; protected set; }
        public string WorkingDir { get; protected set; }
        public string Username { get; protected set; }
        public string Password { get; protected set; }
    }

    public class GitSettingsFromEnvironmentVariables : GitSettings
    {
        public GitSettingsFromEnvironmentVariables()
        {
            SourceUrl = Environment.GetEnvironmentVariable("pandora_git_url");
            if (string.IsNullOrEmpty(SourceUrl)) throw new ArgumentNullException(nameof(SourceUrl));

            string workingDir = Environment.GetEnvironmentVariable("pandora_git_workingdir") ?? ".pandora";
            WorkingDir = Path.Combine(workingDir, DateTime.UtcNow.ToString("yyyyMMddhhmmss"));

            Username = Environment.GetEnvironmentVariable("pandora_git_username") ?? string.Empty;
            Password = Environment.GetEnvironmentVariable("pandora_git_password") ?? string.Empty;
        }
    }
}
