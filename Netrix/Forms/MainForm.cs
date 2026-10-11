using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;
using Microsoft.VisualStudio.Threading;
using Microsoft.Win32;
using Netrix.Controllers;
using Netrix.Enums;
using Netrix.Forms.ModeForms;
using Netrix.Interfaces;
using Netrix.Models;
using Netrix.Models.Modes;
using Netrix.Properties;
using Netrix.Services;
using Netrix.Utils;

namespace Netrix.Forms;

[Fody.ConfigureAwait(true)]
public partial class MainForm : Form
{
    #region Start

    private readonly Dictionary<string, object> _mainFormText = new();

    private bool _textRecorded;
    private readonly ToolTip _tips;

    public MainForm()
    {
        InitializeComponent();
        NotifyIcon.Icon = Icon = Resources.icon;

        _tips = Tips.Create(this,
            (EditServerPictureBox, "Edit the selected server"),
            (DeleteServerPictureBox, "Delete the selected server"),
            (SpeedPictureBox, "Test the delay (ping) of all servers"),
            (CopyLinkPictureBox, "Copy the share link of the selected server"),
            (EditModePictureBox, "Edit the selected mode"),
            (DeleteModePictureBox, "Delete the selected mode"),
            (ServerLabel, "The proxy server your traffic goes through. Add servers from the Server menu."),
            (ServerComboBox, "The proxy server your traffic goes through. Add servers from the Server menu.\nThe number is its delay in ms: lower is faster."),
            (ModeLabel, "What gets proxied. Add or edit modes from the Mode menu."),
            (ModeComboBox, "What gets proxied:\nProcess = chosen apps\nTUN = the whole PC\nShare = other devices on your network\nWeb = the Windows system proxy"),
            (ProfileLabel, "Name for a quick-start profile."),
            (ProfileNameText, "Name for a quick-start profile.\nType a name, pick a server and a mode, then Ctrl+Click a profile button to save."),
            (ControlButton, "Start or stop the proxy. Netrix needs to run as administrator."),
            (SettingsButton, "Open the settings"));
        Tips.Set("Download speed", DownloadSpeedLabel);
        Tips.Set("Upload speed", UploadSpeedLabel);
        Tips.Set("Data used since you pressed Start", UsedBandwidthLabel);
        Tips.Set("Delay of a web request through the proxy", HttpStatusLabel);
        Tips.Set("NAT type through the proxy: 1 open, 2 restricted, 3 symmetric (hard for games). Click to test again.", NatTypeStatusLabel, NatTypeStatusLightLabel);
        Tips.Set("A new version is available. Click to update.", NewVersionLabel);
        Tips.Set("Netrix version. Click to open the releases page.", VersionLabel);

        AddAddServerToolStripMenuItems();

        #region i18N Translations

        if (Flags.NoSupport)
            _mainFormText.Add(Name, new[] { "{0} ({1})", UpdateChecker.Name, "No Support" });

        _mainFormText.Add(UninstallServiceToolStripMenuItem.Name, new[] { "Uninstall {0}", "NF Service" });

        #endregion

        // Listen for power events
        SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
    }

    private void AddAddServerToolStripMenuItems()
    {
        foreach (var serversUtil in ServerHelper.ServerUtilDictionary.Values.OrderBy(i => i.Priority).Where(i => !string.IsNullOrEmpty(i.FullName)))
        {
            var fullName = serversUtil.FullName;
            var control = new ToolStripMenuItem
            {
                Name = $"Add{fullName}ServerToolStripMenuItem",
                Size = new Size(259, 22),
                Text = i18N.TranslateFormat("Add [{0}] Server", fullName),
                Tag = serversUtil
            };

            _mainFormText.Add(control.Name, new[] { "Add [{0}] Server", fullName });
            control.Click += AddServerToolStripMenuItem_Click;
            ServerToolStripMenuItem.DropDownItems.Add(control);
        }
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
        // Compute the target width for drawing the ComboBox
        RecordSize();

        LoadServers();
        SelectLastServer();
        DelayTestHelper.UpdateTick(true);

        ModeService.Instance.Load();

        // Load translations
        TranslateControls();

        // Hide ConnectivityStatusLabel
        ConnectivityStatusVisible(false);

        // Load quick-start profiles
        LoadProfiles();

        // Check for updates
        if (Global.Settings.CheckUpdateWhenOpened)
            CheckUpdateAsync().Forget();

        // Check for subscription updates
        if (Global.Settings.UpdateServersWhenOpened)
            UpdateServersFromSubscriptionAsync().Forget();

        // Start the proxy when the app opens by raising a click on the Start button
        if (Global.Settings.StartWhenOpened)
            ControlButton.PerformClick();

        Program.SingleInstance.StartListening(Program.SingleInstance_MessageReceived);
    }

    private void RecordSize()
    {
        _numberBoxWidth = ServerComboBox.Width / 10;
        _numberBoxX = _numberBoxWidth * 9;
        _numberBoxWrap = _numberBoxWidth / 30;

        _configurationGroupBoxHeight = ConfigurationGroupBox.Height;
        _profileConfigurationHeight = ConfigurationGroupBox.Controls[0].Height / 3; // AutoSize makes this the total height of the controls
        _profileGroupBoxPaddingHeight = ProfileGroupBox.Height - ProfileTable.Height;
        _profileTableHeight = ProfileTable.Height;
    }

