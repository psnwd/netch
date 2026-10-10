using System.Net;
using System.Text;
using Microsoft.VisualStudio.Threading;

namespace Netch.Utils;

public static class WebUtil
{
    public const string DefaultUserAgent =
        @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/94.0.4606.61 Safari/537.36 Edg/94.0.992.31";

    private static readonly HttpRequestOptionsKey<int> TimeoutKey = new("Netch.Timeout");

    // system proxy by default, same as WebRequest
    private static readonly HttpClient Client = new() { Timeout = Timeout.InfiniteTimeSpan };

    private static int DefaultGetTimeout => Global.Settings.RequestTimeout;

    public static HttpRequestMessage CreateRequest(string url, int? timeout = null, string? userAgent = null)
    {
        var req = new HttpRequestMessage(HttpMethod.Get, url);
        req.Headers.TryAddWithoutValidation("User-Agent", string.IsNullOrWhiteSpace(userAgent) ? DefaultUserAgent : userAgent);
        req.Headers.TryAddWithoutValidation("Accept", "*/*");
        req.Headers.TryAddWithoutValidation("Accept-Charset", "utf-8");
        req.Options.Set(TimeoutKey, timeout ?? DefaultGetTimeout);
        return req;
    }

    /// <summary>
    ///     Throws <see cref="HttpRequestException" /> on a non-success status (as WebRequest did)
    /// </summary>
    public static async Task<(HttpStatusCode, string)> DownloadStringAsync(HttpRequestMessage req, Encoding? encoding = null, string? proxyServer = null)
    {
        // ponytail: the timeout covers the whole transfer here (WebRequest timed out per read), fine for small text bodies
        using var cts = new CancellationTokenSource(req.Options.TryGetValue(TimeoutKey, out var ms) ? ms : DefaultGetTimeout);
        using var proxied = CreateProxiedClient(proxyServer);
        using var response = await (proxied ?? Client).SendAsync(req, HttpCompletionOption.ResponseHeadersRead, cts.Token);
        response.EnsureSuccessStatusCode();

        await using var input = await response.Content.ReadAsStreamAsync(cts.Token);
        using var reader = new StreamReader(input, encoding ?? Encoding.UTF8);
        return (response.StatusCode, await reader.ReadToEndAsync(cts.Token));
    }

    public static Task DownloadFileAsync(string address, string fileFullPath, IProgress<int>? progress = null)
    {
        return DownloadFileAsync(CreateRequest(address), fileFullPath, progress);
    }

    public static async Task DownloadFileAsync(HttpRequestMessage req, string fileFullPath, IProgress<int>? progress)
    {
        var fileStream = new FileStream(fileFullPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true);
        await using (fileStream)
        {
            // timeout only limits waiting for the response headers; the body may take as long as it needs
            using var cts = new CancellationTokenSource(req.Options.TryGetValue(TimeoutKey, out var ms) ? ms : DefaultGetTimeout);
            using var response = await Client.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, cts.Token);
            response.EnsureSuccessStatusCode();

            await using var input = await response.Content.ReadAsStreamAsync();
            using var downloadTask = input.CopyToAsync(fileStream);
            if (progress != null && response.Content.Headers.ContentLength is > 0 and var total)
                ReportProgressAsync(total, downloadTask, fileStream, progress, 200).Forget();

            await downloadTask;
        }

        progress?.Report(100);
    }

    private static HttpClient? CreateProxiedClient(string? proxyServer)
    {
        return string.IsNullOrEmpty(proxyServer)
            ? null
            : new HttpClient(new HttpClientHandler { Proxy = new WebProxy(proxyServer) }, true) { Timeout = Timeout.InfiniteTimeSpan };
    }

    private static async Task ReportProgressAsync(long total, IAsyncResult downloadTask, Stream stream, IProgress<int> progress, int interval)
    {
        var n = 0;
        while (!downloadTask.IsCompleted)
        {
            var n1 = (int)((double)stream.Length / total * 100);
            if (n != n1)
            {
                n = n1;
                progress.Report(n);
            }

            await Task.Delay(interval);
        }
    }
}
