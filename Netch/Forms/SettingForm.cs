using System.Net;
using Netch.Properties;
using Netch.Utils;

namespace Netch.Forms;

[Fody.ConfigureAwait(true)]
public partial class SettingForm : BindingForm
{
    public SettingForm()
    {
        InitializeComponent();
        Icon = Resources.icon;
        i18N.TranslateForm(this);
        AcceptButton = ControlButton;
        Tips.Create(this,
            // General
            (Socks5PortLabel, "Port of the local SOCKS5 proxy that Netch opens on this PC."),
            (Socks5PortTextBox, "Port of the local SOCKS5 proxy that Netch opens on this PC."),
            (AllowDevicesCheckBox, "Let other devices on your network use this PC's proxy.\nOff = only this PC (safer)."),
            (ServerPingTypeLabel, "How the server delay is measured."),
            (ICMPingRadioBtn, "Ping with ICMP. Some servers and firewalls block it."),
            (TCPingRadioBtn, "Connect with TCP to the server port. Works almost everywhere (recommended)."),
            (ProfileCountLabel, "How many quick-start profile buttons to show on the main window. 0 hides them."),
            (ProfileCountTextBox, "How many quick-start profile buttons to show on the main window. 0 hides them."),
            (DetectionTickLabel, "Test the delay of all servers every N seconds while Netch is open. 0 turns it off."),
            (DetectionTickTextBox, "Test the delay of all servers every N seconds while Netch is open. 0 turns it off."),
            (StartedPingLabel, "After you press Start, wait N seconds, then test the delay once. -1 turns it off."),
            (StartedPingIntervalTextBox, "After you press Start, wait N seconds, then test the delay once. -1 turns it off."),
            (STUNServerLabel, "Server used for the NAT type test in the status bar. It must support RFC 5780, e.g. stun.1und1.de."),
            (STUN_ServerComboBox, "Server used for the NAT type test in the status bar. It must support RFC 5780, e.g. stun.1und1.de."),
            (LanguageLabel, "Language of the Netch interface."),
            (LanguageComboBox, "Language of the Netch interface."),
            // Process mode
            (FilterTCPCheckBox, "Send the TCP traffic of chosen programs through the proxy (web, most apps)."),
            (FilterUDPCheckBox, "Send the UDP traffic of chosen programs through the proxy (games, voice, QUIC). The server must support UDP."),
            (FilterICMPCheckBox, "Also handle ping (ICMP) of chosen programs."),
            (ICMPDelayLabel, "Delay added to ping replies in milliseconds."),
            (ICMPDelayTextBox, "Delay added to ping replies in milliseconds."),
            (FilterDNSCheckBox, "Redirect the DNS lookups of chosen programs to the DNS server below, so names resolve correctly through the proxy."),
            (DNSHijackLabel, "DNS server used when DNS handling is on."),
            (DNSHijackHostTextBox, "DNS server used when DNS handling is on, as address:port, e.g. 1.1.1.1:53."),
            (HandleProcDNSCheckBox, "Only redirect DNS lookups made by the chosen programs. Off = every program's DNS."),
            (DNSProxyCheckBox, "Send the DNS lookups through the proxy server instead of straight out."),
            (ChildProcessHandleCheckBox, "Also handle programs started by the chosen programs."),
            // TUN
            (TUNTAPAddressLabel, "IP address of the virtual network adapter. Change only if it clashes with your network."),
            (TUNTAPAddressTextBox, "IP address of the virtual network adapter. Change only if it clashes with your network."),
            (TUNTAPNetmaskLabel, "Netmask of the virtual adapter."),
            (TUNTAPNetmaskTextBox, "Netmask of the virtual adapter."),
            (TUNTAPGatewayLabel, "Gateway of the virtual adapter. Normally the first address of its network."),
            (TUNTAPGatewayTextBox, "Gateway of the virtual adapter. Normally the first address of its network."),
            (UseCustomDNSCheckBox, "Use the DNS server below instead of Netch's own DNS (AioDNS)."),
            (TUNTAPDNSLabel, "DNS server used when custom DNS is on."),
            (TUNTAPDNSTextBox, "DNS server used when custom DNS is on."),
            (ProxyDNSCheckBox, "Send the custom DNS lookups through the proxy."),
            (GlobalBypassIPsButton, "IP addresses and ranges that never go through the proxy in any TUN mode, e.g. 192.168.1.0/24."),
            // V2Ray
            (XrayConeCheckBox, "Full cone NAT for UDP (better for games). The server must run Xray-core 1.3.0 or newer."),
            (TLSAllowInsecureCheckBox, "Accept invalid or self-signed TLS certificates. Less safe: only turn on if you trust the server."),
            (UseMuxCheckBox, "Share one connection for many requests. Starts connections faster, but can lower single-download speed."),
            (TCPFastOpenBox, "Send data with the TCP handshake to save a round trip. Needs support on both ends."),
            (KCPGroupBox, "Advanced mKCP transport options. Leave as they are unless your server tells you otherwise."),
            // Others
            (ExitWhenClosedCheckBox, "Closing the window quits Netch. Off = it keeps running in the tray."),
            (StopWhenExitedCheckBox, "Stop the proxy when Netch quits."),
            (StartWhenOpenedCheckBox, "Start the proxy automatically when Netch opens, using the selected server and mode."),
            (MinimizeWhenStartedCheckBox, "Hide the window to the tray after the proxy starts."),
            (RunAtStartupCheckBox, "Start Netch when you sign in to Windows."),
            (CheckUpdateWhenOpenedCheckBox, "Look for a new version each time Netch opens."),
            (NoSupportDialogCheckBox, "Hide the support warning dialog."),
            (CheckBetaUpdateCheckBox, "Also offer pre-release versions when checking for updates."),
            (UpdateServersWhenOpenedCheckBox, "Refresh your subscriptions each time Netch opens."),
            // AioDNS
            (ChinaDNSLabel, "DNS server for domestic (China) lookups. Format: tcp://223.5.5.5:53 or tls://host:853."),
            (ChinaDNSTextBox, "DNS server for domestic (China) lookups. Format: tcp://223.5.5.5:53 or tls://host:853."),
            (OtherDNSLabel, "DNS server for all other lookups. Format: tcp://1.1.1.1:53 or tls://host:853."),
            (OtherDNSTextBox, "DNS server for all other lookups. Format: tcp://1.1.1.1:53 or tls://host:853."),
            (AioDNSListenPortLabel, "Local port of Netch's own DNS service. Change only if 53 or this port is taken."),
            (AioDNSListenPortTextBox, "Local port of Netch's own DNS service. Change only if this port is taken."),
            // Save
            (ControlButton, "Save the settings (Enter)."));

        #region General

        BindTextBox<ushort>(Socks5PortTextBox, p => true, p => Global.Settings.Socks5LocalPort = p, Global.Settings.Socks5LocalPort);

        BindCheckBox(AllowDevicesCheckBox,
            c => Global.Settings.LocalAddress = AllowDevicesCheckBox.Checked ? "0.0.0.0" : "127.0.0.1",
            Global.Settings.LocalAddress switch { "127.0.0.1" => false, "0.0.0.0" => true, _ => false });

        BindRadioBox(ICMPingRadioBtn, _ => { }, !Global.Settings.ServerTCPing);

        BindRadioBox(TCPingRadioBtn, c => Global.Settings.ServerTCPing = c, Global.Settings.ServerTCPing);

        BindTextBox<int>(ProfileCountTextBox, i => i > -1, i => Global.Settings.ProfileCount = i, Global.Settings.ProfileCount);
        BindTextBox<int>(DetectionTickTextBox,
            i => DelayTestHelper.Range.InRange(i),
            i => Global.Settings.DetectionTick = i,
            Global.Settings.DetectionTick);

        BindTextBox<int>(StartedPingIntervalTextBox, _ => true, i => Global.Settings.StartedPingInterval = i, Global.Settings.StartedPingInterval);

        object[]? stuns;
        try
        {
            stuns = File.ReadLines(Constants.STUNServersFile).Cast<object>().ToArray();
        }
        catch (Exception e)
        {
            Log.Warning(e, "Load stun.txt failed");
            stuns = null;
        }

        BindComboBox(STUN_ServerComboBox,
            s =>
            {
                var split = s.SplitRemoveEmptyEntriesAndTrimEntries(':');
                if (!split.Any())
                    return false;

                var port = split.ElementAtOrDefault(1);
                if (port != null)
                    if (!ushort.TryParse(split[1], out _))
                        return false;

                return true;
            },
            o =>
            {
                var split = o.ToString().SplitRemoveEmptyEntriesAndTrimEntries(':');
                Global.Settings.STUN_Server = split[0];

                var port = split.ElementAtOrDefault(1);
                Global.Settings.STUN_Server_Port = port != null ? ushort.Parse(port) : 3478;
            },
            Global.Settings.STUN_Server + ":" + Global.Settings.STUN_Server_Port,
            stuns);

        BindListComboBox(LanguageComboBox, o => Global.Settings.Language = o.ToString(), i18N.GetTranslateList(), Global.Settings.Language);

        #endregion

        #region Process Mode

        BindCheckBox(FilterTCPCheckBox, b => Global.Settings.Redirector.FilterTCP = b, Global.Settings.Redirector.FilterTCP);

        BindCheckBox(FilterUDPCheckBox, b => Global.Settings.Redirector.FilterUDP = b, Global.Settings.Redirector.FilterUDP);

        BindCheckBox(FilterICMPCheckBox, b => Global.Settings.Redirector.FilterICMP = b, Global.Settings.Redirector.FilterICMP);

        BindTextBox<int>(ICMPDelayTextBox, s => true, s => Global.Settings.Redirector.ICMPDelay = s, Global.Settings.Redirector.ICMPDelay);

        BindCheckBox(FilterDNSCheckBox, b => Global.Settings.Redirector.FilterDNS = b, Global.Settings.Redirector.FilterDNS);

        // TODO validate Redirector AioDNS DNS
        BindTextBox(DNSHijackHostTextBox, s => true, s => Global.Settings.Redirector.DNSHost = s, Global.Settings.Redirector.DNSHost);

        BindCheckBox(ChildProcessHandleCheckBox, s => Global.Settings.Redirector.FilterParent = s, Global.Settings.Redirector.FilterParent);

        BindCheckBox(DNSProxyCheckBox, b => Global.Settings.Redirector.DNSProxy = b, Global.Settings.Redirector.DNSProxy);

        BindCheckBox(HandleProcDNSCheckBox, b => Global.Settings.Redirector.HandleOnlyDNS = b, Global.Settings.Redirector.HandleOnlyDNS);

        #endregion

        #region TUN/TAP

        BindTextBox(TUNTAPAddressTextBox, s => IPAddress.TryParse(s, out _), s => Global.Settings.TUNTAP.Address = s, Global.Settings.TUNTAP.Address);

        BindTextBox(TUNTAPNetmaskTextBox, s => IPAddress.TryParse(s, out _), s => Global.Settings.TUNTAP.Netmask = s, Global.Settings.TUNTAP.Netmask);

        BindTextBox(TUNTAPGatewayTextBox, s => IPAddress.TryParse(s, out _), s => Global.Settings.TUNTAP.Gateway = s, Global.Settings.TUNTAP.Gateway);

        BindCheckBox(UseCustomDNSCheckBox, b => { Global.Settings.TUNTAP.UseCustomDNS = b; }, Global.Settings.TUNTAP.UseCustomDNS);

        BindTextBox(TUNTAPDNSTextBox,
            s => UseCustomDNSCheckBox.Checked ? IPAddress.TryParse(s, out _) : true,
            s =>
            {
                if (UseCustomDNSCheckBox.Checked)
                    Global.Settings.TUNTAP.DNS = s;
            },
            Global.Settings.TUNTAP.DNS);

        BindCheckBox(ProxyDNSCheckBox, b => Global.Settings.TUNTAP.ProxyDNS = b, Global.Settings.TUNTAP.ProxyDNS);

        #endregion

        #region V2Ray
        BindCheckBox(XrayConeCheckBox, b => Global.Settings.V2RayConfig.XrayCone = b, Global.Settings.V2RayConfig.XrayCone);

        BindCheckBox(TLSAllowInsecureCheckBox, b => Global.Settings.V2RayConfig.AllowInsecure = b, Global.Settings.V2RayConfig.AllowInsecure);
        BindCheckBox(UseMuxCheckBox, b => Global.Settings.V2RayConfig.UseMux = b, Global.Settings.V2RayConfig.UseMux);
        BindCheckBox(TCPFastOpenBox, b => Global.Settings.V2RayConfig.TCPFastOpen = b, Global.Settings.V2RayConfig.TCPFastOpen);

        BindTextBox<int>(mtuTextBox, i => true, i => Global.Settings.V2RayConfig.KcpConfig.mtu = i, Global.Settings.V2RayConfig.KcpConfig.mtu);
        BindTextBox<int>(ttiTextBox, i => true, i => Global.Settings.V2RayConfig.KcpConfig.tti = i, Global.Settings.V2RayConfig.KcpConfig.tti);
        BindTextBox<int>(uplinkCapacityTextBox,
            i => true,
            i => Global.Settings.V2RayConfig.KcpConfig.uplinkCapacity = i,
            Global.Settings.V2RayConfig.KcpConfig.uplinkCapacity);

        BindTextBox<int>(downlinkCapacityTextBox,
            i => true,
            i => Global.Settings.V2RayConfig.KcpConfig.downlinkCapacity = i,
            Global.Settings.V2RayConfig.KcpConfig.downlinkCapacity);

        BindTextBox<int>(readBufferSizeTextBox,
            i => true,
            i => Global.Settings.V2RayConfig.KcpConfig.readBufferSize = i,
            Global.Settings.V2RayConfig.KcpConfig.readBufferSize);

        BindTextBox<int>(writeBufferSizeTextBox,
            i => true,
            i => Global.Settings.V2RayConfig.KcpConfig.writeBufferSize = i,
            Global.Settings.V2RayConfig.KcpConfig.writeBufferSize);

        BindCheckBox(congestionCheckBox, b => Global.Settings.V2RayConfig.KcpConfig.congestion = b, Global.Settings.V2RayConfig.KcpConfig.congestion);

        #endregion

        #region Others

        BindCheckBox(ExitWhenClosedCheckBox, b => Global.Settings.ExitWhenClosed = b, Global.Settings.ExitWhenClosed);

        BindCheckBox(StopWhenExitedCheckBox, b => Global.Settings.StopWhenExited = b, Global.Settings.StopWhenExited);

        BindCheckBox(StartWhenOpenedCheckBox, b => Global.Settings.StartWhenOpened = b, Global.Settings.StartWhenOpened);

        BindCheckBox(MinimizeWhenStartedCheckBox, b => Global.Settings.MinimizeWhenStarted = b, Global.Settings.MinimizeWhenStarted);

        BindCheckBox(RunAtStartupCheckBox, b => Global.Settings.RunAtStartup = b, Global.Settings.RunAtStartup);

        BindCheckBox(CheckUpdateWhenOpenedCheckBox, b => Global.Settings.CheckUpdateWhenOpened = b, Global.Settings.CheckUpdateWhenOpened);

        BindCheckBox(CheckBetaUpdateCheckBox, b => Global.Settings.CheckBetaUpdate = b, Global.Settings.CheckBetaUpdate);

        BindCheckBox(UpdateServersWhenOpenedCheckBox, b => Global.Settings.UpdateServersWhenOpened = b, Global.Settings.UpdateServersWhenOpened);

        BindCheckBox(NoSupportDialogCheckBox, b => Global.Settings.NoSupportDialog = b, Global.Settings.NoSupportDialog);

        #endregion

        #region AioDNS

        BindTextBox(ChinaDNSTextBox, _ => true, s => Global.Settings.AioDNS.ChinaDNS = s, Global.Settings.AioDNS.ChinaDNS);

        BindTextBox(OtherDNSTextBox, _ => true, s => Global.Settings.AioDNS.OtherDNS = s, Global.Settings.AioDNS.OtherDNS);

        BindTextBox(AioDNSListenPortTextBox,
            s => ushort.TryParse(s, out _),
            s => Global.Settings.AioDNS.ListenPort = ushort.Parse(s),
            Global.Settings.AioDNS.ListenPort);

        #endregion
    }

