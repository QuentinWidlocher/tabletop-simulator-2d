using System.Linq;
using Godot;
using Extensions;

class Sandbox : Node
{
    [Export]
    public NodePath? ElementPath;

    private Token? element;

    private int i;

    public override void _Ready()
    {
        this.AssertNotNull(ElementPath, "ElementPath");

        element = GetNode(ElementPath) as Token;

        this.AssertNotNull(element, "Element");
    }

    public override void _Process(float delta)
    {
        if (element == null)
            return;

        element?.Move(element.Position.x + 1, element.Position.y + 1);

        GD.Print(++i);

        if (i >= 200)
        {
            element!.Visibility = Visibility.Hidden;
        }
    }
}
