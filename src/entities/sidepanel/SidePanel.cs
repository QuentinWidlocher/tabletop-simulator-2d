using Godot;

class SidePanel : ColorRect
{
  private SelectService selectService
  {
    get => GetNode<SelectService>("/root/SelectService");
  }

  public SidePanelToken SidePanelToken { get => GetNode<SidePanelToken>("Container/SidePanelToken"); }
  public SidePanelDefault SidePanelDefault { get => GetNode<SidePanelDefault>("Container/SidePanelDefault"); }

  public override void _Ready()
  {
    selectService.SelectedTokenChanged += OnSelectedTokenChanged;
  }

  public override void _ExitTree()
  {
    selectService.SelectedTokenChanged -= OnSelectedTokenChanged;
  }

  private void OnSelectedTokenChanged(Token.Token? token)
  {
    if (token != null)
    {
      if (token is Token.RootToken)
      {
        SidePanelToken.Visible = false;
        SidePanelDefault.Visible = true;
      }
      else if (token is Token.BaseToken)
      {
        SidePanelToken.Token = (Token.BaseToken)token;
        SidePanelToken.Visible = true;

        SidePanelDefault.Visible = false;
      }
    }
  }
}
