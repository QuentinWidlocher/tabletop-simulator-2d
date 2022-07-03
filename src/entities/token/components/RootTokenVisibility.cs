namespace Token
{
  class RootTokenVisibility : TokenVisibility<RootToken>
  {
    public RootTokenVisibility(RootToken instance) : base(instance) { }

    // Return the visibility of the token as stored in the visibilities list
    public override EVisibility? GetLocalState(int forPlayer) => EVisibility.Visible;

    // Return the visibility of the token, taking into account the visibilities of its parents
    public override EVisibility GetState(int forPlayer) => EVisibility.Visible;
  }
}