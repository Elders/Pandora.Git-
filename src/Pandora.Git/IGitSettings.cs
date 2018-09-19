namespace Pandora.Git
{
    public interface IPandoraGitSettings
    {
        string JarsLocation { get; }
        string Password { get; }
        string SourceUrl { get; }
        string Username { get; }
        string WorkingDir { get; }
    }
}