    private void TranslateControls()
    {
        #region Record English

        if (!_textRecorded)
        {
            void RecordText(Component component)
            {
                try
                {
                    switch (component)
                    {
                        case TextBoxBase:
                        case ListControl:
                            break;
                        case Control c:
                            _mainFormText.Add(c.Name, c.Text);
                            break;
                        case ToolStripItem c:
                            _mainFormText.Add(c.Name, c.Text);
                            break;
                    }
                }
                catch (ArgumentException)
                {
                    // ignored
                }
            }

            Utils.Utils.ComponentIterator(this, RecordText);
            Utils.Utils.ComponentIterator(NotifyMenu, RecordText);
            _textRecorded = true;
        }

        #endregion

        #region Translate

        void TranslateText(Component component)
        {
            switch (component)
            {
                case TextBoxBase:
                case ListControl:
                    break;
                case Control c:
                    if (_mainFormText.ContainsKey(c.Name))
                        c.Text = ControlText(c.Name);

                    break;
                case ToolStripItem c:
                    if (_mainFormText.ContainsKey(c.Name))
                        c.Text = ControlText(c.Name);

                    break;
            }

            string ControlText(string name)
            {
                var value = _mainFormText[name];
                if (value.Equals(string.Empty))
                    return string.Empty;

                if (value is object[] values)
                    return i18N.TranslateFormat((string)values.First(), values.Skip(1).ToArray());

                return i18N.Translate(value);
            }
        }

        Utils.Utils.ComponentIterator(this, TranslateText);
        Utils.Utils.ComponentIterator(NotifyMenu, TranslateText);

        #endregion

        UsedBandwidthLabel.Text = $@"{i18N.Translate("Used", ": ")}0 KB";
        State = State;
        VersionLabel.Text = UpdateChecker.Version;
    }

    #endregion

    #region Controls

    #region MenuStrip

    #region Server

    private async void ImportServersFromClipboardToolStripMenuItem_Click(object sender, EventArgs e)
    {
        var texts = Clipboard.GetText();
        if (string.IsNullOrWhiteSpace(texts))
            return;

        var servers = ShareLink.ParseText(texts);
        foreach (var server in servers)
            server.Group = Constants.DefaultGroup;

        Global.Settings.Server.AddRange(servers);
        NotifyTip(i18N.TranslateFormat("Import {0} server(s) form Clipboard", servers.Count));

        LoadServers();
        await Configuration.SaveAsync();
    }

    private async void AddServerToolStripMenuItem_Click([NotNull] object? sender, EventArgs? e)
    {
        if (sender == null)
            throw new ArgumentNullException(nameof(sender));

        var util = (IServerUtil)((ToolStripMenuItem)sender).Tag;

        Hide();
        util.Create();

        LoadServers();
        await Configuration.SaveAsync();
        Show();
    }

    #endregion

    #region Mode

    private void CreateProcessModeToolStripButton_Click(object sender, EventArgs e)
    {
        Hide();
        new ProcessForm().ShowDialog();
        Show();
    }

    private void createRouteTableModeToolStripMenuItem_Click(object sender, EventArgs e)
    {
        Hide();
        new RouteForm().ShowDialog();
        Show();
    }

    private void ReloadModesToolStripMenuItem_Click(object sender, EventArgs e)
    {
        Enabled = false;
        try
        {
            ModeService.Instance.Load();
        }
        finally
        {
            Enabled = true;
        }
    }

    #endregion

    #region Subscription

    private void ManageSubscriptionLinksToolStripMenuItem_Click(object sender, EventArgs e)
    {
        Hide();
        new SubscriptionForm().ShowDialog();
        LoadServers();
        Show();
    }

    private async void UpdateServersFromSubscriptionLinksToolStripMenuItem_Click(object sender, EventArgs e)
    {
        await UpdateServersFromSubscriptionAsync();
    }

    private async Task UpdateServersFromSubscriptionAsync()
    {
        void DisableItems(bool v)
        {
            MenuStrip.Enabled = ConfigurationGroupBox.Enabled = ProfileGroupBox.Enabled = ControlButton.Enabled = v;
        }

        if (Global.Settings.Subscription.Count <= 0)
        {
            MessageBoxX.Show(i18N.Translate("No subscription link"));
            return;
        }

        StatusText(i18N.Translate("Updating servers"));
        DisableItems(false);

        try
        {
            await SubscriptionUtil.UpdateServersAsync();

            LoadServers();
            await Configuration.SaveAsync();
            StatusText(i18N.Translate("Servers updated"));
        }
        catch (Exception e)
        {
            NotifyTip(i18N.Translate("Unhandled update servers error") + "\n" + e.Message, info: false);
            Log.Error(e, "Unhandled Update servers error");
        }
        finally
        {
            DisableItems(true);
        }
    }

    #endregion

    #region Options

    private async void CheckForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
    {
        void OnNewVersionNotFound(object? o, EventArgs? args)
        {
            NotifyTip(i18N.Translate("Already latest version"));
        }

        void OnNewVersionFoundFailed(object? o, EventArgs? args)
        {
            NotifyTip(i18N.Translate("Check for update failed"), info: false);
        }

        try
        {
            UpdateChecker.NewVersionNotFound += OnNewVersionNotFound;
            UpdateChecker.NewVersionFoundFailed += OnNewVersionFoundFailed;
            await CheckUpdateAsync();
        }
        finally
        {
            UpdateChecker.NewVersionNotFound -= OnNewVersionNotFound;
            UpdateChecker.NewVersionFoundFailed -= OnNewVersionFoundFailed;
        }
    }

    private void OpenDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
    {
        Utils.Utils.Open(".\\");
    }

    private async void CleanDNSCacheToolStripMenuItem_Click(object sender, EventArgs e)
    {
        try
        {
            await Task.Run(() =>
            {
                NativeMethods.RefreshDNSCache();
                DnsUtils.ClearCache();
            });

            NotifyTip(i18N.Translate("DNS cache cleanup succeeded"));
        }
        catch (Exception)
        {
            // ignored
        }
        finally
        {
            StatusText();
        }
    }

