using Netrix.Properties;
using Netrix.Utils;

namespace Netrix.Forms;

[Fody.ConfigureAwait(true)]
public partial class AboutForm : Form
{
    public AboutForm()
    {
        InitializeComponent();
        Icon = Resources.icon;
        KeyPreview = true;
        KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Escape)
                Close();
        };
    }

    private void AboutForm_Load(object sender, EventArgs e)
    {
        i18N.TranslateForm(this);
    }

    private void NetrixPictureBox_Click(object sender, EventArgs e)
    {
        Utils.Utils.Open("https://github.com/psnwd/netrix");
    }

    private void RepoLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        Utils.Utils.Open("https://github.com/psnwd/netrix");
    }
}