    private void SettingForm_Load(object sender, EventArgs e)
    {
        TUNTAPUseCustomDNSCheckBox_CheckedChanged(null, null);
    }

    protected override void BindTextBox<T>(TextBoxBase control, Func<T, bool> check, Action<T> save, object value)
    {
        base.BindTextBox(control, check, save, value);
        control.TextChanged += (_, _) =>
        {
            if (Validate(control))
            {
                errorProvider.SetError(control, null);
            }
            else
            {
                errorProvider.SetError(control, i18N.Translate("Invalid value"));
            }
        };
    }

    protected new void BindComboBox(ComboBox control, Func<string, bool> check, Action<string> save, string value, object[]? values = null)
    {
        base.BindComboBox(control, check, save, value, values);

        control.TextChanged += (_, _) =>
        {
            if (Validate(control))
            {
                errorProvider.SetError(control, null);
            }
            else
            {
                errorProvider.SetError(control, i18N.Translate("Invalid value"));
            }
        };
    }

    private void TUNTAPUseCustomDNSCheckBox_CheckedChanged(object? sender, EventArgs? e)
    {
        if (UseCustomDNSCheckBox.Checked)
            TUNTAPDNSTextBox.Text = Global.Settings.TUNTAP.DNS;
        else
            TUNTAPDNSTextBox.Text = "AioDNS";
    }

    private void GlobalBypassIPsButton_Click(object sender, EventArgs e)
    {
        Hide();
        new GlobalBypassIPForm().ShowDialog();
        Show();
    }

    private async void ControlButton_Click(object sender, EventArgs e)
    {
        Utils.Utils.ComponentIterator(this, component => Utils.Utils.ChangeControlForeColor(component, Color.Black));

        #region Check

        var checkNotPassControl = GetInvalidateValueControls();

        if (checkNotPassControl.Any())
        {
            var failControl = checkNotPassControl.First();

            // switch to fail control's tab page
            var p = failControl.Parent;
            while (p != null)
            {
                if (p is TabPage tabPage)
                {
                    TabControl.SelectedTab = tabPage;
                    break;
                }

                p = p.Parent;
            }

            return;
        }

        #endregion

        #region Save

        SaveBinds();

        #endregion

        Utils.Utils.RegisterNetchStartupItem();

        await Configuration.SaveAsync();
        MessageBoxX.Show(i18N.Translate("Saved"));
        Close();
    }
}