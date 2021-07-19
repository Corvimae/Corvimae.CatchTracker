using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corvimae.CatchTracker {
  public class SpeciesDictionary {
    [JsonIgnore]
    public string Filename { get; set; }
    
    [JsonProperty("spriteDirectory")]
    public string SpriteDirectory { get; set; } = "./resources/sprites/kanto";

    [JsonProperty("pokemon")]
    public List<PokemonSpecies> Pokemon { get; set; } = new List<PokemonSpecies>();
  }
}
