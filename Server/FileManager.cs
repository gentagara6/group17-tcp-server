using System;
using System.IO;

public class FileManager
{
    private readonly string folderPath;

    public FileManager()
    {
        folderPath = "server_files";

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
    }
}