using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Netrix.Interfaces;
using Netrix.Interops;
using Netrix.Models;
using Netrix.Models.Modes;
using Netrix.Models.Modes.TunMode;
using Netrix.Servers;
using Netrix.Utils;
using static Netrix.Interops.tun2socks;

namespace Netrix.Controllers
{
    public class TUNController : IModeController
    {
        private readonly DNSController _aioDnsController = new();

        private TunMode _mode = null!;
        private IPAddress? _serverRemoteAddress;
        private TUNConfig _tunConfig = null!;

        private NetRoute _tun;
        private NetRoute _outbound;

        // unique local address for the adapter, only used as the source of the IPv6 default route
        private const string TunAddressV6 = "fd00:236::10";

        public string Name => "tun2socks";
        public string interfaceName => "netrix";

        public ModeFeature Features => ModeFeature.SupportSocks5Auth;

        public async Task StartAsync(Socks5Server server, Mode mode)
        {
            if (mode is not TunMode tunMode)
                throw new InvalidOperationException();

            _mode = tunMode;
            _tunConfig = Global.Settings.TUNTAP;

            if (server.RemoteHostname.ValueOrDefault() != null)
                _serverRemoteAddress = await DnsUtils.LookupAsync(server.RemoteHostname!);
            else
                _serverRemoteAddress = await DnsUtils.LookupAsync(server.Hostname);

            if (_serverRemoteAddress != null && IPAddress.IsLoopback(_serverRemoteAddress))
                _serverRemoteAddress = null;

            _outbound = NetRoute.GetBestRouteTemplate();
            CheckDriver();

            Dial(NameList.TYPE_ADAPMTU, "1500");
            Dial(NameList.TYPE_BYPBIND, _outbound.Gateway);
            Dial(NameList.TYPE_BYPLIST, "disabled");

            #region Server

            Dial(NameList.TYPE_TCPREST, "");
            Dial(NameList.TYPE_TCPTYPE, "Socks5");

            Dial(NameList.TYPE_UDPREST, "");
            Dial(NameList.TYPE_UDPTYPE, "Socks5");

            Dial(NameList.TYPE_TCPHOST, $"{await server.AutoResolveHostnameAsync()}:{server.Port}");

            Dial(NameList.TYPE_UDPHOST, $"{await server.AutoResolveHostnameAsync()}:{server.Port}");

            if (server.Auth())
            {
                Dial(NameList.TYPE_TCPUSER, server.Username!);
                Dial(NameList.TYPE_TCPPASS, server.Password!);

                Dial(NameList.TYPE_UDPUSER, server.Username!);
                Dial(NameList.TYPE_UDPPASS, server.Password!);
            }

            #endregion

            #region DNS

            if (_tunConfig.UseCustomDNS)
            {
                Dial(NameList.TYPE_DNSADDR, DnsUtils.AppendPort(_tunConfig.DNS));
            }
            else
            {
                await _aioDnsController.StartAsync();
                Dial(NameList.TYPE_DNSADDR, $"127.0.0.1:{Global.Settings.AioDNS.ListenPort}");
            }

            #endregion

            // IPv6 enters the tunnel (SetupIPv6). A server without IPv6 egress accepts those connections and then stalls, so
            // dual-stack sites hang: have tun2socks reject them instead, clients fall back to IPv4 at once and nothing leaks
            var blockIPv6 = ProxyIPv6Routed && !await Socks5ServerTestUtils.IPv6WorksAsync(server);
            if (blockIPv6)
                Log.Warning("The server has no working IPv6 path, IPv6 connections are rejected so clients use IPv4");

            Dial(NameList.TYPE_BLOCKIPV6, blockIPv6 ? "1" : "0");

            if (!Init())
                throw new MessageException("tun2socks start failed.");

            var tunIndex = (int)RouteHelper.ConvertLuidToIndex(tun_luid());
            _tun = NetRoute.TemplateBuilder(_tunConfig.Gateway, tunIndex);

            RouteHelper.CreateUnicastIP(AddressFamily.InterNetwork,
                _tunConfig.Address,
                (byte)Utils.Utils.SubnetToCidr(_tunConfig.Netmask),
                (ulong)tunIndex);

            var routeTimer = System.Diagnostics.Stopwatch.StartNew();
            SetupRouteTable();
            Log.Information("Route table: {Count} rules in {Ms} ms", _mode.Handle.Count() + _mode.Bypass.Count(), routeTimer.ElapsedMilliseconds);
            SetupIPv6(tunIndex);
        }

        // the bypass route for the server is IPv4 only, an IPv6 server would loop back into the tunnel
        private bool ProxyIPv6Routed => _tunConfig.ProxyIPv6 && _serverRemoteAddress?.AddressFamily != AddressFamily.InterNetworkV6;

