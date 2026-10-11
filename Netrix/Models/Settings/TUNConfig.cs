namespace Netrix.Models;

/// <summary>
///     TUN/TAP adapter settings
/// </summary>
public class TUNConfig
{
    /// <summary>
    ///     Address
    /// </summary>
    public string Address { get; set; } = "10.0.236.10";

    /// <summary>
    ///     DNS
    /// </summary>
    public string DNS { get; set; } = Constants.DefaultPrimaryDNS;

    /// <summary>
    ///     Gateway
    /// </summary>
    public string Gateway { get; set; } = "10.0.236.1";

    /// <summary>
    ///     Netmask
    /// </summary>
    public string Netmask { get; set; } = "255.255.255.0";

    /// <summary>
    ///     Whether DNS goes through the proxy in mode 2
    /// </summary>
    public bool ProxyDNS { get; set; } = false;

    /// <summary>
    ///     Use custom DNS settings
    /// </summary>
    public bool UseCustomDNS { get; set; } = false;

    /// <summary>
    ///     Also send IPv6 traffic through the tunnel; otherwise IPv6 (YouTube, Google, ...) bypasses it
    /// </summary>
    public bool ProxyIPv6 { get; set; } = true;

    /// <summary>
    ///     Global bypass IPs
    /// </summary>
    public List<string> BypassIPs { get; set; } = new();
}