using System.IO;

namespace MyCleaner;

public static class CleanerService
{
    private static IEnumerable<string> Folders()
    {
        yield return Path.GetTempPath();
        yield return Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Windows), "Temp");
    }

    private static List<FileInfo> FilesIn(string folder)
    {
        try
        {
            if (!Directory.Exists(folder)) return new List<FileInfo>();
            var opts = new EnumerationOptions { RecurseSubdirectories = true, IgnoreInaccessible = true };
            return new DirectoryInfo(folder).EnumerateFiles("*", opts).ToList();
        }
        catch
        {
            return new List<FileInfo>();
        }
    }

    public static long Scan()
    {
        long total = 0;
        foreach (var folder in Folders())
            foreach (var f in FilesIn(folder))
                try { total += f.Length; } catch { }
        return total;
    }

    public static (long deleted, int failed) Clean()
    {
        long deleted = 0;
        int failed = 0;
        foreach (var folder in Folders())
        {
            foreach (var f in FilesIn(folder))
            {
                try
                {
                    long size = f.Length;
                    f.Delete();
                    deleted += size;
                }
                catch
                {
                    failed++;
                }
            }
        }
        return (deleted, failed);
    }
}
