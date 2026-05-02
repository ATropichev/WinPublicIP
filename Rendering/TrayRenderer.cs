using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;

namespace WinPublicIP.Rendering;

public static class TrayRenderer
{
    private static readonly Assembly Asm = Assembly.GetExecutingAssembly();
    private static Icon? _lastIcon;

    public static Icon Render(string countryCode, bool vpnOn)
    {
        var flag = FlagLoader.Load(countryCode);
        var shield = LoadResource(vpnOn ? "shield_on.png" : "shield_off.png");
        return Compose(flag, shield);
    }

    public static Icon RenderOffline()
    {
        var offline = LoadResource("offline.png");
        return Compose(offline, null);
    }

    private static Icon Compose(Bitmap background, Bitmap? overlay)
    {
        using var bmp = new Bitmap(32, 32);
        using var g = Graphics.FromImage(bmp);
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.DrawImage(background, 0, 0, 32, 32);

        if (overlay is not null)
            g.DrawImage(overlay, 16, 16, 16, 16);

        var handle = bmp.GetHicon();
        var icon = Icon.FromHandle(handle);

        // Клонируем иконку и освобождаем хэндл GDI, чтобы не было утечки
        var result = (Icon)icon.Clone();
        icon.Dispose();
        DestroyIcon(handle);

        _lastIcon?.Dispose();
        _lastIcon = result;
        return result;
    }

    private static Bitmap LoadResource(string filename)
    {
        var name = $"WinPublicIP.Resources.{filename}";
        using var stream = Asm.GetManifestResourceStream(name);
        if (stream is null) return CreatePlaceholder();
        return new Bitmap(stream);
    }

    private static Bitmap CreatePlaceholder()
    {
        var bmp = new Bitmap(32, 32);
        using var g = Graphics.FromImage(bmp);
        g.Clear(Color.DimGray);
        return bmp;
    }

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern bool DestroyIcon(IntPtr handle);
}
