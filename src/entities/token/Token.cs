namespace Token
{
  using Godot;
  using System;
  using System.Collections.Generic;
  using System.Linq;

  interface WithVisibility<Tk, Vs> where Tk : Token where Vs : TokenVisibility<Tk>
  {
    public abstract Vs GetVisibility();
    public abstract void SetVisibility(Vs value);
  }

  abstract class Token : Control
  {
    [Export]
    public Texture? Texture;

    public string Id { get; protected set; }

    #region Components
    #endregion

    #region Family Tree
    public RootToken Root
    {
      get
      {
        Token root = (Token)this;
        while (!(root is RootToken))
        {
          root = root.Parent;
        }
        return (RootToken)root;
      }
    }
    public abstract Token Parent { get; }
    public List<BaseToken> Children { get => GetChildren().OfType<BaseToken>().ToList(); }
    public List<BaseToken> Siblings { get => Parent.Children; }
    public abstract List<Token> Ancestors { get; }
    public List<BaseToken> Descendants { get => Children.Concat(Children.SelectMany(c => c.Descendants)).ToList(); }
    #endregion

    #region Nodes
    public Area2D TokenBody { get => GetNode<Area2D>("TokenBody"); }
    public CollisionShape2D CollisionShape2D { get => GetNode<CollisionShape2D>("TokenBody/CollisionShape2D"); }
    public TextureRect VisibilityToggle { get => GetNode<TextureRect>("VisibilityToggle"); }
    #endregion

    protected SelectService selectService { get => GetNode<SelectService>("/root/SelectService"); }

    public Token()
    {
      Id = Guid.NewGuid().ToString();
    }

    public override void _Ready()
    {
      GD.Print($"♟ Token {Name} created: {Id}");
    }

    public abstract void OnTokenGuiInput(InputEvent evt);
    public abstract void OnFocusChange(bool focused);
  }

}