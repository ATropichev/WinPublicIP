using WinPublicIP.Config;

namespace WinPublicIP;

internal sealed class SettingsForm : Form
{
    private readonly AppSettings _settings;
    private readonly NumericUpDown _intervalInput;

    public SettingsForm(AppSettings settings)
    {
        _settings = settings;

        Text = "WinPublicIP — Настройки";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(320, 100);

        var label = new Label
        {
            Text = "Интервал обновления (сек):",
            Location = new Point(12, 16),
            AutoSize = true
        };

        _intervalInput = new NumericUpDown
        {
            Minimum = 10,
            Maximum = 3600,
            Value = Math.Clamp(settings.RefreshIntervalSeconds, 10, 3600),
            Location = new Point(210, 12),
            Width = 80
        };

        var ok = new Button
        {
            Text = "OK",
            DialogResult = DialogResult.OK,
            Location = new Point(152, 60),
            Width = 75
        };

        var cancel = new Button
        {
            Text = "Отмена",
            DialogResult = DialogResult.Cancel,
            Location = new Point(233, 60),
            Width = 75
        };

        ok.Click += (_, _) => Save();
        AcceptButton = ok;
        CancelButton = cancel;

        Controls.AddRange([label, _intervalInput, ok, cancel]);
    }

    private void Save()
    {
        _settings.RefreshIntervalSeconds = (int)_intervalInput.Value;
        _settings.Save();
    }
}
