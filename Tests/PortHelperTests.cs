using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class PortHelperTests
    {
        [TestMethod]
        public void GetProcessByUsedTcpPort_FindsListener()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            try
            {
                var port = (ushort)((IPEndPoint)listener.LocalEndpoint).Port;
                var type = typeof(Netrix.Program).Assembly.GetType("Netrix.Utils.PortHelper")!;
                var method = type.GetMethod("GetProcessByUsedTcpPort", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)!;
                var found = (IEnumerable<Process>)method.Invoke(null, new object[] { port, AddressFamily.InterNetwork })!;
                Assert.IsTrue(found.Any(p => p.Id == Environment.ProcessId));
            }
            finally
            {
                listener.Stop();
            }
        }
    }
}
