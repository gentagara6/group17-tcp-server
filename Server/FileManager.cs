using System;
using System.IO;
using System.Linq;

public class FileManager {
    private readonly string folderPath;

    public FileManager() {
        folderPath = "server_files";

        if (!Directory.Exists(folderPath)) {
            Directory.CreateDirectory(folderPath);
        }
    }

    public string ListFiles() {
        var files = Directory.GetFiles(folderPath);

        return string.Join("\n", files.Select(Path.GetFileName));    
    }

    public string ReadFile(string fileName) {
        string path = Path.Combine(folderPath, fileName);

        if (!File.Exists(path)) {
            return "Fajlli nuk u gjet";
        }
        return File.ReadAllText(path);
    }

    public string WriteFile(string fileName, string content) {
        string path = Path.Combine(folderPath, fileName);

        File.WriteAllText(path, content);
        return "Fajlli u shkrua me sukses";
    }

    public byte[] DownloadFile(string fileName) {
        string path = Path.Combine(folderPath, fileName);

        if (!File.Exists(path)) {
            return null;
        }
        return File.ReadAllBytes(path);
    }

    public string DeleteFile(string fileName) {
        string path = Path.Combine(folderPath, fileName);

        if (!File.Exists(path)) {
            return "Fajlli nuk u gjet";
        }

        File.Delete(path);
        return "Fajlli u fshi me sukses";
    }

}