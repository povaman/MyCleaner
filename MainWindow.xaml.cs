using System.Windows;

namespace MyCleaner;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        VersionText.Text = "الإصدار " + UpdateService.CurrentVersion;
    }

    private void Scan_Click(object sender, RoutedEventArgs e)
    {
        long bytes = CleanerService.Scan();
        StatusText.Text = $"المساحة اللي هتتوفر: {Format(bytes)}";
    }

    private void Clean_Click(object sender, RoutedEventArgs e)
    {
        var confirm = MessageBox.Show("هيتم حذف الملفات المؤقتة. تكمل؟", "تأكيد",
            MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (confirm != MessageBoxResult.Yes) return;

        var (deleted, failed) = CleanerService.Clean();
        StatusText.Text = $"تم توفير {Format(deleted)} ({failed} ملف كان مستخدم ومتحذفش)";
    }

    private async void Update_Click(object sender, RoutedEventArgs e)
    {
        UpdateBtn.IsEnabled = false;
        try
        {
            StatusText.Text = "بفحص التحديثات...";
            var info = await UpdateService.CheckAsync();
            if (info == null)
            {
                StatusText.Text = "أنت على آخر إصدار";
                return;
            }

            var answer = MessageBox.Show(
                $"فيه إصدار جديد {info.Version}\n\n{info.Notes}\n\nتحمّله وتثبّته دلوقتي؟",
                "تحديث", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (answer != MessageBoxResult.Yes)
            {
                StatusText.Text = "تم تأجيل التحديث";
                return;
            }

            StatusText.Text = "بنحمّل التحديث...";
            Progress.IsIndeterminate = true;
            var path = await UpdateService.DownloadAsync(info);
            UpdateService.Install(path);
        }
        catch (Exception ex)
        {
            StatusText.Text = "حصلت مشكلة في التحديث";
            MessageBox.Show(ex.Message, "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            Progress.IsIndeterminate = false;
            UpdateBtn.IsEnabled = true;
        }
    }

    private static string Format(long bytes)
    {
        string[] units = { "B", "KB", "MB", "GB" };
        double size = bytes;
        int i = 0;
        while (size >= 1024 && i < units.Length - 1)
        {
            size /= 1024;
            i++;
        }
        return $"{size:0.##} {units[i]}";
    }
}
