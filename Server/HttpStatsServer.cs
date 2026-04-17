using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

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
            
            Console.WriteLine("[HTTP] Duke trajtuar kerkesen...");

            context.Response.StatusCode = 200;
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes("Serveri eshte aktiv");
            await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            context.Response.Close();
        }
    }
}