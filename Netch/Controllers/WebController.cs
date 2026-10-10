using Microsoft.Win32;
using Netch.Interfaces;
using Netch.Models;
using Netch.Models.Modes;
using Netch.Models.Modes.WebMode;
using Netch.Servers;
using Windows.Win32;

namespace Netch.Controllers;

/// <summary>
///     Sets the Windows system proxy (WinINet) to the SOCKS5 server; only apps that honour it are proxied
/// </summary>
public class WebController : IModeController
{
    private const string KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Internet Settings";

    // ponytail: restored on Stop/exit only; a hard crash leaves the proxy set (Settings > Network > Proxy to clear)
    private (object? Enable, object? Server, object? Override)? _saved;

    public string Name => "System Proxy";

    public ModeFeature Features => ModeFeature.SupportSocks5;

    public async Task StartAsync(Socks5Server server, Mode mode)
    {
        if (mode is not WebMode webMode)
            throw new InvalidOperationException();

        var address = $"{await server.AutoResolveHostnameAsync()}:{server.Port}";

        using var key = Registry.CurrentUser.OpenSubKey(KeyPath, true) ?? throw new MessageException("Open Internet Settings failed");
        _saved = (key.GetValue("ProxyEnable"), key.GetValue("ProxyServer"), key.GetValue("ProxyOverride"));

        key.SetValue("ProxyServer", $"socks={address}");
        key.SetValue("ProxyOverride", webMode.Bypass);
        key.SetValue("ProxyEnable", 1, RegistryValueKind.DWord);
        Notify();
    }

    public Task StopAsync()
    {
        if (_saved is not var (enable, server, over))
            return Task.CompletedTask;

        _saved = null;
        using var key = Registry.CurrentUser.OpenSubKey(KeyPath, true);
        if (key == null)
            return Task.CompletedTask;

        Restore(key, "ProxyEnable", enable, RegistryValueKind.DWord);
        Restore(key, "ProxyServer", server, RegistryValueKind.String);
        Restore(key, "ProxyOverride", over, RegistryValueKind.String);
        Notify();
        return Task.CompletedTask;
    }

    private static void Restore(RegistryKey key, string name, object? value, RegistryValueKind kind)
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
