using System.Net;
using Netch.Controllers;
using Netch.Enums;
using Netch.Models.Modes;
using Netch.Models.Modes.ProcessMode;
using Netch.Properties;
using Netch.Services;
using Netch.Utils;

namespace Netch.Forms.ModeForms;

[Fody.ConfigureAwait(true)]
public partial class ProcessForm : BindingForm
{
    private readonly bool IsCreateMode;

    private readonly Redirector _mode;

    /// <summary>
    ///     编辑模式
    /// </summary>
    /// <param name="mode">模式</param>
    public ProcessForm(Mode? mode = null)
    {
        switch (mode)
        {
            case Redirector processMode:
                IsCreateMode = false;
                _mode = processMode;
                break;
            case null:
                IsCreateMode = true;
                _mode = new Redirector();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        InitializeComponent();

        Tips.Create(this,
            (RemarkLabel, "Name of this mode as shown in the mode list."),
            (RemarkTextBox, "Name of this mode as shown in the mode list."),
            (FilenameLabel, "File the mode is saved in, inside the mode folder."),
            (FilenameTextBox, "File the mode is saved in, inside the mode folder."),
            (HandleLabel, "Programs that go through the proxy. One rule per line. A rule is a regular expression matched on the program path, e.g. chrome.exe"),
            (HandleRuleRichTextBox, "Programs that go through the proxy. One rule per line. A rule is a regular expression matched on the program path, e.g. chrome.exe"),
            (BypassLabel, "Programs that never go through the proxy, even if a Handle rule matches. One rule per line."),
            (BypassRuleRichTextBox, "Programs that never go through the proxy, even if a Handle rule matches. One rule per line."),
            (HandleSelectButton, "Choose a folder: adds a rule for every program inside it."),
            (BypassSelectButton, "Choose a folder: adds a rule for every program inside it."),
            (HandleScanButton, "Choose a folder: lists the .exe files inside it as rules (up to 50)."),
            (BypassScanButton, "Choose a folder: lists the .exe files inside it as rules (up to 50)."),
            (ValidationButton, "Check that the Bypass rules are valid."),
            (HandleTCPCheckBox, "Send the TCP traffic of these programs through the proxy (web, most apps)."),
            (HandleUDPCheckBox, "Send the UDP traffic of these programs through the proxy (games, voice). The server must support UDP."),
            (HandleICMPCheckBox, "Also handle ping (ICMP)."),
            (ICMPDelayLabel, "Delay added to ping replies in milliseconds."),
            (ICMPDelayTextBox, "Delay added to ping replies in milliseconds."),
            (HandleDNSCheckBox, "Redirect the DNS lookups of these programs to the DNS server below."),
            (DNSLabel, "DNS server used when DNS handling is on, as address:port, e.g. 1.1.1.1:53."),
            (DNSTextBox, "DNS server used when DNS handling is on, as address:port, e.g. 1.1.1.1:53."),
            (HandleProcDNSCheckBox, "Only redirect DNS lookups made by the handled programs."),
            (ProxyDNSCheckBox, "Send the DNS lookups through the proxy."),
            (HandleLoopbackCheckBox, "Also handle traffic to this PC itself (127.0.0.1)."),
            (HandleLANCheckBox, "Also handle traffic to your local network. Off = LAN traffic stays direct."),
            (HandleChildProcCheckBox, "Also handle programs started by these programs."),
            (ControlButton, "Save the mode."));
        Icon = Resources.icon;
        InitBindings();

        var g = Global.Settings.Redirector;
        BindTextBox(RemarkTextBox, _ => true, s => _mode.i18NRemark = s, _mode.i18NRemark);
        BindSyncGlobalCheckBox(HandleTCPCheckBox, b => _mode.FilterTCP = b, _mode.FilterTCP, g.FilterTCP);
        BindSyncGlobalCheckBox(HandleUDPCheckBox, b => _mode.FilterUDP = b, _mode.FilterUDP, g.FilterUDP);
        BindSyncGlobalCheckBox(HandleDNSCheckBox, b => _mode.FilterDNS = b, _mode.FilterDNS, g.FilterDNS);
        BindTextBox(DNSTextBox, s => IPEndPoint.TryParse(s, out _), s => _mode.DNSHost = s, _mode.DNSHost ?? $"{Constants.DefaultPrimaryDNS}:53");

        BindSyncGlobalCheckBox(HandleProcDNSCheckBox, b => _mode.HandleOnlyDNS = b, _mode.HandleOnlyDNS, g.HandleOnlyDNS);
        BindSyncGlobalCheckBox(ProxyDNSCheckBox, b => _mode.DNSProxy = b, _mode.DNSProxy, g.DNSProxy);
        BindSyncGlobalCheckBox(HandleICMPCheckBox, b => _mode.FilterICMP = b, _mode.FilterICMP, g.FilterICMP);
        BindTextBox<int>(ICMPDelayTextBox, s => s >= 0, s => _mode.ICMPDelay = s, _mode.ICMPDelay ?? 10);
        BindCheckBox(HandleLoopbackCheckBox, b => _mode.FilterLoopback = b, _mode.FilterLoopback);
        BindCheckBox(HandleLANCheckBox, b => _mode.FilterIntranet = b, _mode.FilterIntranet);
        BindSyncGlobalCheckBox(HandleChildProcCheckBox, b => _mode.FilterParent = b, _mode.FilterParent, g.FilterParent);

        BindTextBox(BypassRuleRichTextBox, s => true, s => _mode.Bypass = s.GetLines().ToList(), string.Join(Constants.EOF, _mode.Bypass));
        BindTextBox(HandleRuleRichTextBox, s => true, s => _mode.Handle = s.GetLines().ToList(), string.Join(Constants.EOF, _mode.Handle));
    }

    private void InitBindings()
    {
        DNSTextBox.DataBindings.Add(new Binding("Enabled", HandleDNSCheckBox, "Checked", true));
        HandleProcDNSCheckBox.DataBindings.Add(new Binding("Enabled", HandleDNSCheckBox, "Checked", true));
        ProxyDNSCheckBox.DataBindings.Add(new Binding("Enabled", HandleDNSCheckBox, "Checked", true));
        ICMPDelayTextBox.DataBindings.Add(new Binding("Enabled", HandleICMPCheckBox, "Checked", true));
    }

    public void ModeForm_Load(object sender, EventArgs e)
    {
        if (!IsCreateMode)
        {
            Text = "Edit Process Mode";

            RemarkTextBox.TextChanged -= RemarkTextBox_TextChanged;
            RemarkTextBox.Text = _mode.i18NRemark;
            FilenameTextBox.Text = ModeService.Instance.GetRelativePath(_mode.FullName);

            if (!_mode.FullName.EndsWith(".json"))
                ControlButton.Enabled = false;
        }

        i18N.TranslateForm(this);
    }

    private void SelectButton_Click(object sender, EventArgs e)
    {
        RichTextBox ruleRichTextBox;
        if (sender == HandleSelectButton)
            ruleRichTextBox = HandleRuleRichTextBox;
        else if (sender == BypassSelectButton)
            ruleRichTextBox = BypassRuleRichTextBox;
        else
        {
            throw new InvalidOperationException();
        }

        var dialog = new FolderBrowserDialog();

        if (dialog.ShowDialog() == DialogResult.OK)
        {
            var path = dialog.SelectedPath;
            if (!path.EndsWith(@"\"))
                path += @"\";

            AppendText(ruleRichTextBox, $"^{path.ToRegexString()}");
        }
    }

    private static void AppendText(Control ruleTextBox, string value)
    {
        if (ruleTextBox.Text.Any())
            ruleTextBox.Text = ruleTextBox.Text.Trim() + Constants.EOF + value;
        else
            ruleTextBox.Text = value;
    }

    public void ControlButton_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(RemarkTextBox.Text))
        {
            MessageBoxX.Show(i18N.Translate("Please enter a mode remark"));
            return;
        }

        SaveBinds();

        if (IsCreateMode)
        {
            var relativePath = FilenameTextBox.Text;
            var fullName = ModeService.Instance.GetFullPath(relativePath);
            if (File.Exists(fullName))
            {
                MessageBoxX.Show(i18N.Translate("File already exists.\n Please Change the filename"));
                return;
            }

            _mode.FullName = fullName;

            ModeService.Instance.Add(_mode);
            MessageBoxX.Show(i18N.Translate("Mode added successfully"));
        }
        else
        {
            _mode.WriteFile();
            MessageBoxX.Show(i18N.Translate("Mode updated successfully"));
        }
        Global.MainForm.ModeComboBox.SelectedItem = _mode;
        Close();
    }

    private void RemarkTextBox_TextChanged(object? sender, EventArgs? e)
    {
        if (!IsHandleCreated)
            return;

        BeginInvoke(() =>
        {
            FilenameTextBox.Text = FilenameTextBox.Text = ModeEditorUtils.GetCustomModeRelativePath(RemarkTextBox.Text);
        });
    }

    private void ScanButton_Click(object sender, EventArgs e)
    {
        RichTextBox ruleRichTextBox;
        if (sender == HandleScanButton)
            ruleRichTextBox = HandleRuleRichTextBox;
        else if (sender == BypassScanButton)
            ruleRichTextBox = BypassRuleRichTextBox;
        else
        {
            throw new InvalidOperationException();
        }

        var dialog = new FolderBrowserDialog();

        if (dialog.ShowDialog() == DialogResult.OK)
        {
            var path = dialog.SelectedPath;
            var list = new List<string>();
            const uint maxCount = 50;
            try
            {
                ScanDirectory(path, list, maxCount);
            }
            catch
            {
                MessageBoxX.Show(i18N.Translate($"The number of executable files in the \"{path}\" directory is greater than {maxCount}"),
                    LogLevel.WARNING);

                return;
            }

            AppendText(ruleRichTextBox, string.Join(Constants.EOF, list));
        }
    }

    private void ScanDirectory(string directory, List<string> list, uint maxCount = 30)
    {
        foreach (var dir in Directory.GetDirectories(directory))
            ScanDirectory(dir, list, maxCount);

        list.AddRange(
            Directory.GetFiles(directory).Select(s => Path.GetFileName(s)).Where(s => s.EndsWith(".exe")).Select(s => s.ToRegexString()));

        if (maxCount != 0 && list.Count > maxCount)
            throw new Exception("The number of results is greater than maxCount");
    }

    private void ValidationButton_Click(object sender, EventArgs e)
    {
        if (!NFController.CheckRules(_mode.Bypass, out var results))
            MessageBoxX.Show(NFController.GenerateInvalidRulesMessage(results), LogLevel.WARNING);
        else
            MessageBoxX.Show("Fine");
    }
}