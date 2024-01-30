namespace Rebus.Extensions.Configuration.FileSystem;

using System.ComponentModel.DataAnnotations;

public class FileSystemRebusTransportOptions
{
    [Required] public string BaseDirectory { get; set; } = string.Empty;

    /// <summary>
    ///     How many files to prefetch - i.e keep locks on.
    /// </summary>
    public int? Prefetch { get; set; }

    public string GetBaseDirectoryExpanded()
    {
        var currentDir = Directory.GetCurrentDirectory();

        var baseDir = BaseDirectory.Replace("{PWD}", Directory.GetCurrentDirectory());
        baseDir = baseDir.Replace("{ROOT}", Directory.GetDirectoryRoot(currentDir));
        baseDir = baseDir.Replace("{APPDATA}", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
        return baseDir;
    }
}
