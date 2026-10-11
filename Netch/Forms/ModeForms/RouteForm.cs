using Netch.Models.Modes;
using Netch.Models.Modes.TunMode;
using Netch.Properties;
using Netch.Services;
using Netch.Utils;

namespace Netch.Forms.ModeForms;

[Fody.ConfigureAwait(true)]
public partial class RouteForm : BindingForm
{
    private readonly bool IsCreateMode;

    private readonly TunMode _mode;

    public RouteForm(Mode? mode = null)
    {
        switch (mode)
        {
            case null:
                IsCreateMode = true;
                _mode = new TunMode();
                break;
            case TunMode tunMode:
                IsCreateMode = false;
                _mode = tunMode;
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
            (HandleLabel, "IP addresses and ranges that go through the proxy. One per line, e.g. 8.8.8.8 or 1.2.3.0/24."),
            (HandleRuleRichTextBox, "IP addresses and ranges that go through the proxy. One per line, e.g. 8.8.8.8 or 1.2.3.0/24."),
            (BypassLabel, "IP addresses and ranges that never go through the proxy. One per line, e.g. 192.168.0.0/16."),
            (BypassRuleRichTextBox, "IP addresses and ranges that never go through the proxy. One per line, e.g. 192.168.0.0/16."),
            (HandleDNSCheckBox, "Send DNS lookups through the proxy too."),
            (UseCustomDNSCheckBox, "Use the DNS server below for this mode."),
            (DNSLabel, "DNS server for this mode."),
            (DNSTextBox, "DNS server for this mode."),
            (ControlButton, "Save the mode."));
        Icon = Resources.icon;


        BindTextBox(RemarkTextBox, _ => true, s => _mode.i18NRemark = s, _mode.i18NRemark);
        // TODO Options Not implemented

        BindTextBox(BypassRuleRichTextBox, s => true, s => _mode.Bypass = s.GetLines().ToList(), string.Join(Constants.EOF, _mode.Bypass));
        BindTextBox(HandleRuleRichTextBox, s => true, s => _mode.Handle = s.GetLines().ToList(), string.Join(Constants.EOF, _mode.Handle));
    }

    private void Route_Load(object sender, EventArgs e)
    {
        if (!IsCreateMode)
        {
            Text = "Edit Route Table Rule";

            RemarkTextBox.TextChanged -= RemarkTextBox_TextChanged;
            RemarkTextBox.Text = _mode.i18NRemark;
            FilenameTextBox.Text = ModeService.Instance.GetRelativePath(_mode.FullName);

            if (!_mode.FullName.EndsWith(".json"))
                ControlButton.Enabled = false;
        }

        i18N.TranslateForm(this);
    }

    private void ControlButton_Click(object sender, EventArgs e)
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
}