using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Netrix.Interfaces;
using Netrix.Models;
using Netrix.Utils;

namespace Netrix.Servers;

public class WireGuardUtil : IServerUtil
{
    public ushort Priority { get; } = 4;

    public string TypeName { get; } = "WireGuard";

    public string FullName { get; } = "WireGuard";

    public string ShortName { get; } = "WG";

    public string[] UriScheme { get; } = { "wireguard" };

    public Type ServerType { get; } = typeof(WireGuardServer);

    public void Edit(Server s)
    {
        new WireGuardForm((WireGuardServer)s).ShowDialog();
    }

    public void Create()
    {
        new WireGuardForm().ShowDialog();
    }

    public string GetShareLink(Server s)
    {
        // wireguard://privatekey@host:port?publickey=..&address=..&mtu=..[&presharedkey=..]#remark
        var server = (WireGuardServer)s;
        var query = $"publickey={Uri.EscapeDataString(server.PeerPublicKey)}&address={Uri.EscapeDataString(server.LocalAddresses)}&mtu={server.MTU}";
        if (!string.IsNullOrEmpty(server.PreSharedKey))
            query += $"&presharedkey={Uri.EscapeDataString(server.PreSharedKey)}";

        return $"wireguard://{Uri.EscapeDataString(server.PrivateKey)}@{server.Hostname}:{server.Port}?{query}#{Uri.EscapeDataString(server.Remark)}";
    }

    public IServerController GetController()
    {
        return new V2rayController();
    }

    public IEnumerable<Server> ParseUri(string text)
    {
        var data = new WireGuardServer();

        if (text.Contains('#'))
        {
            data.Remark = Uri.UnescapeDataString(text.Split('#')[1]);
            text = text.Split('#')[0];
        }

        var parameter = System.Web.HttpUtility.ParseQueryString(text.Contains('?') ? text.Split('?')[1] : "");
        var match = System.Text.RegularExpressions.Regex.Match(text.Split('?')[0], @"^wireguard://(?<key>.+)@(?<server>.+):(?<port>\d+)$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        if (!match.Success)
            throw new FormatException();

        data.PrivateKey = Uri.UnescapeDataString(match.Groups["key"].Value);
        data.Hostname = match.Groups["server"].Value;
        data.Port = ushort.Parse(match.Groups["port"].Value);
        data.PeerPublicKey = parameter.Get("publickey") ?? string.Empty;
        data.LocalAddresses = parameter.Get("address") ?? data.LocalAddresses;
        data.PreSharedKey = parameter.Get("presharedkey");
        if (int.TryParse(parameter.Get("mtu"), out var mtu))
            data.MTU = mtu;

        return new[] { data };
    }

    public bool CheckServer(Server s)
    {
        return true;
    }
}