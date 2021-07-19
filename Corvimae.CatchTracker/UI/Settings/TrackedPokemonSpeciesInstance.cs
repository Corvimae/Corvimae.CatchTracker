namespace Corvimae.CatchTracker {
  public class TrackedPokemonSpeciesInstance {
    public TrackedPokemonSpecies Definition { get; set; }
    public TrackerState State { get; set; }

    public TrackedPokemonSpeciesInstance(TrackedPokemonSpecies definition) {
      Definition = definition;
      State = definition.DefaultState;
    }
  }
}
