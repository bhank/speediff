using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CoyneSolutions.SpeeDiff
{
    public class SynchronizedScrollRichTextBox : RichTextBox
    {
        // http://stackoverflow.com/questions/1827323/c-synchronize-scroll-position-of-two-richtextboxes
        // http://stackoverflow.com/questions/3322741/synchronizing-multiline-textbox-positions-in-c-sharp
        // https://gist.github.com/jkingry/593809

        private readonly List<SynchronizedScrollRichTextBox> peers = new List<SynchronizedScrollRichTextBox>();

        public void AddPeer(SynchronizedScrollRichTextBox peer, bool addReversePeer = false)
        {
            peers.Add(peer);
            if (addReversePeer)
            {
                peer.peers.Add(this);
            }
        }

        #region Bad WndProc way

        //private const int WM_VSCROLL = 0x115;
        //private const int WM_MOUSEWHEEL = 0x20a;
        //protected override void WndProc(ref Message msg)
        //{
        //    if (msg.Msg == WM_VSCROLL || msg.Msg == WM_MOUSEWHEEL)
        //    {
        //        foreach (var peer in peers)
        //        {
        //            var peerMessage = Message.Create(peer.Handle, msg.Msg, msg.WParam, msg.LParam);
        //            peer.DirectWndProc(ref peerMessage);
        //            Debug.WriteLine("Resending!");
        //        }
        //    }
        //    base.WndProc(ref msg);
        //}

        //private void DirectWndProc(ref Message msg)
        //{
        //    base.WndProc(ref msg);
        //}

        #endregion Bad WndProc way

        #region Bad GetScrollPos/SetScrollPos way

        //[DllImport("user32.dll", CharSet = CharSet.Auto)]
        //public static extern int GetScrollPos(IntPtr hWnd, int nBar);

        //[DllImport("user32.dll")]
        //private static extern int SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool bRedraw);

        //private const int SB_HORZ = 0x0;
        //private const int SB_VERT = 0x1;

        //public int HorizontalPosition
        //{
        //    get { return GetScrollPos((IntPtr)this.Handle, SB_HORZ); }
        //    set { SetScrollPos((IntPtr)this.Handle, SB_HORZ, value, true); }
        //}

        //public int VerticalPosition
        //{
        //    get { return GetScrollPos((IntPtr)this.Handle, SB_VERT); }
        //    set { SetScrollPos((IntPtr)this.Handle, SB_VERT, value, true); } // Setting it moves the scroll bar, but doesn't update the content!
        //}

        #endregion Bad GetScrollPos/SetScrollPos way

        #region Good EM_GETSCROLLPOS/EM_SETSCROLLPOS way

        // This way works-- rapidly messing with the scroll wheel doesn't get it out of sync, and it handles keyboard navigation as well as scroll bar

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                X = x;
                Y = y;
            }

            public POINT(Point pt) : this(pt.X, pt.Y)
            {
            }

            public static implicit operator Point(POINT p)
            {
                return new Point(p.X, p.Y);
            }

            public static implicit operator POINT(Point p)
            {
                return new POINT(p.X, p.Y);
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        //static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, ref POINT lParam); // http://www.pinvoke.net/default.aspx/user32.SendMessage

        private const UInt32 WM_USER = 0x0400;
        private const UInt32 EM_GETSCROLLPOS = WM_USER + 221;
        private const UInt32 EM_SETSCROLLPOS = WM_USER + 222;

        private POINT ScrollPosition
        {
            get
            {
                var point = new POINT();
                SendMessage(Handle, EM_GETSCROLLPOS, IntPtr.Zero, ref point);
                return point;
            }
            set { SendMessage(Handle, EM_SETSCROLLPOS, IntPtr.Zero, ref value); }
        }

        #endregion Good EM_GETSCROLLPOS/EM_SETSCROLLPOS way

        protected override void OnVScroll(EventArgs e)
        {
            base.OnVScroll(e);
            var scrollPosition = ScrollPosition;
            foreach (var peer in peers)
            {
                //peer.VerticalPosition = VerticalPosition;
                //peer.Refresh();
                Debug.WriteLine("{0} setting {1} to {2}, {3}", this.Name, peer.Name, scrollPosition.X, scrollPosition.Y);
                peer.ScrollPosition = scrollPosition;
            }
        }
    }
}