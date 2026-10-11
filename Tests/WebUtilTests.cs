using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Netrix.Utils;

namespace Tests
{
    [TestClass]
    public class WebUtilTests
    {
        private static (HttpListener, string) Serve(Action<HttpListenerContext> handler)
        {
            var port = new Random().Next(20000, 40000);
            var l = new HttpListener();
            l.Prefixes.Add($"http://127.0.0.1:{port}/");
            l.Start();
            _ = Task.Run(async () =>
            {
                while (l.IsListening)
                {
                    try { handler(await l.GetContextAsync()); } catch { }
                }
            });
            return (l, $"http://127.0.0.1:{port}/");
        }

        [TestMethod]
        public async Task String_SendsUserAgentAndReturnsBody()
        {
            var (l, url) = Serve(c =>
            {
                var b = System.Text.Encoding.UTF8.GetBytes(c.Request.UserAgent ?? "");
                c.Response.OutputStream.Write(b, 0, b.Length);
                c.Response.Close();
            });
            try
            {
                var (code, body) = await WebUtil.DownloadStringAsync(WebUtil.CreateRequest(url, 5000, "my-agent"));
                Assert.AreEqual(HttpStatusCode.OK, code);
                Assert.AreEqual("my-agent", body);
                (_, body) = await WebUtil.DownloadStringAsync(WebUtil.CreateRequest(url, 5000));
                Assert.AreEqual(WebUtil.DefaultUserAgent, body);
            }
            finally { l.Stop(); }
        }

        [TestMethod]
        public async Task String_NotFound_Throws()
        {
            var (l, url) = Serve(c => { c.Response.StatusCode = 404; c.Response.Close(); });
            try
            {
                await Assert.ThrowsExactlyAsync<HttpRequestException>(() => WebUtil.DownloadStringAsync(WebUtil.CreateRequest(url, 5000)));
            }
            finally { l.Stop(); }
        }

        [TestMethod]
        public async Task String_Timeout_Throws()
        {
            var (l, url) = Serve(c => System.Threading.Thread.Sleep(3000));
            try
            {
                await Assert.ThrowsExactlyAsync<TaskCanceledException>(() => WebUtil.DownloadStringAsync(WebUtil.CreateRequest(url, 300)));
            }
            finally { l.Stop(); }
        }

        [TestMethod]
        public async Task File_WritesBodyAndReports100()
        {
            var (l, url) = Serve(c =>
            {
                var b = new byte[100_000];
                c.Response.ContentLength64 = b.Length;
                c.Response.OutputStream.Write(b, 0, b.Length);
                c.Response.Close();
            });
            var path = Path.GetTempFileName();
            try
            {
                var last = -1;
                await WebUtil.DownloadFileAsync(WebUtil.CreateRequest(url, 5000), path, new Progress<int>(p => last = p));
                Assert.AreEqual(100_000, new FileInfo(path).Length);
                await Task.Delay(100);
                Assert.AreEqual(100, last);
            }
            finally { l.Stop(); File.Delete(path); }
        }

        [TestMethod]
        public async Task String_ThroughProxy_HitsProxy()
        {
            var hit = false;
            var (proxy, proxyUrl) = Serve(c =>
            {
                hit = true;
                var b = System.Text.Encoding.UTF8.GetBytes("via-proxy");
                c.Response.OutputStream.Write(b, 0, b.Length);
                c.Response.Close();
            });
            try
            {
                var (_, body) = await WebUtil.DownloadStringAsync(WebUtil.CreateRequest("http://example.invalid/x", 5000), proxyServer: proxyUrl);
                Assert.IsTrue(hit);
                Assert.AreEqual("via-proxy", body);
            }
            finally { proxy.Stop(); }
        }
    }
}
