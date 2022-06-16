namespace Token
{
    using Godot;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    class Token : TextureRect
    {
        [Export]
        public bool IsRoot;

        public string Id { get; private set; }

        private bool isMoveHandleHeld;

        #region Components
        public TokenVisibility Visibility { get; private set; }
        public TokenDebugLabel DebugLabel { get; private set; }
        #endregion

        #region Family Tree
        public Token Parent { get => IsRoot ? this : (Token)GetParent(); }
        public List<Token> Children { get => GetChildren().OfType<Token>().ToList(); }
        public List<Token> Siblings { get => Parent.Children; }
        public List<Token> Ancestors { get => Parent.Ancestors; }
        public List<Token> Descendants { get => Children.Concat(Children.SelectMany(c => c.Descendants)).ToList(); }
        #endregion

        #region Nodes
        public Area2D TokenBody { get => GetNode<Area2D>("TokenBody"); }
        public CollisionShape2D CollisionShape2D { get => GetNode<CollisionShape2D>("TokenBody/CollisionShape2D"); }
        public TextureRect Handle { get => GetNode<TextureRect>("Handle"); }
        public TextureRect VisibilityToggle { get => GetNode<TextureRect>("VisibilityToggle"); }
        #endregion

        private SelectService selectService { get => GetNode<SelectService>("/root/SelectService"); }

        public Token()
        {
            Id = Guid.NewGuid().ToString();
            Visibility = new TokenVisibility(this);
            DebugLabel = new TokenDebugLabel(this);
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

                RectPivotOffset = shape.Extents;
                CollisionShape2D.Position += shape.Extents;
            }

            if (IsRoot)
            {
                Handle.Visible = false;
                VisibilityToggle.Visible = false;

                var shape = new RectangleShape2D();
                shape.Extents = GetViewportRect().Size;
                CollisionShape2D.Shape = shape;

                RectSize = GetViewportRect().Size;
            }

            DebugLabel.Ready();
            Visibility.Ready();

            GD.Print($"♟ Token {Name} created: {Id}");
        }

        public override void _Process(float delta)
        {
            if (isMoveHandleHeld)
            {
                RectGlobalPosition = GetGlobalMousePosition() - Handle.RectPosition - Handle.RectPivotOffset;
            }
        }

        public void Move(float x, float y) => Move(new Vector2(x, y), null);
        public void Move(Vector2 pos, Token? newParent = null)
        {
            RectPosition = pos;
            if (newParent != null)
            {
                changeParent(newParent);
            }
        }

        public void OnHandleGuiInput(InputEvent evt)
        {
            if (evt is InputEventMouseButton)
            {
                // We get the event
                var mouse_button = (InputEventMouseButton)evt;

                // If the player is lef-clicking
                if (mouse_button.ButtonIndex == (int)ButtonList.Left)
                {
                    // We store the state of the click (the event is triggered when the mouse is pressed and released)
                    isMoveHandleHeld = mouse_button.IsPressed();

                    // If we're not holding the mouse button, we're not dragging anymore
                    if (!mouse_button.IsPressed())
                    {
                        // We check all the things this Token is overlapping with
                        var overlappingAreas = TokenBody.GetOverlappingAreas();

                        // We only care about the ones that are Token
                        var otherTokens = overlappingAreas.OfType<Area2D>().Where(a =>
                        {
                            // We get the parent Node of the "thing"
                            var parent = a.GetParent();
                            // And we keep the "things" that have a Token parent which is not this Token or this Token children
                            // We can only link to another Token if it's not in the same tree
                            return parent != null && parent is Token && parent != this & !Descendants.Contains(parent);
                        })
                        .Select(a => (Token)a.GetParent()) // We get the "thing"'s parent, which should now be a Token
                        .ToList();

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

        public void OnTokenGuiInput(InputEvent evt)
        {
            if (evt is InputEventMouseButton)
            {
                var mouse_button = (InputEventMouseButton)evt;
                if (mouse_button.ButtonIndex == (int)ButtonList.Left)
                {
                    if (mouse_button.IsPressed())
                    {
                        selectService.ToggleFocus(this);
                        GD.Print($"♟ Token {Name} took the event");
                    }
                }
            }
        }

        public void OnVisibilityToggleGuiInput(InputEvent evt)
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
            var oldPosition = RectGlobalPosition;
            Parent.RemoveChild(this);
            newParent.AddChild(this);
            RectGlobalPosition = oldPosition;

            // TODO: Make this compatible with multiple players
            Visibility.UpdateVisibility(1);
        }
    }

}