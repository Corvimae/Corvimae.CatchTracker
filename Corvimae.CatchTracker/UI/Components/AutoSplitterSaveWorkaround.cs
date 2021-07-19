using Corvimae.CatchTracker.Component;
using LiveSplit.Model;
using LiveSplit.UI;
using LiveSplit.UI.Components;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;

namespace Corvimae.CatchTracker {
  class AutoSplitterSaveWorkaroundSplitter : AutoSplitter {
    public AutoSplitterSaveWorkaroundSplitter(CatchTrackerComponent component) {
      Component = new AutoSplitterSaveWorkaroundComponent(component);
      Games = new List<string>();
      URLs = new List<string>();
    }
  }

  class AutoSplitterSaveWorkaroundComponent : LogicComponent {
    private CatchTrackerComponent component;
    private CatchHistory history;

    public CatchHistory History { get { return history; } }

    public override string ComponentName => "Catch Tracker Data Serializer";

    public AutoSplitterSaveWorkaroundComponent(CatchTrackerComponent component) {
      this.component = component;
      this.history = new CatchHistory();
    }

    public override void Dispose() {}

    public override XmlNode GetSettings(XmlDocument document) {
      XmlElement settingsNode = document.CreateElement("CatchHistory");
      settingsNode.AppendChild(history.Serialize(document));

      return settingsNode;
    }

    public override Control GetSettingsControl(LayoutMode mode) {
      return null;
    }

    public override void SetSettings(XmlNode settings) {
      history = CatchHistory.Deserialize((XmlElement) settings);
    }

    public override void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode) {
    }
  }
}
