using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Netrix.Forms;
using Netrix.Models;
using Netrix.Models.Modes;
using WindowsJobAPI;

namespace Netrix;

public static class Global
{
    /// <summary>
    ///     主窗体的静态实例
    /// </summary>
    private static readonly Lazy<MainForm> LazyMainForm = new(() => new MainForm());

    /// <summary>
    ///     用于读取和写入的配置
    /// </summary>
    public static Setting Settings = new();

    public static readonly JobObject Job = new();

    /// <summary>
    ///     用于存储模式
    /// </summary>
    public static readonly List<Mode> Modes = new();

    public static readonly string NetrixDir;
    public static readonly string NetrixExecutable;

    static Global()
    {
        NetrixExecutable = Application.ExecutablePath;
        NetrixDir = Application.StartupPath;
    }

    /// <summary>
    ///     主窗体的静态实例
    /// </summary>
    public static MainForm MainForm => LazyMainForm.Value;

    public static JsonSerializerOptions NewCustomJsonSerializerOptions() => new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };
}