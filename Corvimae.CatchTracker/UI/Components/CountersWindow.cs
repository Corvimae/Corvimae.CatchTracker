using Corvimae.CatchTracker.Component;
using LiveSplit.Model;
using LiveSplit.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Corvimae.CatchTracker {
  public partial class CountersWindow : Form {
    private static int GRIP_SIZE = 16;

    private static CountersWindow instance = null;
    public static CountersWindow Instance { get { return instance; } }

    protected Font CounterFont { get; set; }

    private CatchTrackerComponent trackerComponent;
    private CatchTrackerComponentSettings settingsControl;
    private LiveSplitState state;

    private List<CounterCell> counterCells = new List<CounterCell>();

    public CountersWindow(CatchTrackerComponent trackerComponent, CatchTrackerComponentSettings settingsControl, LiveSplitState state) {
      instance = this;

      this.trackerComponent = trackerComponent;
      this.settingsControl = settingsControl;
      this.state = state;

      DoubleBuffered = true;
      SetStyle(ControlStyles.ResizeRedraw, true);
      FormBorderStyle = FormBorderStyle.None;
      SizeGripStyle = SizeGripStyle.Show;
      SetStyle(ControlStyles.ResizeRedraw, true);

      TopMost = state.Layout.Settings.AlwaysOnTop;

      InitializeComponent();

      ContextMenu contextMenu = new ContextMenu();

      contextMenu.MenuItems.Add(new MenuItem("Reset", new EventHandler(OnReset)));

      this.ContextMenu = contextMenu;
    }

    protected override void OnShown(EventArgs e) {
      base.OnShown(e);

      RefreshWindow();
      OnBackgroundColorChanged(settingsControl.BackgroundColor);


      LocationChanged += CounterWindow_LocationChanged;
    }

    protected override void OnClosing(CancelEventArgs e) {
      base.OnClosing(e);

      LocationChanged -= CounterWindow_LocationChanged;
    }


    private void OnReset(object sender, EventArgs evt) {
      foreach (var instance in TrackerWindow.Instance?.TrackedInstances) {
        instance.State = instance.Definition.DefaultState;
      }

      trackerComponent.RefreshTrackerWindow(false);
    }

    public void RefreshWindow() {
      SuspendDrawing(this);

      CounterFont = settingsControl.OverrideTrackerFont ? settingsControl.TrackerFont : state.LayoutSettings.TextFont;

      CreateGridElements();

      SetSize(new Size(settingsControl.CounterWindowWidth, settingsControl.CounterWindowHeight));
      SetPosition(new Point(settingsControl.CounterXPosition, settingsControl.CounterYPosition));
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

    public void CreateGridElements() {
      foreach (var element in counterCells) {
        this.counterGrid.Controls.Remove(element);
        element.Dispose();
      }

      counterCells.Clear();


      foreach (var counter in settingsControl.CounterDefinitions) {
        var newCell = new CounterCell(counter);

        newCell.Font = CounterFont;

        this.counterGrid.Controls.Add(newCell);

        counterCells.Add(newCell);
      }
    }

    public void UpdateCounts() {
       foreach(var counter in counterCells) {
        counter.UpdateCount();
      }
    }

    private void ResizeGrid() {
      this.counterGrid.ColumnCount = (int)Math.Floor(1f * settingsControl.CounterWindowWidth / settingsControl.CounterCellWidth);
      this.counterGrid.RowCount = (int)Math.Ceiling(1f * settingsControl.CounterDefinitions.Count() / this.counterGrid.ColumnCount);

      this.counterGrid.ColumnStyles.Clear();
      this.counterGrid.RowStyles.Clear();

      for (int i = 0; i < counterGrid.ColumnCount; i += 1) {
        this.counterGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, settingsControl.CounterCellWidth));
      }

      for (int i = 0; i < counterGrid.RowCount; i += 1) {
        this.counterGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, settingsControl.CounterCellHeight));
      }
    }
    private void OnBackgroundColorChanged(Color color) {
      this.counterGrid.BackColor = color;

      Invalidate();
    }

    private void UpdateSize() {
      if (settingsControl != null) {
        ClientSize = new Size(settingsControl.CounterWindowWidth, settingsControl.CounterWindowHeight);
      }
    }

    private void CounterWindow_ResizeEnd(object sender, EventArgs e) {
      SetSize(Size);
    }

    private void CounterWindow_MouseDown(object sender, MouseEventArgs e) {
      if (e.Button == MouseButtons.Left) {
        ReleaseCapture();
        SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
      }
    }

    private void CounterWindow_MouseMove(object sender, MouseEventArgs e) {
      ReleaseCapture();
      SendMessage(Handle, WM_NCLBUTTONMOVE, HT_CAPTION, 0);
    }

    private void CounterWindow_LocationChanged(object sender, EventArgs e) {
      SetPosition(Location);
    }

    private void SetSize(Size size) {
      ClientSize = size;

      if (settingsControl != null) {
        settingsControl.RequestedCounterWindowWidth = Size.Width;
        ResizeGrid();
        UpdateSize();
      }
    }

    private void SetPosition(Point pos) {
      Location = pos;
      if (settingsControl != null) {
        settingsControl.CounterXPosition = pos.X;
        settingsControl.CounterYPosition = pos.Y;
      }
    }
  }
}
