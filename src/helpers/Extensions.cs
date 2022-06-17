using Godot;

namespace Extensions
{
    static class InputEventExtensions
    {
        public static InputEventMouseButton? GetIfLeftClick(this InputEvent evt)
        {
            if (evt is InputEventMouseButton)
            {
                var mouse_button = (InputEventMouseButton)evt;
                if (mouse_button.ButtonIndex == (int)ButtonList.Left)
                {
                    return mouse_button;
                }
            }

            return null;
        }
    }
}