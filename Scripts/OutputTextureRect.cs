using Godot;

namespace VideoScreenshotsTool.Scripts;

public partial class OutputTextureRect : Control
{
    [Export] private TextureRect _imageTextureRect;
    [Export] private CanvasLayer _imageCanvasLayer;
    [Export] private TextureRect _imageDisplay;
    [Export] private TextureButton _deleteButton;
    
    [Signal]
    public delegate void ImageDeletedEventHandler(int addOrMinus);

    private bool _deleted;
    private Image _image;
    
    public Image GetImage()
    {
        return _image;
    }

    public bool Deleted()
    {
        return _deleted;
    }
    
    public void Initialize(Image image)
    {
        _image = image;
        
        ImageTexture imageTexture = ImageTexture.CreateFromImage(image);

        _imageTextureRect.Texture = imageTexture;
        _imageDisplay.Texture = imageTexture;
    }
    
    private void OnImageTextureRectGuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton input)
        {
            if (input.Pressed && input.ButtonIndex == MouseButton.Left)
            {
                _imageCanvasLayer.Visible = true;
            }
        }
    }
    
    private void OnColorRectGuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton input)
        {
            if (input.Pressed && input.ButtonIndex == MouseButton.Left)
            {
                _imageCanvasLayer.Visible = false;
            }
        }
    }

    public override void _Process(double delta)
    {
        // if X is 100,
        // and image is 1000x2000,
        // I would want Y to be 100/1000 * 2000
        
        // I want to put some margins between X
        var marginY = 20;
        float newY = GetGlobalRect().Size.X/_imageTextureRect.Texture.GetSize().X * _imageTextureRect.Texture.GetSize().Y+_deleteButton.Size.Y;
        CustomMinimumSize = new Vector2( 0, newY);
    }

    private void OnDeleteButtonToggled(bool pressed)
    {
        if (pressed)
        {
            _imageTextureRect.Modulate = Color.FromHtml("ffffff3f");
            _deleted = true;
            EmitSignal(SignalName.ImageDeleted, -1);
        }
        else
        {
            _imageTextureRect.Modulate = Color.FromHtml("ffffff");
            _deleted = false;
            EmitSignal(SignalName.ImageDeleted, 1);
        }
    }
}