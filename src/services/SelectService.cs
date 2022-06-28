using Godot;

class SelectService : Node
{
  private Token.Token? selectedToken;

  public bool isFocused(Token.Token token)
  {
    return token.Id == selectedToken?.Id;
  }

  public void Focus(Token.Token token, bool focus)
  {
    if (selectedToken != null)
    {
      selectedToken.VisibilityToggle.Visible = false;
      selectedToken.TokenTransform.MoveHandle.Visible = false;
      // selectedToken.TokenTransform.RotateHandle.Visible = false;
      // token.TokenTransform.ScaleHandle.Visible = false;
      selectedToken.DebugLabel.DebugLabel.Visible = false;
      selectedToken.SelectShape.Visible = false;
    }

    if (focus)
    {
      selectedToken = token;
      token.VisibilityToggle.Visible = true;
      token.TokenTransform.MoveHandle.Visible = true;
      // token.TokenTransform.RotateHandle.Visible = true;
      // token.TokenTransform.ScaleHandle.Visible = true;
      token.DebugLabel.DebugLabel.Visible = true;
      selectedToken.SelectShape.Visible = !selectedToken.IsRoot;
    }
    else
    {
      selectedToken = null;
    }
  }

  public void ToggleFocus(Token.Token token) => Focus(token, !isFocused(token));
}