using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;
using Netrix.Controllers;
using Netrix.Models;
using Netrix.Models.Modes.WebMode;
using Netrix.Servers;

namespace Tests
{
    [TestClass]
    public class ControllerTests
    {
        // ---- Guard: supervises a child process ----

        private class FakeGuard : Guard
        {
            private readonly string[] _started;
            private readonly string[] _failed;

            public FakeGuard(string[] started, string[] failed) : base("cmd.exe")
            {
                _started = started;
                _failed = failed;
            }

            public override string Name => "FakeGuard";
            protected override IEnumerable<string> StartedKeywords => _started;
            protected override IEnumerable<string> FailedKeywords => _failed;
            public Task Start(string args) => StartGuardAsync(args);

            // the real one opens the log file in the user's editor
            protected override void OnStartFailed()
            {
            }

            // Instance is disposed after Stop, so remember the id while it runs
            public int Pid { get; private set; }

            protected override void OnReadNewLine(string line)
            {
                Pid = Instance.Id;
            }
        }

        [ClassInitialize]
        public static void Init(TestContext _)
        {
            // Guard resolves bin\<file> against the current directory, runs in NetrixDir\bin and logs to logging\
            Directory.CreateDirectory("bin");
            Directory.CreateDirectory("logging");
            Directory.CreateDirectory(Path.Combine(Netrix.Global.NetrixDir, "bin"));
            Directory.CreateDirectory(Path.Combine(Netrix.Global.NetrixDir, "data"));
            File.Copy(Path.Combine(Environment.SystemDirectory, "cmd.exe"), Path.Combine("bin", "cmd.exe"), true);
        }

        [TestMethod]
        public async Task Guard_StartedKeyword_ReturnsAndStopKillsChild()
        {
            var g = new FakeGuard(new[] { "READY" }, new string[0]);
            await g.Start("/c echo READY & ping -n 30 127.0.0.1 >nul");
            var pid = g.Pid;
            Assert.IsTrue(IsAlive(pid));

            await g.StopAsync();
            Assert.IsFalse(IsAlive(pid));
        }

        [TestMethod]
        public async Task Guard_FailedKeyword_Throws()
        {
            var g = new FakeGuard(new[] { "READY" }, new[] { "BOOM" });
            await Assert.ThrowsExactlyAsync<MessageException>(() => g.Start("/c echo BOOM & ping -n 30 127.0.0.1 >nul"));
            Assert.AreNotEqual(0, g.Pid);
            Assert.IsFalse(IsAlive(g.Pid));
        }

