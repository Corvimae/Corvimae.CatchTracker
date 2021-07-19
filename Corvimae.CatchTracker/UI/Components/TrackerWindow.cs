using Corvimae.CatchTracker.Component;
using LiveSplit.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Corvimae.CatchTracker {
  public partial class TrackerWindow : Form {
    private static int GRIP_SIZE = 16;

    public delegate void CatchEventHandler(TrackedPokemonSpecies species, TrackerState state);
    public event CatchEventHandler OnCatch;

    private static TrackerWindow instance = null;
    public static TrackerWindow Instance { get { return instance; } }

    private CatchTrackerComponent trackerComponent;
    private CatchTrackerComponentSettings settingsControl;
    private ILayout layout;

    private List<TrackerButton> trackerButtons = new List<TrackerButton>();

    public List<TrackedPokemonSpeciesInstance> TrackedInstances { get; set; } = new List<TrackedPokemonSpeciesInstance>();

    public TrackerWindow(CatchTrackerComponent trackerComponent, ILayout layout, CatchTrackerComponentSettings settingsControl) {
      instance = this;

      this.trackerComponent = trackerComponent;
      this.layout = layout;
      this.settingsControl = settingsControl;

      DoubleBuffered = true;
      SetStyle(ControlStyles.ResizeRedraw, true);
      FormBorderStyle = FormBorderStyle.None;
      SizeGripStyle = SizeGripStyle.Show;
      SetStyle(ControlStyles.ResizeRedraw, true);

      TopMost = layout.Settings.AlwaysOnTop;

      InitializeComponent();
    }

    protected override void OnShown(EventArgs e) {
      base.OnShown(e);

      RefreshWindow(true);

      LocationChanged += TrackerWindow_LocationChanged;
    }

    protected override void OnClosing(CancelEventArgs e) {
      base.OnClosing(e);

      LocationChanged -= TrackerWindow_LocationChanged;
    }

    public void RefreshWindow(bool speciesListChanged) {
      SuspendDrawing(this);

      if (speciesListChanged) {
        RebuildTrackerInstances();
        CreateGridElements();
      } else {
        RecolorGridElements();
      }

      SetSize(new Size(settingsControl.TrackerWindowWidth, settingsControl.TrackerWindowHeight));
      SetPosition(new Point(settingsControl.TrackerXPosition, settingsControl.TrackerYPosition));
      OnBackgroundColorChanged(settingsControl.BackgroundColor);
      ResumeDrawing(this);
      Invalidate();
    }

    private const int WM_NCHITTEST = 0x84;
    private const int WM_MOUSEMOVE = 0x0200;
    private const int WM_NCLBUTTONDOWN = 0xA1;
    private const int WM_NCLBUTTONMOVE = 0x00A0;
    private const int HT_CAPTION = 0x2;
    private const int HT_BOTTOMRIGHT = 17;
    private const int WM_SETREDRAW = 11;

    protected bool HandleWndProc(ref Message msg) {
      if (msg.Msg == WM_NCHITTEST || msg.Msg == WM_MOUSEMOVE) {
        Point pos = new Point(msg.LParam.ToInt32() & 0xffff, msg.LParam.ToInt32() >> 16);
        pos = PointToClient(pos);

        if (pos.X >= ClientSize.Width - GRIP_SIZE && pos.Y >= ClientSize.Height - GRIP_SIZE) {
          msg.Result = (IntPtr)HT_BOTTOMRIGHT;

          return true;
        }
      }
      return false;
    }

    protected override void WndProc(ref Message m) {
      if (!HandleWndProc(ref m)) {
        base.WndProc(ref m);
      }
    }

    [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
    private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
    [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
    private static extern bool ReleaseCapture();

    public static void SuspendDrawing(Control parent) {
      SendMessage(parent.Handle, WM_SETREDRAW, 0, 0);
    }

    public static void ResumeDrawing(Control parent) {
      SendMessage(parent.Handle, WM_SETREDRAW, 1, 0);
      parent.Refresh();
    }

    public void RebuildTrackerInstances() {
      TrackedInstances.Clear();
      TrackedInstances.AddRange(settingsControl.VisibleSpecies.Select(species => new TrackedPokemonSpeciesInstance(species)));
    }

    private void OnBackgroundColorChanged(Color color) {
      this.catchGridLayout.BackColor = color;

      Invalidate();
    }

    public void RecolorGridElements() {
      foreach (var button in trackerButtons) {
        button.BackColor = GetTrackerButtonColor(button.Species);
      }
    }

    public void CreateGridElements() {
      foreach (var button in trackerButtons) {
        this.catchGridLayout.Controls.Remove(button);
        button.Dispose();
      }

      trackerButtons.Clear();

      var imageDirectory = Path.Combine(Path.GetDirectoryName(settingsControl.SpeciesDictionary.Filename), settingsControl.SpeciesDictionary.SpriteDirectory);

      foreach (var trackedSpecies in TrackedInstances) {
        trackerButtons.Add(CreateTrackerButton(trackedSpecies, imageDirectory));
      }
    }

    private void ResizeGrid() {
      this.catchGridLayout.ColumnCount = (int)Math.Floor(1f * settingsControl.TrackerWindowWidth / settingsControl.TrackerCellLength);
      this.catchGridLayout.RowCount = (int)Math.Ceiling(1f * settingsControl.VisibleSpecies.Count() / this.catchGridLayout.ColumnCount);

      this.catchGridLayout.ColumnStyles.Clear();
      this.catchGridLayout.RowStyles.Clear();

      for (int i = 0; i < catchGridLayout.ColumnCount; i += 1) {
        this.catchGridLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, settingsControl.TrackerCellLength));
      }

      for (int i = 0; i < catchGridLayout.RowCount; i += 1) {
        this.catchGridLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, settingsControl.TrackerCellLength));
      }
    }

    private TrackerButton CreateTrackerButton(TrackedPokemonSpeciesInstance trackedSpecies, string imageDirectory) {
      TrackerButton button = new TrackerButton();

      button.Species = trackedSpecies;
      button.Dock = DockStyle.Fill;
      button.Parent = this.catchGridLayout;
      button.Margin = Padding.Empty;
      button.FlatStyle = FlatStyle.Flat;
      button.FlatAppearance.BorderSize = 0;
      button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      button.BackgroundImage = Image.FromFile(Path.Combine(imageDirectory, $"{trackedSpecies.Definition.DexNumber}.png"));
      button.BackgroundImageLayout = ImageLayout.Zoom;
      button.BackColor = GetTrackerButtonColor(trackedSpecies);

      button.MouseUp += TrackerButton_Click(trackedSpecies, button);

      this.catchGridLayout.Controls.Add(button);

      return button;
    }

    private MouseEventHandler TrackerButton_Click(TrackedPokemonSpeciesInstance instance, Button button) {
      return (o, evt) => {
        TrackerState? nextState = null;

        if (evt.Button == MouseButtons.Left) {
          nextState = GetNextState(instance);
        } else if (evt.Button == MouseButtons.Right) {
          nextState = GetPreviousState(instance);
        }

        if (nextState.HasValue) {
          instance.State = nextState.Value;
          button.BackColor = GetTrackerButtonColor(instance);
          OnCatch.Invoke(instance.Definition, nextState.Value);
        }
      };
    }

    private TrackerState? GetNextState(TrackedPokemonSpeciesInstance instance) {
      switch (instance.State) {
        case TrackerState.DEFAULT:
          return TrackerState.LEFT_CLICK;
        case TrackerState.LEFT_CLICK:
          return TrackerState.DOUBLE_CLICK;
        case TrackerState.RIGHT_CLICK:
          return TrackerState.DEFAULT;
        default:
          return null;
      }
    }
    private TrackerState? GetPreviousState(TrackedPokemonSpeciesInstance instance) {
      switch (instance.State) {
        case TrackerState.DEFAULT:
          return TrackerState.RIGHT_CLICK;
        case TrackerState.LEFT_CLICK:
          return TrackerState.DEFAULT;
        case TrackerState.DOUBLE_CLICK:
          return TrackerState.LEFT_CLICK;
        default:
          return null;
      }
    }

    private Color GetTrackerButtonColor(TrackedPokemonSpeciesInstance instance) {
      switch (instance.State) {
        case TrackerState.DEFAULT:
          return settingsControl.DefaultStateColor;
        case TrackerState.LEFT_CLICK:
          return settingsControl.LeftClickStateColor;
        case TrackerState.DOUBLE_CLICK:
          return settingsControl.DoubleClickStateColor;
        case TrackerState.RIGHT_CLICK:
          return settingsControl.RightClickStateColor;
        default:
          return Color.FromArgb(255, 255, 255);
      }
    }

    private void UpdateSize() {
      if (settingsControl != null) {
        ClientSize = new Size(settingsControl.TrackerWindowWidth, settingsControl.TrackerWindowHeight);
      }
    }

    private void TrackerWindow_ResizeEnd(object sender, EventArgs e) {
      SetSize(Size);
    }

    private void TrackerWindow_MouseDown(object sender, MouseEventArgs e) {
      if (e.Button == MouseButtons.Left) {
        ReleaseCapture();
        SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
      }
    }

    private void TrackerWindow_MouseMove(object sender, MouseEventArgs e) {
      ReleaseCapture();
      SendMessage(Handle, WM_NCLBUTTONMOVE, HT_CAPTION, 0);
    }

    private void TrackerWindow_LocationChanged(object sender, EventArgs e) {
      SetPosition(Location);
    }

    private void SetSize(Size size) {
      ClientSize = size;

      if (settingsControl != null) {
        settingsControl.RequestedTrackerWindowWidth = Size.Width;
        ResizeGrid();
        UpdateSize();
      }
    }

    private void SetPosition(Point pos) {
      Location = pos;
      if (settingsControl != null) {
        settingsControl.TrackerXPosition = pos.X;
        settingsControl.TrackerYPosition = pos.Y;
      }
    }
  }
}