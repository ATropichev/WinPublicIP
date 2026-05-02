using System.Net.NetworkInformation;
using WinPublicIP.Config;

namespace WinPublicIP.Services;

public sealed class VpnDetector
{
    private readonly string[] _patterns;

    public VpnDetector(AppSettings settings)
    {
        _patterns = settings.VpnInterfacePatterns;
    }

    public bool IsActive()
    {
        foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (ni.OperationalStatus != OperationalStatus.Up) continue;

            var descriptor = $"{ni.Name} {ni.Description}";
            foreach (var pattern in _patterns)
            {
                if (descriptor.Contains(pattern, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
        }
        return false;
    }
}