        private static bool IsAlive(int pid)
        {
            try
            {
                return !System.Diagnostics.Process.GetProcessById(pid).HasExited;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        [TestMethod]
        public async Task Guard_ExitBeforeStartedKeyword_FailsFast()
        {
            var g = new FakeGuard(new[] { "READY" }, new string[0]);
            var sw = System.Diagnostics.Stopwatch.StartNew();
            await Assert.ThrowsExactlyAsync<MessageException>(() => g.Start("/c echo hello & exit 3"));
            Assert.IsLessThan(10_000, sw.ElapsedMilliseconds); // used to wait the full 50 s timeout
        }

        // ---- WebController: Windows system proxy ----

        private const string TestRoot = @"Software\NetrixTest";
        private const string TestKey = TestRoot + @"\InternetSettings";

        [TestMethod]
        public async Task Web_StartSetsProxyAndStopRestoresPrevious()
        {
            WebController.KeyPath = TestKey;
            Registry.CurrentUser.DeleteSubKeyTree(TestRoot, false);
            using (var k = Registry.CurrentUser.CreateSubKey(TestKey))
            {
                k.SetValue("ProxyEnable", 0, RegistryValueKind.DWord);
                k.SetValue("ProxyServer", "old:1", RegistryValueKind.String);
                // ProxyOverride deliberately absent: must be absent again after stop
            }

            try
            {
                var c = new WebController();
                await c.StartAsync(new Socks5Server("127.0.0.1", 1080), new WebMode { Bypass = "<local>;10.*" });

                using (var k = Registry.CurrentUser.OpenSubKey(TestKey)!)
                {
                    Assert.AreEqual(1, k.GetValue("ProxyEnable"));
                    Assert.AreEqual("socks=127.0.0.1:1080", k.GetValue("ProxyServer"));
                    Assert.AreEqual("<local>;10.*", k.GetValue("ProxyOverride"));
                }

                await c.StopAsync();

                using (var k = Registry.CurrentUser.OpenSubKey(TestKey)!)
                {
                    Assert.AreEqual(0, k.GetValue("ProxyEnable"));
                    Assert.AreEqual("old:1", k.GetValue("ProxyServer"));
                    Assert.IsNull(k.GetValue("ProxyOverride"));
                }
            }
            finally
            {
                Registry.CurrentUser.DeleteSubKeyTree(TestRoot, false);
            }
        }

        [TestMethod]
        public async Task Web_CrashThenRecover_RestoresPrevious()
        {
            WebController.KeyPath = TestKey;
            Registry.CurrentUser.DeleteSubKeyTree(TestRoot, false);
            using (var k = Registry.CurrentUser.CreateSubKey(TestKey))
                k.SetValue("ProxyServer", "old:2", RegistryValueKind.String);

            try
            {
                await new WebController().StartAsync(new Socks5Server("127.0.0.1", 1080), new WebMode());
                // "crash": the controller is dropped without StopAsync
                WebController.RecoverAfterCrash();

                using var k = Registry.CurrentUser.OpenSubKey(TestKey)!;
                Assert.AreEqual("old:2", k.GetValue("ProxyServer"));
                Assert.IsNull(k.GetValue("ProxyEnable"));
            }
            finally
            {
                Registry.CurrentUser.DeleteSubKeyTree(TestRoot, false);
            }
        }

        // ---- V2ray config generation ----

        private static async Task<JsonElement> Config(Server s)
        {
            var cfg = await V2rayConfigUtils.GenerateClientConfigAsync(s);
            return JsonSerializer.SerializeToElement(cfg, Netrix.Global.NewCustomJsonSerializerOptions());
        }

        [TestMethod]
        public async Task V2ray_InboundIsLocalSocks()
        {
            var c = await Config(new Socks5Server("127.0.0.1", 1080));
            var inbound = c.GetProperty("inbounds")[0];
            Assert.AreEqual("socks", inbound.GetProperty("protocol").GetString());
            Assert.AreEqual(Netrix.Global.Settings.Socks5LocalPort, inbound.GetProperty("port").GetInt32());
        }

        [TestMethod]
        public async Task V2ray_OutboundProtocolPerServerType()
        {
            (Server server, string protocol)[] cases =
            {
                (new Socks5Server("127.0.0.1", 1080), "socks"),
                (new ShadowsocksServer { Hostname = "127.0.0.1", Port = 8388, EncryptMethod = "aes-256-gcm", Password = "p" }, "shadowsocks"),
                (new ShadowsocksRServer { Hostname = "127.0.0.1", Port = 8388, Password = "p" }, "shadowsocks"),
                (new TrojanServer { Hostname = "127.0.0.1", Port = 443, Password = "p" }, "trojan"),
                (new VMessServer { Hostname = "127.0.0.1", Port = 443, UserID = "feb54431-301b-52bb-a6dd-e1e93e81bb9e" }, "vmess"),
                (new VLESSServer { Hostname = "127.0.0.1", Port = 443, UserID = "feb54431-301b-52bb-a6dd-e1e93e81bb9e" }, "vless"),
                (new WireGuardServer { Hostname = "127.0.0.1", Port = 51820, PrivateKey = "k", PeerPublicKey = "p" }, "wireguard"),
            };

            foreach (var (server, protocol) in cases)
            {
                var c = await Config(server);
                Assert.AreEqual(protocol, c.GetProperty("outbounds")[0].GetProperty("protocol").GetString(), server.Type);
            }
        }

        [TestMethod]
        public async Task V2ray_Socks5Auth_OnlyWhenCredentialsSet()
        {
            var withAuth = await Config(new Socks5Server("127.0.0.1", 1080, "u", "p"));
            var user = withAuth.GetProperty("outbounds")[0].GetProperty("settings").GetProperty("servers")[0].GetProperty("users")[0];
            Assert.AreEqual("u", user.GetProperty("user").GetString());
            Assert.AreEqual("p", user.GetProperty("pass").GetString());

            var noAuth = await Config(new Socks5Server("127.0.0.1", 1080));
            Assert.IsFalse(noAuth.GetProperty("outbounds")[0].GetProperty("settings").GetProperty("servers")[0].TryGetProperty("users", out _));
        }

        [TestMethod]
        public async Task V2ray_VMessAddressAndPort()
        {
            var c = await Config(new VMessServer { Hostname = "127.0.0.1", Port = 4443, UserID = "feb54431-301b-52bb-a6dd-e1e93e81bb9e" });
            var vnext = c.GetProperty("outbounds")[0].GetProperty("settings").GetProperty("vnext")[0];
            Assert.AreEqual("127.0.0.1", vnext.GetProperty("address").GetString());
            Assert.AreEqual(4443, vnext.GetProperty("port").GetInt32());
        }
    }
}
