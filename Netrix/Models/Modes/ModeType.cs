namespace Netrix.Models.Modes;

public enum ModeType
{
    /// <summary>
    ///     进程代理
    /// </summary>
    ProcessMode,

    /// <summary>
    ///     网络共享
    /// </summary>
    ShareMode,

    /// <summary>
    ///     网卡代理
    /// </summary>
    TunMode,

    /// <summary>
    ///     系统代理
    /// </summary>
    // appended last: enum values are persisted in mode files
    WebMode
}