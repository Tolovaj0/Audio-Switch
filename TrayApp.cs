namespace AudioSwitch;

public class TrayApp : ApplicationContext
{
    private readonly NotifyIcon _tray;
    private readonly ContextMenuStrip _menu;
    private readonly ToolStripMenuItem _startupItem;

    private string? _id1, _id2;
    private DeviceIcon _icon1, _icon2;

    public TrayApp()
    {
        (_id1, _id2, _icon1, _icon2) = Config.Load();

        _startupItem = new ToolStripMenuItem("Zaženi ob Windows")
        {
            Checked = StartupManager.IsEnabled
        };
        _startupItem.Click += (_, _) =>
        {
            StartupManager.SetEnabled(!_startupItem.Checked);
            _startupItem.Checked = StartupManager.IsEnabled;
        };

        var switchItem = new ToolStripMenuItem("Preklopi napravo");
        switchItem.Click += (_, _) => Switch();

        var settingsItem = new ToolStripMenuItem("Nastavitve...");
        settingsItem.Click += (_, _) => OpenSettings();

        var exitItem = new ToolStripMenuItem("Izhod");
        exitItem.Click += (_, _) => ExitApp();

        _menu = new ContextMenuStrip();
        _menu.Items.Add(switchItem);
        _menu.Items.Add(new ToolStripSeparator());
        _menu.Items.Add(settingsItem);
        _menu.Items.Add(_startupItem);
        _menu.Items.Add(new ToolStripSeparator());
        _menu.Items.Add(exitItem);

        _tray = new NotifyIcon
        {
            Icon = DrawSpeakerIcon(1),
            Visible = true,
            Text = "AudioSwitch",
            ContextMenuStrip = _menu
        };
        _tray.MouseClick += (_, e) =>
        {
            if (e.Button == MouseButtons.Left) Switch();
        };

        UpdateTray();

        if (_id1 == null || _id2 == null)
            OpenSettings();
    }

    private void Switch()
    {
        if (_id1 == null || _id2 == null) { OpenSettings(); return; }
        try
        {
            var current = AudioSwitcher.GetDefaultDeviceId();
            var target = current.Equals(_id1, StringComparison.OrdinalIgnoreCase) ? _id2 : _id1;
            AudioSwitcher.SetDefaultDevice(target);
            UpdateTray();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Napaka pri preklopu:\n" + ex.Message, "AudioSwitch");
        }
    }

    private void OpenSettings()
    {
        using var form = new DeviceSettingsForm(
            _id1,
            _id2,
            _icon1,
            _icon2);
        if (form.ShowDialog() == DialogResult.OK)
        {
            _id1 = form.DeviceId1;
            _id2 = form.DeviceId2;

            _icon1 = form.SelectedIcon1;
            _icon2 = form.SelectedIcon2;

            Config.Save(
                _id1!,
                _id2!,
                _icon1,
                _icon2);
            UpdateTray();
        }
    }

    private void UpdateTray()
    {
        if (_id1 == null || _id2 == null)
        {
            _tray.Icon = DrawSpeakerIcon(1);
            _tray.Text = "AudioSwitch — nastavi napravi";
            return;
        }

        try
        {
            var current = AudioSwitcher.GetDefaultDeviceId();
            var devices = AudioSwitcher.GetOutputDevices();
            var currentDevice = devices.FirstOrDefault(d =>
                d.ID.Equals(current, StringComparison.OrdinalIgnoreCase));
            var name = currentDevice?.FriendlyName ?? "?";

            // Zaznaj tip po imenu naprave - to je zdaj nepomembno, ker uporabljamo izbrane ikone
            bool isDevice1 =
                current.Equals(
                    _id1,
                    StringComparison.OrdinalIgnoreCase);

            int number = isDevice1 ? 1 : 2;

            DeviceIcon icon =
                isDevice1
                    ? _icon1
                    : _icon2;

            _tray.Icon = icon == DeviceIcon.Headphones
                ? DrawHeadphonesIcon(number)
                : DrawSpeakerIcon(number);
            _tray.Text = "AudioSwitch\n" + name;
        }
        catch
        {
            _tray.Icon = DrawSpeakerIcon(1);
            _tray.Text = "AudioSwitch";
        }
    }

    private static bool IsHeadphones(string deviceName)
    {
        var name = deviceName.ToLowerInvariant();
        return name.Contains("headphone") ||
            name.Contains("headset") ||
            name.Contains("slušalk") ||
            name.Contains("usb") ||
            name.Contains("earphone");
    }
    private static Icon DrawSpeakerIcon(int number)
    {
        var bmp = new Bitmap(32, 32);
        using var g = Graphics.FromImage(bmp);
        g.Clear(Color.Transparent);
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        var fill = new SolidBrush(Color.FromArgb(36, 122, 191));
        var outline = new Pen(Color.Black, 1f);
        //var wavePen = new Pen(Color.FromArgb(80, 80, 80), 2f)
        var wavePen = new Pen(Color.FromArgb(39, 138, 255), 2f)
        {
            StartCap = System.Drawing.Drawing2D.LineCap.Round,
            EndCap = System.Drawing.Drawing2D.LineCap.Round
        };

        // Telo
        g.FillRectangle(fill, 3, 11, 6, 10);
        g.DrawRectangle(outline, 3, 11, 6, 10);

        // Stožec
        var cone = new Point[] { new(9, 11), new(18, 5), new(18, 27), new(9, 21) };
        g.FillPolygon(fill, cone);
        g.DrawPolygon(outline, cone);

        // Valovi
        g.DrawArc(wavePen, 19, 9, 5, 13, -60, 120);
        g.DrawArc(wavePen, 20, 5, 8, 21, -60, 120);

        // Številka
        using var font = new Font("Arial", 9f, FontStyle.Bold);
        g.DrawString(number.ToString(), font, Brushes.Black, 23, 19);

        fill.Dispose();
        outline.Dispose();
        wavePen.Dispose();
        return Icon.FromHandle(bmp.GetHicon());
    }

    private static Icon DrawHeadphonesIcon(int number)
    {
        var bmp = new Bitmap(32, 32);
        using var g = Graphics.FromImage(bmp);
        g.Clear(Color.Transparent);
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        var fill = Color.FromArgb(80, 80, 80);
        var arcPen = new Pen(Color.FromArgb(80, 80, 80), 3f)
        {
            StartCap = System.Drawing.Drawing2D.LineCap.Round,
            EndCap = System.Drawing.Drawing2D.LineCap.Round
        };
        var outlinePen = new Pen(Color.IndianRed, 1f);
        //var outlinePen = new Pen(Color.Black, 1f);

        // Lok
        g.DrawArc(arcPen, 5, 3, 20, 16, 180, 180);

        // Leva blazinica
        g.FillRoundedRectangle(fill, 3, 16, 8, 10, 3);
        g.DrawRoundedRectangle(outlinePen, 3, 16, 8, 10, 3);

        // Desna blazinica
        g.FillRoundedRectangle(fill, 21, 16, 8, 10, 3);
        g.DrawRoundedRectangle(outlinePen, 21, 16, 8, 10, 3);

        // Številka
        using var font = new Font("Arial", 9f, FontStyle.Bold);
        g.DrawString(number.ToString(), font, Brushes.Black, 11, 19);
        //g.DrawString(number.ToString(), font, Brushes.Black, 23, 19);

        arcPen.Dispose();
        outlinePen.Dispose();
        return Icon.FromHandle(bmp.GetHicon());
    }

    private void ExitApp()
    {
        _tray.Visible = false;
        _tray.Dispose();
        _menu.Dispose();
        ExitThread();
    }
}