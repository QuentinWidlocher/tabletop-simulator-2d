using System.Collections.Generic;
using Extensions;
using Godot;

namespace Token
{
  class RootToken : Token, WithVisibility<RootToken, RootTokenVisibility>
  {
    public override RootToken Parent { get => this; }
    public override List<Token> Ancestors { get => new List<Token>(new RootToken[] { this }); }

    RootTokenVisibility Visibility;
    public RootTokenVisibility GetVisibility() => Visibility;
    public void SetVisibility(RootTokenVisibility value) => Visibility = value;

    public RootToken() : base()
    {
      Visibility = new RootTokenVisibility(this);
    }

    public override void _Ready()
    {
      VisibilityToggle.Visible = false;

      var shape = new RectangleShape2D();
      shape.Extents = GetViewportRect().Size;
      CollisionShape2D.Shape = shape;

      GetTree().Root.Connect("size_changed", this, nameof(OnResize));

      selectService.Focus(this, true);

      Visibility.Ready();

      base._Ready();
    }

    public override void OnTokenGuiInput(InputEvent evt)
    {
      var mouseButton = evt.GetIfLeftClick();
      if (mouseButton != null)
      {
        if (mouseButton.IsPressed())
        {
          selectService.Focus(this, true);
        }
      }
    }

    protected void OnResize()
    {
      var shape = new RectangleShape2D();
      shape.Extents = GetViewportRect().Size;
      CollisionShape2D.Shape = shape;
    }

    public override void OnFocusChange(bool focused)
    {
      VisibilityToggle.Visible = focused;
    }
  }
}