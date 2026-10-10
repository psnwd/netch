namespace Netch.Models.Modes.WebMode;

public class WebMode : Mode
{
    public override ModeType Type => ModeType.WebMode;

    /// <summary>
    ///     Windows "ProxyOverride" list, semicolon separated
    /// </summary>
    public string Bypass { get; set; } = "<local>";
}
