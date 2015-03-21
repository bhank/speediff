using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using FastColoredTextBoxNS;

namespace CoyneSolutions.SpeeDiff
{
    public class SynchronizedScrollFastColoredTextBox : FastColoredTextBox, ISynchronizedScrollTextBox
    {
        public SynchronizedScrollFastColoredTextBox() : base()
        {
            ShowLineNumbers = false;
        }

        private readonly List<SynchronizedScrollFastColoredTextBox> peers = new List<SynchronizedScrollFastColoredTextBox>();

        public void AddPeer(SynchronizedScrollFastColoredTextBox peer, bool addReversePeer = false)
        {
            peers.Add(peer);
            if (addReversePeer)
            {
                peer.peers.Add(this);
            }
        }

        public void AddPeers(params SynchronizedScrollFastColoredTextBox[] newPeers)
        {
            peers.AddRange(newPeers);
            foreach (var peer in newPeers)
            {
                var currentPeer = peer;
                currentPeer.peers.Add(this);
                var otherPeers = newPeers.Where(p => p != currentPeer).ToList();
                currentPeer.peers.AddRange(otherPeers);
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

        protected override void OnScroll(ScrollEventArgs se)
        {
            base.OnScroll(se);
            if (!settingScrollPosition)
            {
                var x = ScrollPosition;
                Debug.WriteLine(x.X + ", " + x.Y);
                
                var horizontalScroll = HorizontalScroll.Value;
                var verticalScroll = VerticalScroll.Value;
                foreach (var peer in peers)
                {
                    Debug.WriteLine("{0} setting {1} to {2}, {3}", this.Name, peer.Name, horizontalScroll, verticalScroll);
                    peer.HorizontalScroll.Value = horizontalScroll > peer.HorizontalScroll.Maximum ? peer.HorizontalScroll.Maximum : horizontalScroll;
                    peer.VerticalScroll.Value = verticalScroll > peer.VerticalScroll.Maximum ? peer.VerticalScroll.Maximum : verticalScroll;
                }
            }
        }

        private POINT savedScrollPosition;
        public void SaveScrollPosition()
        {
            savedScrollPosition = ScrollPosition;
        }

        public void RestoreScrollPosition()
        {
            ScrollPosition = savedScrollPosition;
        }

        #region Disable repainting

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

        public void AppendText(string text, Color color)
        {
            //Text += text;
            AppendText(text);
        }
    }
}