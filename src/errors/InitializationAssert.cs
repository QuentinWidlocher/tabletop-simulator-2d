using Godot;

namespace Extensions
{
    static class InitializationAssert
    {
        public static void AssertNotNull(
            this Node parent,
            System.Object? obj,
            string objName,
            string message
        )
        {
            if (obj == null)
            {
                throw new InitializationException(parent.Name + ": " + objName + " " + message);
            }
        }

        public static void AssertNotNull(this Node parent, System.Object? obj, string objName)
        {
            parent.AssertNotNull(obj, objName, "must be set");
        }

        public static void AssertNotNull(this Node parent, Node? node)
        {
            parent.AssertNotNull(node, node?.Name ?? "Node", "must be set");
        }

        public static void AssertNotNull(this Node parent, Node? node, string message)
        {
            parent.AssertNotNull(node, node?.Name ?? "Node", message);
        }
    }
}
