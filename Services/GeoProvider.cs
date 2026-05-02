using System.Text.Json;

namespace WinPublicIP.Services;

public sealed record GeoResult(string Country, string CountryCode, string Isp, string Query);

public sealed class GeoProvider
{
    private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(5) };
    private const string Endpoint =
        "http://ip-api.com/json/{0}?fields=country,countryCode,isp,query";

    private readonly Dictionary<string, GeoResult> _cache = new();

    public GeoResult? LastResult { get; private set; }

    public async Task<GeoResult> GetAsync(string ip)
    {
        if (_cache.TryGetValue(ip, out var cached))
        {
            LastResult = cached;
            return cached;
        }

        var url = string.Format(Endpoint, ip);
        var body = await Http.GetStringAsync(url);
        using var doc = JsonDocument.Parse(body);
        var root = doc.RootElement;

        var result = new GeoResult(
            Country: root.GetProperty("country").GetString() ?? "Unknown",
            CountryCode: root.GetProperty("countryCode").GetString()?.ToLowerInvariant() ?? "xx",
            Isp: root.GetProperty("isp").GetString() ?? string.Empty,
            Query: root.GetProperty("query").GetString() ?? ip
        );

        _cache[ip] = result;
        LastResult = result;
        return result;
    }
}
