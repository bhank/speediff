using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CoyneSolutions.SpeeDiff
{
    public class ListViewWithSubitemTooltips : ListView
    {
        private const int WM_NOTIFY = 0x4E;
        private const int TTN_FIRST = -520;
        private const int TTN_GETDISPINFO = (TTN_FIRST - 10);
        private const int WM_USER = 0x0400;
        private const int TTM_SETDELAYTIME = (WM_USER + 3);
        private readonly IntPtr TTDT_AUTOPOP = new IntPtr(2);
        private readonly IntPtr tooltipHideDelay = new IntPtr(32767);
        private const int TTM_SETMAXTIPWIDTH = (WM_USER + 24);
        private readonly IntPtr maxTooltipWidth = new IntPtr(450);

        // http://stackoverflow.com/questions/1328266/how-to-set-tooltips-on-listview-subitems-in-net
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_NOTIFY)
            {
                var nmhdr = (NMHDR) m.GetLParam(typeof (NMHDR));
                if (nmhdr.code == TTN_GETDISPINFO)
                {
                    var info = HitTest(PointToClient(Cursor.Position));
                    if (info.Item != null && info.SubItem != null)
                    {
                        var dispInfo = (NMTTDISPINFO) m.GetLParam(typeof (NMTTDISPINFO));
                        // https://msdn.microsoft.com/en-us/library/windows/desktop/hh298403%28v=vs.85%29.aspx
                        SendMessage(dispInfo.hdr.hwndFrom, TTM_SETMAXTIPWIDTH, IntPtr.Zero, maxTooltipWidth); // This is required to enable multi-line tooltips; otherwise it's truncated at the first line break.
                        // https://social.msdn.microsoft.com/Forums/windows/en-US/fd1f21d7-160a-4553-8f9e-428e961a23a1/is-there-a-way-to-have-a-tooltip-display-for-longer-than-5-seconds?forum=winforms
                        SendMessage(dispInfo.hdr.hwndFrom, TTM_SETDELAYTIME, TTDT_AUTOPOP, tooltipHideDelay); // Show the tooltip for more than 5 seconds
                        dispInfo.lpszText = info.Item.ToolTipText;
                        if (RightToLeft == RightToLeft.Yes)
                        {
                            dispInfo.uFlags |= 4;
                        }
                        Marshal.StructureToPtr(dispInfo, m.LParam, false);
                        return; // Stop processing the message
                    }
                }
            }
            base.WndProc(ref m);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct NMHDR
    {
        public IntPtr hwndFrom;
        public IntPtr idFrom;
        public int code;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct NMTTDISPINFO
    {
        public NMHDR hdr;
        [MarshalAs(UnmanagedType.LPTStr)] public string lpszText;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)] public string szText;
        public IntPtr hinst;
        public int uFlags;
        public IntPtr lParam;
    }
}