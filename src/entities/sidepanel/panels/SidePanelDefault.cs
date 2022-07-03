using System.Linq;
using Godot;

class SidePanelDefault : Control
{
  public FileDialog FileDialog { get => GetNode<FileDialog>("FileDialog"); }
  public SelectService SelectService { get => GetNode<SelectService>("/root/SelectService"); }

  private void OnAddTokenButtonPressed()
  {
    FileDialog.Popup_(GetViewportRect().Grow(-50));
  }

  private void onFileDialogFileSelected(string path)
  {
    if (SelectService.SelectedToken != null && SelectService.SelectedToken is Token.RootToken)
    {
      var newToken = GD.Load<PackedScene>("res://nodes/Token.tscn").Instance() as Token.BaseToken;
      newToken.RectPosition = SelectService.SelectedToken.RectPosition;

      StreamTexture streamTexture = ResourceLoader.Load<StreamTexture>(path);
      newToken.Sprite.Texture = streamTexture;
      newToken.Name = path.Split('/').Last().Split('.').First();

      newToken.TokenTransform.UpdateBoundingBox();

      SelectService.SelectedToken.AddChild(newToken);

      SelectService.Focus(newToken, true);
    }
  }
}
