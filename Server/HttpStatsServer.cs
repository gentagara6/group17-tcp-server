using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Text;

namespace Server
{
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

        public async Task StartAsync(){
            listener.Start();
            Console.WriteLine($"[HTTP] Serveri u nis ne portin {port}");

           while (listener.IsListening){
            HttpListenerContext context = await listener.GetContextAsync();
            await HandleRequest(context);
            }
            
        }
        public void Stop(){
            if (listener.IsListening){
                listener.Stop();
            }

            listener.Close();
            Console.WriteLine("[HTTP] Serveri u ndal.");
        }

        private async Task HandleRequest(HttpListenerContext context){
            string path = context.Request.Url?.AbsolutePath?.ToLower() ?? "/";
            string method = context.Request.HttpMethod.ToUpper();

            if (method != "GET"){
                await SendResponse(context, 405, "Only GET method is allowed.");
                return;
            }

            if (path == "/"){
                string html = @"
            <html>
            <head>
                <meta charset='UTF-8'>
                <title>Server Stats</title>
            </head>
            <body>
                <h2>HTTP Stats Server</h2>
                <p>Available endpoints:</p>
                <ul>
                    <li><a href='/stats'>/stats</a></li>
                </ul>
            </body>
            </html>";

            await SendResponse(context, 200, html, "text/html");
            return;
            }

            await SendResponse(context, 404, "404 Not Found");
        }

        private async Task SendResponse(HttpListenerContext context, int statusCode, string content, string contentType = "text/plain"){
            byte[] buffer = Encoding.UTF8.GetBytes(content);

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = contentType + "; charset=utf-8";
            context.Response.ContentLength64 = buffer.Length;

            await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            context.Response.Close();
        }
    }
}