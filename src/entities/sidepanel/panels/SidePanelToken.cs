using Godot;

class SidePanelToken : Control
{
  public LineEdit NameInput { get => GetNode<LineEdit>("NameInput"); }
  public SelectService SelectService { get => GetNode<SelectService>("/root/SelectService"); }

  private Token.BaseToken token;
  public Token.BaseToken Token
  {
    get => token;
    set
    {
      token = value;
      NameInput.Text = token.Name;
    }
  }

  public void OnNameInputTextChanged(string newName)
  {
    if (token != null)
    {
      token.Name = newName;
      token.DebugLabel.UpdateDebugLabel();
    }
  }


  private void OnDeleteButtonPressed()
  {
    if (token != null)
    {
      SelectService.Focus(token.Root, true);
      token.QueueFree();
    }
  }
}
