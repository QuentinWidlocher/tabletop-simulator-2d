using Token;
using Godot;
using Extensions;

class Sandbox : Control
{
  [Export]
  public NodePath? ElementPath;

  private Token.Token? element;
  private FileDialog fileDialog;

  private PackedScene tokenScene = GD.Load<PackedScene>("res://nodes/Token.tscn");

  private int i;

  public override void _Ready()
  {
    this.AssertNotNull(ElementPath, "ElementPath");

    element = GetNode(ElementPath) as Token.Token;
    fileDialog = GetParent().GetNode<FileDialog>("FileDialog");

    this.AssertNotNull(element, "Element");
    this.AssertNotNull(fileDialog, "FileDialog");
  }

  public override void _Process(float delta)
  {
    if (element == null)
      return;

    // element.Move(element.Position.x + 1, element.Position.y + 1);
    // element.Rotate(element.Rotation + 0.001f);

    i++;
  }

  public void OnFileDialogFileSelected(string path)
  {
    var newToken = tokenScene.Instance() as Token.Token;
    element.AddChild(newToken);
    newToken.RectPosition = GetViewportRect().GetCenter();
    newToken.Sprite.Texture = ResourceLoader.Load<StreamTexture>(path);
    newToken.TokenTransform.UpdateBoundingBox();
    newToken.UpdateShape();
  }

  public void OnAddTokenButtonPressed()
  {
    fileDialog.Popup_();
  }
}
