using Godot;

[System.Serializable]
public class InitializationException : System.Exception
{
  public InitializationException() { }
  public InitializationException(string message) : base(message) {
    GD.PrintErr(message);
   }
  public InitializationException(string message, System.Exception inner) : base(message, inner) { }
  protected InitializationException(
    System.Runtime.Serialization.SerializationInfo info,
    System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}