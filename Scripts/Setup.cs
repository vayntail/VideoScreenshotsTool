using System.Collections.Generic;
using System.IO;
using System.Linq;
using Godot;
using FFMpegCore;

namespace VideoScreenshotsTool.Scripts;

public partial class Setup : Control
{
    private readonly List<string> VideoExtensionsList = new List<string>() { ".MP4", ".MOV", ".AVI", ".WMV", ".MKV" };

    [Export] private FileDialog _vidFileDialog;
    [Export] private Button _vidFileButton;
    [Export] private Button _runButton;
    [Export] private Control _disableControl;
    [Export] private OptionButton _intervalOptionButton;
    [Export] private Label _vidFileErrorLabel;


    [Signal]
    public delegate void RunButtonPressedEventHandler();

    private Global _global;
    private bool _videoSelected;
    private bool _isVideoFile;

    public override void _Ready()
    {
        // Initalize Global
        _global = GetNode<Global>("/root/Global");

        _disableControl.Visible = false;
        _intervalOptionButton.Disabled = true;
        _runButton.Disabled = true;
    }

    public void Reset()
    {
        _disableControl.Visible = false;
        _intervalOptionButton.Disabled = false;
        _runButton.Disabled = false;
    }
    public void MouseDisabled(bool disabled)
    {
        _disableControl.Visible = disabled;

        _intervalOptionButton.Disabled = disabled;
        _vidFileButton.Disabled = disabled;
        
        GD.Print(_disableControl.Visible);
    }

    public override void _Process(double delta)
    {
        _disableControl.CustomMinimumSize = GetGlobalRect().Size;

        if (_vidFileButton.Disabled)
        {
            _vidFileButton.MouseDefaultCursorShape = CursorShape.Arrow;
        }
    }
    
    private void OnVidFileButtonPressed()
    {
        _vidFileDialog.Popup();
    }

    private void OnVidFileDialogFileSelected(string path)
    {
        _global.InitializeVidFilePath(path);
        _vidFileButton.Text = _global.VidFilePath;

        

        _videoSelected = true;

        GD.Print(VideoExtensionsList.Contains(Path.GetExtension(path).ToUpper()));
        // Check if selected file is a video file
        if (VideoExtensionsList.Contains(Path.GetExtension(path).ToUpper()))
        {
            _intervalOptionButton.Disabled = false;
            _runButton.Disabled = false;
            _vidFileErrorLabel.Visible = false;
        }
        else
        {
            _intervalOptionButton.Disabled = true;
            _runButton.Disabled = true;
            _vidFileErrorLabel.Visible = true;
        }
            

    }

    private void OnRunButtonPressed()
    {
        _global.IntervalSec = _intervalOptionButton.GetSelectedId();
        EmitSignal(SignalName.RunButtonPressed);
    }
    
}