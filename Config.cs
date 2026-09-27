namespace AudioSwitch;

public enum DeviceIcon
{
    Speaker = 0,
    Headphones = 1
}

public static class Config
{
    private static readonly string Path = System.IO.Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "AudioSwitch",
        "devices.txt");

    public static (string? id1, string? id2, DeviceIcon icon1, DeviceIcon icon2) Load()
    {
        try
        {
            var lines = File.ReadAllLines(Path);

            if (lines.Length >= 2)
            {
                var icon1 = DeviceIcon.Speaker;
                var icon2 = DeviceIcon.Headphones;

                if (lines.Length >= 3 &&
                    int.TryParse(lines[2], out var value1) &&
                    Enum.IsDefined(typeof(DeviceIcon), value1))
                {
                    icon1 = (DeviceIcon)value1;
                }

                if (lines.Length >= 4 &&
                    int.TryParse(lines[3], out var value2) &&
                    Enum.IsDefined(typeof(DeviceIcon), value2))
                {
                    icon2 = (DeviceIcon)value2;
                }

                return (lines[0], lines[1], icon1, icon2);
            }
        }
        catch
        {
        }

        return (null, null, DeviceIcon.Speaker, DeviceIcon.Headphones);
    }

    public static void Save(
        string id1,
        string id2,
        DeviceIcon icon1,
        DeviceIcon icon2)
    {
        Directory.CreateDirectory(
            System.IO.Path.GetDirectoryName(Path)!);

        File.WriteAllLines(
            Path,
            [
                id1,
                id2,
                ((int)icon1).ToString(),
                ((int)icon2).ToString()
            ]);
    }
}