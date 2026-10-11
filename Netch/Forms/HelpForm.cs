using Netch.Properties;

namespace Netch.Forms;

/// <summary>
///     Built-in documentation window (Help menu): topic list on the left, formatted text on the right.
/// </summary>
/// <remarks>
///     Topic text mini-format: "# x" sub-heading, "- Lead|text" bullet with a bold lead, "- text" plain bullet,
///     anything else is a paragraph. URLs become links.
/// </remarks>
public class HelpForm : Form
{
    private static readonly (string Title, string[] Lines)[] Topics =
    {
        ("Getting started", new[]
        {
            "Netch sends your traffic through a proxy server.",
            "# Three steps",
            "- 1. Add a server|Server menu: add one by hand, import a share link from the clipboard, or add a subscription.",
            "- 2. Pick a mode|Process, TUN, Share or Web. If unsure, use a TUN mode such as \"Bypass LAN\".",
            "- 3. Press Start|The delay column shows how fast each server answers (ms). Lower is better.",
            "Run Netch as administrator: TUN and Process modes install drivers and change routes."
        }),
        ("Servers", new[]
        {
            "# Supported",
            "- Shadowsocks, ShadowsocksR, SOCKS5, SSH, Trojan, VMess, VLESS, WireGuard",
            "# Adding",
            "- By hand|Server menu, or the Add button.",
            "- Share link|Copy a link (ss://, vmess://, trojan://, ...) then Server > Import Servers From Clipboard.",
            "- Subscription|Subscription > Manage Subscriptions to add the link, Update Servers to refresh. They also refresh when Netch opens.",
            "# Delay",
            "- Re-tested every few seconds while the window is open. Paused when Netch is in the tray."
        }),
        ("Modes", new[]
        {
            "# Process",
            "Proxies chosen programs, or everything except chosen programs. Uses the NetFilter driver.",
            "- Handle|programs to proxy. Bypass: programs to skip. Entries are regular expressions, e.g. chrome.exe.",
            "- Global Bypass Apps|ready-made mode: proxy everything except a list of apps.",
            "# TUN",
            "A virtual network adapter. All traffic of the PC goes through it, including IPv6.",
            "- Skips by IP address, not by program.|Handle = IPs to proxy, Bypass = IPs to skip (1.2.3.0/24).",
            "- Settings > TUN tab > Global Bypass IPs|skips IPs in every TUN mode.",
            "# Share",
            "Shares the proxy with other devices on your network. Needs Npcap.",
            "# Web",
            "Sets the Windows system proxy (HTTP/HTTPS) and restores it when you stop."
        }),
        ("Skip an app or IP", new[]
        {
            "# Skip a program",
            "- Use a Process mode and add the exe name to its Bypass list (edit button next to the mode list, or Mode > Create Process Mode).",
            "- TUN mode cannot skip by program: Windows routes by address only.",
            "# Skip an IP or range",
            "- Use a TUN mode and add the address to its Bypass list, or to Settings > TUN tab > Global Bypass IPs.",
            "# IPv6",
            "- If the server has no IPv6, Netch rejects IPv6 connections so apps fall back to IPv4 at once.",
            "- Turn IPv6 handling off with TUNTAP.ProxyIPv6 in data\\settings if you do not want IPv6 in the tunnel."
        }),
        ("Status bar", new[]
        {
            "# NAT type",
            "- 1|open, best for games and voice.",
            "- 2|restricted.",
            "- 3|symmetric, hard for peer-to-peer.",
            "- NoUDP|the server does not relay UDP, or the STUN server is blocked.",
            "- UnsupportedServer|the STUN server cannot do the test. Pick another in Settings.",
            "Click the NAT label to test again.",
            "# Speed",
            "- TUN mode shows download and upload separately."
        }),
        ("Files", new[]
        {
            "Folders next to Netch.exe:",
            "- data|settings and server list. Back this up.",
            "- mode|mode files, editable as text or JSON.",
            "- logging|logs, including tun2socks.log.",
            "- bin|helper programs and drivers."
        }),
        ("Troubleshooting", new[]
        {
            "# Nothing connects",
            "- Check the server delay, then read logging\\application.log.",
            "# TUN will not start",
            "- Run as administrator. Check that bin\\wintun.dll exists.",
            "# NAT shows NoUDP",
            "- Try another STUN server. If all fail, the proxy server blocks UDP.",
            "# Internet gone after a crash",
            "- Start Netch again: it restores the system proxy. Also try Options > Remove Netch Firewall Rules.",
            "# A site hangs, others work",
            "- It may be IPv6. Look for \"no working IPv6 path\" in the log.",
            "# Update broke it",
            "- A failed update rolls back to the old files automatically."
        }),
        ("Links", new[]
        {
            "- Documentation|https://docs.netch.org",
            "- Source|https://github.com/NetchX/Netch",
            "- Telegram|https://t.me/Netch"
        })
    };

