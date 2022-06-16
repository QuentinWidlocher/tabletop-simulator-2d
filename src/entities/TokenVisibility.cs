using Godot;
using System.Collections.Generic;
using System.Linq;
using System;

class TokenVisibility : NodeComponent<Token>
{
    const string HIDDEN_ICON = "res://assets/icons/flip_tails.png";
    const string VISIBLE_ICON = "res://assets/icons/flip_head.png";
    const string INHERIT_ICON = "res://assets/icons/flip_empty.png";

    public TokenVisibility(Token instance) : base(instance) { }

    [Export]
    private List<Visibility> _visibilities = new List<Visibility>();

    // Return the visibility of the token as stored in the visibilities list
    public EVisibility? GetLocalState(int forPlayer)
    {
        // We search for the visibility of this token, for this player
        EVisibility? visibilityForPlayer = null;
        try
        {
            visibilityForPlayer = _visibilities.Where(v => v.Player == forPlayer).Select(v => v.State).First();
        }
        catch (InvalidOperationException)
        {
            visibilityForPlayer = null;
        }

        return visibilityForPlayer;
    }

    // Return the visibility of the token, taking into account the visibilities of its parents
    public EVisibility GetState(int forPlayer)
    {
        // The root is always visible.
        if (_instance.IsRoot)
            return EVisibility.Visible;

        var visibilityForPlayer = GetLocalState(forPlayer);

        // If we don't have any visibility for this player...
        if (visibilityForPlayer == null || visibilityForPlayer == EVisibility.Inherit)
        {
            // If the visibility for this player is not yet set, we set it to inherit
            if (visibilityForPlayer == null)
            {
                SetState(EVisibility.Inherit, forPlayer, false);
            }

            // If it's inherit, we return the visibility of the parent
            return _instance.Parent.Visibility.GetState(forPlayer);
        }
        else
        {
            // If we have a visibility for this player, we return it
            return (EVisibility)visibilityForPlayer;
        }

    }

    public void SetState(EVisibility state, int forPlayer, bool shouldUpdate = true)
    {
        // We update the visibility of this token, for this player
        _visibilities.RemoveAll(v => v.Player == forPlayer);
        _visibilities.Add(new Visibility { Player = forPlayer, State = state });

        switch (state)
        {
            case EVisibility.Visible:
                _instance.SelfModulate = new Color(1, 1, 1, 1);
                _instance.VisibilityToggle.GetNode<Sprite>("Sprite").Texture = ResourceLoader.Load<StreamTexture>(VISIBLE_ICON);
                break;
            case EVisibility.Hidden:
                _instance.SelfModulate = new Color(1, 1, 1, 0.1f);
                _instance.VisibilityToggle.GetNode<Sprite>("Sprite").Texture = ResourceLoader.Load<StreamTexture>(HIDDEN_ICON);
                break;
            case EVisibility.Inherit:
                _instance.SelfModulate = new Color(1, 1, 1, 1);
                _instance.VisibilityToggle.GetNode<Sprite>("Sprite").Texture = ResourceLoader.Load<StreamTexture>(INHERIT_ICON);
                break;
        }

        if (shouldUpdate)
        {
            // We update the actual visibility (the Godot boolean) of this token and its children
            UpdateVisibility(forPlayer);
        }
    }

    public void Toggle(int forPlayer)
    {
        switch (GetLocalState(forPlayer))
        {
            case EVisibility.Visible:
                SetState(EVisibility.Hidden, forPlayer);
                break;

            case EVisibility.Hidden:
                SetState(EVisibility.Inherit, forPlayer);
                break;

            default:
            case EVisibility.Inherit:
                SetState(EVisibility.Visible, forPlayer);
                break;
        }
    }

    public void UpdateVisibility(int forPlayer, bool recursive = true)
    {
        var localState = GetLocalState(forPlayer);
        var globalState = GetState(forPlayer);


        bool isVisible = globalState == EVisibility.Visible;
        var newModulate = _instance.SelfModulate;
        newModulate.a = isVisible ? 1f : 0.1f;
        _instance.SelfModulate = newModulate;

        _instance.DebugLabel.UpdateDebugLabel();

        GD.Print("♟ Token " + _instance.Name + " visibility updated: local is " + localState.ToString() + " and global is " + globalState.ToString() + "");

        if (recursive)
        {
            foreach (Token child in _instance.Children)
            {
                child.Visibility.UpdateVisibility(forPlayer, recursive);
            }
        }
    }
}