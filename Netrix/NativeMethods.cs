using System.Runtime.InteropServices;

namespace Netrix;

public static class NativeMethods
{
    [DllImport("dnsapi", EntryPoint = "DnsFlushResolverCache")]
    public static extern uint RefreshDNSCache();
}