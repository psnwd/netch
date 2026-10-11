using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Netch.Servers;
using Netch.Utils;

namespace Tests
{
    [TestClass]
    public class IPv6ProbeTests
    {
        // fake SOCKS5 server: accepts CONNECT, then either answers the HTTP request or stays silent (like a server without IPv6 egress)
        private static ushort Serve(bool answer)
        {
            var l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            _ = Task.Run(async () =>
            {
                using var c = await l.AcceptTcpClientAsync();
                var s = c.GetStream();
                var b = new byte[64];
                await s.ReadAsync(b, 0, 3); // greeting
                await s.WriteAsync(new byte[] { 5, 0 });
                await s.ReadAsync(b, 0, 22); // CONNECT to an IPv6 address
                await s.WriteAsync(new byte[] { 5, 0, 0, 1, 0, 0, 0, 0, 0, 0 });
                await s.ReadAsync(b, 0, b.Length); // HEAD request
                if (answer)
                    await s.WriteAsync(new byte[] { (byte)'H', (byte)'T', (byte)'T', (byte)'P' });
                else
                    await Task.Delay(8000);
                l.Stop();
            });
            return (ushort)((IPEndPoint)l.LocalEndpoint).Port;
        }

        [TestMethod]
        public async Task Works_when_data_comes_back()
        {
            Assert.IsTrue(await Socks5ServerTestUtils.IPv6WorksAsync(new Socks5Server("127.0.0.1", Serve(true))));
        }

        [TestMethod]
        public async Task Fails_when_connection_stalls()
        {
            Assert.IsFalse(await Socks5ServerTestUtils.IPv6WorksAsync(new Socks5Server("127.0.0.1", Serve(false))));
        }
    }
}
