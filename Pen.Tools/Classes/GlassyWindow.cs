using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Media;

namespace Pen.Tools
{
    /// <summary>
    /// Convert a window to a glassy window
    /// </summary>
    public class GlassyWindow
    {
        #region convert Window to glassy window
        [StructLayout(LayoutKind.Sequential)]
        private struct MARGINS
        {
            public int cxLeftWidth;      // width of left border that retains its size
            public int cxRightWidth;     // width of right border that retains its size
            public int cyTopHeight;      // height of top border that retains its size
            public int cyBottomHeight;   // height of bottom border that retains its size
        };

        [DllImport("DwmApi.dll")]
        private static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS pMarInset);

        /// <summary>
        /// convert this window sent to a glassy window
        /// </summary>
        /// <param name="window"></param>
        public static void LoadGlassyWindow(object window)
        {
            if (!(window is Window))
                throw new Exception("This is not a type of Window \nThis message is form LoadGlassyWindow method()\n");
            
            WindowInteropHelper windowInteropHelper = new WindowInteropHelper((Window)window);
            IntPtr myHwnd = windowInteropHelper.Handle;
            HwndSource mainWindowSrc = System.Windows.Interop.HwndSource.FromHwnd(myHwnd);
            mainWindowSrc.CompositionTarget.BackgroundColor = Color.FromArgb(0, 0, 0, 0);

            int value = -1;
            MARGINS margins = new MARGINS()
            {
                cxLeftWidth = value,
                cxRightWidth = value,
                cyBottomHeight = value,
                cyTopHeight = value
            };
            DwmExtendFrameIntoClientArea(myHwnd, ref margins);
        }
        #endregion
    }
}
