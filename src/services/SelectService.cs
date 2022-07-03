using System;
using Godot;

class SelectService : Node
{
  private Token.Token? selectedToken;
  public Token.Token? SelectedToken
  {
    get => selectedToken;
  }
  public event Action<Token.Token?> SelectedTokenChanged;

  public bool isFocused(Token.Token token)
  {
    return token.Id == selectedToken?.Id;
  }

  public void Focus(Token.Token token, bool focus)
  {
    // We remove the focus from the previous token
    selectedToken?.OnFocusChange(false);

    if (focus)
    {
      // If we're giving the focus to a new token, we set it as the selected token
      selectedToken = token;
      // and we trigger its OnFocusChange event
      selectedToken.OnFocusChange(true);
    }
    else
    {
      // If we're removing the focus from a token, we set it as null
      // The first line of the method already called the OnFocusChange event of the token
      selectedToken = null;
    }

    // now we trigger the event for all the listeners
    SelectedTokenChanged?.Invoke(selectedToken);
  }

  public void ToggleFocus(Token.Token token) => Focus(token, !isFocused(token));
}