using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Corvimae.CatchTracker {
  public class CounterDefinition {
    public const string SETTING_KEY_COUNTER_DEFINITION = "CounterDefinition";
    private const string SETTING_KEY_TRACKED_STATE = "TrackedState";

    public string Name { get; set; }
    public List<TrackerState> TrackedStates { get; set; } 

    public bool TrackDefaultState {
      get {
        return TrackedStates.Contains(TrackerState.DEFAULT);
      }
      set {
        if (value) {
          TrackedStates.Add(TrackerState.DEFAULT);
        } else {
          TrackedStates.Remove(TrackerState.DEFAULT);
        }
      }
    }
    public bool TrackLeftClickState {
      get {
        return TrackedStates.Contains(TrackerState.LEFT_CLICK);
      }
      set {
        if (value) {
          TrackedStates.Add(TrackerState.LEFT_CLICK);
        } else {
          TrackedStates.Remove(TrackerState.LEFT_CLICK);
        }
      }
    }
    public bool TrackDoubleClickState {
      get {
        return TrackedStates.Contains(TrackerState.DOUBLE_CLICK);
      }
      set {
        if (value) {
          TrackedStates.Add(TrackerState.DOUBLE_CLICK);
        } else {
          TrackedStates.Remove(TrackerState.DOUBLE_CLICK);
        }
      }
    }
    public bool TrackRightClickState {
      get {
        return TrackedStates.Contains(TrackerState.RIGHT_CLICK);
      }
      set {
        if (value) {
          TrackedStates.Add(TrackerState.RIGHT_CLICK);
        } else {
          TrackedStates.Remove(TrackerState.RIGHT_CLICK);
        }
      }
    }

    public CounterDefinition() {
      Name = "";
      TrackedStates = new List<TrackerState>();
    }

    public CounterDefinition(string name, List<TrackerState> trackedStates) {
      Name = name;
      TrackedStates = trackedStates;
    }

    public XmlElement Serialize(XmlDocument document) {
      var element = document.CreateElement(SETTING_KEY_COUNTER_DEFINITION);

      element.SetAttribute("name", Name);

      foreach (var trackedState in TrackedStates) {
        var trackedStateElement = document.CreateElement(SETTING_KEY_TRACKED_STATE);

        trackedStateElement.InnerText = trackedState.ToString();
        element.AppendChild(trackedStateElement);
      }

      return element;
    }

    public static CounterDefinition Deserialize(XmlElement element) {
      element.GetElementsByTagName(SETTING_KEY_TRACKED_STATE);

      return new CounterDefinition(
        name: element.GetAttribute("name"),
        trackedStates: element.GetElementsByTagName(SETTING_KEY_TRACKED_STATE).Cast<XmlElement>().Select(elem => (TrackerState)Enum.Parse(typeof(TrackerState), elem.InnerText)).ToList()
      );
    }

    public override string ToString() {
      return $"${Name}/${string.Join(",", TrackedStates)}";
    }
  }
}
