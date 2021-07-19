using LiveSplit.Model;
using LiveSplit.UI;
using LiveSplit.UI.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Corvimae.CatchTracker.Component {
  public class CatchTrackerComponent : LogicComponent {
    public const string NAME = "Catch Tracker";

    private CatchTrackerComponentSettings settingsControl;
    private TrackerWindow tracker;
    private CountersWindow counters;
    private LiveSplitState state;

    private GraphicsCache cache;
    private GraphicsCache trackedSpeciesCache;
    private GraphicsCache counterCache;

    public CatchTrackerComponent(LiveSplitState state) {
      this.state = state;
      settingsControl = new CatchTrackerComponentSettings(state, this);

      cache = new GraphicsCache();
      trackedSpeciesCache = new GraphicsCache();
      counterCache = new GraphicsCache();
    }

    public override string ComponentName => NAME;

    public override void Dispose() {
      HideTracker();
      HideCounters();
    }

    public override XmlNode GetSettings(XmlDocument document) {
      return settingsControl.GetSettings(document);
    }

    public override void SetSettings(XmlNode settings) {
      settingsControl.SetSettings(settings);
    }


    public override Control GetSettingsControl(LayoutMode mode) {
      return settingsControl;
    }

    public void RefreshCountersWindow() {
      counters = CountersWindow.Instance;

      if (counters != null) {
        counters.RefreshWindow();
      }
    }

    public void RefreshTrackerWindow(bool speciesListUpdated) {
      tracker = TrackerWindow.Instance;

      if (tracker != null) {
        tracker.RefreshWindow(speciesListUpdated);
      }
    }

    public override void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode) {
      if (state == null || state.Form == null || !state.Form.Visible || state.Layout == null) return;

      try {
        tracker = TrackerWindow.Instance;
        counters = CountersWindow.Instance;

        trackedSpeciesCache.Restart();
        trackedSpeciesCache["VisibleSpecies"] = string.Join("-", settingsControl.VisibleSpecies.Select(x => x.ToString()));

        if (tracker == null) {
          tracker = new TrackerWindow(this, state.Layout, settingsControl);
          tracker.Show(state.Form);
          tracker.OnCatch += Tracker_OnCatch;
          state.Form.Focus();
        } else {
          cache.Restart();

          cache["BackgroundColor"] = settingsControl.BackgroundColor;
          cache["CellLength"] = settingsControl.TrackerCellLength;
          cache["DefaultStateColor"] = settingsControl.DefaultStateColor;
          cache["RightClickStateColor"] = settingsControl.RightClickStateColor;
          cache["LeftClickStateColor"] = settingsControl.LeftClickStateColor;
          cache["DoubleClickStateColor"] = settingsControl.DoubleClickStateColor;
       
          if (tracker != null && (cache.HasChanged || trackedSpeciesCache.HasChanged)) {
            RefreshTrackerWindow(trackedSpeciesCache.HasChanged);
          }
        }

        if (counters == null) {
          counters = new CountersWindow(this, settingsControl, state);
          counters.Show(state.Form);
        } else {
          counterCache.Restart();
          counterCache["OverrideTrackerFont"] = settingsControl.OverrideTrackerFont;
          counterCache["TrackerFont"] = settingsControl.TrackerFont;
          counterCache["BackgroundColor"] = settingsControl.BackgroundColor;
          counterCache["CounterDefinitions"] = string.Join("-", settingsControl.CounterDefinitions.Select(x => x.ToString()));

          if (counters != null && (counterCache.HasChanged || trackedSpeciesCache.HasChanged)) {
            RefreshCountersWindow();
          }
        }

        if (state.Run.AutoSplitter == null) {
          state.Run.AutoSplitter = new AutoSplitterSaveWorkaroundSplitter(this);
          state.Run.AutoSplitter.Component.SetSettings(state.Run.AutoSplitterSettings);
        }
      } catch (Exception ex) {
        MessageBox.Show($"Unable to show catch tracker.\n\nError message: {ex.Message}\n\nDetails:\n\n{ex.StackTrace}");

        if (tracker != null) {
          tracker.Close();
          tracker.Dispose();
          tracker = null;
        }

        if (counters != null) {
          counters.Close();
          counters.Dispose();
          counters = null;
        }
      }
    }

    private void HideTracker() {
      if (tracker != null) {
        tracker.Close();
        tracker = null;
      }
    }
    private void HideCounters() {
      if (counters != null) {
        counters.Close();
        counters = null;
      }
    }
    private void Tracker_OnCatch(TrackedPokemonSpecies species, TrackerState trackerState) {
      if (state.Run.AutoSplitter.Component is AutoSplitterSaveWorkaroundComponent && state.CurrentPhase == TimerPhase.Running) {
        var runHistory = ((AutoSplitterSaveWorkaroundComponent)state.Run.AutoSplitter.Component).History;
        var maxAttemptIndex = state.Run.AttemptHistory.DefaultIfEmpty().Max(x => x.Index);
        var runId = Math.Max(0, maxAttemptIndex + 1);

        runHistory.AddEvent(runId, state.CurrentSplit.Name, species, trackerState, state.CurrentTime);
      }

      if (counters != null) {
        counters.UpdateCounts();
      }
    }
  }
}
