using System;
using System.IO;

namespace Pandora.Git
{
    public class GitSettings
    {
        /// <summary>
        /// The constructor for the basic settings to make git work
        /// </summary>
        /// <param name="sourceUrl">The url for the remote repository</param>
        /// <param name="workingDir">The directory in which the repository will be cloned and deleted from. Defaults to .pandora</param>
        /// <param name="username">The username is needed if the repository is private</param>
        /// <param name="password">The password is needed if the repository is private</param>
        public GitSettings(string sourceUrl, string workingDir = ".pandora", string username = "", string password = "")
        {
            if (string.IsNullOrEmpty(workingDir)) throw new ArgumentNullException(nameof(workingDir));
            if (string.IsNullOrEmpty(sourceUrl)) throw new ArgumentNullException(nameof(sourceUrl));

            SourceUrl = sourceUrl;
            WorkingDir = Path.Combine(workingDir, DateTime.UtcNow.ToString("yyyyMMddhhmmss"));
            Username = username;
            Password = password;
        }

        public string SourceUrl { get; private set; }
        public string WorkingDir { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
    }
}
