using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Pen.Service
{
    public partial class VirtualCursor : Form
    {
        #region DWM API
        int en;

        public struct MARGINS
        {
            public int m_Left;
            public int m_Right;
            public int m_Top;
            public int m_Buttom;
        };

        [System.Runtime.InteropServices.DllImport("dwmapi.dll")]
        public extern static int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS margin);

        [System.Runtime.InteropServices.DllImport("dwmapi.dll")]
        public extern static int DwmIsCompositionEnabled(ref int en);
        #endregion        

        /// <summary>
        /// Give each new instance created from this form a new picture.
        /// </summary>
        private static int cursorID = 0;

        /// <summary>
        /// Image of the imageBox (i.e virtual cursor).
        /// </summary>
        private Image img = null;

        /// <summary>
        /// After a short time (in seconds), the timer fadesOut the cursor (change opacity)
        /// because it has been unmoved for a this amount of time.
        /// </summary>
        private Timer timerCursorFadeOut = new Timer();
       
        /// <summary>
        /// Gets or sets the point that represents of the upper left corner of the cursor. Also 
        /// this runs a self-timer that makes the cursor fade out (change opacity)
        /// when it is active for along time.
        /// </summary>
        public Point Location_
        {
            get 
            {                
                return this.Location;                
            }

            set 
            {
                this.Opacity = 1;
                this.Location = value;
                timerCursorFadeOut.Stop();
                timerCursorFadeOut.Start();
            }
        }              

        // constructor
        public VirtualCursor()
        {
            InitializeComponent();
            
            // set some properties
            cursorID++;
            this.Size = new Size(40, 40);
            this.Opacity = 0.25;

            // set timer
            timerCursorFadeOut.Interval = 3500;
            timerCursorFadeOut.Tick += new EventHandler((object sender, EventArgs e) =>
            {
                // fadeOut                
                this.Opacity = 0.25;
                timerCursorFadeOut.Stop();
            });

            // set image 
            switch (cursorID)
            {
                case 1:
                    img = Properties.Resources.basketball;
                    break;

                case 2:
                    img = Properties.Resources.tree;
                    break;

                case 3:
                    img = Properties.Resources.idea;
                    break;

                case 4:
                    img = Properties.Resources.house;
                    break;

                default:
                    break;
            }
            this.Paint += VirtualCursor_Paint;

            #region Required to show glassy form
            en = 0;
            MARGINS mg = new MARGINS();
            mg.m_Buttom = 0;
            mg.m_Right = 0;
            mg.m_Left = 0;
            mg.m_Top = pictureBox1.Height;

            DwmIsCompositionEnabled(ref en);  //check if the desktop composition is enabled
            if (en > 0)
            {
                DwmExtendFrameIntoClientArea(this.Handle, ref mg);
            }
            else
            {
                MessageBox.Show("Desktop composition is disabled");
            }
            #endregion            
        }        

        /// <summary>
        /// OnPaint event Handler. Used to draw the image of the imageBox (i.e the virtual cursor).
        /// </summary>        
        private void VirtualCursor_Paint(object sender, PaintEventArgs e)
        {
            this.Size = new Size(40, 40);
            GlassText glasstxt = new GlassText();
            glasstxt.FillBlackRegion(e.Graphics, pictureBox1.ClientRectangle);
            e.Graphics.DrawImage(img, 0, 0);
            //this.Paint -= VirtualCursor_Paint;
        }        
    }
}
