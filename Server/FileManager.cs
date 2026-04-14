using System;
using System.IO;
using System.Linq;

public class FileManager {
    private readonly string folderPath;

    public FileManager() {
        folderPath = "server_files";

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
    }

    public string ListFiles() {
    var files = Directory.GetFiles(folderPath);

    return string.Join("\n", files);
    }

    public string ReadFile(string fileName) {
        string path = Path.Combine(folderPath, fileName);

        if (!File.Exists(path))
        {
            return "File not found";
        }

        return File.ReadAllText(path);
    }
}