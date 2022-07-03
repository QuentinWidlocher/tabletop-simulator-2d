using System;

namespace Token
{
  abstract class TokenVisibility<T> : NodeComponent<T> where T : Token
  {
    protected TokenVisibility(T instance) : base(instance) { }

    // Return the visibility of the token as stored in the visibilities list
    public abstract EVisibility? GetLocalState(int forPlayer);

    // Return the visibility of the token, taking into account the visibilities of its parents
    public abstract EVisibility GetState(int forPlayer);
  }
}