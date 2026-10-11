using Netrix.Enums;
using Netrix.Utils;

namespace Netrix.Forms;

public static class MessageBoxX
{
    /// <summary>
    /// </summary>
    /// <param name="text">Message text</param>
    /// <param name="title">Custom title</param>
    /// <param name="level">Dialog level (title, icon)</param>
    /// <param name="confirm">Needs confirmation</param>
    /// <param name="owner">Blocks owner.Focus() until the message box is closed</param>
    public static DialogResult Show(string text,
        LogLevel level = LogLevel.INFO,
        string title = "",
        bool confirm = false,
        IWin32Window? owner = null)
    {
        MessageBoxIcon msgIcon;
        if (string.IsNullOrWhiteSpace(title))
            title = level switch
            {
                LogLevel.INFO => "Information",
                LogLevel.WARNING => "Warning",
                LogLevel.ERROR => "Error",
                _ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
            };

        msgIcon = level switch
        {
            LogLevel.INFO => MessageBoxIcon.Information,
            LogLevel.WARNING => MessageBoxIcon.Warning,
            LogLevel.ERROR => MessageBoxIcon.Exclamation,
            _ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
        };

        return MessageBox.Show(owner, text, i18N.Translate(title), confirm ? MessageBoxButtons.OKCancel : MessageBoxButtons.OK, msgIcon);
    }
}