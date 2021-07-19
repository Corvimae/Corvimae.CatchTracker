using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Corvimae.CatchTracker {
  public class CounterCell : UserControl {
    private TransparentTable tableLayoutPanel1;
    private TransparentLabel counterNameLbl;
    private TransparentLabel counterCountLbl;

    public event MouseEventHandler HandleMouseMove;
    public event MouseEventHandler HandleMouseDown;

    public CounterDefinition Definition { get; set; }
    public int Count { get; set; } = 0;

    public CounterCell(CounterDefinition definition) {
      Definition = definition;

      InitializeComponent();

      counterNameLbl.Text = definition.Name;
      UpdateCount();
    }

    protected override void WndProc(ref Message m) {
      const int WM_NCHITTEST = 0x0084;
      const int HTTRANSPARENT = (-1);

      if (m.Msg == WM_NCHITTEST) {
        m.Result = (IntPtr)HTTRANSPARENT;
      } else {
        base.WndProc(ref m);
      }
    }

    public void UpdateCount() {
      counterCountLbl.Text = "" + (TrackerWindow.Instance?.TrackedInstances.Where(x => Definition.TrackedStates.Contains(x.State)).Count() ?? 0);
    }

    private void InitializeComponent() {
      this.tableLayoutPanel1 = new Corvimae.CatchTracker.TransparentTable();
      this.counterNameLbl = new Corvimae.CatchTracker.TransparentLabel();
      this.counterCountLbl = new Corvimae.CatchTracker.TransparentLabel();
      this.tableLayoutPanel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.BackColor = Color.Transparent;
      this.tableLayoutPanel1.ColumnCount = 1;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel1.Controls.Add(this.counterNameLbl, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.counterCountLbl, 0, 1);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 2;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(150, 150);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // counterNameLbl
      // 
      this.counterNameLbl.BackColor = Color.Transparent;
      this.counterNameLbl.AutoSize = true;
      this.counterNameLbl.Dock = System.Windows.Forms.DockStyle.Fill;
      this.counterNameLbl.Location = new System.Drawing.Point(3, 0);
      this.counterNameLbl.Name = "counterNameLbl";
      this.counterNameLbl.Size = new System.Drawing.Size(144, 75);
      this.counterNameLbl.TabIndex = 0;
      this.counterNameLbl.Text = "label1";
      this.counterNameLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // counterCountLbl
      // 
      this.counterCountLbl.BackColor = Color.Transparent;
      this.counterCountLbl.AutoSize = true;
      this.counterCountLbl.Dock = System.Windows.Forms.DockStyle.Fill;
      this.counterCountLbl.Location = new System.Drawing.Point(3, 75);
      this.counterCountLbl.Name = "counterCountLbl";
      this.counterCountLbl.Size = new System.Drawing.Size(144, 75);
      this.counterCountLbl.TabIndex = 1;
      this.counterCountLbl.Text = "label1";
      this.counterCountLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // CounterCell
      // 
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "CounterCell";
      this.BackColor = Color.Transparent;

      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.ResumeLayout(false);

    }

    protected virtual void CounterCell_MouseMove(object sender, MouseEventArgs e) {
      HandleMouseMove.Invoke(this, e);
    }
    protected virtual void CounterCell_MouseDown(object sender, MouseEventArgs e) {
      HandleMouseDown.Invoke(this, e);
    }
  }
}
