using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class ClientHandler{
    private static int TIMEOUT_SECONDS = 30;

    public static void Handle(TcpClient TcpClient, string clientIP){
        NetworkStream stream = TcpClient.GetStream();
        stream.ReadTimeout = TIMEOUT_SECONDS * 1000;
        ClientInfo? info = null;

        try{
            info = DoLogin(stream, clientIP);
            if(info == null){
                TcpClient.Close();
                return;
            }

            lock(Server.LockObject){
                Server.ActiveClients.Add(info);
            }

            Console.WriteLine($"[LOGIN OK] {info.Username} ({info.Role}) nga {clientIP}");
            string welcome = $"LOGIN_SUCCESS|{info.Role}";
            Send(stream, welcome);

            ReceiveLoop(stream, info);
        }catch(System.IO.IOException){
            Console.WriteLine($"[TIMEOUT] {clientIP} u shkeput per shkak te inaktivitetit");
        }catch(Exception ex){
            Console.WriteLine($"[GABIM] {clientIP}: {ex.Message}");
        }finally{
            if(info != null){
                lock (Server.LockObject){
                    Server.ActiveClients.Remove(info);
                }
                Console.WriteLine($"[LARGUAR] {info.Username} u largua. Aktiv: {Server.ActiveClients.Count}");
            }
            TcpClient.Close();
        }
    }

    private static ClientInfo? DoLogin(NetworkStream stream, string clientIP){
        Send(stream, "USERNAME:");
        string username = Receive(stream);

        Send(stream, "PASSWORD:");
        string password = Receive(stream);

        string role = Authenticate(username, password);
        if(role == null){
            Send(stream, "LOGIN_FAILED");
            Console.WriteLine($"[LOGIN DESHTOI] {clientIP} tentoi me {username}");
            return null;
        }

        return new ClientInfo{
            Username = username,
            IP = clientIP,
            Role = role,
            ConnectedAt = DateTime.Now,
            MessageCount = 0,
            LastMessage = ""
        };
    }

    private static string Authenticate(string username, string password){
        if (username == "admin" && password == "admin123")   return "admin";
        if (username == "user1"  && password == "user123")   return "read";
        if (username == "user2"  && password == "user123")   return "read";
        if (username == "user3"  && password == "user123")   return "read";
        return null;
    }

    private static void ReceiveLoop(NetworkStream stream, ClientInfo info){
        while(true){
            string message = Receive(stream);
            if(string.IsNullOrEmpty(message)) break;

            Console.WriteLine($"[MESAZH] {info.Username}: {message}");

            lock(Server.LockObject){
                Server.AllMessages.Add($"[{DateTime.Now:HH:mm:ss}] {info.Username}: {message}");
                info.MessageCount++;
                info.LastMessage = message;
            }

            string response = FileManager.ProcessCommand(message, info.Role);
            Send(stream, response);
        }
    }

    private static void Send(NetworkStream stream, string message){
        byte[] data = Encoding.UTF8.GetBytes(message);
        stream.Write(data, 0, data.Length);
    }

    private static string Receive(NetworkStream stream){
        byte[] buffer = new byte[4096];
        int bytes = stream.Read(buffer, 0, buffer.Length);
        if(bytes == 0) return "";
        return Encoding.UTF8.GetString(buffer, 0, bytes).Trim();
    }
}