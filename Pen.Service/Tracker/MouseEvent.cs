using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;

namespace Pen.Service
{
    /// <summary>
    /// Raise Events for a Cursor, eg. Click, double Click
    /// Press Down and Press Up 
    /// </summary>
    class MouseEvent
    {
        [DllImport("user32")]
        private static extern int SetCursorPos(int x, int y);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        private const int MOUSEEVENTF_MOVE = 0x01;
        private const int MOUSEEVENTF_ABSOLUTE = 0x8000;
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
        
        //Call the imported function with the cursor's current position
        // save cursor position when firing last one-click event, it helps in firing double click event
        //private int x = Cursor.Position.X;
        //private int y = Cursor.Position.Y;
        private int x, y;

        public void Click()
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }

        public void DoubleClick()
        {
            // manually double click mouse [call one click twice]
            // that's for tackling the probelm of fast double click time from windows options
            // also the problem of the cursor may be moved form the time of first-click event
            // to the time of the second-click event (which is about 700 ms at max.)

            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);            
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }

        public void PressDown()
        {
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);            
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
        }

        public void PressDown(int dx, int dy)
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN,(uint)dx, (uint)dy, 0, 0);
        }

        public void PressUp()
        {
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }

        public void PressUp(int dx, int dy)
        {
            mouse_event(MOUSEEVENTF_LEFTUP, (uint)dx, (uint)dy, 0, 0);
        }

        public void CursorPosition(int x, int y)
        {
            SetCursorPos(x, y);
            //mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE, 0, 0, 0, 0);
            //Cursor.Position = new Point(Cursor.Position.X + cursorDx, Cursor.Position.Y + cursorDy);
        }       
    }
}
