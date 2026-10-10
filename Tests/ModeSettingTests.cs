using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Netch;
using Netch.JsonConverter;
using Netch.Models;
using Netch.Models.Modes;
using Netch.Models.Modes.ProcessMode;
using Netch.Models.Modes.ShareMode;
using Netch.Models.Modes.TunMode;
using Netch.Models.Modes.WebMode;
using Netch.Utils;

namespace Tests
{
    [TestClass]
    public class ModeSettingTests
    {
        static T RoundTrip<T>(T mode) where T : Mode
        {
            mode.FullName = Path.Combine(Path.GetTempPath(), $"netch-test-{System.Guid.NewGuid()}.json");
            try
            {
                mode.WriteFile();
                return (T)ModeHelper.LoadMode(mode.FullName);
            }
            finally
            {
                File.Delete(mode.FullName);
            }
        }

        [TestMethod]
        public void ProcessMode_RoundTrip()
        {
            var m = new Redirector { FilterTCP = true, FilterLoopback = true, Bypass = { "a.exe" }, Handle = { "b.exe", "c.exe" }, Remark = { ["en"] = "p" } };
            var b = RoundTrip(m);
            Assert.AreEqual(true, b.FilterTCP);
            Assert.IsTrue(b.FilterLoopback);
            CollectionAssert.AreEqual(m.Bypass, b.Bypass);
            CollectionAssert.AreEqual(m.Handle, b.Handle);
            Assert.AreEqual("p", b.Remark["en"]);
        }

        [TestMethod]
        public void TunMode_RoundTrip()
        {
            var b = RoundTrip(new TunMode { Bypass = { "1.1.1.0/24" }, Handle = { "0.0.0.0/0" } });
            CollectionAssert.AreEqual(new[] { "1.1.1.0/24" }, b.Bypass);
            CollectionAssert.AreEqual(new[] { "0.0.0.0/0" }, b.Handle);
        }

        [TestMethod]
        public void ShareMode_RoundTrip() => Assert.AreEqual("--preset x", RoundTrip(new ShareMode { Argument = "--preset x" }).Argument);

        [TestMethod]
        public void WebMode_RoundTrip() => Assert.AreEqual("<local>;10.*", RoundTrip(new WebMode { Bypass = "<local>;10.*" }).Bypass);

        [TestMethod]
        public void SampleWebModeFile_Loads()
        {
            var dir = new DirectoryInfo(System.AppContext.BaseDirectory);
            while (!File.Exists(Path.Combine(dir.FullName, "Storage", "mode", "System Proxy.json")))
                dir = dir.Parent!;

            var m = (WebMode)ModeHelper.LoadMode(Path.Combine(dir.FullName, "Storage", "mode", "System Proxy.json"));
            Assert.AreEqual("<local>", m.Bypass);
            Assert.IsTrue(m.Remark.ContainsKey("en"));
        }

        [TestMethod]
        public void Setting_EmptyJson_UsesDefaults()
        {
            var o = Netch.Global.NewCustomJsonSerializerOptions();
            o.Converters.Add(new ServerConverterWithTypeDiscriminator());
            o.Converters.Add(new JsonStringEnumConverter());
            var s = JsonSerializer.Deserialize<Setting>("{}", o);
            Assert.IsNotNull(s);
            Assert.IsNotNull(s.V2RayConfig);
            Assert.IsNotNull(s.Redirector);
            Assert.IsNotNull(s.TUNTAP);
        }
    }
}