    private readonly ListBox _topics = new()
    {
        Dock = DockStyle.Left,
        Width = 190,
        BorderStyle = BorderStyle.None,
        Font = new Font("Segoe UI", 10.5f),
        ItemHeight = 30,
        DrawMode = DrawMode.OwnerDrawFixed,
        IntegralHeight = false,
        BackColor = SystemColors.ControlLight
    };

    private readonly Panel _page = new() { Dock = DockStyle.Fill, Padding = new Padding(32, 24, 24, 12), BackColor = SystemColors.Window };

    private readonly RichTextBox _text = new()
    {
        Dock = DockStyle.Fill,
        ReadOnly = true,
        BorderStyle = BorderStyle.None,
        BackColor = SystemColors.Window,
        DetectUrls = true,
        Font = new Font("Segoe UI", 10.5f)
    };

    private readonly Font _h1 = new("Segoe UI Semibold", 18f);
    private readonly Font _h2 = new("Segoe UI Semibold", 12f);
    private readonly Font _body = new("Segoe UI", 10.5f);
    private readonly Font _bold = new("Segoe UI Semibold", 10.5f);
    private readonly Font _gap = new("Segoe UI", 4f); // tiny blank line = paragraph spacing

    public HelpForm()
    {
        Icon = Resources.icon;
        Text = "Netch Help";
        Size = new Size(880, 620);
        MinimumSize = new Size(640, 420);
        StartPosition = FormStartPosition.CenterParent;
        KeyPreview = true;
        KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Escape)
                Close();
        };

        _text.LinkClicked += (_, e) =>
        {
            if (e.LinkText is { } link)
                Utils.Utils.Open(link);
        };
        _topics.DrawItem += DrawTopic;
        _topics.SelectedIndexChanged += (_, _) => Render(_topics.SelectedIndex);
        _topics.Items.AddRange(Topics.Select(t => (object)t.Title).ToArray());

        _text.BulletIndent = 14;
        _page.Controls.Add(_text);
        Controls.Add(_page);
        Controls.Add(_topics);
        _topics.SelectedIndex = 0;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _h1.Dispose();
            _h2.Dispose();
            _body.Dispose();
            _bold.Dispose();
            _gap.Dispose();
            _page.Dispose();
            _topics.Font.Dispose();
            _text.Font.Dispose();
        }

        base.Dispose(disposing);
    }

    private void DrawTopic(object? sender, DrawItemEventArgs e)
    {
        if (e.Index < 0)
            return;

        var selected = (e.State & DrawItemState.Selected) != 0;
        using var back = new SolidBrush(selected ? SystemColors.Highlight : _topics.BackColor);
        e.Graphics.FillRectangle(back, e.Bounds);

        var bounds = e.Bounds;
        bounds.X += 14;
        TextRenderer.DrawText(e.Graphics,
            Topics[e.Index].Title,
            _topics.Font,
            bounds,
            selected ? SystemColors.HighlightText : SystemColors.ControlText,
            TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
    }

    private void Render(int index)
    {
        var (title, lines) = Topics[index];

        _text.SuspendLayout();
        _text.Clear();
        _text.SelectionIndent = 0;

        Add(title + "\n", _h1, SystemColors.ControlText, 0);

        foreach (var line in lines)
        {
            if (line.StartsWith("# "))
            {
                Add(line[2..] + "\n", _h2, SystemColors.Highlight, 0, spaceBefore: true);
            }
            else if (line.StartsWith("- "))
            {
                var item = line[2..];
                var split = item.IndexOf('|');
                if (split > 0)
                {
                    Add(item[..split] + " – ", _bold, SystemColors.ControlText, 6, bullet: true, hanging: 14);
                    Add(item[(split + 1)..] + "\n", _body, SystemColors.ControlText, 6, bullet: true, hanging: 14);
                }
                else
                {
                    Add(item + "\n", _body, SystemColors.ControlText, 6, bullet: true, hanging: 14);
                }
            }
            else
            {
                Add(line + "\n", _body, SystemColors.ControlText, 0);
            }

            if (!line.StartsWith("# "))
                Add("\n", _gap, SystemColors.ControlText, 0);
        }

        _text.SelectionStart = 0;
        _text.SelectionLength = 0;
        _text.ResumeLayout();
    }

    private void Add(string s, Font font, Color color, int indent, bool bullet = false, bool spaceBefore = false, int hanging = 0)
    {
        _text.SelectionStart = _text.TextLength;
        _text.SelectionLength = 0;
        _text.SelectionIndent = indent;
        _text.SelectionHangingIndent = hanging;
        _text.SelectionBullet = bullet;
        _text.SelectionFont = font;
        _text.SelectionColor = color;
        _text.SelectedText = spaceBefore ? "\n" + s : s;
    }
}
