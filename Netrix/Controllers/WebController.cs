using System.Text.Json;
using Microsoft.Win32;
using Netrix.Interfaces;
using Netrix.Models;
using Netrix.Models.Modes;
using Netrix.Models.Modes.WebMode;
using Netrix.Servers;
using Windows.Win32;

namespace Netrix.Controllers;

/// <summary>
///     Sets the Windows system proxy (WinINet) to the SOCKS5 server; only apps that honour it are proxied
/// </summary>
public class WebController : IModeController
{
    // not const: tests point it at a scratch key instead of the real Internet Settings
    internal static string KeyPath =@"Software\Microsoft\Windows\CurrentVersion\Internet Settings";

    // previous values are also written to disk so a crash can be undone on the next launch (RecoverAfterCrash)
    private static string SavedFile => Path.Combine(Global.NetrixDir, "data", "webmode.restore.json");

    private record Saved(int? Enable, string? Server, string? Override);

    private Saved? _saved;

    public string Name => "System Proxy";

    public ModeFeature Features => ModeFeature.SupportSocks5;

    public async Task StartAsync(Socks5Server server, Mode mode)
    {
        if (mode is not WebMode webMode)
            throw new InvalidOperationException();

        var address = $"{await server.AutoResolveHostnameAsync()}:{server.Port}";

        using var key = Registry.CurrentUser.OpenSubKey(KeyPath, true) ?? throw new MessageException("Open Internet Settings failed");
        _saved = new Saved(key.GetValue("ProxyEnable") as int?, key.GetValue("ProxyServer") as string, key.GetValue("ProxyOverride") as string);
        File.WriteAllText(SavedFile, JsonSerializer.Serialize(_saved));

        key.SetValue("ProxyServer", $"socks={address}");
        key.SetValue("ProxyOverride", webMode.Bypass);
        key.SetValue("ProxyEnable", 1, RegistryValueKind.DWord);
        Notify();
    }

    public Task StopAsync()
    {
        if (_saved != null)
            Restore(_saved);

        _saved = null;
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Call at startup: if a previous run died with the proxy set, put the old values back
    /// </summary>
    public static void RecoverAfterCrash()
    {
        try
        {
            if (File.Exists(SavedFile))
                Restore(JsonSerializer.Deserialize<Saved>(File.ReadAllText(SavedFile))!);
        }
        catch (Exception e)
        {
            Log.Error(e, "Recover system proxy failed");
        }
    }

    private static void Restore(Saved saved)
    {
        using var key = Registry.CurrentUser.OpenSubKey(KeyPath, true);
        if (key == null)
            return;

        Set(key, "ProxyEnable", saved.Enable, RegistryValueKind.DWord);
        Set(key, "ProxyServer", saved.Server, RegistryValueKind.String);
        Set(key, "ProxyOverride", saved.Override, RegistryValueKind.String);
        Notify();
        File.Delete(SavedFile);
    }

    private static void Set(RegistryKey key, string name, object? value, RegistryValueKind kind)
    {
        if (value == null)
            key.DeleteValue(name, false);
        else
            key.SetValue(name, value, kind);
    }

    private static unsafe void Notify()
    {
        PInvoke.InternetSetOption(null, PInvoke.INTERNET_OPTION_SETTINGS_CHANGED, null, 0);
        PInvoke.InternetSetOption(null, PInvoke.INTERNET_OPTION_REFRESH, null, 0);
    }
}
