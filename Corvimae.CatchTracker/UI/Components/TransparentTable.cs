using System;
using System.Windows.Forms;

namespace Corvimae.CatchTracker{
  public class TransparentTable : TableLayoutPanel {
    protected override void WndProc(ref Message m) {
      const int WM_NCHITTEST = 0x0084;
      const int HTTRANSPARENT = (-1);

      if (m.Msg == WM_NCHITTEST) {
        m.Result = (IntPtr)HTTRANSPARENT;
      } else {
        base.WndProc(ref m);
      }
    }
  }
}
