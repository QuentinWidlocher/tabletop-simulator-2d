using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

class Token : Sprite
{
    [Export]
    public bool IsRoot = false;

    public string Id { get; private set; }

    [Export]
    private Visibility _visibility = Visibility.Inherit;
    public Visibility GetVisibility()
    {
        if (IsRoot)
        {
            return Visibility.Visible;
        }
        else if (_visibility == Visibility.Inherit)
            return Parent.GetVisibility();
        else
            return _visibility;
    }
    public void SetVisibility(Visibility value)
    {
        _visibility = value;
        UpdateVisibility();
    }

    public Token Parent
    {
        get
        {
            if (!IsRoot)
                return (Token)GetParent();
            else
                return this;
        }
    }

    public List<Token> Children
    {
        get => GetChildren().OfType<Token>().ToList();
    }

    public Area2D TokenBody { get => GetNode<Area2D>("TokenBody"); }
    public CollisionShape2D CollisionShape2D { get => GetNode<CollisionShape2D>("TokenBody/CollisionShape2D"); }
    public Node2D Handle { get => GetNode<Node2D>("Handle"); }

    private bool isHandleFocused;

    public override void _Ready()
    {
        Id = Guid.NewGuid().ToString();
        if ((GetParent() == null || GetParent().GetType() != typeof(Token)) && !IsRoot)
        {
            throw new InitializationException($"Token {Name} must be a child of a Token");
        }

        if (Texture != null)
        {
            var shape = new RectangleShape2D();
            shape.Extents = this.Texture.GetSize() / 2;
            CollisionShape2D.Shape = shape;

            Handle.Position = new Vector2(Handle.Position.x, Handle.Position.y + shape.Extents.y);
        }

        if (IsRoot)
        {
            Handle.Visible = false;

            var shape = new RectangleShape2D();
            shape.Extents = GetViewportRect().Size;
            CollisionShape2D.Shape = shape;
        }

        GD.Print($"♟ Token {Name} created: {Id}");
    }

    public override void _Process(float delta)
    {
        if (GetVisibility() == Visibility.Visible)
        {
            if (isHandleFocused)
            {
                Move(GetGlobalMousePosition() - Handle.Position - Parent.Position);
            }
        }
    }

    public void Move(float x, float y) => Move(new Vector2(x, y), null);
    public void Move(Vector2 pos, Token? newParent = null)
    {
        Position = pos;
        if (newParent != null)
        {
            changeParent(newParent);
        }
    }

    public void UpdateVisibility(bool recursive = true)
    {
        Visible = GetVisibility() == Visibility.Visible;
        if (recursive)
        {
            foreach (Token child in Children)
            {
                child.UpdateVisibility(recursive);
            }
        }
    }

    public void OnHandleInputEvent(Node viewport, InputEvent evt, int shape_idx)
    {
        if (evt is InputEventMouseButton)
        {
            var mouse_button = (InputEventMouseButton)evt;
            if (mouse_button.ButtonIndex == (int)ButtonList.Left)
            {
                isHandleFocused = mouse_button.IsPressed();

                if (!mouse_button.IsPressed())
                {
                    var overlappingAreas = TokenBody.GetOverlappingAreas();

                    var otherTokens = overlappingAreas.OfType<Area2D>().Where(a =>
                    {
                        var parent = a.GetParent();
                        return parent != null && parent is Token && parent != Parent && parent != this & !Children.Contains(parent);
                    }).Select(a => (Token)a.GetParent()).ToList();

                    // If we released the token, not over its parent, but over *something else*
                    if (otherTokens.Count > 0)
                    {
                        var newParent = otherTokens.Last();
                        if (newParent != null)
                        {
                            changeParent(newParent);
                        }
                    }
                }
            }
        }
    }

    public void OnTokenBodyInputEvent(Node viewport, InputEvent evt, int shape_idx) { }

    private void changeParent(Token newParent)
    {
        GD.Print("♟ Token " + Name + " moved from " + Parent.Name + " to " + newParent.Name);
        var oldPosition = GlobalPosition;
        Parent.RemoveChild(this);
        newParent.AddChild(this);
        GlobalPosition = oldPosition;
    }
}
