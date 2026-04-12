using System;
using System.Collections.Generic;
using System.Net;

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
    }
}