    private async void UninstallServiceToolStripMenuItem_Click(object sender, EventArgs e)
    {
        Enabled = false;
        StatusText(i18N.TranslateFormat("Uninstalling {0}", "NF Service"));
        try
        {
            var task = Task.Run(NFController.UninstallDriver);

            if (await task)
                NotifyTip(i18N.TranslateFormat("{0} has been uninstalled", "NF Service"));
        }
        finally
        {
            StatusText();
            Enabled = true;
        }
    }

    private void RemoveNetrixFirewallRulesToolStripMenuItem_Click(object sender, EventArgs e)
    {
        Firewall.RemoveNetrixFwRules();
    }

    private void ShowHideConsoleToolStripMenuItem_Click(object sender, EventArgs e)
    {
        var windowStyles = (WINDOW_STYLE)PInvoke.GetWindowLong(Program.ConsoleHwnd, WINDOW_LONG_PTR_INDEX.GWL_STYLE);
        var visible = windowStyles.HasFlag(WINDOW_STYLE.WS_VISIBLE);
        PInvoke.ShowWindow(Program.ConsoleHwnd, visible ? SHOW_WINDOW_CMD.SW_HIDE : SHOW_WINDOW_CMD.SW_SHOWNOACTIVATE);
    }

    #endregion

    /// <summary>
    ///     Menu: force exit
    /// </summary>
    private void ForceExitToolStripMenuItem_Click(object sender, EventArgs e)
    {
        Exit(true);
    }

    private void VersionLabel_Click(object sender, EventArgs e)
    {
        Utils.Utils.Open($"https://github.com/{UpdateChecker.Owner}/{UpdateChecker.Repo}/releases");
    }

    private async void NewVersionLabel_Click(object sender, EventArgs e)
    {
        if (ModifierKeys == Keys.Control || !UpdateChecker.LatestRelease!.assets.Any())
        {
            Utils.Utils.Open(UpdateChecker.LatestVersionUrl!);
            return;
        }

        if (MessageBoxX.Show(i18N.Translate($"Download and install now?\n\n{UpdateChecker.GetLatestReleaseContent()}"), confirm: true) !=
            DialogResult.OK)
            return;

        NotifyTip(i18N.Translate("Start downloading new version"));
        NewVersionLabel.Enabled = false;
        NewVersionLabel.Text = "...";

        try
        {
            var progress = new Progress<int>();
            progress.ProgressChanged += (_, percentage) => { NewVersionLabel.Text = $"{percentage}%"; };

            string downloadDirectory = Path.Combine(Global.NetrixDir, "data");

            var (updateFileName, sha256) = UpdateChecker.GetLatestUpdateFileNameAndHash();
            // the release body lists the hash by file name: take the asset with that name, not whichever is first
            var updateFileUrl = UpdateChecker.LatestRelease.assets.FirstOrDefault(a => a.name == updateFileName)?.browser_download_url ??
                                throw new MessageException($"Release has no asset named {updateFileName}");

            var updateFileFullName = Path.Combine(downloadDirectory, updateFileName);
            var updater = new Updater(updateFileFullName, Global.NetrixDir);

            var downloaded = false;
            if (File.Exists(updateFileFullName))
            {
                var fileHash = await Utils.Utils.Sha256CheckSumAsync(updateFileFullName);
                if (fileHash.Equals(sha256, StringComparison.OrdinalIgnoreCase))
                    downloaded = true;
                else
                    File.Delete(updateFileFullName);
            }

            if (!downloaded)
            {
                try
                {
                    await WebUtil.DownloadFileAsync(updateFileUrl, updateFileFullName, progress);
                }
                catch (Exception e1)
                {
                    Log.Warning(e1, "Download Update File Failed");
                    throw new MessageException($"Download Update File Failed: {e1.Message}");
                }

                var fileHash = await Utils.Utils.Sha256CheckSumAsync(updateFileFullName);
                if (!fileHash.Equals(sha256, StringComparison.OrdinalIgnoreCase))
                {
                    File.Delete(updateFileFullName);
                    throw new MessageException(i18N.Translate("The downloaded file has the wrong hash"));
                }
            }

            // the hash comes from the same release as the file: also require the signature made with the key that is not on GitHub
            var signatureUrl = UpdateChecker.LatestRelease.assets.FirstOrDefault(a => a.name == updateFileName + ".sig")?.browser_download_url ??
                               throw new MessageException(i18N.Translate("The release is not signed, so Netrix will not install it. Download it from the release page."));

            var (_, signature) = await WebUtil.DownloadStringAsync(WebUtil.CreateRequest(signatureUrl));
            if (!UpdateSignature.Verify(updateFileFullName, signature))
            {
                File.Delete(updateFileFullName);
                throw new MessageException(i18N.Translate("The update has an invalid signature and was not installed"));
            }

            await StopAsync();
            await Configuration.SaveAsync();

            // Update
            await Task.Run(updater.ApplyUpdate);

            // release mutex, exit
            Program.SingleInstance.Dispose();
            Process.Start(Global.NetrixExecutable);
            Environment.Exit(0);
        }
        catch (MessageException exception)
        {
            NotifyTip(exception.Message, info: false);
        }
        catch (Exception exception)
        {
            Log.Error(exception, "Unhandled Update error");
            NotifyTip(exception.Message, info: false);
        }
        finally
        {
            NewVersionLabel.Visible = false;
            NewVersionLabel.Enabled = true;
        }
    }

