using Token;
using Godot;
using Extensions;

class Sandbox : Control
{
  [Export]
  public NodePath? ElementPath;

  private Token.Token? element;

  private PackedScene tokenScene = GD.Load<PackedScene>("res://nodes/Token.tscn");

  private SelectService selectService { get => GetNode<SelectService>("/root/SelectService"); }

  private int i;

  public override void _Ready()
  {
    this.AssertNotNull(ElementPath, "ElementPath");

    element = GetNode(ElementPath) as Token.Token;

    this.AssertNotNull(element, "Element");

    selectService.Focus(element, true);
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

    selectService.Focus(newToken, true);
  }
}
