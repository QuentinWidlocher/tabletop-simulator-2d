using Godot;

abstract class NodeComponent<T> where T : Node
{
    protected T _instance;

    public NodeComponent(T instance)
    {
        _instance = instance;
    }

    public virtual void Ready() { }
}