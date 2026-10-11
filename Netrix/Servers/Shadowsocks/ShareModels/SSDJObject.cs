#nullable disable
namespace Netrix.Servers;

public class SSDJObject
{
    /// <summary>
    ///     Provider name
    /// </summary>
    public string airport;

    /// <summary>
    ///     Encryption method
    /// </summary>
    public string encryption;

    /// <summary>
    ///     Password
    /// </summary>
    public string password;

    /// <summary>
    ///     Plugin
    /// </summary>
    public string plugin;

    /// <summary>
    ///     Plugin parameter
    /// </summary>
    public string plugin_options;

    /// <summary>
    ///     Port
    /// </summary>
    public ushort port;

    /// <summary>
    ///     Server array
    /// </summary>
    public List<SSDServerJObject> servers;
}