using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

class Token : Sprite
{
    [Export]
    public bool IsRoot = false;

    public string Id { get; private set; }

    public TokenVisibility Visibility { get; private set; }

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
    public Node2D VisibilityToggle { get => GetNode<Node2D>("VisibilityToggle"); }

    private bool isHandleFocused;

    public Token()
    {
        Id = Guid.NewGuid().ToString();
        Visibility = new TokenVisibility(this);
    }

    public override void _Ready()
    {
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
            VisibilityToggle.Position = new Vector2(VisibilityToggle.Position.x, VisibilityToggle.Position.y - shape.Extents.y);
        }

        if (IsRoot)
        {
            Handle.Visible = false;
            VisibilityToggle.Visible = false;

            var shape = new RectangleShape2D();
            shape.Extents = GetViewportRect().Size;
            CollisionShape2D.Shape = shape;
        }

        GD.Print($"♟ Token {Name} created: {Id}");
    }

    public override void _Process(float delta)
    {
        if (isHandleFocused)
        {
            Move(GetGlobalMousePosition() - Handle.Position - Parent.Position);
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

    public void UpdateVisibility(int forPlayer, bool recursive = true)
    {
        var localState = Visibility.GetLocalState(forPlayer);
        var globalState = Visibility.GetState(forPlayer);


        bool isVisible = globalState == EVisibility.Visible;
        var newModulate = this.SelfModulate;
        newModulate.a = isVisible ? 1f : 0.1f;
        this.SelfModulate = newModulate;

        GD.Print("♟ Token " + Name + " visibility updated: local is " + localState.ToString() + " and global is " + globalState.ToString() + "");

        if (recursive)
        {
            foreach (Token child in Children)
            {
                child.UpdateVisibility(forPlayer, recursive);
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

    public void OnVisibilityToggleInputEvent(Node viewport, InputEvent evt, int shape_idx)
    {
        if (evt is InputEventMouseButton)
        {
            var mouse_button = (InputEventMouseButton)evt;
            if (mouse_button.ButtonIndex == (int)ButtonList.Left)
            {
                if (mouse_button.IsPressed())
                {
                    // TODO: Make this compatible with multiple players
                    Visibility.Toggle(1);
                }
            }
        }
    }

    private void changeParent(Token newParent)
    {
        GD.Print("♟ Token " + Name + " moved from " + Parent.Name + " to " + newParent.Name);
        var oldPosition = GlobalPosition;
        Parent.RemoveChild(this);
        newParent.AddChild(this);
        GlobalPosition = oldPosition;

        // TODO: Make this compatible with multiple players
        UpdateVisibility(1);
    }
}
