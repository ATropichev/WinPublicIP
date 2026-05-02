using System.Text.Json;
using WinPublicIP.Config;

namespace WinPublicIP.Services;

public sealed class IpProvider
{
    // PooledConnectionLifetime=1s гарантирует свежее соединение на каждый запрос,
    // иначе при переключении VPN пул отдаёт старое соединение мимо VPN-туннеля.
    private static readonly HttpClient Http = new(
        new SocketsHttpHandler { PooledConnectionLifetime = TimeSpan.FromSeconds(1) })
    { Timeout = TimeSpan.FromSeconds(5) };
    private readonly string[] _providers;

    public IpProvider(AppSettings settings)
    {
        _providers = settings.IpProviders;
    }

    public async Task<string> GetAsync()
    {
        foreach (var url in _providers)
        {
            try
            {
                var body = await Http.GetStringAsync(url);
                var ip = ParseIp(body);
                if (!string.IsNullOrWhiteSpace(ip)) return ip.Trim();
            }
            catch { }
        }
        throw new HttpRequestException("Все источники IP недоступны.");
    }

    private static string ParseIp(string body)
    {
        body = body.Trim();
        if (body.StartsWith('{'))
        {
            using var doc = JsonDocument.Parse(body);
            if (doc.RootElement.TryGetProperty("ip", out var prop))
                return prop.GetString() ?? string.Empty;
        }
        return body;
    }
}