    private void AboutToolStripButton_Click(object sender, EventArgs e)
    {
        Hide();
        new AboutForm().ShowDialog();
        Show();
    }

    private void DocumentationToolStripMenuItem_Click(object sender, EventArgs e)
    {
        new HelpForm().ShowDialog(this);
    }

    private void fAQToolStripMenuItem_Click(object sender, EventArgs e)
    {
        Utils.Utils.Open("https://github.com/psnwd/netrix/issues");
    }

    #endregion

    #region ControlButton

    private async void ControlButton_Click(object? sender, EventArgs? e)
    {
        if (!IsWaiting())
        {
            await StopCoreAsync();
            return;
        }

        Configuration.SaveAsync().Forget();

        // A server and a mode must be selected
        if (ServerComboBox.SelectedItem is not Server server)
        {
            MessageBoxX.Show(i18N.Translate("Please select a server first"));
            return;
        }

        if (ModeComboBox.SelectedItem is not Mode mode)
        {
            MessageBoxX.Show(i18N.Translate("Please select a mode first"));
            return;
        }

        State = State.Starting;

        try
        {
            await MainController.StartAsync(server, mode);
        }
        catch (Exception exception)
        {
            State = State.Stopped;
            StatusText(i18N.Translate("Start failed"));
            MessageBoxX.Show(exception.Message, LogLevel.ERROR);
            return;
        }

        State = State.Started;

        Task.Run(Bandwidth.NetTraffic).Forget();
        DiscoveryNatTypeAsync().Forget();
        HttpConnectAsync().Forget();

        if (Global.Settings.MinimizeWhenStarted)
            Minimize();

        // Test the delay automatically
        async Task StartedPingAsync()
        {
            while (State == State.Started)
            {
                if (Global.Settings.StartedPingInterval >= 0)
                {
                    await server.PingAsync();
                    ServerComboBox.Refresh();

                    await Task.Delay(Global.Settings.StartedPingInterval * 1000);
                }
                else
                {
                    await Task.Delay(5000);
                }
            }
        }

        StartedPingAsync().Forget();
    }

    #endregion

    #region SettingsButton

    private void SettingsButton_Click(object sender, EventArgs e)
    {
        var oldSettings = Global.Settings.ShallowCopy();

        Hide();
        new SettingForm().ShowDialog();

        if (oldSettings.Language != Global.Settings.Language)
        {
            i18N.Load(Global.Settings.Language);
            TranslateControls();
            LoadModes();
            LoadProfiles();
        }

        if (oldSettings.DetectionTick != Global.Settings.DetectionTick)
            DelayTestHelper.UpdateTick(true);

        if (oldSettings.ProfileCount != Global.Settings.ProfileCount)
            LoadProfiles();

        Show();
    }

    #endregion

    #region Server

    private void LoadServers()
    {
        ServerComboBox.Items.Clear();
        ServerComboBox.Items.AddRange(Global.Settings.Server.Cast<object>().ToArray());
        SelectLastServer();
    }

    private void SelectLastServer()
    {
        // If the value is valid, select that position
        if (Global.Settings.ServerComboBoxSelectedIndex > 0 && Global.Settings.ServerComboBoxSelectedIndex < ServerComboBox.Items.Count)
            ServerComboBox.SelectedIndex = Global.Settings.ServerComboBoxSelectedIndex;
        // If the value is invalid and ServerComboBox has items, select the first one
        else if (ServerComboBox.Items.Count > 0)
            ServerComboBox.SelectedIndex = 0;

        // If ServerComboBox is empty, do nothing
    }

    private void ServerComboBox_SelectionChangeCommitted(object sender, EventArgs o)
    {
        Global.Settings.ServerComboBoxSelectedIndex = ServerComboBox.SelectedIndex;
    }

    private async void EditServerPictureBox_Click(object sender, EventArgs e)
    {
        // ServerComboBox has at least one item
        if (!(ServerComboBox.SelectedItem is Server server))
        {
            MessageBoxX.Show(i18N.Translate("Please select a server first"));
            return;
        }

        Hide();
        ServerHelper.GetUtilByTypeName(server.Type).Edit(server);
        LoadServers();
        await Configuration.SaveAsync();
        Show();
    }

    private async void SpeedPictureBox_Click(object sender, EventArgs e)
    {
        void Enable()
        {
            ServerComboBox.Refresh();
            Enabled = true;
            StatusText();
        }

        Enabled = false;
        StatusText(i18N.Translate("Testing"));

        if (!IsWaiting() || ModifierKeys == Keys.Control)
        {
            (ServerComboBox.SelectedItem as Server)?.PingAsync();
            Enable();
        }
        else
        {
            await DelayTestHelper.PerformTestAsync(true);
            Enable();
        }
    }

    private void CopyLinkPictureBox_Click(object sender, EventArgs e)
    {
        // ServerComboBox has at least one item
        if (!(ServerComboBox.SelectedItem is Server server))
        {
            MessageBoxX.Show(i18N.Translate("Please select a server first"));
            return;
        }

        try
        {
            //Windows APIs fail now and then, so catch it :D
            string text;
            if (ModifierKeys == Keys.Control)
                text = ShareLink.GetNetrixLink(server);
            else
                text = ShareLink.GetShareLink(server);

            Clipboard.SetText(text);
        }
        catch (Exception)
        {
            // ignored
        }
    }

    private void DeleteServerPictureBox_Click(object sender, EventArgs e)
    {
        // ServerComboBox has at least one item
        if (!(ServerComboBox.SelectedItem is Server server))
        {
            MessageBoxX.Show(i18N.Translate("Please select a server first"));
            return;
        }

        Global.Settings.Server.Remove(server);
        LoadServers();
    }

