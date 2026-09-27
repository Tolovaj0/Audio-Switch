using Microsoft.Win32;

namespace AudioSwitch;

public static class StartupManager
{
    private const string AppName = "AudioSwitch";
    private const string RegPath = @"Software\Microsoft\Windows\CurrentVersion\Run";

    public static bool IsEnabled
    {
        get
        {
            using var key = Registry.CurrentUser.OpenSubKey(RegPath);
            return key?.GetValue(AppName) != null;
        }
    }

    public static void SetEnabled(bool enabled)
    {
        using var key = Registry.CurrentUser.OpenSubKey(RegPath, true)!;
        if (enabled)
        {
            var exe = Environment.ProcessPath ?? "";
            key.SetValue(AppName, $"\"{exe}\"");
        }
        else
        {
            key.DeleteValue(AppName, false);
        }
    }
}