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
        // http://www.codeproject.com/Questions/293542/VB-Net-Custome-RichTextBox-SetScrollPos

        private readonly List<SynchronizedScrollRichTextBox> peers = new List<SynchronizedScrollRichTextBox>();

        public void AddPeer(SynchronizedScrollRichTextBox peer, bool addReversePeer = false)
        {
            peers.Add(peer);
            if (addReversePeer)
            {
                peer.peers.Add(this);
            }
        }

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
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, ref POINT lParam); // http://www.pinvoke.net/default.aspx/user32.SendMessage

        private const UInt32 WM_USER = 0x0400;
        private const UInt32 EM_GETSCROLLPOS = WM_USER + 221;
        private const UInt32 EM_SETSCROLLPOS = WM_USER + 222;

        private bool settingScrollPosition;

        private POINT ScrollPosition
        {
            get
            {
                var point = new POINT();
                SendMessage(Handle, EM_GETSCROLLPOS, IntPtr.Zero, ref point);
                return point;
            }
            set
            {
                settingScrollPosition = true;
                SendMessage(Handle, EM_SETSCROLLPOS, IntPtr.Zero, ref value);
                settingScrollPosition = false;
            }
        }

        protected override void OnVScroll(EventArgs e)
        {
            base.OnVScroll(e);
            if (!settingScrollPosition)
            {
                var scrollPosition = ScrollPosition;
                foreach (var peer in peers)
                {
                    Debug.WriteLine("{0} setting {1} to {2}, {3}", this.Name, peer.Name, scrollPosition.X, scrollPosition.Y);
                    peer.ScrollPosition = scrollPosition;
                }
            }
        }

        #region Disable repainting

        // http://stackoverflow.com/questions/192413/how-do-you-prevent-a-richtextbox-from-refreshing-its-display

        private const UInt32 EM_GETEVENTMASK = WM_USER + 59;
        private const UInt32 EM_SETEVENTMASK = WM_USER + 69;
        private const UInt32 WM_SETREDRAW          = 0x000B;
        private IntPtr eventMask;

        public void StopRepaint()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(StopRepaint));
            }
            else
            {
                // Stop redrawing:
                SendMessage(Handle, WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero);
                // Stop sending of events:
                eventMask = SendMessage(Handle, EM_GETEVENTMASK, IntPtr.Zero, IntPtr.Zero);
            }
        }

        public void StartRepaint()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(StartRepaint));
            }
            else
            {
                // turn on events
                SendMessage(Handle, EM_SETEVENTMASK, IntPtr.Zero, eventMask);
                // turn on redrawing
                SendMessage(Handle, WM_SETREDRAW, new IntPtr(1), IntPtr.Zero);
                // this forces a repaint, which for some reason is necessary in some cases.
                Invalidate();
            }
        }

        #endregion
    }
}