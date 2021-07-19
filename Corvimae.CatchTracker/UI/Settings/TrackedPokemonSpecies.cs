using LiveSplit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Corvimae.CatchTracker {
  public class TrackedPokemonSpecies {
    public const string SETTING_KEY_SPECIES_ITEM = "Species";

    public int DexNumber { get; set; }
    public string Name { get; set; }
    public TrackerState DefaultState { get; set; } = TrackerState.DEFAULT;
    public bool Hidden { get; set; } = false;

    public TrackedPokemonSpecies(int dexNumber, string name, TrackerState defaultState, bool hidden) {
      DexNumber = dexNumber;
      Name = name;
      DefaultState = defaultState;
      Hidden = hidden;
    }

    public TrackedPokemonSpecies(PokemonSpecies source) {
      DexNumber = source.DexNumber;
      Name = source.Name;
    }

    public override string ToString() {
      return $"{DexNumber}/${Name}/${DefaultState}/${Hidden}";
    }

    public XmlElement Serialize(XmlDocument document) {
      var element = document.CreateElement(SETTING_KEY_SPECIES_ITEM);

      element.SetAttribute("dexNumber", DexNumber.ToString());
      element.SetAttribute("name", Name);
      element.SetAttribute("defaultState", DefaultState.ToString());
      element.SetAttribute("hidden", Hidden.ToString());

      return element;
    }

    public static TrackedPokemonSpecies Deserialize(XmlElement element) {
      return new TrackedPokemonSpecies(
        dexNumber: int.Parse(element.GetAttribute("dexNumber")),
        name: element.GetAttribute("name"),
        defaultState: (TrackerState)Enum.Parse(typeof(TrackerState), element.GetAttribute("defaultState")),
        hidden: bool.Parse(element.GetAttribute("hidden"))
      );
    }
  }
}
