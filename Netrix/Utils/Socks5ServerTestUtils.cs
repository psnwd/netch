using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Netrix.Models;
using Netrix.Servers;
using Serilog;
using Socks5.Models;
using STUN.Client;
using STUN.Enums;
using STUN.Proxy;
using STUN.StunResult;

namespace Netrix.Utils;

public static class Socks5ServerTestUtils
{
    public static async Task<NatTypeTestResult> DiscoveryNatTypeAsync(Socks5Server socks5, CancellationToken ctx = default)
    {
        var stunServer = Global.Settings.STUN_Server;
        var port = (ushort)Global.Settings.STUN_Server_Port;
        var local = new IPEndPoint(IPAddress.Any, 0);

        var socks5Option = new Socks5CreateOption
        {
            Address = await DnsUtils.LookupAsync(socks5.Hostname),
            Port = socks5.Port,
            UsernamePassword = new UsernamePassword
            {
                UserName = socks5.Username,
                Password = socks5.Password
            }
        };

        var ip = await DnsUtils.LookupAsync(stunServer);
        if (ip == null)
        {
            return new NatTypeTestResult { Result = "Wrong STUN Server!" };
        }

        using IUdpProxy proxy = new Socks5UdpProxy(new IPEndPoint(IPAddress.Loopback, 0), socks5Option);
        using var client = new StunClient5389UDP(new IPEndPoint(ip, port), local, proxy);

        await client.ConnectProxyAsync(ctx);
        try
        {
            await client.QueryAsync(ctx);
        }
        finally
        {
            await client.CloseProxyAsync(ctx);
        }

        var res = client.State;
        var result = GetSimpleResult(res);

        return new NatTypeTestResult
        {
            Result = result,
            LocalEnd = res.LocalEndPoint?.ToString(),
            PublicEnd = res.PublicEndPoint?.ToString()
        };
    }

    private static string GetSimpleResult(StunResult5389 res)
    {
        switch (res.BindingTestResult, res.MappingBehavior, res.FilteringBehavior)
        {
            case (BindingTestResult.Fail, _, _):
                return "NoUDP";
            case (not BindingTestResult.Success, _, _):
                return res.BindingTestResult.ToString();
            case (_, MappingBehavior.Direct or MappingBehavior.EndpointIndependent, FilteringBehavior.EndpointIndependent):
                return "1";
            case (_, MappingBehavior.Direct or MappingBehavior.EndpointIndependent, _):
                return "2";
            case (_, MappingBehavior.AddressDependent or MappingBehavior.AddressAndPortDependent, _):
                return "3";
            case (_, MappingBehavior.Fail, _):
                return MappingBehavior.Fail.ToString();
            default:
                return res.FilteringBehavior.ToString();
        }
    }

    /// <summary>
    ///     True if the SOCKS5 server can reach an IPv6 host and data comes back. A server without IPv6 egress accepts the
    ///     connection and then stalls, so routing IPv6 into the tunnel would hang every dual-stack site.
    /// </summary>
    public static async Task<bool> IPv6WorksAsync(Socks5Server socks5, CancellationToken ctx = default)
    {
        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ctx);
            cts.CancelAfter(TimeSpan.FromSeconds(5));
            var token = cts.Token;

            using var tcp = new TcpClient();
            await tcp.ConnectAsync(await DnsUtils.LookupAsync(socks5.Hostname) ?? throw new InvalidOperationException(), socks5.Port, token);
            var s = tcp.GetStream();
            var buf = new byte[262];

            // greeting: no auth, or username/password when the server has it
            await s.WriteAsync(socks5.Auth() ? new byte[] { 5, 1, 2 } : new byte[] { 5, 1, 0 }, token);
            await s.ReadExactlyAsync(buf.AsMemory(0, 2), token);
            if (buf[0] != 5 || buf[1] == 0xFF)
                return false;

            if (buf[1] == 2)
            {
                var user = Encoding.UTF8.GetBytes(socks5.Username ?? "");
                var pass = Encoding.UTF8.GetBytes(socks5.Password ?? "");
                await s.WriteAsync(new byte[] { 1, (byte)user.Length }.Concat(user).Append((byte)pass.Length).Concat(pass).ToArray(), token);
                await s.ReadExactlyAsync(buf.AsMemory(0, 2), token);
                if (buf[1] != 0)
                    return false;
            }

            // CONNECT [2606:4700:4700::1111]:80 (Cloudflare), then ask for headers
            var connect = new byte[] { 5, 1, 0, 4 }.Concat(IPAddress.Parse("2606:4700:4700::1111").GetAddressBytes()).Concat(new byte[] { 0, 80 }).ToArray();
            await s.WriteAsync(connect, token);
            await s.ReadExactlyAsync(buf.AsMemory(0, 10), token); // reply header, IPv4 bound address
            if (buf[1] != 0)
                return false;

            if (buf[3] == 4)
                await s.ReadExactlyAsync(buf.AsMemory(0, 12), token); // IPv6 bound address is 12 bytes longer

            await s.WriteAsync(Encoding.ASCII.GetBytes("HEAD / HTTP/1.0\r\nHost: one.one.one.one\r\n\r\n"), token);
            return await s.ReadAsync(buf.AsMemory(), token) > 0;
        }
        catch (Exception e) when (e is not OutOfMemoryException)
        {
            Log.Debug(e, "IPv6 probe failed");
            return false;
        }
    }

    public static async Task<int?> HttpConnectAsync(Socks5Server socks5, CancellationToken ctx)
    {
        var socks5Option = new Socks5CreateOption
        {
            Address = await DnsUtils.LookupAsync(socks5.Hostname),
            Port = socks5.Port,
            UsernamePassword = new UsernamePassword
            {
                UserName = socks5.Username,
                Password = socks5.Password
            }
        };

        var stopwatch = Stopwatch.StartNew();

        var result = await Socks5.Utils.Socks5TestUtils.Socks5ConnectAsync(socks5Option, token: ctx);

        stopwatch.Stop();
        if (result)
            return Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds);

        return null;
    }
}