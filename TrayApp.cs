using WinPublicIP.Config;
using WinPublicIP.Rendering;
using WinPublicIP.Services;

namespace WinPublicIP;

public sealed class TrayApp : ApplicationContext
{
    private readonly NotifyIcon _notifyIcon;
    private readonly System.Windows.Forms.Timer _timer;
    private readonly AppSettings _settings;
    private readonly IpProvider _ipProvider;
    private readonly GeoProvider _geoProvider;
    private readonly VpnDetector _vpnDetector;

    private ToolStripMenuItem _ipMenuItem = null!;
    private string? _lastIp;

    public TrayApp()
    {
        _settings = AppSettings.Load();
        _ipProvider = new IpProvider(_settings);
        _geoProvider = new GeoProvider();
        _vpnDetector = new VpnDetector(_settings);

        _notifyIcon = new NotifyIcon
        {
            Visible = true,
            Text = "WinPublicIP — запуск...",
            Icon = TrayRenderer.RenderOffline(),
            ContextMenuStrip = BuildMenu()
        };

        _timer = new System.Windows.Forms.Timer
        {
            Interval = _settings.RefreshIntervalSeconds * 1000
        };
        _timer.Tick += async (_, _) => await RefreshAsync();
        _timer.Start();

        _ = RefreshAsync();
    }

    private ContextMenuStrip BuildMenu()
    {
        var menu = new ContextMenuStrip();

        _ipMenuItem = new ToolStripMenuItem("IP: —") { Enabled = false };
        menu.Items.Add(_ipMenuItem);
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add("Обновить сейчас", null, async (_, _) => await RefreshAsync());
        menu.Items.Add("Копировать IP", null, (_, _) =>
        {
            if (_lastIp is not null) Clipboard.SetText(_lastIp);
        });
        menu.Items.Add(new ToolStripSeparator());

        var autorun = new ToolStripMenuItem("Запускать с Windows")
        {
            Checked = _settings.StartWithWindows
        };
        autorun.Click += (_, _) =>
        {
            _settings.StartWithWindows = !_settings.StartWithWindows;
            autorun.Checked = _settings.StartWithWindows;
            _settings.SetAutorun(_settings.StartWithWindows);
            _settings.Save();
        };
        menu.Items.Add(autorun);
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add("Настройки...", null, (_, _) => OpenSettings());
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add("Выход", null, (_, _) => ExitThread());

        return menu;
    }

    private void OpenSettings()
    {
        using var form = new SettingsForm(_settings);
        if (form.ShowDialog() == DialogResult.OK)
            _timer.Interval = _settings.RefreshIntervalSeconds * 1000;
    }

    private async Task RefreshAsync()
    {
        try
        {
            var ip = await _ipProvider.GetAsync();
            var geo = await _geoProvider.GetAsync(ip);
            var vpnOn = _vpnDetector.IsActive();

            if (_settings.NotifyOnIpChange && _lastIp is not null && ip != _lastIp)
                _notifyIcon.ShowBalloonTip(3000, "IP изменился", ip, ToolTipIcon.Info);
            var icon = TrayRenderer.Render(geo.CountryCode, vpnOn);

            var vpnText = vpnOn ? "on" : "off";
            var tooltip = Truncate($"IP: {ip}\n{geo.Country}\nVPN: {vpnText}", 63);

            _notifyIcon.Icon = icon;
            _notifyIcon.Text = tooltip;
            _ipMenuItem.Text = $"IP: {ip}";
            _lastIp = ip;
        }
        catch (HttpRequestException)
        {
            _notifyIcon.Icon = TrayRenderer.RenderOffline();
            _notifyIcon.Text = "Нет подключения";
            _ipMenuItem.Text = "IP: недоступен";
        }
        catch (Exception ex)
        {
            _notifyIcon.Text = Truncate($"Ошибка: {ex.Message}", 63);
        }
    }

    private static string Truncate(string s, int max) =>
        s.Length <= max ? s : s[..max];

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _timer.Stop();
            _timer.Dispose();
            _notifyIcon.Visible = false;
            _notifyIcon.Dispose();
        }
        base.Dispose(disposing);
    }
}
