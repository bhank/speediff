using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CoyneSolutions.SpeeDiff
{
    public class SynchronizedScrollRichTextBox : RichTextBox, ISynchronizedScrollTextBox
    {
        // http://stackoverflow.com/questions/1827323/c-synchronize-scroll-position-of-two-richtextboxes
        // http://stackoverflow.com/questions/3322741/synchronizing-multiline-textbox-positions-in-c-sharp
        // https://gist.github.com/jkingry/593809
        // http://www.codeproject.com/Questions/293542/VB-Net-Custome-RichTextBox-SetScrollPos

        private readonly List<SynchronizedScrollRichTextBox> horizontalScrollPeers = new List<SynchronizedScrollRichTextBox>();
        private readonly List<SynchronizedScrollRichTextBox> verticalScrollPeers = new List<SynchronizedScrollRichTextBox>();

        public void AddPeers(params SynchronizedScrollRichTextBox[] newPeers)
        {
            AddHorizontalScrollPeers(newPeers);
            AddVerticalScrollPeers(newPeers);
        }

        public void AddVerticalScrollPeers(params SynchronizedScrollRichTextBox[] newPeers)
        {
            verticalScrollPeers.AddRange(newPeers);
            foreach (var peer in newPeers)
            {
                var currentPeer = peer;
                currentPeer.verticalScrollPeers.Add(this);
                var otherPeers = newPeers.Where(p => p != currentPeer).ToList();
                currentPeer.verticalScrollPeers.AddRange(otherPeers);
            }
        }

        public void AddHorizontalScrollPeers(params SynchronizedScrollRichTextBox[] newPeers)
        {
            horizontalScrollPeers.AddRange(newPeers);
            foreach (var peer in newPeers)
            {
                var currentPeer = peer;
                currentPeer.horizontalScrollPeers.Add(this);
                var otherPeers = newPeers.Where(p => p != currentPeer).ToList();
                currentPeer.horizontalScrollPeers.AddRange(otherPeers);
            }
        }

        // This way works-- rapidly messing with the scroll wheel doesn't get it out of sync, and it handles keyboard navigation as well as scroll bar

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, ref Point lParam); // System.Drawing.Point works the same as a pinvoke-defined POINT

        private const UInt32 WM_USER = 0x0400;
        private const UInt32 EM_GETSCROLLPOS = WM_USER + 221;
        private const UInt32 EM_SETSCROLLPOS = WM_USER + 222;

        public bool DisableScrollSync { get; set; }

        private Point ScrollPosition
        {
            get
            {
                return RawScrollPosition;
            }
            set
            {
                var adjustedValue = value;
                adjustedValue.Y = (int)(adjustedValue.Y * scrollOffset);
                RawScrollPosition = adjustedValue;
            }
        }

        private Point RawScrollPosition
        {
            get
            {
                var point = new Point();
                SendMessage(Handle, EM_GETSCROLLPOS, IntPtr.Zero, ref point);
                return point;
            }
            set
            {
                var oldDisableScrollSyncValue = DisableScrollSync;
                DisableScrollSync = true;
                SendMessage(Handle, EM_SETSCROLLPOS, IntPtr.Zero, ref value);
                DisableScrollSync = oldDisableScrollSyncValue;
            }
        }

        protected override void OnVScroll(EventArgs e)
        {
            base.OnVScroll(e);
            SyncScrollPosition(true);
        }

        protected override void OnHScroll(EventArgs e)
        {
            base.OnHScroll(e);
            SyncScrollPosition(false);
        }

        private void SyncScrollPosition(bool vertical)
        {
            if (!DisableScrollSync)
            {
                var peers = vertical ? verticalScrollPeers : horizontalScrollPeers;
                var scrollPosition = ScrollPosition;
                foreach (var peer in peers)
                {
                    var peerPosition = peer.ScrollPosition;
                    if (vertical)
                    {
                        scrollPosition.X = peerPosition.X;
                    }
                    else
                    {
                        scrollPosition.Y = peerPosition.Y;
                    }
                    Debug.WriteLine("{0} setting {1} to {2}, {3}", Name, peer.Name, scrollPosition.X, scrollPosition.Y);
                    peer.ScrollPosition = scrollPosition;
                }
            }
        }

        private double scrollOffset = 1;
        public void CalculateScrollOffset()
        {
            scrollOffset = 1;
            Debug.Print("{0} CalculateScrollOffset", Name);
            SelectionStart = TextLength;
            ScrollToCaret();
            Debug.Print("{0} scrolled to caret: {1}, {2}", Name, RawScrollPosition.X, RawScrollPosition.Y);
            var yPos = RawScrollPosition.Y;
            ScrollPosition = new Point(0, yPos);
            Debug.Print("{0} scrolled to {1}: {2}, {3}", Name, yPos, RawScrollPosition.X, RawScrollPosition.Y);
            scrollOffset = (double) yPos/RawScrollPosition.Y;
            Debug.Print("{0} scrollOffset = {1}", Name, scrollOffset);
        }

        private int savedFirstVisibleLineNumber;
        private int savedSelectedLineNumber;
        public void SavePosition()
        {
            var firstCharIndex = GetCharIndexFromPosition(new Point(0, Font.Height/2)); // At 0,0 we might be on the previous line. Could be on the previous line even further down, if we have scrolled halfway between lines. Looking half the font size down seems reasonable.
            var firstLine = GetLineFromCharIndex(firstCharIndex);
            savedFirstVisibleLineNumber = firstLine;
            savedSelectedLineNumber = GetLineFromCharIndex(SelectionStart);
            Debug.Print("{0} saved first line {1}, selected line {2}", Name, savedFirstVisibleLineNumber, savedSelectedLineNumber);
        }

        public void RestorePosition()
        {
            Debug.Print("{0} restoring first line {1}, selected line {2}", Name, savedFirstVisibleLineNumber, savedSelectedLineNumber);
            var firstCharIndex = GetFirstCharIndexFromLine(savedFirstVisibleLineNumber);
            if (firstCharIndex == -1)
            {
                firstCharIndex = TextLength; // If we moved to a revision with a smaller file, and we were off the end of it, move to the end of the new revision.
            }
            SelectionStart = firstCharIndex;
            ScrollToCaret();
            var charIndex = GetFirstCharIndexFromLine(savedSelectedLineNumber);
            if (charIndex == -1) charIndex = 0;
            SelectionStart = charIndex;
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

        public bool ShowScrollBars
        {
            get { return ScrollBars != RichTextBoxScrollBars.None; }
            set { ScrollBars = value ? RichTextBoxScrollBars.Both : RichTextBoxScrollBars.None; }
        }

        public void AppendText(string text, Color color)
        {
            SelectionStart = TextLength;
            SelectionLength = 0;

            if (color != Color.Empty)
            {
                SelectionBackColor = color;
            }
            AppendText(text);
            SelectionBackColor = BackColor;

        }

        public void EnsureLongLineDoesNotWrap(string line)
        {
            var textWidth = TextRenderer.MeasureText(line, Font).Width;
            if (textWidth > RightMargin)
            {
                RightMargin = textWidth;
            }
        }
    }
}