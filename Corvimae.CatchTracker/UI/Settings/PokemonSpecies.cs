using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corvimae.CatchTracker {
  public class PokemonSpecies {
    [JsonProperty("id")]
    public int DexNumber { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
  }
}
