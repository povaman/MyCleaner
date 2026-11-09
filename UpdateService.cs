using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text.Json;

namespace MyCleaner;

public class UpdateInfo
{
    public string Version { get; set; } = "";
    public string Url { get; set; } = "";
    public string Sha256 { get; set; } = "";
    public string Notes { get; set; } = "";
}

public static class UpdateService
{
    // غيّر الرابط لو اسم الـ repo اتغير
    public const string VersionUrl = "https://raw.githubusercontent.com/povaman/MyCleaner/main/version.json";

    private static readonly HttpClient Http = new HttpClient { Timeout = TimeSpan.FromMinutes(5) };

    public static string CurrentVersion =>
        typeof(UpdateService).Assembly.GetName().Version?.ToString(3) ?? "1.0.0";

    public static async Task<UpdateInfo?> CheckAsync()
    {
        var json = await Http.GetStringAsync(VersionUrl);
        var info = JsonSerializer.Deserialize<UpdateInfo>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (info == null || string.IsNullOrWhiteSpace(info.Version)) return null;

        return System.Version.Parse(info.Version) > System.Version.Parse(CurrentVersion) ? info : null;
    }

    public static async Task<string> DownloadAsync(UpdateInfo info)
    {
        var bytes = await Http.GetByteArrayAsync(info.Url);
        var path = Path.Combine(Path.GetTempPath(), $"MyCleaner-Setup-{info.Version}.exe");
        await File.WriteAllBytesAsync(path, bytes);

        if (!string.IsNullOrWhiteSpace(info.Sha256))
        {
            var hash = Convert.ToHexString(SHA256.HashData(bytes));
            if (!hash.Equals(info.Sha256, StringComparison.OrdinalIgnoreCase))
            {
                File.Delete(path);
                throw new InvalidOperationException("ملف التحديث متغيّر أو تالف، اتلغى التثبيت.");
            }
        }
        return path;
    }

    public static void Install(string setupPath)
    {
        Process.Start(new ProcessStartInfo(setupPath, "/SILENT /CLOSEAPPLICATIONS /RESTARTAPPLICATIONS")
        {
            UseShellExecute = true
        });
        System.Windows.Application.Current.Shutdown();
    }
}