    #endregion

    #region Mode

    public void LoadModes()
    {
        if (InvokeRequired)
        {
            Invoke(LoadModes);
            return;
        }

        ModeComboBox.Items.Clear();
        ModeComboBox.Items.AddRange(Global.Modes.Cast<object>().ToArray());
        ModeComboBox.Tag = null;
        SelectLastMode();
    }

    private void SelectLastMode()
    {
        // If the value is valid, select that position
        if (Global.Settings.ModeComboBoxSelectedIndex > 0 && Global.Settings.ModeComboBoxSelectedIndex < ModeComboBox.Items.Count)
            ModeComboBox.SelectedIndex = Global.Settings.ModeComboBoxSelectedIndex;
        // If the value is invalid and ModeComboBox has items, select the first one
        else if (ModeComboBox.Items.Count > 0)
            ModeComboBox.SelectedIndex = 0;

        // If ModeComboBox is empty, do nothing
    }

    private void ModeComboBox_SelectionChangeCommitted(object sender, EventArgs o)
    {
        try
        {
            Global.Settings.ModeComboBoxSelectedIndex = Global.Modes.IndexOf((Mode)ModeComboBox.SelectedItem);
        }
        catch
        {
            Global.Settings.ModeComboBoxSelectedIndex = 0;
        }
    }

    private void EditModePictureBox_Click(object sender, EventArgs e)
    {
        // ModeComboBox has at least one item
        if (ModeComboBox.SelectedIndex == -1)
        {
            MessageBoxX.Show(i18N.Translate("Please select a mode first"));
            return;
        }

        var mode = (Mode)ModeComboBox.SelectedItem;
        if (ModifierKeys == Keys.Control)
        {
            Utils.Utils.Open(mode.FullName);
            return;
        }

        switch (mode.Type)
        {
            case ModeType.ProcessMode:
                Hide();
                new ProcessForm(mode).ShowDialog();
                Show();
                break;
            case ModeType.TunMode:
                Hide();
                new RouteForm(mode).ShowDialog();
                Show();
                break;
            case ModeType.ShareMode:
            // throw new NotImplementedException();
            default:
                Utils.Utils.Open(mode.FullName);
                break;
        }
    }

    private void DeleteModePictureBox_Click(object sender, EventArgs e)
    {
        // ModeComboBox has at least one item
        if (ModeComboBox.Items.Count <= 0 || ModeComboBox.SelectedIndex == -1)
        {
            MessageBoxX.Show(i18N.Translate("Please select a mode first"));
            return;
        }

        ModeService.Delete((Mode)ModeComboBox.SelectedItem);
        SelectLastMode();
    }

    #endregion

    #region Profile

    private int _configurationGroupBoxHeight;
    private int _profileConfigurationHeight;
    private int _profileGroupBoxPaddingHeight;
    private int _profileTableHeight;

    private void LoadProfiles()
    {
        // Clear
        foreach (var button in ProfileTable.Controls)
            ((Button)button).Dispose();

        ProfileTable.Controls.Clear();
        ProfileTable.ColumnStyles.Clear();
        ProfileTable.RowStyles.Clear();

        var profileCount = Global.Settings.ProfileCount;
        if (profileCount == 0)
        {
            // Hide Profile GroupBox, Change window size
            configLayoutPanel.RowStyles[2].SizeType = SizeType.Percent;
            configLayoutPanel.RowStyles[2].Height = 0;
            ProfileGroupBox.Visible = false;

            ConfigurationGroupBox.Height = _configurationGroupBoxHeight - _profileConfigurationHeight;
        }
        else
        {
            // Load Profiles

            if (Global.Settings.ProfileTableColumnCount == 0)
                Global.Settings.ProfileTableColumnCount = 5;

            var columnCount = Global.Settings.ProfileTableColumnCount;

            ProfileTable.ColumnCount = profileCount >= columnCount ? columnCount : profileCount;
            ProfileTable.RowCount = (int)Math.Ceiling(profileCount / (float)columnCount);

            for (var i = 0; i < profileCount; ++i)
            {
                var profile = Global.Settings.Profiles.SingleOrDefault(p => p.Index == i);
                var b = new Button
                {
                    Dock = DockStyle.Fill,
                    Text = profile?.ProfileName ?? i18N.Translate("None"),
                    Tag = profile
                };

                b.Click += ProfileButton_Click;
                _tips.SetToolTip(b, i18N.Translate("Quick-start profile\nClick: use the saved server and mode, and start\nCtrl+Click: save the current server and mode here\nShift+Click: delete"));
                ProfileTable.Controls.Add(b, i % columnCount, i / columnCount);
            }

            // equal column
            for (var i = 1; i <= ProfileTable.RowCount; i++)
                ProfileTable.RowStyles.Add(new RowStyle(SizeType.Percent, 1));

            for (var i = 1; i <= ProfileTable.ColumnCount; i++)
                ProfileTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 1));

