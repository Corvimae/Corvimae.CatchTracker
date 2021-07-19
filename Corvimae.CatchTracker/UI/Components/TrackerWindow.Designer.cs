
namespace Corvimae.CatchTracker {
  partial class TrackerWindow {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if(disposing && (components != null)) {
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
      this.catchGridLayout = new Corvimae.CatchTracker.TransparentTable();
      this.SuspendLayout();
      // 
      // catchGridLayout
      // 
      this.catchGridLayout.AutoSize = true;
      this.catchGridLayout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.catchGridLayout.ColumnCount = 2;
      this.catchGridLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.catchGridLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.catchGridLayout.Dock = System.Windows.Forms.DockStyle.Fill;
      this.catchGridLayout.ImeMode = System.Windows.Forms.ImeMode.On;
      this.catchGridLayout.Location = new System.Drawing.Point(0, 0);
      this.catchGridLayout.Margin = new System.Windows.Forms.Padding(0);
      this.catchGridLayout.Name = "catchGridLayout";
      this.catchGridLayout.RowCount = 2;
      this.catchGridLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.catchGridLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.catchGridLayout.Size = new System.Drawing.Size(800, 450);
      this.catchGridLayout.TabIndex = 0;
      this.catchGridLayout.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TrackerWindow_MouseDown);
      this.catchGridLayout.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TrackerWindow_MouseMove);
      // 
      // TrackerWindow
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(800, 450);
      this.Controls.Add(this.catchGridLayout);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
      this.Name = "TrackerWindow";
      this.Text = "TrackerWindow";
      this.ResizeEnd += new System.EventHandler(this.TrackerWindow_ResizeEnd);
      this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TrackerWindow_MouseDown);
      this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TrackerWindow_MouseMove);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private TransparentTable catchGridLayout;
  }
}