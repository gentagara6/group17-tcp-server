
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Client
{
    
    static string SERVER_IP = "127.0.0.1";
    static int SERVER_PORT = 5000;

    static NetworkStream stream;
    static string currentRole = "";
    static string currentUsername = "";

    static void Main(string[] args)
    {
        Console.WriteLine("=== KLIENTI - Grupi 17 ===");
        Console.Write("Shkruaj IP-ne e serverit (Enter per localhost): ");
        string inputIP = Console.ReadLine().Trim();
        if (!string.IsNullOrEmpty(inputIP))
            SERVER_IP = inputIP;

        Console.WriteLine($"Duke u lidhur me {SERVER_IP}:{SERVER_PORT}...");

        try
        {
            TcpClient client = new TcpClient();
            client.Connect(SERVER_IP, SERVER_PORT);
            stream = client.GetStream();
            Console.WriteLine("U lidh me sukses!\n");

            
            bool loggedIn = DoLogin();
            if (!loggedIn)
            {
                Console.WriteLine("Login deshtoi. Duke u mbyllur...");
                client.Close();
                return;
            }

            CommandHandler.ShowMenu(currentRole, currentUsername);

            
            Thread receiveThread = new Thread(ReceiveMessages);
            receiveThread.IsBackground = true;
            receiveThread.Start();

            
            while (true)
            {
                Console.Write($"\n[{currentUsername}] > ");
                string input = Console.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(input)) continue;

                if (input == "/exit")
                {
                    Console.WriteLine("Duke u shkeppur...");
                    break;
                }

        
                if (!CommandHandler.HasPermission(input, currentRole))
                {
                    Console.WriteLine("[LEJE REFUZUAR] Vetem admin mund ta ekzekutoje kete komande!");
                    continue;
                }

                
                SendMessage(input);

                
                if (currentRole == "admin")
                    Thread.Sleep(100);
                else
                    Thread.Sleep(300);
            }

            client.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GABIM] Nuk mund te lidhet: {ex.Message}");
            Console.WriteLine("Kontrollo nese serveri eshte aktiv dhe IP-ja eshte e sakte.");
        }
    }

    static bool DoLogin()
    {
        try
        {
            byte[] buffer = new byte[1024];
            int bytes;


            bytes = stream.Read(buffer, 0, buffer.Length);
            Console.Write(Encoding.UTF8.GetString(buffer, 0, bytes));
            string username = Console.ReadLine();
            SendMessage(username);

            
            bytes = stream.Read(buffer, 0, buffer.Length);
            Console.Write(Encoding.UTF8.GetString(buffer, 0, bytes));
            string password = ReadPassword();
            SendMessage(password);

            
            bytes = stream.Read(buffer, 0, buffer.Length);
            string response = Encoding.UTF8.GetString(buffer, 0, bytes);

            if (response.StartsWith("LOGIN_SUCCESS"))
            {
                currentUsername = username;
                currentRole = response.Contains("admin") ? "admin" : "read";
                Console.WriteLine($"\nMiresevini {username}! Roli: {currentRole}");
                return true;
            }
            else
            {
                Console.WriteLine("\nLogin deshtoi! Username ose password gabim.");
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Gabim gjate loginit: {ex.Message}");
            return false;
        }
    }

    static void ReceiveMessages()
    {
        byte[] buffer = new byte[4096];
        while (true)
        {
            try
            {
                int bytes = stream.Read(buffer, 0, buffer.Length);
                if (bytes == 0) break;

                string message = Encoding.UTF8.GetString(buffer, 0, bytes);

                
                if (message.StartsWith("DOWNLOAD:"))
                {
                    CommandHandler.SaveDownloadedFile(message);
                }
                else
                {
                    Console.WriteLine($"\n[SERVER] {message}");
                    Console.Write($"[{currentUsername}] > ");
                }
            }
            catch
            {
                Console.WriteLine("\n[SHKEPPUR] Lidhja me serverin u pre.");
                break;
            }
        }
    }

    static void SendMessage(string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        stream.Write(data, 0, data.Length);
    }

    static string ReadPassword()
    {
        string password = "";
        ConsoleKeyInfo key;
        do
        {
            key = Console.ReadKey(intercept: true);
            if (key.Key != ConsoleKey.Enter && key.Key != ConsoleKey.Backspace)
            {
                password += key.KeyChar;
                Console.Write("*");
            }
            else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                password = password.Substring(0, password.Length - 1);
                Console.Write("\b \b");
            }
        } while (key.Key != ConsoleKey.Enter);
        Console.WriteLine();
        return password;
    }
}