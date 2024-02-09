using System.IO;
using Godot;
using FileAccess = System.IO.FileAccess;

namespace VideoScreenshotsTool.Scripts;

public partial class Global : Node
{
    public string VidFilePath;
    public string VidFileName;
    public string OutputFolderPath;
    public int IntervalSec;


    public override void _Ready()
    {
        SetOutputFolder();
    }

    private void SetOutputFolder()
    {
        OutputFolderPath = ProjectSettings.GlobalizePath("user://user_outputs");
        // If directory does not exist, create a new one
        if (!Directory.Exists(OutputFolderPath))
        {
            Directory.CreateDirectory(OutputFolderPath);
        }
        // If directory contains files, delete them all
        var filePaths = Directory.GetFiles(OutputFolderPath);
        if (filePaths.Length > 0)
        {
            foreach (var filePath in filePaths)
            {
                File.Delete(filePath);
            }
        }
    }

    public void InitializeVidFilePath(string path)
    {
        VidFilePath = path;
        VidFileName = Path.GetFileName(path);
    }
}