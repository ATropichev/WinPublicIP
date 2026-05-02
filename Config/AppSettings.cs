using Microsoft.Win32;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WinPublicIP.Config;

public sealed class AppSettings
{
    private static readonly string ConfigPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "WinPublicIP", "config.json");

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public int RefreshIntervalSeconds { get; set; } = 60;
    public string HomeCountry { get; set; } = "RU";
    public string[] IpProviders { get; set; } =
    [
        "https://api.ipify.org?format=json",
        "https://ifconfig.me/ip",
        "https://icanhazip.com"
    ];
    public string GeoProvider { get; set; } =
        "http://ip-api.com/json/{ip}?fields=country,countryCode,isp,query";
    public string[] VpnInterfacePatterns { get; set; } =
        ["TAP", "tun", "WireGuard", "OpenVPN", "Tailscale", "ZeroTier", "PPP", "WAN Miniport (IKEv2)"];
    public bool NotifyOnIpChange { get; set; } = true;
    public bool StartWithWindows { get; set; } = false;

    public static AppSettings Load()
    {
        try
        {
            if (File.Exists(ConfigPath))
            {
                var json = File.ReadAllText(ConfigPath);
                return JsonSerializer.Deserialize<AppSettings>(json, JsonOptions) ?? new AppSettings();
            }
        }
        catch { }

        var defaults = new AppSettings();
        defaults.Save();
        return defaults;
    }

    public void Save()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath)!);
            File.WriteAllText(ConfigPath, JsonSerializer.Serialize(this, JsonOptions));
        }
        catch { }
    }

    public void SetAutorun(bool enable)
    {
        const string keyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
        const string valueName = "WinPublicIP";

        using var key = Registry.CurrentUser.OpenSubKey(keyPath, writable: true);
        if (key is null) return;

        if (enable)
            key.SetValue(valueName, $"\"{Application.ExecutablePath}\"");
        else
            key.DeleteValue(valueName, throwOnMissingValue: false);
    }
}
