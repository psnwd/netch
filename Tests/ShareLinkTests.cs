using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Netch.Servers;
using Netch.Utils;

namespace Tests
{
    [TestClass]
    public class ShareLinkTests
    {
        [TestMethod]
        public void Trojan_RoundTrip()
        {
            var s = new TrojanServer { Hostname = "example.com", Port = 443, Password = "p@ss", Host = "sni.example.com", Remark = "r1" };
            var back = (TrojanServer)ShareLink.ParseText(ShareLink.GetShareLink(s)).Single();
            Assert.AreEqual(s.Hostname, back.Hostname);
            Assert.AreEqual(s.Port, back.Port);
            Assert.AreEqual(s.Password, back.Password);
            Assert.AreEqual(s.Host, back.Host);
            Assert.AreEqual(s.Remark, back.Remark);
        }

        [TestMethod]
        public void Shadowsocks_RoundTrip()
        {
            var s = new ShadowsocksServer { Hostname = "1.2.3.4", Port = 8388, EncryptMethod = "aes-256-gcm", Password = "pw", Remark = "r2" };
            var back = (ShadowsocksServer)ShareLink.ParseText(ShareLink.GetShareLink(s)).Single();
            Assert.AreEqual(s.Hostname, back.Hostname);
            Assert.AreEqual(s.Port, back.Port);
            Assert.AreEqual(s.EncryptMethod, back.EncryptMethod);
            Assert.AreEqual(s.Password, back.Password);
            Assert.AreEqual(s.Remark, back.Remark);
        }

        static void RoundTrip<T>(T s, params Func<T, object>[] props) where T : Netch.Models.Server
        {
            var back = (T)ShareLink.ParseText(ShareLink.GetShareLink(s)).Single();
            Assert.AreEqual(s.Hostname, back.Hostname);
            Assert.AreEqual(s.Port, back.Port);
            Assert.AreEqual(s.Remark, back.Remark);
            foreach (var p in props)
                Assert.AreEqual(p(s), p(back));
        }

        [TestMethod]
        public void SSR_RoundTrip() =>
            RoundTrip(new ShadowsocksRServer { Hostname = "h.example", Port = 1234, Password = "pw", Protocol = "auth_aes128_md5", OBFS = "tls1.2_ticket_auth", Remark = "r3", ProtocolParam = "pp", OBFSParam = "op" },
                s => s.Password, s => s.EncryptMethod, s => s.Protocol, s => s.OBFS, s => s.ProtocolParam, s => s.OBFSParam);

        [TestMethod]
        public void VMess_RoundTrip() =>
            RoundTrip(new VMessServer { Hostname = "h.example", Port = 443, UserID = "feb54431-301b-52bb-a6dd-e1e93e81bb9e", Remark = "r4" },
                s => s.UserID, s => s.AlterID, s => s.EncryptMethod, s => s.TransferProtocol);

        [TestMethod]
        public void VLESS_RoundTrip() =>
            RoundTrip(new VLESSServer { Hostname = "h.example", Port = 443, UserID = "feb54431-301b-52bb-a6dd-e1e93e81bb9e", Remark = "r5" },
                s => s.UserID, s => s.EncryptMethod, s => s.TransferProtocol);

        [TestMethod]
        public void Socks5_RoundTrip() =>
            RoundTrip(new Socks5Server { Hostname = "127.0.0.1", Port = 1080, Username = "u", Password = "p" },
                s => s.Username, s => s.Password);

        [TestMethod]
        public void WireGuard_RoundTrip() =>
            RoundTrip(new WireGuardServer { Hostname = "h.example", Port = 51820, PrivateKey = "a+b/c=", PeerPublicKey = "d+e/f=", PreSharedKey = "g+h/i=", Remark = "r7" },
                s => s.PrivateKey, s => s.PeerPublicKey, s => s.PreSharedKey, s => s.LocalAddresses, s => s.MTU);
    }
}
