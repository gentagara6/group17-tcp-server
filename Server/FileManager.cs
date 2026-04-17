using System;
using System.IO;
using System.Linq;

public static class FileManager {
    private static readonly string folderPath = "server_files";

    static FileManager() {

        if (!Directory.Exists(folderPath)) {
            Directory.CreateDirectory(folderPath);
        }
    }

    public static string ListFiles() {
        var files = Directory.GetFiles(folderPath);

        return string.Join("\n", files.Select(Path.GetFileName));    
    } 

    public static string ReadFile(string fileName) {
        string path = Path.Combine(folderPath, fileName);

        if (!File.Exists(path)) {
            return "Fajlli nuk ekziston";
        }
        return File.ReadAllText(path);
    }

    public static string WriteFile(string fileName, string content) {
        string path = Path.Combine(folderPath, fileName);

        File.WriteAllText(path, content);
        return "Fajlli u shkrua me sukses";
    }

    public static byte[] DownloadFile(string fileName) {
        string path = Path.Combine(folderPath, fileName);

        if (!File.Exists(path)) {
            return null;
        }
        return File.ReadAllBytes(path);
    }

    public static string DeleteFile(string fileName) {
        string path = Path.Combine(folderPath, fileName);

        if (!File.Exists(path)) {
            return "Fajlli nuk ekziston";
        }

        File.Delete(path);
        return "Fajlli u fshi me sukses";
    }

    public static string SearchFiles(string keyword){
        var files = Directory.GetFiles(folderPath)
            .Where(f => Path.GetFileName(f).Contains(keyword, StringComparison.OrdinalIgnoreCase))
            .Select(Path.GetFileName);

            if (!files.Any())
                return "Nuk u gjet asnje fajll";

            return string.Join("\n", files);
    }

    public static string GetFileInfo(string fileName) {
        string path = Path.Combine(folderPath, fileName);

        if (!File.Exists(path)) {
            return "Fajlli nuk ekziston";
        }

        FileInfo info = new FileInfo(path);
        return $"Emri: {info.Name}\nMadhesia: {info.Length} bytes\nKrijuar: {info.CreationTime}\n + $Modifikuar: {info.LastWriteTime}";

    }

    public static string ProcessCommand(string command, string role) {

        string[] parts = command.Split(' ', 2);
        string cmd = parts[0].ToLower();
        string arg = parts.Length > 1 ? parts[1] : "";

        if (role != "admin") {
            if (cmd != "/list" && cmd != "/read") {
                return "Nuk keni privilegje per kete komande!";
            }
        }

        switch (cmd) {
            case "/list":
                return ListFiles();

            case "/read":
                return ReadFile(arg);

            case "/upload":
                return WriteFile(arg, "Text content");

            case "/download":
                byte[] fileData = DownloadFile(arg);
                return fileData != null ? $"Fajlli {arg} u shkarkua me sukses" : "Fajlli nuk ekziston";

            case "/delete":
                return DeleteFile(arg);

            case "/search":
                return SearchFiles(arg);

            case "/info":
                return GetFileInfo(arg);

            default:
                return "Komande e panjohur";
        }
    }

}