using System;
using System.IO;

namespace Pandora.Git
{
    public class GitSettings
    {
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
