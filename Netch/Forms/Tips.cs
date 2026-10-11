namespace Netch.Forms;

/// <summary>
///     Hover help for controls. Texts are plain English and go through <see cref="i18N.Translate" />.
/// </summary>
internal static class Tips
{
    public static ToolTip Create(Form form, params (Control Control, string Text)[] tips)
    {
        var tip = new ToolTip { AutoPopDelay = 25000, InitialDelay = 400, ReshowDelay = 200 };
        form.Disposed += (_, _) => tip.Dispose();

        foreach (var (control, text) in tips)
            tip.SetToolTip(control, Utils.i18N.Translate(text));

        return tip;
    }

    /// <summary>
    ///     Status bar and tool strip items are not controls, they carry their own tooltip.
    /// </summary>
    public static void Set(string text, params ToolStripItem[] items)
    {
        foreach (var item in items)
            item.ToolTipText = Utils.i18N.Translate(text);
    }
}
