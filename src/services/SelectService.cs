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
            selectedToken.SelfModulate = new Color(1, 1, 1, selectedToken.SelfModulate.a);
            selectedToken.VisibilityToggle.Visible = false;
            selectedToken.MoveHandle.Visible = false;
            selectedToken.RotateHandle.Visible = false;
            selectedToken.DebugLabel.DebugLabel.Visible = false;
        }

        if (focus)
        {
            selectedToken = token;

            token.SelfModulate = new Color(0.5f, 0.5f, 1, token.SelfModulate.a);
            token.VisibilityToggle.Visible = true;
            token.MoveHandle.Visible = true;
            token.RotateHandle.Visible = true;
            token.DebugLabel.DebugLabel.Visible = true;
        }
        else
        {
            selectedToken = null;
        }
    }

    public void ToggleFocus(Token.Token token) => Focus(token, !isFocused(token));
}