        /// <summary>
        ///     Without this IPv6 traffic keeps using the physical adapter and never enters the tunnel
        /// </summary>
        private void SetupIPv6(int tunIndex)
        {
            if (!ProxyIPv6Routed)
                return;

            if (!RouteHelper.CreateUnicastIP(AddressFamily.InterNetworkV6, TunAddressV6, 64, (ulong)tunIndex))
            {
                Log.Warning("Failed to add IPv6 address to the tunnel adapter, IPv6 traffic stays outside the tunnel");
                return;
            }

            // empty gateway = on-link, like a VPN adapter without neighbor discovery
            if (!RouteUtils.CreateRoute(NetRoute.TemplateBuilder(string.Empty, tunIndex).FillTemplate("::", 0), AddressFamily.InterNetworkV6))
                Log.Warning("Failed to add the IPv6 default route, IPv6 traffic stays outside the tunnel");
        }

        public async Task StopAsync()
        {
            var tasks = new[]
            {
                FreeAsync(),
                Task.Run(ClearRouteTable),
                _aioDnsController.StopAsync()
            };

            await Task.WhenAll(tasks);
        }

        private void CheckDriver()
        {
            string binDriver = Path.Combine(Global.NetrixDir, Constants.WintunDllFile);
            string sysDriver = $@"{Environment.SystemDirectory}\wintun.dll";

            // File.Copy keeps the write time, so same size + time means this file was installed earlier
            // ponytail: a different file with identical size and time goes undetected; drop the shortcut if that matters
            FileInfo bin = new(binDriver), sys = new(sysDriver);
            if (sys.Exists && bin.Length == sys.Length && bin.LastWriteTimeUtc == sys.LastWriteTimeUtc)
                return;

            var binHash = Utils.Utils.Sha256CheckSumAsync(binDriver).Result;
            var sysHash = Utils.Utils.Sha256CheckSumAsync(sysDriver).Result;
            Log.Information("Built-in  wintun.dll Hash: {Hash}", binHash);
            Log.Information("Installed wintun.dll Hash: {Hash}", sysHash);
            if (binHash == sysHash)
                return;

            try
            {
                Log.Information("Copy wintun.dll to System Directory");
                File.Copy(binDriver, sysDriver, true);
            }
            catch (Exception e)
            {
                Log.Error(e, "Copy wintun.dll failed");
                throw new MessageException($"Failed to copy wintun.dll to system directory: {e.Message}");
            }
        }

        #region Route

        private void SetupRouteTable()
        {
            Global.MainForm.StatusText(i18N.Translate("Setup Route Table Rule"));

            var tunNetworkInterface = NetworkInterfaceUtils.Get(_tun.InterfaceIndex);
            // Server Address
            if (_serverRemoteAddress != null)
                RouteUtils.CreateRoute(_outbound.FillTemplate(_serverRemoteAddress.ToString(), 32));

            // Global Bypass IPs
            RouteUtils.CreateRouteFill(_outbound, _tunConfig.BypassIPs);

            // rule
            RouteUtils.CreateRouteFill(_tun, _mode.Handle);
            RouteUtils.CreateRouteFill(_outbound, _mode.Bypass);

            // dns

            if (_tunConfig.UseCustomDNS)
            {
                if (_tunConfig.ProxyDNS)
                {
                    // NOTICE: DNS metric is network interface metric
                    RouteUtils.CreateRoute(_tun.FillTemplate(_tunConfig.DNS, 32));
                }

                tunNetworkInterface.SetDns(_tunConfig.DNS);
            }
            else
            {
                RouteUtils.CreateRoute(_outbound.FillTemplate(Utils.Utils.GetHostFromUri(Global.Settings.AioDNS.ChinaDNS), 32));
                RouteUtils.CreateRoute(_tun.FillTemplate(Utils.Utils.GetHostFromUri(Global.Settings.AioDNS.OtherDNS), 32));
            }

            NetworkInterfaceUtils.SetInterfaceMetric(_tun.InterfaceIndex, 0);
        }

        /// <summary>
        ///     Every step is independent: when the PC lost its network the physical adapter may be gone, and one failing step
        ///     (e.g. resetting the metric of a vanished interface) must not skip the rest or break Stop.
        /// </summary>
        private void ClearRouteTable()
        {
            if (_serverRemoteAddress != null)
                Try(() => RouteUtils.DeleteRoute(_outbound.FillTemplate(_serverRemoteAddress.ToString(), 32)));

            if (_outbound.Gateway != null)
            {
                Try(() => RouteUtils.DeleteRouteFill(_outbound, Global.Settings.TUNTAP.BypassIPs));
                Try(() => RouteUtils.DeleteRoute(_outbound.FillTemplate(Utils.Utils.GetHostFromUri(Global.Settings.AioDNS.ChinaDNS), 32)));
                Try(() => NetworkInterfaceUtils.SetInterfaceMetric(_outbound.InterfaceIndex));
            }

            if (_mode != null)
                Try(() => RouteUtils.DeleteRouteFill(_outbound, _mode.Bypass));
        }

        private static void Try(Action step)
        {
            try
            {
                step();
            }
            catch (Exception e)
            {
                Log.Warning(e, "Route cleanup step failed, continuing");
            }
        }

        #endregion
    }
}