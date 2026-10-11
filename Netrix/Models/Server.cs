using System.Net.Sockets;
using System.Text.Json.Serialization;
using Netrix.Utils;

namespace Netrix.Models;

public abstract class Server : ICloneable
{
    /// <summary>
    ///     Delay
    /// </summary>
    [JsonIgnore]
    public int Delay { get; private set; } = -1;

    /// <summary>
    ///     Group
    /// </summary>
    public string Group { get; set; } = Constants.DefaultGroup;

    /// <summary>
    ///     Address
    /// </summary>
    public string Hostname { get; set; } = string.Empty;

    /// <summary>
    ///     Port
    /// </summary>
    public ushort Port { get; set; }

    /// <summary>
    ///     Rate multiplier
    /// </summary>
    public double Rate { get; } = 1.0;

    /// <summary>
    ///     Remark
    /// </summary>
    public string Remark { get; set; } = "";

    /// <summary>
    ///     Proxy type
    /// </summary>
    [JsonPropertyOrder(int.MinValue)]
    public abstract string Type { get; }

    public object Clone()
    {
        return MemberwiseClone();
    }

    /// <summary>
    ///     Get the remark
    /// </summary>
    /// <returns>Remark</returns>
    public override string ToString()
    {
        var remark = string.IsNullOrWhiteSpace(Remark) ? $"{Hostname}:{Port}" : Remark;

        var shortName = ServerHelper.GetUtilByTypeName(Type).ShortName;

        return $"[{shortName}][{Group}] {remark}";
    }

    public abstract string MaskedData();

    /// <summary>
    ///     Test the delay
    /// </summary>
    /// <returns>Delay</returns>
    public async Task<int> PingAsync()
    {
        try
        {
            var destination = await DnsUtils.LookupAsync(Hostname);
            if (destination == null)
                return Delay = -2;

            var list = new Task<int>[3];
            for (var i = 0; i < 3; i++)
            {
                Task<int> PingCoreAsync()
                {
                    try
                    {
                        return Global.Settings.ServerTCPing ? Utils.Utils.TCPingAsync(destination, Port) : Utils.Utils.ICMPingAsync(destination);
                    }
                    catch (Exception)
                    {
                        return Task.FromResult(-4);
                    }
                }

                list[i] = PingCoreAsync();
            }

            var resTask = await Task.WhenAny(list[0], list[1], list[2]);

            return Delay = await resTask;
        }
        catch (Exception)
        {
            return Delay = -4;
        }
    }
}

public static class ServerExtension
{
    public static async Task<string> AutoResolveHostnameAsync(this Server server, AddressFamily inet = AddressFamily.Unspecified)
    {
        // ! MainController cached
        return (await DnsUtils.LookupAsync(server.Hostname, inet))!.ToString();
    }

    public static bool IsInGroup(this Server server)
    {
        return server.Group is not Constants.DefaultGroup;
    }
}