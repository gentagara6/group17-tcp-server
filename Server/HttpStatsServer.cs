using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

// Mban statistikat qe do shfaqen ne /stats
public class ServerStats
{
    public int ActiveConnections { get; set; }
    public List<string> ClientIps { get; set; } = new List<string>();
    public int TotalMessages { get; set; }
    public List<string> ClientMessages { get; set; } = new List<string>();
}

public class HttpStatsServer
{
    private readonly HttpListener listener;
    private readonly int port;
    private readonly Func<ServerStats> getStats;

    public HttpStatsServer(int port, Func<ServerStats> statsFunction, string host = "localhost")
    {
        this.port = port;
        this.getStats = statsFunction;

        listener = new HttpListener();
        listener.Prefixes.Add($"http://{host}:{port}/");
    }

    // Nis HTTP serverin
    public async Task StartAsync()
    {
        try
        {
            listener.Start();
            Console.WriteLine($"[HTTP] Server running on port {port}");
            Console.WriteLine("[HTTP] Try /stats endpoint from browser");

            while (listener.IsListening)
            {
                HttpListenerContext context;

                try
                {
                    context = await listener.GetContextAsync();
                }
                catch (HttpListenerException)
                {
                    break;
                }
                catch (ObjectDisposedException)
                {
                    break;
                }

                _ = Task.Run(() => HandleRequestSafe(context));
            }
        }
        catch (HttpListenerException ex)
        {
            Console.WriteLine("[HTTP ERROR] Could not start HTTP server.");
            Console.WriteLine("[HTTP ERROR] " + ex.Message);
            Console.WriteLine("[HTTP INFO] Try running as Administrator or use localhost.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("[HTTP ERROR] " + ex.Message);
        }
    }

    // Ndal serverin
    public void Stop()
    {
        try
        {
            if (listener.IsListening)
                listener.Stop();

            listener.Close();
            Console.WriteLine("[HTTP] Server stopped.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("[HTTP ERROR] " + ex.Message);
        }
    }

    private async Task HandleRequestSafe(HttpListenerContext context)
    {
        try
        {
            await HandleRequest(context);
        }
        catch (Exception ex)
        {
            Console.WriteLine("[HTTP ERROR] Request failed: " + ex.Message);
            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes("500 Internal Server Error");
                context.Response.StatusCode = 500;
                context.Response.ContentType = "text/plain; charset=utf-8";
                context.Response.ContentLength64 = buffer.Length;
                await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            }
            catch { }
        }
        finally
        {
            try { context.Response.Close(); } catch { }
        }
    }

    private async Task HandleRequest(HttpListenerContext context)
    {
        string path = context.Request.Url?.AbsolutePath?.ToLower() ?? "/";
        string method = context.Request.HttpMethod.ToUpper();

        if (method != "GET")
        {
            await SendResponse(context, 405, "Only GET method is allowed.");
            return;
        }

        if (path == "/")
        {
            string html = @"
<html>
<head>
    <meta charset='UTF-8'>
    <title>Server Stats</title>
</head>
<body style='font-family: Arial, sans-serif;'>
    <h2>HTTP Stats Server - Grupi 17</h2>
    <p>Available endpoints:</p>
    <ul>
        <li><a href='/stats'>/stats</a> (JSON)</li>
        <li><a href='/stats?format=text'>/stats?format=text</a> (Text)</li>
    </ul>
</body>
</html>";
            await SendResponse(context, 200, html, "text/html");
            return;
        }

        if (path == "/stats")
        {
            ServerStats stats = getStats();
            string format = context.Request.QueryString["format"]?.ToLower() ?? "json";

            if (format == "text")
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("=== SERVER STATS ===");
                sb.AppendLine($"Active connections: {stats.ActiveConnections}");
                sb.AppendLine("Client IPs:");
                foreach (var ip in stats.ClientIps)
                    sb.AppendLine("  - " + ip);
                sb.AppendLine($"Total messages: {stats.TotalMessages}");
                sb.AppendLine("Messages:");
                foreach (var msg in stats.ClientMessages)
                    sb.AppendLine("  - " + msg);

                await SendResponse(context, 200, sb.ToString(), "text/plain");
            }
            else
            {
                string json = JsonSerializer.Serialize(stats, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                await SendResponse(context, 200, json, "application/json");
            }
            return;
        }

        await SendResponse(context, 404, "404 Not Found");
    }

    private async Task SendResponse(HttpListenerContext context, int statusCode, string content, string contentType = "text/plain")
    {
        byte[] buffer = Encoding.UTF8.GetBytes(content);
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = contentType + "; charset=utf-8";
        context.Response.ContentLength64 = buffer.Length;
        await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
    }
}