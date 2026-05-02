using System.Drawing;
using System.Reflection;

namespace WinPublicIP.Rendering;

public static class FlagLoader
{
    private static readonly Assembly Asm = Assembly.GetExecutingAssembly();
    private static readonly string Prefix = "WinPublicIP.Resources.Flags.";
    private static readonly Dictionary<string, Bitmap> Cache = new();

    public static Bitmap Load(string countryCode)
    {
        var code = countryCode.ToLowerInvariant();
        if (Cache.TryGetValue(code, out var cached)) return cached;

        var bmp = LoadFromResource(code) ?? LoadFromResource("xx") ?? CreateFallback();
        Cache[code] = bmp;
        return bmp;
    }

    private static Bitmap? LoadFromResource(string code)
    {
        var name = $"{Prefix}{code}.png";
        using var stream = Asm.GetManifestResourceStream(name);
        if (stream is null) return null;
        return new Bitmap(stream);
    }

    private static Bitmap CreateFallback()
    {
        var bmp = new Bitmap(32, 32);
        using var g = Graphics.FromImage(bmp);
        g.Clear(Color.Gray);
        return bmp;
    }
}
