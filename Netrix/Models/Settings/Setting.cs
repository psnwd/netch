using System.Text.Json;

namespace Netrix.Models;

/// <summary>
///     Settings class for reading and writing
/// </summary>
public class Setting
{
    public RedirectorConfig Redirector { get; set; } = new();

    /// <summary>
    ///     Server list
    /// </summary>
    public List<Server> Server { get; set; } = new();

    public AioDNSConfig AioDNS { get; set; } = new();

    /// <summary>
    ///     Whether to check for beta updates
    /// </summary>
    public bool CheckBetaUpdate { get; set; } = false;

    /// <summary>
    ///     Whether to check for updates when the app opens
    /// </summary>
    public bool CheckUpdateWhenOpened { get; set; } = true;

    /// <summary>
    ///     Interval in seconds for testing all servers
    /// </summary>
    public int DetectionTick { get; set; } = 10;

    /// <summary>
    ///     Whether closing the window exits the app
    /// </summary>
    public bool ExitWhenClosed { get; set; } = false;

    /// <summary>
    ///     HTTP local port
    /// </summary>
    public ushort HTTPLocalPort { get; set; } = 2802;

    /// <summary>
    ///     Language setting
    /// </summary>
    public string Language { get; set; } = "System";

    /// <summary>
    ///     Local address of the HTTP and SOCKS5 proxy
    /// </summary>
    public string LocalAddress { get; set; } = "127.0.0.1";

    /// <summary>
    ///     Whether to minimize after starting
    /// </summary>
    public bool MinimizeWhenStarted { get; set; } = false;

    /// <summary>
    ///     Selected mode position
    /// </summary>
    public int ModeComboBoxSelectedIndex { get; set; } = -1;

    /// <summary>
    ///     Number of quick-start profiles
    /// </summary>
    public int ProfileCount { get; set; } = 4;

    /// <summary>
    ///     Saved quick-start profiles
    /// </summary>
    public List<Profile> Profiles { get; set; } = new();

    /// <summary>
    ///     Maximum number of profile columns
    /// </summary>
    public byte ProfileTableColumnCount { get; set; } = 5;

    /// <summary>
    ///     Web request timeout in milliseconds
    /// </summary>
    public int RequestTimeout { get; set; } = 10000;

    /// <summary>
    ///     Whether to start the app with Windows
    /// </summary>
    public bool RunAtStartup { get; set; } = false;

    /// <summary>
    ///     Selected server position
    /// </summary>
    public int ServerComboBoxSelectedIndex { get; set; } = -1;

    /// <summary>
    ///     Server test method: false = ICMPing, true = TCPing
    /// </summary>
    public bool ServerTCPing { get; set; } = true;

    /// <summary>
    ///     SOCKS5 local port
    /// </summary>
    public ushort Socks5LocalPort { get; set; } = 2801;

    /// <summary>
    ///     Delay test interval after starting, in seconds
    /// </summary>
    public int StartedPingInterval { get; set; } = -1;

    /// <summary>
    ///     Whether to start the proxy when the app opens
    /// </summary>
    public bool StartWhenOpened { get; set; } = false;

    /// <summary>
    ///     Whether to stop the proxy on exit
    /// </summary>
    public bool StopWhenExited { get; set; } = false;

    /// <summary>
    ///     STUN test server
    /// </summary>
    public string STUN_Server { get; set; } = "stun.1und1.de";

    /// <summary>
    ///     STUN test server port
    /// </summary>
    public int STUN_Server_Port { get; set; } = 3478;

    /// <summary>
    ///     Subscription link list
    /// </summary>
    public List<Subscription> Subscription { get; set; } = new();

    /// <summary>
    ///     TUNTAP adapter settings
    /// </summary>
    public TUNConfig TUNTAP { get; set; } = new();

    /// <summary>
    ///     Whether to update subscriptions when the app opens
    /// </summary>
    public bool UpdateServersWhenOpened { get; set; } = false;

    public V2rayConfig V2RayConfig { get; set; } = new();

    public bool NoSupportDialog { get; set; } = false;

    #region Migration

    [Obsolete]
    public JsonElement SubscribeLink
    {
        set
        {
            if (Subscription == null! || !Subscription.Any())
                Subscription = value.Deserialize<List<Subscription>>()!;
        }
    }

    #endregion

    public Setting ShallowCopy()
    {
        return (Setting)MemberwiseClone();
    }

    public void Set(Setting value)
    {
        foreach (var p in typeof(Setting).GetProperties())
            p.SetValue(this, p.GetValue(value));
    }
}