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
    ///     Static instance of the main form
    /// </summary>
    private static readonly Lazy<MainForm> LazyMainForm = new(() => new MainForm());

    /// <summary>
    ///     Settings for reading and writing
    /// </summary>
    public static Setting Settings = new();

    public static readonly JobObject Job = new();

    /// <summary>
    ///     Storage for modes
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
    ///     Static instance of the main form
    /// </summary>
    public static MainForm MainForm => LazyMainForm.Value;

    public static JsonSerializerOptions NewCustomJsonSerializerOptions() => new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };
}