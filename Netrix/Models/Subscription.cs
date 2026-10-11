namespace Netrix.Models;

public class Subscription
{
    /// <summary>
    ///     Enabled
    /// </summary>
    public bool Enable { get; set; } = true;

    /// <summary>
    ///     Link
    /// </summary>
    public string Link { get; set; } = string.Empty;

    /// <summary>
    ///     Remark
    /// </summary>
    public string Remark { get; set; } = string.Empty;

    /// <summary>
    ///     User Agent
    /// </summary>
    public string UserAgent { get; set; } = string.Empty;
}