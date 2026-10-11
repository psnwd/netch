using System;
using System.IO;
using System.IO.Compression;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Netrix.Models;
using Netrix.Services;

namespace Tests
{
    [TestClass]
    public class UpdaterTests
    {
        private static string NewDir()
        {
            var d = Path.Combine(Path.GetTempPath(), "netrix-upd-" + Guid.NewGuid());
            Directory.CreateDirectory(d);
            return d;
        }

        private static string MakeZip(string dir, params (string name, string content)[] files)
        {
            var zip = Path.Combine(dir, "update.zip");
            using var za = ZipFile.Open(zip, ZipArchiveMode.Create);
            foreach (var (name, content) in files)
            {
                using var w = new StreamWriter(za.CreateEntry("Netrix/" + name).Open());
                w.Write(content);
            }

            return zip;
        }

        [TestMethod]
        public void Apply_ReplacesFiles()
        {
            var work = NewDir();
            var install = NewDir();
            File.WriteAllText(Path.Combine(install, "Netrix.exe"), "old");
            var zip = MakeZip(work, ("Netrix.exe", "new"));

            new Updater(zip, install).ApplyUpdate();

            Assert.AreEqual("new", File.ReadAllText(Path.Combine(install, "Netrix.exe")));
        }

        [TestMethod]
        public void Apply_FailsHalfway_RestoresOldFiles()
        {
            var work = NewDir();
            var install = NewDir();
            File.WriteAllText(Path.Combine(install, "Netrix.exe"), "old-exe");
            File.WriteAllText(Path.Combine(install, "a.dll"), "old-a");
            // a directory where the update wants a file: moving that file over it fails after Netrix.exe was already replaced
            Directory.CreateDirectory(Path.Combine(install, "zzz.dll"));
            var zip = MakeZip(work, ("Netrix.exe", "new-exe"), ("a.dll", "new-a"), ("zzz.dll", "new-z"));

            Assert.ThrowsExactly<MessageException>(() => new Updater(zip, install).ApplyUpdate());

            Assert.AreEqual("old-exe", File.ReadAllText(Path.Combine(install, "Netrix.exe")));
            Assert.AreEqual("old-a", File.ReadAllText(Path.Combine(install, "a.dll")));
            Assert.AreEqual(0, Directory.GetFiles(install, "*.old", SearchOption.AllDirectories).Length);
        }
    }
}
