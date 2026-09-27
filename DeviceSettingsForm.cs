using NAudio.CoreAudioApi;
using System.Drawing;

namespace AudioSwitch;

public class DeviceSettingsForm : Form
{
    private readonly ComboBox _combo1 =
        new()
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Width = 300
        };

    private readonly ComboBox _combo2 =
        new()
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Width = 300
        };

    private readonly Button _speakerButton1;
    private readonly Button _headphonesButton1;
    private readonly Button _speakerButton2;
    private readonly Button _headphonesButton2;

    private readonly List<MMDevice> _devices;

    private DeviceIcon _selectedIcon1;
    private DeviceIcon _selectedIcon2;

    public string? DeviceId1 { get; private set; }
    public string? DeviceId2 { get; private set; }

    public DeviceIcon SelectedIcon1 => _selectedIcon1;
    public DeviceIcon SelectedIcon2 => _selectedIcon2;

    public DeviceSettingsForm(
        string? currentId1,
        string? currentId2,
        DeviceIcon currentIcon1 = DeviceIcon.Speaker,
        DeviceIcon currentIcon2 = DeviceIcon.Headphones)
    {
        Text = "AudioSwitch — Nastavitve";

        ClientSize = new Size(640, 235);

        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;

        _selectedIcon1 = currentIcon1;
        _selectedIcon2 = currentIcon2;

        _devices = AudioSwitcher.GetOutputDevices();

        foreach (var device in _devices)
        {
            _combo1.Items.Add(
                new DeviceItem(device.FriendlyName, device.ID));

            _combo2.Items.Add(
                new DeviceItem(device.FriendlyName, device.ID));
        }

        Select(_combo1, currentId1);
        Select(_combo2, currentId2);

        // Oznaki naprav
        var label1 = new Label
        {
            Text = "Naprava 1:",
            AutoSize = true,
            Left = 20,
            Top = 32
        };

        var label2 = new Label
        {
            Text = "Naprava 2:",
            AutoSize = true,
            Left = 20,
            Top = 92
        };

        // Gumbi z ikonami
        _speakerButton1 = CreateIconButton("🔊");
        _headphonesButton1 = CreateIconButton("🎧");

        _speakerButton2 = CreateIconButton("🔊");
        _headphonesButton2 = CreateIconButton("🎧");

        _speakerButton1.Left = 125;
        _speakerButton1.Top = 18;

        _headphonesButton1.Left = 175;
        _headphonesButton1.Top = 18;

        _speakerButton2.Left = 125;
        _speakerButton2.Top = 78;

        _headphonesButton2.Left = 175;
        _headphonesButton2.Top = 78;

        // Izbira ikon
        _speakerButton1.Click += (_, _) =>
        {
            _selectedIcon1 = DeviceIcon.Speaker;
            UpdateIconButtons();
        };

        _headphonesButton1.Click += (_, _) =>
        {
            _selectedIcon1 = DeviceIcon.Headphones;
            UpdateIconButtons();
        };

        _speakerButton2.Click += (_, _) =>
        {
            _selectedIcon2 = DeviceIcon.Speaker;
            UpdateIconButtons();
        };

        _headphonesButton2.Click += (_, _) =>
        {
            _selectedIcon2 = DeviceIcon.Headphones;
            UpdateIconButtons();
        };

        // Izbira naprav
        _combo1.Left = 235;
        _combo1.Top = 24;

        _combo2.Left = 235;
        _combo2.Top = 84;

        var saveButton = new Button
        {
            Text = "Shrani",
            Width = 90,
            Height = 32,
            Left = 440,
            Top = 160
        };

        var cancelButton = new Button
        {
            Text = "Prekliči",
            Width = 90,
            Height = 32,
            Left = 540,
            Top = 160
        };

        saveButton.Click += (_, _) =>
        {
            if (_combo1.SelectedItem is not DeviceItem device1 ||
                _combo2.SelectedItem is not DeviceItem device2)
            {
                MessageBox.Show(
                    "Izberi obe napravi.",
                    "AudioSwitch");

                return;
            }

            if (device1.Id == device2.Id)
            {
                MessageBox.Show(
                    "Napravi morata biti različni.",
                    "AudioSwitch");

                return;
            }

            DeviceId1 = device1.Id;
            DeviceId2 = device2.Id;

            DialogResult = DialogResult.OK;
            Close();
        };

        cancelButton.Click += (_, _) =>
        {
            DialogResult = DialogResult.Cancel;
            Close();
        };

        Controls.AddRange(
        [
            label1,
            label2,

            _speakerButton1,
            _headphonesButton1,
            _speakerButton2,
            _headphonesButton2,

            _combo1,
            _combo2,

            saveButton,
            cancelButton
        ]);

        UpdateIconButtons();
    }

    private static Button CreateIconButton(string icon)
    {
        return new Button
        {
            Text = icon,
            Width = 42,
            Height = 42,
            Font = new Font(
                "Segoe UI Emoji",
                18f,
                FontStyle.Regular),

            TextAlign = ContentAlignment.MiddleCenter,

            FlatStyle = FlatStyle.Standard,
            UseVisualStyleBackColor = false,

            BackColor = Color.White,
            ForeColor = Color.FromArgb(65, 65, 65),

            TabStop = false,
            Cursor = Cursors.Hand
        };
    }

    private void UpdateIconButtons()
    {
        SetButtonState(
            _speakerButton1,
            _selectedIcon1 == DeviceIcon.Speaker);

        SetButtonState(
            _headphonesButton1,
            _selectedIcon1 == DeviceIcon.Headphones);

        SetButtonState(
            _speakerButton2,
            _selectedIcon2 == DeviceIcon.Speaker);

        SetButtonState(
            _headphonesButton2,
            _selectedIcon2 == DeviceIcon.Headphones);
    }

    private static void SetButtonState(
        Button button,
        bool selected)
    {
        if (selected)
        {
            // Izbrani gumb: temnejši in nekoliko "pritisnjen"
            button.BackColor = Color.FromArgb(205, 205, 205);
            button.FlatAppearance.BorderColor =
                Color.FromArgb(70, 70, 70);
            button.FlatAppearance.BorderSize = 2;
        }
        else
        {
            // Neizbrani gumb
            button.BackColor = Color.FromArgb(245, 245, 245);
            button.FlatAppearance.BorderColor =
                Color.FromArgb(180, 180, 180);
            button.FlatAppearance.BorderSize = 1;
        }
    }

    private void Select(ComboBox combo, string? id)
    {
        if (id == null)
            return;

        for (int i = 0; i < combo.Items.Count; i++)
        {
            if (combo.Items[i] is DeviceItem item &&
                item.Id.Equals(
                    id,
                    StringComparison.OrdinalIgnoreCase))
            {
                combo.SelectedIndex = i;
                return;
            }
        }
    }

    private record DeviceItem(string Name, string Id)
    {
        public override string ToString() => Name;
    }
}