namespace Netrix.Models.Modes;

public enum ModeType
{
    /// <summary>
    ///     Process proxy
    /// </summary>
    ProcessMode,

    /// <summary>
    ///     Network sharing
    /// </summary>
    ShareMode,

    /// <summary>
    ///     Adapter proxy
    /// </summary>
    TunMode,

    /// <summary>
    ///     System proxy
    /// </summary>
    // appended last: enum values are persisted in mode files
    WebMode
}