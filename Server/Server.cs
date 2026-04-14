using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Text;

class Server{
    public static string IP = "0.0.0.0";
    public static int PORT = 5000;
    public static int MAX_CLIENTS = 10;

    public static List<ClientInfo> ActiveClients = new List<ClientInfo>();
    public static List<string> AllMessages = new List<string>();
    public static object LockObject = new object();


    static void Main(string[] args){
        Thread httpThread = new Thread(HttpStatsServer.Start);
        httpThread.IsBackground = true;
        httpThread.Start();
        Console.Write("[SERVER] HTTP Stats Server filloi ne port 8080");

        TcpListener listener = new TcpListener(IPAddress.Parse(IP), PORT);
        listener.Start();
        Console.WriteLine($"[SERVER] TCP Serveri filloi ne port {PORT}");
        Console.WriteLine("[SERVER] Duke pritur klientet...\n");

        while(true){
            TcpClient client = listener.AcceptTcpClient();
            string clientIP = ((IPEndPoint)client.RemoteEndPoint).Adders.ToString();

            lock(LockObject){
                if(ActiveClients.Count >= MAX_CLIENTS){
                    Console.WriteLine($"[REFUZUAR] {clientIP} - Serveri eshte plot!");
                    NetworkStream ns = client.GetStream();
                    byte[] msg = Encoding.UTF8.GetBytes("SERVER_FULL");
                    ns.Write(msg, 0, msg.Length);
                    client.Close();
                    continue;
                }
            }

            Console.WriteLine($"[LIDHUR] Klient i ri nga: {clientIP}");
            Thread t = new Thread(()=> ClientHandler.Handle(client, clientIP));
            t.IsBackground = true;
            t.Start();
        }
    }

    public class ClientInfo{
        public string Username{get; set;}
        public string IP{get;set;}
        public string Role{get;set;}
        public DateTime ConnectedAt{get;set;}
        public int MessageCount{get;set;}
        public string LastMessage{get;set;}
    }
}