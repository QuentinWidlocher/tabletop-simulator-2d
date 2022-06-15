using Godot;
using System.Collections.Generic;
using System.Linq;
using System;

class TokenVisibility
{
    private Token _instance;

    public TokenVisibility(Token instance)
    {
        _instance = instance;
    }

    [Export]
    private List<Visibility> _visibilities = new List<Visibility>();

    public EVisibility GetState(int forPlayer)
    {
        // The root is always visible.
        if (_instance.IsRoot)
            return EVisibility.Visible;

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

        if (shouldUpdate)
        {
            // We update the actual visibility (the Godot boolean) of this token and its children
            _instance.UpdateVisibility(forPlayer, state);
        }
    }

    public void Toggle(int forPlayer)
    {
        switch (GetState(forPlayer))
        {
            case EVisibility.Visible:
                SetState(EVisibility.Hidden, forPlayer);
                break;
            case EVisibility.Hidden:
                SetState(EVisibility.Visible, forPlayer);
                break;
            case EVisibility.Inherit:
                SetState(EVisibility.Hidden, forPlayer);
                break;
        }
    }
}