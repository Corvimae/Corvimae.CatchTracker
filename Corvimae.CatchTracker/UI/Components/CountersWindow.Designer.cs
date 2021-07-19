
namespace Corvimae.CatchTracker {
  partial class CountersWindow {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.counterGrid = new TransparentTable();
      this.SuspendLayout();
      // 
      // counterGrid
      //
      this.counterGrid.AutoSize = true;
      this.counterGrid.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.counterGrid.ColumnCount = 2;
      this.counterGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.counterGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.counterGrid.Dock = System.Windows.Forms.DockStyle.Fill;
      this.counterGrid.ImeMode = System.Windows.Forms.ImeMode.On;
      this.counterGrid.Location = new System.Drawing.Point(0, 0);
      this.counterGrid.Margin = new System.Windows.Forms.Padding(0);
      this.counterGrid.Name = "counterGrid";
      this.counterGrid.RowCount = 2;
      this.counterGrid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.counterGrid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.counterGrid.Size = new System.Drawing.Size(800, 450);
      this.counterGrid.TabIndex = 0;
      this.counterGrid.MouseDown += new System.Windows.Forms.MouseEventHandler(this.CounterWindow_MouseDown);
      this.counterGrid.MouseMove += new System.Windows.Forms.MouseEventHandler(this.CounterWindow_MouseMove);
      // 
      // CountersWindow
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(800, 450);
      this.Controls.Add(this.counterGrid);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
      this.Name = "CountersWindow";
      this.Text = "CountersWindow";
      this.ResizeEnd += new System.EventHandler(this.CounterWindow_ResizeEnd);
      this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.CounterWindow_MouseDown);
      this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.CounterWindow_MouseMove);
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel counterGrid;
  }
}