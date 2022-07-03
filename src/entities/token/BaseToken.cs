using System.Collections.Generic;
using System.Linq;
using Extensions;
using Godot;

namespace Token
{
  class BaseToken : Token, WithVisibility<BaseToken, BaseTokenVisibility>
  {
    [Export]
    public bool DebugLabelVisible = false;

    public override Token Parent { get => (Token)GetParent(); }
    public override List<Token> Ancestors
    {
      get
      {
        var ancestors = new List<Token>();
        var parent = Parent;
        while (parent != null && !(parent is RootToken))
        {
          ancestors.Add(parent);
          parent = parent.Parent;
        }
        return ancestors;
      }
    }
    public Panel SelectShape { get => GetNode<Panel>("SelectShape"); }
    public TextureRect Sprite { get => GetNode<TextureRect>("Sprite"); }
    public Label TokenName { get => GetNode<Label>("Name"); }

    public TokenDebugLabel DebugLabel { get; protected set; }
    public TokenTransform TokenTransform { get; protected set; }

    BaseTokenVisibility Visibility;
    public BaseTokenVisibility GetVisibility() => Visibility;
    public void SetVisibility(BaseTokenVisibility value) => Visibility = value;

    public BaseToken() : base()
    {
      Visibility = new BaseTokenVisibility(this);
      DebugLabel = new TokenDebugLabel(this);
      TokenTransform = new TokenTransform(this);
    }

    public override void _Ready()
    {
      if ((GetParent() == null || !(GetParent() is RootToken)))
      {
        throw new InitializationException($"Token {Name} must be a child of a Token");
      }

      if (Texture != null)
      {
        Sprite.Texture = Texture;
      }

      UpdateShape();

      DebugLabel.Ready();
      TokenTransform.Ready();
      Visibility.Ready();

      base._Ready();
    }

    public override void _Process(float delta)
    {
      TokenTransform.Process(delta);

      // FIXME : Don't sync the name like that, maybe add a setter to Name ?
      if (Name != TokenName.Text)
      {
        TokenName.Text = Name;
      }
    }

    public override void OnTokenGuiInput(InputEvent evt)
    {
      var mouseButton = evt.GetIfLeftClick();
      if (mouseButton != null)
      {
        if (mouseButton.IsPressed())
        {
          selectService.ToggleFocus(this);
        }
      }
    }

    public void Move(float x, float y) => Move(new Vector2(x, y), null);
    public void Move(Vector2 pos, RootToken? newParent = null)
    {
      RectPosition = pos;
      if (newParent != null)
      {
        changeParent(newParent);
      }
    }

    public void OnMoveHandleGuiInput(InputEvent evt)
    {
      var mouseButton = evt.GetIfLeftClick();
      if (mouseButton != null)
      {
        TokenTransform.isMoveHandleHeld = mouseButton.IsPressed();

        if (!mouseButton.IsPressed())
        {
          TokenTransform.isMoveHandleHeld = false;

          // We check all the things this Token is overlapping with
          var overlappingAreas = TokenBody.GetOverlappingAreas();

          // We only care about the ones that are Token
          var otherTokens = overlappingAreas.OfType<Area2D>().Where(a =>
          {
            // We get the parent Node of the "thing"
            var parent = a.GetParent();
            // And we keep the "things" that have a Token parent which is not this Token or this Token children
            // We can only link to another Token if it's not in the same tree
            return parent != null && parent is Token && parent != this && !Descendants.Contains(parent);
          })
          .Select(a => (Token)a.GetParent()) // We get the "thing"'s parent, which should now be a Token
          .ToList();

          // If we released the token, not over its parent, but over *something else*
          if (otherTokens.Count > 0)
          {
            var newParent = otherTokens.Last();
            if (newParent != null && newParent != Parent)
            {
              changeParent(newParent);
            }
          }
        }
      }
    }

    public void OnRotateHandleGuiInput(InputEvent evt)
    {
      var mouseButton = evt.GetIfLeftClick();
      if (mouseButton != null)
      {
        TokenTransform.isRotateHandleHeld = mouseButton.IsPressed();
      }
    }

    public void OnScaleHandleGuiInput(InputEvent evt)
    {
      var mouseButton = evt.GetIfLeftClick();
      if (mouseButton != null)
      {
        TokenTransform.isScaleHandleHeld = mouseButton.IsPressed();
        GD.Print(TokenTransform.isScaleHandleHeld);
      }
    }

    protected void changeParent(Token newParent)
    {
      GD.Print("♟ Token " + Name + " moved from " + Parent.Name + " to " + newParent.Name);
      var oldPosition = RectGlobalPosition;
      var oldRotation = Ancestors.Sum(a => a.RectRotation) % 360;
      Parent.RemoveChild(this);
      newParent.AddChild(this);
      RectGlobalPosition = oldPosition;
      RectRotation = oldRotation;

      // TODO: Make this compatible with multiple players
      GetVisibility().UpdateVisibility(1);
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
            GetVisibility().Toggle(1);
          }
        }
      }
    }

    public void UpdateShape()
    {
      if (Sprite.Texture != null)
      {
        var shape = new RectangleShape2D();
        shape.Extents = Sprite.Texture.GetSize() / 2;
        CollisionShape2D.Shape = shape;

        RectPivotOffset = shape.Extents;
        Sprite.RectPivotOffset = shape.Extents;
        RectSize = Sprite.Texture.GetSize();

        CollisionShape2D.Position += shape.Extents;
      }
    }

    public override void OnFocusChange(bool focused)
    {
      TokenTransform.MoveHandle.Visible = focused;
      // TokenTransform.RotateHandle.Visible = focused;
      // TokenTransform.ScaleHandle.Visible = focused;
      DebugLabel.DebugLabel.Visible = DebugLabelVisible && focused;
      SelectShape.Visible = focused;
      VisibilityToggle.Visible = focused;
    }

  }
}