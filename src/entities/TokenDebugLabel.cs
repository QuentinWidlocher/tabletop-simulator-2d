using System;
using Godot;

class TokenDebugLabel : NodeComponent<Token>
{
    public TokenDebugLabel(Token instance) : base(instance) { }

    public Label DebugLabel { get => _instance.GetNode<Label>("DebugLabel"); }

    public override void Ready()
    {
        UpdateDebugLabel();
    }

    public void UpdateDebugLabel()
    {
        if (_instance.Texture != null)
        {
            var shape = new RectangleShape2D();
            shape.Extents = _instance.Texture.GetSize() / 2;
            DebugLabel.RectPosition = new Vector2(shape.Extents.x + 10, -shape.Extents.y);
        }

        string debugText = "";
        debugText += "ID : " + _instance.Id.Substr(0, 8) + "...\n";
        debugText += "Name : " + _instance.Name + "\n";
        debugText += "LocalVisibility : " + _instance.Visibility.GetLocalState(1) + "\n";
        debugText += "GlobalVisibility : " + _instance.Visibility.GetState(1) + "\n";
        if (_instance.Parent != null)
        {
            debugText += "Parent {\n";
            debugText += "  ID : " + _instance.Parent.Id.Substr(0, 8) + "...\n";
            debugText += "  Name : " + _instance.Parent.Name + "\n";
            debugText += "}";
        }

        DebugLabel.Text = debugText;
    }
}