            configLayoutPanel.RowStyles[2].SizeType = SizeType.AutoSize;
            ProfileGroupBox.Visible = true;
            ProfileGroupBox.Height = ProfileTable.RowCount * _profileTableHeight + _profileGroupBoxPaddingHeight;
            ConfigurationGroupBox.Height = _configurationGroupBoxHeight;
        }
    }

    private async void ProfileButton_Click([NotNull] object? sender, EventArgs? e)
    {
        if (sender == null)
            throw new InvalidOperationException();

        var button = (Button)sender;
        var profile = (Profile?)button.Tag;
        var index = ProfileTable.Controls.IndexOf(button);

        switch (ModifierKeys)
        {
            case Keys.Control:
                // Save Profile
                if (ServerComboBox.SelectedItem is not Server server)
                {
                    MessageBoxX.Show(i18N.Translate("Please select a server first"));
                    return;
                }

                if (ModeComboBox.SelectedItem is not Mode mode)
                {
                    MessageBoxX.Show(i18N.Translate("Please select a mode first"));
                    return;
                }

                var name = ProfileNameText.Text;

                Global.Settings.Profiles.RemoveAll(p => p.Index == index);
                profile = new Profile(server, mode, name, index);
                Global.Settings.Profiles.Add(profile);
                button.Tag = profile;
                button.Text = profile.ProfileName;

                ProfileNameText.Clear();
                return;
            case Keys.Shift:
                // Delete Profile
                if (profile == null)
                    return;

                Global.Settings.Profiles.Remove(profile);
                button.Tag = null;
                button.Text = i18N.Translate("None");
                return;
        }

        // Activate Profile

        if (profile == null)
        {
            MessageBoxX.Show(i18N.Translate("No saved profile here. Save a profile first by Ctrl+Click on the button"));
            return;
        }

        try
        {
            ProfileNameText.Text = profile.ProfileName;

            var server = ServerComboBox.Items.Cast<Server>().FirstOrDefault(s => s.Remark.Equals(profile.ServerRemark));
            var mode = ModeComboBox.Items.Cast<Mode>().FirstOrDefault(m => m.Remark.Any(s => s.Value.Equals(profile.ModeRemark)));

            if (server == null)
                throw new MessageException("Server not found.");

            if (mode == null)
                throw new MessageException("Mode not found.");

            // set active server and mode
            ServerComboBox.SelectedItem = server;
            ModeComboBox.SelectedItem = mode;
        }
        catch (MessageException exception)
        {
            MessageBoxX.Show(exception.Message, LogLevel.ERROR);
            return;
        }

        await StopAsync();
        ControlButton.PerformClick();
    }

    #endregion

    #region State

    private State _state = State.Waiting;

    /// <summary>
    ///     Current state
    /// </summary>
    public State State
    {
        get => _state;
        private set
        {
            void StartDisableItems(bool enabled)
            {
                ServerComboBox.Enabled = ModeComboBox.Enabled = EditModePictureBox.Enabled =
                    EditServerPictureBox.Enabled = DeleteModePictureBox.Enabled = DeleteServerPictureBox.Enabled = enabled;

                // Controls to disable while starting
                ServerToolStripMenuItem.Enabled = ModeToolStripMenuItem.Enabled =
                    SubscriptionToolStripMenuItem.Enabled = UninstallServiceToolStripMenuItem.Enabled = enabled;
            }

            _state = value;

            DelayTestHelper.Enabled = IsWaiting(_state);

            StatusText();
            switch (value)
            {
                case State.Waiting:
                    ControlButton.Enabled = true;
                    ControlButton.Text = i18N.Translate("Start");

                    break;
                case State.Starting:
                    ControlButton.Enabled = false;
                    ControlButton.Text = "...";

                    ProfileGroupBox.Enabled = false;
                    StartDisableItems(false);
                    break;
                case State.Started:
                    ControlButton.Enabled = true;
                    ControlButton.Text = i18N.Translate("Stop");

                    ProfileGroupBox.Enabled = true;

                    break;
                case State.Stopping:
                    ControlButton.Enabled = false;
                    ControlButton.Text = "...";

                    ProfileGroupBox.Enabled = false;
                    BandwidthState(false);
                    ConnectivityStatusVisible(false);
                    break;
                case State.Stopped:
                    ControlButton.Enabled = true;
                    ControlButton.Text = i18N.Translate("Start");

                    LastUploadBandwidth = 0;
                    LastDownloadBandwidth = 0;
                    Bandwidth.Stop();

                    ProfileGroupBox.Enabled = true;
                    StartDisableItems(true);
                    break;
            }
        }
    }

    public async Task StopAsync()
    {
        if (IsWaiting())
            return;

        await StopCoreAsync();
    }

    private async Task StopCoreAsync()
    {
        State = State.Stopping;

        try
        {
            // CancelAsync: Cancel() runs the callbacks of a stuck NAT/HTTP test on this (UI) thread
            if (_discoveryNatCts is { } nat)
                await nat.CancelAsync();

            if (_httpConnectCts is { } http)
                await http.CancelAsync();

            await MainController.StopAsync();
        }
        catch (Exception e)
        {
            Log.Error(e, "Stop failed");
        }
        finally
        {
            // never stay in "Stopping": the Start button would stay disabled and the window looks frozen
            State = State.Stopped;
        }
    }

    private bool IsWaiting() => IsWaiting(_state);

    private static bool IsWaiting(State state)
    {
        return state is State.Waiting or State.Stopped;
    }

    /// <summary>
    ///     Update the status bar text
    /// </summary>
    /// <param name="text"></param>
    public void StatusText(string? text = null)
    {
        if (InvokeRequired)
        {
            BeginInvoke(() => StatusText(text));
            return;
        }

        text ??= i18N.Translate(StateExtension.GetStatusString(State));
        if (_state == State.Started)
            text += StatusPortInfoText.Value;

        StatusLabel.Text = i18N.Translate("Status", ": ") + text;
    }

    public void BandwidthState(bool state)
    {
        if (InvokeRequired)
        {
            BeginInvoke(() => BandwidthState(state));
            return;
        }

        if (IsWaiting())
            return;

        UsedBandwidthLabel.Visible = DownloadSpeedLabel.Visible = state;
        if (!state)
            UploadSpeedLabel.Visible = false;
    }

    private void UpdateNatTypeStatusLabelText(string? text, string? country = null)
    {
        if (!string.IsNullOrEmpty(text))
        {
            if (country == null)
                NatTypeStatusLabel.Text = $"NAT{i18N.Translate(": ")}{text} ";
            else
                NatTypeStatusLabel.Text = $"NAT{i18N.Translate(": ")}{text} [{country}]";

            UpdateNatTypeLight(int.TryParse(text, out var natType) ? natType : -1);
        }
        else
        {
            NatTypeStatusLabel.Text = $@"NAT{i18N.Translate(": ", "Test failed")}";
        }

        NatTypeStatusLabel.Visible = true;
    }

    private void ConnectivityStatusVisible(bool visible)
    {
        if (!visible)
            HttpStatusLabel.Text = NatTypeStatusLabel.Text = "";

        HttpStatusLabel.Visible = NatTypeStatusLabel.Visible = NatTypeStatusLightLabel.Visible = visible;
    }

    /// <summary>
    ///     Update the NAT indicator color
    /// </summary>
    /// <param name="natType">NAT Type. keep default(-1) to Hide Light</param>
    private void UpdateNatTypeLight(int natType = -1)
    {
        if (natType > 0 && natType < 5)
        {
            NatTypeStatusLightLabel.Visible = Flags.IsWindows10Upper;
            var c = natType switch
            {
                1 => Color.LimeGreen,
                2 => Color.Yellow,
                3 => Color.Red,
                4 => Color.Black,
                _ => throw new ArgumentOutOfRangeException(nameof(natType), natType, null)
            };

            NatTypeStatusLightLabel.ForeColor = c;
        }
        else
        {
            NatTypeStatusLightLabel.Visible = false;
        }
    }

    private async void TcpStatusLabel_Click(object sender, EventArgs e)
    {
        await HttpConnectAsync();
    }

    private async void NatTypeStatusLabel_Click(object sender, EventArgs e)
    {
        await DiscoveryNatTypeAsync();
    }

    private CancellationTokenSource? _discoveryNatCts;

    private async Task DiscoveryNatTypeAsync()
    {
        NatTypeStatusLabel.Enabled = false;
        UpdateNatTypeStatusLabelText(i18N.Translate("Testing NAT Type"));

        _discoveryNatCts = new CancellationTokenSource();

        try
        {
            var res = await MainController.DiscoveryNatTypeAsync(_discoveryNatCts.Token);
            if (_discoveryNatCts.IsCancellationRequested)
                return;

            if (!string.IsNullOrEmpty(res.PublicEnd))
            {
                var country = await Utils.Utils.GetCityCodeAsync(res.PublicEnd);

                UpdateNatTypeStatusLabelText(res.Result, country);
                if (int.TryParse(res.Result, out var natType))
                    UpdateNatTypeLight(natType);
                else
                    UpdateNatTypeLight();
            }
            else
            {
                UpdateNatTypeStatusLabelText(res.Result ?? "Error");
                NatTypeStatusLightLabel.Visible = false;
            }
        }
        finally
        {
            _discoveryNatCts.Dispose();
            _discoveryNatCts = null;
            NatTypeStatusLabel.Enabled = true;
        }
    }

    private CancellationTokenSource? _httpConnectCts;

    private async Task HttpConnectAsync()
    {
        HttpStatusLabel.Enabled = false;

        _httpConnectCts = new CancellationTokenSource();

        try
        {
            var res = await MainController.HttpConnectAsync(_httpConnectCts.Token);
            if (_httpConnectCts.IsCancellationRequested)
                return;

            if (res != null)
                HttpStatusLabel.Text = $"HTTP{i18N.Translate(": ")}{res}ms";
            else
                HttpStatusLabel.Text = $"HTTP{i18N.Translate(": ", "Timeout")}";

            HttpStatusLabel.Visible = true;
        }
        finally
        {
            _httpConnectCts.Dispose();
            _httpConnectCts = null;
            HttpStatusLabel.Enabled = true;
        }
    }

    #endregion

    #endregion

    #region Misc

    #region PowerEvent

    private bool _resumeFlag;

    private async void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
    {
        switch (e.Mode)
        {
            case PowerModes.Suspend: //The operating system is about to suspend
                if (!IsWaiting())
                {
                    _resumeFlag = true;
                    Log.Information("OS Suspend, Stop");
                    await StopAsync();
                }

                break;
            case PowerModes.Resume: //The operating system is about to resume from suspend
                if (_resumeFlag)
                {
                    _resumeFlag = false;
                    Log.Information("OS Resume, Restart");
                    ControlButton.PerformClick();
                }

                break;
        }
    }

    #endregion

    private void Minimize()
    {
        // Shrink the window toward the bottom-right corner on close
        WindowState = FormWindowState.Minimized;

        if (_isFirstCloseWindow)
        {
            // Show the hint
            NotifyTip(i18N.Translate("Netrix is now minimized to the notification bar, double click this icon to restore."));
            _isFirstCloseWindow = false;
        }

        Hide();
    }

    public async void Exit(bool forceExit = false, bool saveConfiguration = true)
    {
        if (!IsWaiting() && !Global.Settings.StopWhenExited && !forceExit)
        {
            MessageBoxX.Show(i18N.Translate("Please press Stop button first"));

            ShowMainFormToolStripButton.PerformClick();
            return;
        }

        // State = State.Terminating;
        NotifyIcon.Visible = false;
        Hide();

        if (saveConfiguration)
            await Configuration.SaveAsync();

        foreach (var file in new[] { Constants.TempConfig, Constants.TempRouteFile })
            if (File.Exists(file))
                File.Delete(file);

        await StopAsync();

        Dispose();
        Environment.Exit(Environment.ExitCode);
    }

    #region FormClosingButton

    private bool _isFirstCloseWindow = true;

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (e.CloseReason == CloseReason.UserClosing && State != State.Terminating)
        {
            // Cancel the "close window" event
            e.Cancel = true; // Cancel closing the form 

            // If "Exit when closed" is off, hide to the tray icon
            if (!Global.Settings.ExitWhenClosed)
                Minimize();
            // If "Exit when closed" is on, click the Exit button
            else
                Exit();
        }
    }

    #endregion

    #region Updater

    private async Task CheckUpdateAsync()
    {
        try
        {
            UpdateChecker.NewVersionFound += OnUpdateCheckerOnNewVersionFound;
            await UpdateChecker.CheckAsync(Global.Settings.CheckBetaUpdate);
            if (Flags.AlwaysShowNewVersionFound)
                OnUpdateCheckerOnNewVersionFound(null!, null!);
        }
        finally
        {
            UpdateChecker.NewVersionFound -= OnUpdateCheckerOnNewVersionFound;
        }

        void OnUpdateCheckerOnNewVersionFound(object? o, EventArgs? eventArgs)
        {
            NotifyTip($"{i18N.Translate(@"New version available", ": ")}{UpdateChecker.LatestVersionNumber}");
            NewVersionLabel.Text = i18N.Translate("New version available");
            NewVersionLabel.Enabled = true;
            NewVersionLabel.Visible = true;
        }
    }

    #endregion

    #region NetTraffic

    /// <summary>
    ///     Download traffic at the last update
    /// </summary>
    public ulong LastDownloadBandwidth;

    /// <summary>
    ///     Upload traffic at the last update
    /// </summary>
    public ulong LastUploadBandwidth;

    public void OnBandwidthUpdated(ulong download, ulong? upload = null)
    {
        if (InvokeRequired)
        {
            BeginInvoke(() => OnBandwidthUpdated(download, upload));
            return;
        }

        try
        {
            UsedBandwidthLabel.Text = $"{i18N.Translate("Used", ": ")}{Bandwidth.Compute(download)}";
            // ETW only sees received bytes, so without an upload value the single label stays "↑↓"
            DownloadSpeedLabel.Text = $"{(upload is null ? "↑↓" : "↓")}: {Bandwidth.Compute(download - LastDownloadBandwidth)}/s";
            if (upload is { } up)
            {
                UploadSpeedLabel.Visible = true;
                UploadSpeedLabel.Text = $"↑: {Bandwidth.Compute(up - LastUploadBandwidth)}/s";
                LastUploadBandwidth = up;
            }

            LastDownloadBandwidth = download;
            Refresh();
        }
        catch
        {
            // ignored
        }
    }

    #endregion

    #region NotifyIcon

    private void ShowMainFormToolStripButton_Click(object sender, EventArgs e)
    {
        Utils.Utils.ActivateVisibleWindows();
    }

    /// <summary>
    ///     Tray icon menu: exit
    /// </summary>
    private void ExitToolStripButton_Click(object sender, EventArgs e)
    {
        Exit();
    }

    private void NotifyIcon_MouseDoubleClick(object? sender, MouseEventArgs? e)
    {
        ShowMainFormToolStripButton.PerformClick();
    }

    public void NotifyTip(string text, int timeout = 0, bool info = true)
    {
        // Blocks the thread for timeout seconds (?)
        NotifyIcon.ShowBalloonTip(timeout, UpdateChecker.Name, text, info ? ToolTipIcon.Info : ToolTipIcon.Error);
    }

    #endregion

    #region ComboBox_DrawItem

    private readonly SolidBrush _greenBrush = new(Color.FromArgb(50, 255, 56));
    private int _numberBoxWidth;
    private int _numberBoxX;
    private int _numberBoxWrap;

    private void ComboBox_DrawItem(object sender, DrawItemEventArgs e)
    {
        if (sender is not ComboBox cbx)
            return;

        // Draw the background color
        e.Graphics.FillRectangle(Brushes.White, e.Bounds);

        if (e.Index < 0)
            return;

        // Draw the remark / name string
        TextRenderer.DrawText(e.Graphics, cbx.Items[e.Index].ToString(), cbx.Font, e.Bounds, Color.Black, TextFormatFlags.Left);

        switch (cbx.Items[e.Index])
        {
            case Server item:
            {
                // Compute the delay background color
                var numBoxBackBrush = item.Delay switch { > 200 => Brushes.Red, > 80 => Brushes.Yellow, >= 0 => _greenBrush, _ => Brushes.Gray };

                // Draw the delay background
                e.Graphics.FillRectangle(numBoxBackBrush, _numberBoxX, e.Bounds.Y, _numberBoxWidth, e.Bounds.Height);

                // Draw the delay string
                TextRenderer.DrawText(e.Graphics,
                    item.Delay.ToString(),
                    cbx.Font,
                    new Point(_numberBoxX + _numberBoxWrap, e.Bounds.Y),
                    Color.Black,
                    TextFormatFlags.Left);

                break;
            }
            case Mode item:
            {
                /*
                // Draw the mode box background
                e.Graphics.FillRectangle(Brushes.Gray, _numberBoxX, e.Bounds.Y, _numberBoxWidth, e.Bounds.Height);

                // Draw the mode line count string
                TextRenderer.DrawText(e.Graphics,
                    item.Content.Count.ToString(),
                    cbx.Font,
                    new Point(_numberBoxX + _numberBoxWrap, e.Bounds.Y),
                    Color.Black,
                    TextFormatFlags.Left);
                    */

                break;
            }
        }
    }

    #endregion

    #endregion
}