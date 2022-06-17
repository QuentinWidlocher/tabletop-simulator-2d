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
            selectedToken.Sprite.SelfModulate = new Color(1, 1, 1, selectedToken.Sprite.SelfModulate.a);
            selectedToken.VisibilityToggle.Visible = false;
            selectedToken.TokenMovement.MoveHandle.Visible = false;
            selectedToken.TokenMovement.RotateHandle.Visible = false;
            selectedToken.DebugLabel.DebugLabel.Visible = false;
        }

        if (focus)
        {
            selectedToken = token;

            token.Sprite.SelfModulate = new Color(0.5f, 0.5f, 1, token.Sprite.SelfModulate.a);
            token.VisibilityToggle.Visible = true;
            token.TokenMovement.MoveHandle.Visible = true;
            token.TokenMovement.RotateHandle.Visible = true;
            token.DebugLabel.DebugLabel.Visible = true;
        }
        else
        {
            selectedToken = null;
        }
    }

    public void ToggleFocus(Token.Token token) => Focus(token, !isFocused(token));
}