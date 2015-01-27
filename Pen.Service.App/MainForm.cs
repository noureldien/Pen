using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Pen.Service.App
{
    public partial class MainForm : Form
    {
        private TouchTrackerImproved tracker;
        private bool isRunning = false;
        
        #region opjects required in painting the form as glassy
        private DwmApi.MARGINS m_glassMargins;
        private enum RenderMode { None, EntireWindow, TopWindow, Region };
        private RenderMode m_RenderMode;
        private Region m_blurRegion;
        #endregion

        #region methods required in painting the form glassy and original
        // This is a trick to get Windows to treat glass as part of the caption
        // area I learned from Daniel Moth.
        protected override void WndProc(ref Message msg)
        {
            base.WndProc(ref msg); // let the normal winproc process it

            const int WM_DWMCOMPOSITIONCHANGED = 0x031E;
            const int WM_NCHITTEST = 0x84;
            const int HTCLIENT = 0x01;

            switch (msg.Msg)
            {
                case WM_NCHITTEST:
                    if (HTCLIENT == msg.Result.ToInt32())
                    {
                        // it's inside the client area

                        // Parse the WM_NCHITTEST message parameters
                        // get the mouse pointer coordinates (in screen coordinates)
                        Point p = new Point();
                        p.X = (msg.LParam.ToInt32() & 0xFFFF);// low order word
                        p.Y = (msg.LParam.ToInt32() >> 16); // hi order word

                        // convert screen coordinates to client area coordinates
                        p = PointToClient(p);

                        // if it's on glass, then convert it from an HTCLIENT
                        // message to an HTCAPTION message and let Windows handle it from then on
                        if (PointIsOnGlass(p))
                            msg.Result = new IntPtr(2);
                    }
                    break;

                case WM_DWMCOMPOSITIONCHANGED:
                    if (!DwmApi.DwmIsCompositionEnabled())
                    {
                        m_RenderMode = RenderMode.None;
                        m_glassMargins = null;
                        if (m_blurRegion != null)
                        {
                            m_blurRegion.Dispose();
                            m_blurRegion = null;
                        }
                    }
                    break;
            }
        }

        private bool PointIsOnGlass(Point p)
        {
            // test for region or entire client area
            // or if upper window glass and inside it.
            // not perfect, but you get the idea
            return m_glassMargins != null &&
                (m_glassMargins.cyTopHeight <= 0 ||
                 m_glassMargins.cyTopHeight > p.Y);
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            if (DwmApi.DwmIsCompositionEnabled())
            {
                e.Graphics.FillRectangle(Brushes.Black, this.ClientRectangle);
                //switch (m_RenderMode)
                //{
                //    case RenderMode.EntireWindow:
                //        e.Graphics.FillRectangle(Brushes.Black, this.ClientRectangle);
                //        break;

                //    case RenderMode.TopWindow:
                //        e.Graphics.FillRectangle(Brushes.Black, Rectangle.FromLTRB(0, 0, this.ClientRectangle.Width, m_glassMargins.cyTopHeight));
                //        break;

                //    case RenderMode.Region:
                //        if (m_blurRegion != null) e.Graphics.FillRegion(Brushes.Black, m_blurRegion);
                //        break;
                //}
            }

            // You can experiment with text colors & opacities (255 = opaque, 0-0-0 = black)
            using (SolidBrush textBrush = new SolidBrush(Color.FromArgb(255, 0, 0, 0)))
            {
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
                e.Graphics.DrawString("This is writing on glass", this.Font, textBrush, 10, 10);
            }
        }
        #endregion

        /// <summary>
        /// Constructor. Intialize components.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();

            #region  Converting / rendering glassy lock of the form
            m_RenderMode = RenderMode.None;
            m_glassMargins = new DwmApi.MARGINS(-1, 0, 0, 0);
            m_RenderMode = RenderMode.EntireWindow;

            if (DwmApi.DwmIsCompositionEnabled())
                DwmApi.DwmExtendFrameIntoClientArea(this.Handle, m_glassMargins);
            this.Invalidate();
            #endregion
        }

        /// <summary>
        /// Form load. Set position and size. Start camera tracker and touch driver.
        /// </summary>        
        private void MainForm_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.calibrated)
            {
                tracker = new TouchTrackerImproved();
                GetSettings();
                tracker.CalculateCalibrationParameters();
                button.PerformClick();                
            }
            else
            {
                MessageBox.Show("Sorry, you have to calibrate the camera first.", "Notify", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                this.Close();
                Application.Exit();
                return;
            }   

            int screenHeight = SystemInformation.VirtualScreen.Height;
            int screenWidth = SystemInformation.VirtualScreen.Width;
            this.Location = new Point(screenWidth - this.Width - 5, 370);
        }

        /// <summary>
        /// Form first shown.
        /// </summary>       
        private void MainForm_Shown(object sender, EventArgs e)
        {
              
        }

        /// <summary>
        /// Dispose objects and release all resources.
        /// </summary>        
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isRunning)
            {
                tracker.StopProcessing();
                tracker.Dispose();
            }
        }

        /// <summary>
        /// Start/pause the tracker.
        /// </summary>        
        private void button_Click(object sender, EventArgs e)
        {
            if (isRunning)
            {
                tracker.StopProcessing();
                button.Text = "  Start ";
                button.Image = global::Pen.Service.App.Properties.Resources.play;
                //button1.Checked = false;
            }

            else
            {
                tracker.StartProcessing();
                button.Text = "  Stop ";
                button.Image = global::Pen.Service.App.Properties.Resources.pause;
                //button1.Checked = true;
            }

            isRunning = !isRunning;

        }

        /// <summary>
        /// Retreive some setting values from setting.setting.
        /// </summary>
        private void GetSettings()
        {
            tracker.timerIntervalTime = Properties.Settings.Default.timerIntervalTime;
            tracker.grayLowValue = Properties.Settings.Default.grayLowValue;
            tracker.smoothGaussianValue = Properties.Settings.Default.smoothGaussianValue;

            tracker.calibrationRegion.x1 = Properties.Settings.Default.x1;
            tracker.calibrationRegion.x2 = Properties.Settings.Default.x2;
            tracker.calibrationRegion.x3 = Properties.Settings.Default.x3;
            tracker.calibrationRegion.x4 = Properties.Settings.Default.x4;

            tracker.calibrationRegion.y1 = Properties.Settings.Default.y1;
            tracker.calibrationRegion.y2 = Properties.Settings.Default.y2;
            tracker.calibrationRegion.y3 = Properties.Settings.Default.y3;
            tracker.calibrationRegion.y4 = Properties.Settings.Default.y4;
        }
    }
}
