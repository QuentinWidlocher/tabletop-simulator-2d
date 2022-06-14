using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

class Token : Node2D
{
    [Export]
    public bool IsRoot = false;

    public string Id { get; private set; }

    [Export]
    private Visibility _visibility = Visibility.Inherit;
    public Visibility Visibility
    {
        get
        {
            if (IsRoot)
            {
                return Visibility.Visible;
            }
            else if (_visibility == Visibility.Inherit)
                return Parent.Visibility;
            else
                return _visibility;
        }
        set
        {
            _visibility = value;
            UpdateVisibility();
        }
    }

    public Token Parent
    {
        get { return (Token)GetParent(); }
    }
    public List<Token> Children
    {
        get => GetChildren().OfType<Token>().ToList();
    }

    public override void _Ready()
    {
        Id = Guid.NewGuid().ToString();
        GD.Print($"♟ Token {Name} created: {Id}");

        if ((GetParent() == null || GetParent().GetType() != typeof(Token)) && !IsRoot)
        {
            throw new InitializationException($"Token {Name} must be a child of a Token");
        }

        GD.Print($"♟ Token {Name} visibility: {Visibility.ToString()}");
    }

    public void Move(float x, float y)
    {
        Position = new Vector2(x, y);
    }

    public void UpdateVisibility(bool recursive = true)
    {
        Visible = Visibility == Visibility.Visible;
        if (recursive)
        {
            foreach (Token child in Children)
            {
                child.UpdateVisibility(recursive);
            }
        }
    }
}
