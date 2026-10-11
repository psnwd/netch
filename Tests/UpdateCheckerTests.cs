using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Netrix.Controllers;
using Netrix.Models.GitHubRelease;

namespace Tests
{
    [TestClass]
    public class UpdateCheckerTests
    {
        [TestMethod]
        public void No_releases_means_no_update_instead_of_throwing()
        {
            Assert.IsNull(UpdateChecker.GetLatestRelease(new List<Release>(), false));
            // only a pre-release exists and pre-releases are not wanted
            Assert.IsNull(UpdateChecker.GetLatestRelease(new List<Release> { new() { tag_name = "2.0.1-beta", prerelease = true } }, false));
        }

        [TestMethod]
        public void Tag_with_v_prefix_is_a_version()
        {
            Assert.IsTrue(Netrix.Models.GitHubRelease.VersionUtil.CompareVersion("v2.0.1", "2.0.0") > 0);
            Assert.IsTrue(Netrix.Models.GitHubRelease.VersionUtil.CompareVersion("v2.0.0", "2.0.0") == 0);
            Assert.IsTrue(Netrix.Models.GitHubRelease.VersionUtil.CompareVersion("2.0.0", "v2.1.0") < 0);
        }

        [TestMethod]
        public void Picks_highest_version()
        {
            var releases = new List<Release> { new() { tag_name = "2.0.0" }, new() { tag_name = "2.1.0" }, new() { tag_name = "1.9.9" } };
            Assert.AreEqual("2.1.0", UpdateChecker.GetLatestRelease(releases, false)!.tag_name);
        }
    }
}
