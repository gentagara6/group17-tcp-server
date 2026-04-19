using System;
using System.IO;

class CommandHandler
{
    static string[] adminOnly = { "/upload", "/download", "/delete" };

    
    public static bool HasPermission(string command, string role)
    {
        foreach (string cmd in adminOnly)
        {
            if (command.StartsWith(cmd) && role != "admin")
                return false;
        }
        return true;
    }

    
    public static void ShowMenu(string role, string username)
    {
        Console.WriteLine("\n========================================");
        if (role == "admin")
        {
            Console.WriteLine($"ROLI: ADMIN - Ke qasje te plote!");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("  /list                - Listo file-at");
            Console.WriteLine("  /read <file>          - Lexo nje file");
            Console.WriteLine("  /upload <file>        - Ngarko file ne server");
            Console.WriteLine("  /download <file>      - Shkarko file");
            Console.WriteLine("  /delete <file>        - Fshi file");
            Console.WriteLine("  /search <fjala>       - Kerko file");
            Console.WriteLine("  /info <file>          - Info per file");
        }
        else
        {
            Console.WriteLine($"ROLI: READ-ONLY - Ke qasje te kufizuar");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("  /list                - Listo file-at");
            Console.WriteLine("  /read <file>          - Lexo nje file");
            Console.WriteLine("  /search <fjala>       - Kerko file");
            Console.WriteLine("  /info <file>          - Info per file");
            Console.WriteLine("\n  [/upload /download /delete - vetem admin]");
        }
        Console.WriteLine("  /exit                 - Shkeputu");
        Console.WriteLine("========================================\n");
    }

    
    public static void SaveDownloadedFile(string message)
    {
        try
        {
            string content = message.Substring("DOWNLOAD:".Length);
            int separatorIndex = content.IndexOf('|');

            if (separatorIndex == -1)
            {
                Console.WriteLine("\n[GABIM] Format i gabuar i download-it.");
                return;
            }

            string filename = content.Substring(0, separatorIndex);
            string fileContent = content.Substring(separatorIndex + 1);

            
            string savePath = Path.Combine("./downloads/", filename);
            Directory.CreateDirectory("./downloads/");
            File.WriteAllText(savePath, fileContent);

            Console.WriteLine($"\n[DOWNLOAD] File-i '{filename}' u ruajt ne: ./downloads/{filename}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n[GABIM] Nuk u ruajt file-i: {ex.Message}");
        }
    }
}