using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using FFMpegCore;
using Godot;

namespace VideoScreenshotsTool.Scripts;

public partial class App : Control
{
    [Export] private Setup _setup;
    [Export] private GridContainer _gridContainer;
    [Export] private PackedScene _outputTextureRectPackedScene;
    [Export] private ProgressBar _loadingBar;
    [Export] private Label _imgCountLabel;
    [Export] private Control _loadingControl;
    [Export] private Label _loadingLabel;
    [Export] private TextureButton _downloadButton;
    [Export] private FileDialog _downloadFileDialog;
    
    
    private Global _global;
    private bool _completed;
    private int _finalImagesCount; // used for final download X thing

    
    public override void _Ready()
    {
        // Initalize Global
        _global = GetNode<Global>("/root/Global");

        // Don't show loading bar thing at start
        ToggleLoading(false);
        
        // Run Program
        _setup.RunButtonPressed += HandleRunButtonPressed;
    }

    private void ToggleLoading(bool onoff)
    {
        _loadingControl.Visible = onoff;
        _downloadButton.Visible = onoff;
    }

    private void HandleRunButtonPressed()
    {
        ToggleLoading(true);
        RunCommandAsync();
    }

    private void OnDownloadButtonPressed()
    {
        _downloadFileDialog.Popup();
    }

    private void OnDownloadFileDialogueFolderSelected(string folderPath)
    {
        for (int i = 0; i < _gridContainer.GetChildren().Count; i++)
        {
            OutputTextureRect textureRect = _gridContainer.GetChild<OutputTextureRect>(i);
            
            // Create a new folder inside the selected folderPath to save these PNG's in
            string saveFolder = folderPath + "/" + _global.VidFileName;
            Directory.CreateDirectory(saveFolder);
            
            // Only save if image is not deleted
            if (!textureRect.Deleted())
            {
                textureRect.GetImage().SavePng(saveFolder+"/" + i+1.ToString() + ".png");
            }
        }
    }
    
    private async Task RunCommandAsync()
    {
        var totalSec = (int)(FFProbe.Analyse(_global.VidFilePath).Duration.TotalSeconds); // total video length in seconds

        var totalImages = totalSec / _global.IntervalSec;
        
        // PRINT
        GD.Print("total video seconds: ", totalSec);
        GD.Print("time interval: ", _global.IntervalSec);
        GD.Print("total number of images: ", totalImages);
        
        
        
        _loadingBar.MaxValue = totalImages;
        _imgCountLabel.Text = "0/" + totalImages.ToString();

        
        _finalImagesCount = totalImages;
        // theres two counters, because one of them i want to keep track of what's being deleted etc during, and the
        // other one i want to use just to keep it as is
        
        var imgCount = 1;
        
        _setup.MouseDisabled(true);

        // Re-set
        foreach (Node child in _gridContainer.GetChildren())
        {
            _gridContainer.RemoveChild(child);
        }
        
        // Loop
        for (int i = 1; i < totalSec; i++)
        {

            var outputFile = _global.OutputFolderPath + "/" + i.ToString() + ".png";
            await FFMpeg.SnapshotAsync(input: _global.VidFilePath, outputFile, null, TimeSpan.FromSeconds(i)).ConfigureAwait(true);
            
            // Display Image
            OutputTextureRect display = _outputTextureRectPackedScene.Instantiate<OutputTextureRect>();
            display.Initialize(Image.LoadFromFile(outputFile));

            _gridContainer.AddChild(display);
                
            // Display loadingBar
            _loadingBar.Value = imgCount;
            _imgCountLabel.Text = imgCount.ToString() + "/" + totalImages.ToString();
            
            GD.Print("image Count: ", imgCount);
            GD.Print("current Second: ", i);
            
            i += _global.IntervalSec - 1;
            imgCount += 1;


            // X button pressed
            display.ImageDeleted += HandleImageDeleted;
        }
        
        // COMPLETED
        CompleteRun();
    }

    private void CompleteRun()
    {
        GD.Print("complete!");
        
        _setup.MouseDisabled(false);
        
        _completed = true;
        _loadingLabel.Text = "complete!";
        _imgCountLabel.Text = "download " + _finalImagesCount + " images";
        _downloadButton.Visible = true;
        
        

    }
    
    private void HandleImageDeleted(int addOrMinus)
    {
        _finalImagesCount += addOrMinus;
        if (_completed)
        {
            _imgCountLabel.Text = "download " + _finalImagesCount + " images";
        }
    }
}