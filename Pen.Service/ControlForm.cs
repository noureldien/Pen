// Documentation of My Project 
// 17.10.2010 10:33 AM [New strategy\approach in mouse movement]
// Now I'm having new strategy regarding how to move the cursor, which is 
// instead of calibration and concidence of the user virtual desktop with the screen
// we need some more virtualization, as the touch pad does
// douch bad don't comincide (match) your fingure position to the cursor position
// on the screen. Instead, it claculates dx and dy then add them to the coordinates
// of values of the cursor position on the screen

// 19.10.2010 6:11 PM [simulate mouse double click event]
// Now, I've successfully fininshed a well-defined mouse tracking and one click event
// I'm going now to develop the double click event
// I will depend on time staps in implementing one-click and double-click events
// as an example one-click must have a press-on time (100-500)ms
// if one-click event is called again whereas the time of last one-clickEvent 
// was < 700 ms, then perform double-click instead of one-click :)

// 19.10.2010 7:00 PM [Simulate drag and drop, scroll(scroll not virtual scroll)]
// 20.10.2010 12:8 AM [Finish Almost all mouse events successfully Alhamdole ALLAH]
// Finish Click, double click, DragDrop. Only missing right click
// In the right click, NN should be used to invovle the concept of Mouse Gesture Recognition 
// in our application

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;
using OpenCvSharp.UserInterface;
using System.Collections;
using System.Xml;
using System.Configuration;
using System.Speech.Recognition;

namespace Pen.Service
{
    partial class ControlForm : Form
    {        
        private Tracker tracking;                        // object from the class that do all the image processing and traking
        private bool isTracking = false;                 // indicates if traking is running or it is stopped
        private Microsoft.Ink.InkCollector inkCollector; // required to enable drawing ink strokes
        private Point formLocation = new Point();        // size and location of the windows form
        private Size formSize = new Size();              // size of the application window
        private readonly int screenHeight = SystemInformation.VirtualScreen.Height;
        private readonly int screenWidth = SystemInformation.VirtualScreen.Width;

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
        
        // constructor
        public ControlForm()
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
        /// event handler for form onLoad
        /// </summary>
        private void ControlForm_Load(object sender, EventArgs e)
        {
            // adjust starting position
            //this.StartPosition = FormStartPosition.Manual;            
            //int bottom = Screen.PrimaryScreen.WorkingArea.Bottom;
            //SystemInformation.VirtualScreen.Bottom;           
            this.Location = new Point(screenWidth - this.Width - 4, 60);
            formLocation = this.Location;
            formSize = this.Size;            
        }

        /// <summary>
        /// Event handler for the application window first show.
        /// </summary>       
        private void ControlForm_Shown(object sender, EventArgs e)
        {                    
            // initialize the tracker            
            tracking = new Tracker(ref label1, ref pictureBoxRegions, ref pictureBox1);
            // get parameter values of image processing required in mouseTrack
            GetSettings();
            // At the first time, all required objects for the camera capturing are defined.
            tracking.InitilizeCamera();
            InitializeInkCollector();
            InitializeSpeechRecognition();
            // list box selected item = 1st item 
            listBoxRegion.SelectedIndex = 0;
            // return focus back to the windows form as it was unfocused due
            // to initializing camera window
            this.Focus();
        }        

        /// <summary>
        /// safely dispose some objects to free resources
        /// </summary>        
        private void ControlForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isTracking)            
                tracking.StopProcessing();            

            SaveSettings();
            tracking.Dispose();
            this.Dispose(true);            
        }

        /// <summary>
        ///  do some actions when keypressed on this form
        /// </summary>        
        private void ControlForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
            else if (e.KeyCode == Keys.Space)
                buttonCapture.PerformClick();
            else if (e.KeyCode == Keys.S)
                tabControl.SelectedIndex = 0;
            else if (e.KeyCode == Keys.R)
                tabControl.SelectedIndex = 1;
            else if (e.KeyCode == Keys.C)
                tabControl.SelectedIndex = 2;

            // jusr for test purpose
            //else if (e.KeyCode == Keys.Q)
            //    mouseHandler.PressDown();
            //else if (e.KeyCode == Keys.W)
            //    mouseHandler.PressUp();
            //else if (e.KeyCode == Keys.E)
            //    mouseHandler.Click();
        }

        /// <summary>
        /// initalize ink Collector that uses the groupBoxRegions handle ot draw
        /// the users' reigons of interest. Also set drawing attributes.
        /// </summary>
        private void InitializeInkCollector()
        {
            // Create a new ink collector
            inkCollector = new Microsoft.Ink.InkCollector(groupBoxRegions.Handle);
            // Turn the ink collector on
            inkCollector.Enabled = true;
            // modify the drawwing attributes, these are the defaults
            // 
            //      AntiAliased     = true
            //      Color           = black
            //      FitToCurve      = false
            //      Height          = 1
            //      IgnorePressure  = false
            //      PenTip          = ball
            //      RasterOperation = copy pen
            //      Transparency    = 0
            //      Width           = 53 (2 pixels on a 96 dpi screen)
            inkCollector.DefaultDrawingAttributes.Height = inkCollector.DefaultDrawingAttributes.Width = 2;
            inkCollector.DefaultDrawingAttributes.Transparency = 170;
            inkCollector.DefaultDrawingAttributes.Color = Color.FromArgb(45, 94, 162);
        }

        /// <summary>
        /// capture video stream
        /// </summary>        
        private void buttonCapture_Click(object sender, EventArgs e)
        {
            if (isTracking)
            {
                tracking.StopProcessing();
                buttonCapture.Image = Pen.Service.Properties.Resources.play;
                //buttonCapture.Text = "Capture";
            }
            else
            {
                tracking.StartProcessing();
                buttonCapture.Image = Pen.Service.Properties.Resources.pause;
                //buttonCapture.Text = "Stop";
            }
            isTracking = !isTracking;
        }        
        
        /// <summary>
        /// change some parameters of blob detection and tracking
        /// </summary>        
        private void numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            switch (((NumericUpDown )sender).Name)
            {
                case "numericUpDown1":
                    tracking.TimerIntervalTime = (int)numericUpDown1.Value;
                    break;
                case "numericUpDown2":
                    tracking.grayLowValue = (int)numericUpDown2.Value;;
                    break;
                case "numericUpDown3":
                    tracking.smoothGaussianValue = (int)numericUpDown3.Value;
                    break;
                case "numericUpDown4":
                    tracking.blobCounterMaxSize = (int)numericUpDown4.Value;
                    break;
                case "numericUpDown5":
                    tracking.blobCounterMinSize = (int)numericUpDown5.Value;
                    break;
                case "numericUpDown6":
                    tracking.cursorMoveFactor = (int)numericUpDown6.Value;
                    break;
                case "numericUpDown7":
                    tracking.ResizeFactor = numericUpDown7.Value;
                    break;
                case "numericUpDown8":
                    tracking.zoomValue = (int)numericUpDown8.Value;
                    break;
                default:
                    break;
            }

        }
                
        /// <summary>
        /// checkboxes in the settings tab (H. and V. flip, play sound)
        /// </summary>        
        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            tracking.playSound = checkBox1.Checked;
            tracking.invertHorizontal = checkBox2.Checked;
            tracking.invertVertical = checkBox3.Checked;
        }       

        #region Calibration Restart (by click or by Voice Recognition)
        // speech recognition object
        //SpeechRecognizer speechRec;
        SpeechRecognitionEngine speechRecEngine;

        /// <summary>
        /// Intialize objects and grammars required for speech recognition 
        /// using Microsoft Speeach API (SAPI).
        /// </summary>
        private void InitializeSpeechRecognition()
        {
            //speechRec = new SpeechRecognizer();
            speechRecEngine = new SpeechRecognitionEngine();

            // Train the system to recognize these sentences            
            Choices c = new Choices();
            c.Add("Restart");
            c.Add("Reset");

            var grammarBuilder = new GrammarBuilder(c);
            Grammar grammar = new Grammar(grammarBuilder);

            //speechRecEngine.LoadGrammar(new DictationGrammar());
            speechRecEngine.LoadGrammar(grammar);
            speechRecEngine.SetInputToDefaultAudioDevice();
            speechRecEngine.SpeechRecognized += Rec_SpeechRecognized;
            // start/stop the speech recognizer si done when the user chooses calibration tab
        }

        /// <summary>
        /// Event handler for voice recognition. Called when a new voice is recognized.
        /// </summary>        
        private void Rec_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if ((e.Result.Text == "Restart" || e.Result.Text == "Reset")                
                && e.Result.Confidence > 0.90)
            
                tracking.RestartCalibrate();
                //MessageBox.Show(e.Result.Confidence.ToString());
        }

        /// <summary>
        /// Reatart calibrating process required for tracking with touch-screen concept.
        /// </summary>        
        private void buttonRestart_Click(object sender, EventArgs e)
        {
            tracking.RestartCalibrate();
        }
        #endregion       

        #region Show/ hide tooltip of how to define a region
        /// <summary>
        /// Show tooltip to help user on how to define regions of interest.
        /// </summary>               
        private void buttonHelp_MouseHover(object sender, EventArgs e)
        {
            // tooltip size x,y = 230,112
            toolTip.InitialDelay = 350;
            toolTip.ToolTipIcon = ToolTipIcon.Info;
            toolTip.BackColor = Color.FromArgb(255, 0, 0);
            toolTip.ForeColor = Color.FromArgb(255, 0, 0);
            toolTip.ToolTipTitle = "How to define regions?";

            string txt = "1. Select a certain user number.\n" +
                         "2. Start moving the pen to define.\n" +
                         "your region of interest.You are Done.\n\n" +
                         "-Click clear to re-define a new region.\n" +
                         "-Choose the first item if you are done.";

            toolTip.Show(txt, buttonHelp, 0, -114, 8000);

        }

        /// <summary>
        /// Hide tooltip if it is active/shown.
        /// </summary>       
        private void buttonHelp_MouseLeave(object sender, EventArgs e)
        {
            if (toolTip.Active)
                toolTip.Hide(buttonHelp);
        }
        #endregion

        #region Change Tracking modes (CursorSimpulation, Region Selection and Calibration)
        /// <summary>
        /// Change the tracking mode when user changes tab selection
        /// </summary>
        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl.SelectedTab == tabControl.TabPages[0])
            {
                if (radioButton1.Checked)
                    tracking.trackingPurpose = TrackingPurpose.CursorSimulation_TouchpadConcept;
                else
                    tracking.trackingPurpose = TrackingPurpose.CursorSimulation_TouchScreenConcept;
                // stop speech recognition
                speechRecEngine.RecognizeAsyncStop();
            }
            else if (tabControl.SelectedTab == tabControl.TabPages[1])
            {
                tracking.trackingPurpose = TrackingPurpose.RegionDetection;
                // stop speech recognition
                speechRecEngine.RecognizeAsyncStop();
            }
            else if (tabControl.SelectedTab == tabControl.TabPages[2])
            {
                tracking.RestartCalibrate();
                tracking.trackingPurpose = TrackingPurpose.Calibration;
                // start speech recognition                
                speechRecEngine.RecognizeAsync(RecognizeMode.Multiple);
            }            
        }

        /// <summary>
        /// Toggle the tracking concept
        /// </summary>       
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                tracking.trackingPurpose = TrackingPurpose.CursorSimulation_TouchpadConcept;
                tracking.ShowHideCursors(true);
            }
            else if (radioButton2.Checked)
            {
                tracking.trackingPurpose = TrackingPurpose.CursorSimulation_TouchScreenConcept;
                tracking.ShowHideCursors(false);
            }
            else if (radioButton3.Checked)
            {
                tracking.trackingPurpose = TrackingPurpose.None;
                tracking.ShowHideCursors(false);
            }
        }
        #endregion

        #region Region Detection for users
        // indicates whether the region detection timer is on or off
        //bool regionDetection = false;
        //int currentRegion = 0;        

        // Change current selected user
        private void listBoxRegion_SelectedIndexChanged(object sender, EventArgs e)
        {
            tracking.currentUser = listBoxRegion.SelectedIndex;
        }
                
        /// <summary>
        /// clear all region settings for this selected user
        /// </summary>        
        private void buttonClear_Click(object sender, EventArgs e)
        {

        }
        #endregion              

        #region Save and Get Settings of Image Processing
        /// <summary>
        /// read text file containing settings and set these settings
        /// </summary>
        private void GetSettings()
        {            
            numericUpDown1.Value = tracking.TimerIntervalTime = Properties.Settings.Default.timerIntervalTime;            
            numericUpDown2.Value = tracking.grayLowValue = Properties.Settings.Default.grayLowValue;
            numericUpDown3.Value = tracking.smoothGaussianValue = Properties.Settings.Default.smoothGaussianValue;
            numericUpDown4.Value = tracking.blobCounterMaxSize = Properties.Settings.Default.blobCounterMaxSize;
            numericUpDown5.Value = tracking.blobCounterMinSize = Properties.Settings.Default.blobCounterMinSize;
            numericUpDown6.Value = tracking.cursorMoveFactor = Properties.Settings.Default.cursorMoveFactor;
            numericUpDown7.Value = tracking.ResizeFactor = Properties.Settings.Default.resizeFactor;
            numericUpDown8.Value = tracking.zoomValue = Properties.Settings.Default.zoomValue;

            checkBox1.Checked = tracking.playSound = Properties.Settings.Default.playSound;
            checkBox2.Checked = tracking.invertHorizontal = Properties.Settings.Default.invertHorizontal;
            checkBox3.Checked = tracking.invertVertical = Properties.Settings.Default.invertVertical;
            tracking.calibrated = Properties.Settings.Default.calibrated;
            
            tracking.calibrationRegion.x1 = Properties.Settings.Default.x1;
            tracking.calibrationRegion.x2 = Properties.Settings.Default.x2;
            tracking.calibrationRegion.x3 = Properties.Settings.Default.x3;
            tracking.calibrationRegion.x4 = Properties.Settings.Default.x4;
            tracking.calibrationRegion.y1 = Properties.Settings.Default.y1;
            tracking.calibrationRegion.y2 = Properties.Settings.Default.y2;
            tracking.calibrationRegion.y3 = Properties.Settings.Default.y3;
            tracking.calibrationRegion.y4 = Properties.Settings.Default.y4;

            switch (Properties.Settings.Default.cursorSimulationConcept)
            {
                case CheckState.Checked:
                    radioButton1.Checked = true;
                    radioButton2.Checked = false;
                    radioButton3.Checked = false;
                    tracking.trackingPurpose = TrackingPurpose.CursorSimulation_TouchpadConcept;
                    break;
                case CheckState.Indeterminate:
                    radioButton1.Checked = false;
                    radioButton2.Checked = true;
                    radioButton3.Checked = false;
                    tracking.trackingPurpose = TrackingPurpose.CursorSimulation_TouchScreenConcept;
                    break;
                case CheckState.Unchecked:
                    radioButton1.Checked = false;
                    radioButton2.Checked = false;
                    radioButton3.Checked = true;
                    tracking.trackingPurpose = TrackingPurpose.None;
                    break;
                default:
                    break;
            }
        }
        
        /// <summary>
        /// save settings in the settings file
        /// </summary> 
        private void SaveSettings()
        {
            Properties.Settings.Default.timerIntervalTime = tracking.TimerIntervalTime;
            Properties.Settings.Default.grayLowValue = tracking.grayLowValue;
            Properties.Settings.Default.smoothGaussianValue = tracking.smoothGaussianValue;
            Properties.Settings.Default.blobCounterMaxSize = tracking.blobCounterMaxSize;
            Properties.Settings.Default.blobCounterMinSize = tracking.blobCounterMinSize;
            Properties.Settings.Default.cursorMoveFactor = tracking.cursorMoveFactor;
            Properties.Settings.Default.resizeFactor = tracking.ResizeFactor;
            Properties.Settings.Default.zoomValue = tracking.zoomValue;
                       
            Properties.Settings.Default.invertHorizontal = tracking.invertHorizontal;
            Properties.Settings.Default.invertVertical = tracking.invertVertical;
            Properties.Settings.Default.playSound = tracking.playSound;
            Properties.Settings.Default.calibrated = tracking.calibrated;            
            
            Properties.Settings.Default.x1 = tracking.calibrationRegion.x1;            
            Properties.Settings.Default.x2 = tracking.calibrationRegion.x2;
            Properties.Settings.Default.x3 = tracking.calibrationRegion.x3;
            Properties.Settings.Default.x4 = tracking.calibrationRegion.x4;
            Properties.Settings.Default.y1 = tracking.calibrationRegion.y1;
            Properties.Settings.Default.y2 = tracking.calibrationRegion.y2;
            Properties.Settings.Default.y3 = tracking.calibrationRegion.y3;
            Properties.Settings.Default.y4 = tracking.calibrationRegion.y4;

            if (radioButton1.Checked)
                Properties.Settings.Default.cursorSimulationConcept = CheckState.Checked;
            if (radioButton2.Checked)
                Properties.Settings.Default.cursorSimulationConcept = CheckState.Indeterminate;
            if (radioButton3.Checked)
                Properties.Settings.Default.cursorSimulationConcept = CheckState.Unchecked;            

            Properties.Settings.Default.Save();
            SaveToXML();
        }

        /// <summary>
        /// Save settings to XML config file of of Pen.Service.App
        /// </summary>
        private void SaveToXML()
        {
            DirectoryInfo directory = new DirectoryInfo(Environment.CurrentDirectory);
            string path = directory.Parent.Parent.Parent.FullName + "\\Pen.Service.App\\bin\\Debug\\Pen.Service.App.exe.config";

            XmlDocument doc = new XmlDocument();
            XmlTextReader reader = new XmlTextReader(path);
            doc.Load(reader);
            reader.Close();
            XmlNodeList nodes = doc.GetElementsByTagName("Pen.Service.App.Properties.Settings")[0].ChildNodes;
            nodes[0].ChildNodes[0].InnerXml = tracking.TimerIntervalTime.ToString();
            nodes[1].ChildNodes[0].InnerXml = tracking.grayLowValue.ToString();            
            nodes[2].ChildNodes[0].InnerXml = tracking.smoothGaussianValue .ToString();
            
            nodes[3].ChildNodes[0].InnerXml = tracking.calibrationRegion.x1.ToString();
            nodes[4].ChildNodes[0].InnerXml = tracking.calibrationRegion.x2.ToString();
            nodes[5].ChildNodes[0].InnerXml = tracking.calibrationRegion.x3.ToString();
            nodes[6].ChildNodes[0].InnerXml = tracking.calibrationRegion.x4.ToString();

            nodes[7].ChildNodes[0].InnerXml = tracking.calibrationRegion.y1.ToString();
            nodes[8].ChildNodes[0].InnerXml = tracking.calibrationRegion.y2.ToString();
            nodes[9].ChildNodes[0].InnerXml = tracking.calibrationRegion.y3.ToString();
            nodes[10].ChildNodes[0].InnerXml = tracking.calibrationRegion.y4.ToString();

            nodes[11].ChildNodes[0].InnerXml = tracking.calibrated.ToString();
            doc.Save(path);
        }
        #endregion                                      
                       
        #region Trials
        //System.Windows.Forms.Timer timer_;
        //private void button2_Click(object sender, EventArgs e)
        //{
        //    timer_ = new System.Windows.Forms.Timer();
        //    timer_.Interval = 1000 / 30;
        //    timer_.Tick += new EventHandler((object obj, EventArgs eventArgs) =>
        //    {
        //        Cursor.Position = new Point(Cursor.Position.X + 1, Cursor.Position.Y +1);
        //        Cursor.Position = new Point(Cursor.Position.X + 1, Cursor.Position.Y + 1);
        //        Cursor.Position = new Point(Cursor.Position.X + 1, Cursor.Position.Y + 1);
        //        Cursor.Position = new Point(Cursor.Position.X + 1, Cursor.Position.Y + 1);
        //        Cursor.Position = new Point(Cursor.Position.X + 1, Cursor.Position.Y + 1);
        //    }
        //    );
        //    Cursor.Position = new Point(0,0);
        //    timer_.Start();
        //}

        //private void button3_Click(object sender, EventArgs e)
        //{
        //    timer_.Stop();
        //}

        ///// <summary>
        ///// Export the calibration settings to Pen.Driver configuration file
        ///// </summary>        
        //private void ReadandWriteXML()
        //{
        //    DirectoryInfo directory = new DirectoryInfo(Environment.CurrentDirectory);
        //    string path = directory.Parent.Parent.Parent.FullName + "\\Pen.Driver\\bin\\Debug\\Pen.Driver.exe.config";

        //    XmlDocument doc = new XmlDocument();
        //    XmlTextReader reader = new XmlTextReader(path);
        //    doc.Load(reader);
        //    reader.Close();
        //    XmlNodeList nodes = doc.GetElementsByTagName("Pen.Driver.Properties.Settings")[0].ChildNodes;
        //    nodes[0].ChildNodes[0].InnerXml = tracking.TimerIntervalTime.ToString();
        //    nodes[1].ChildNodes[0].InnerXml = tracking.grayLowValue.ToString();
        //    nodes[2].ChildNodes[0].InnerXml = tracking.blobCounterMaxSize.ToString();
        //    nodes[3].ChildNodes[0].InnerXml = tracking.blobCounterMaxSize.ToString();
        //    nodes[4].ChildNodes[0].InnerXml = tracking.blobCounterMinSize.ToString();
        //    nodes[5].ChildNodes[0].InnerXml = tracking.cursorMoveFactor.ToString();
        //    nodes[6].ChildNodes[0].InnerXml = tracking.ResizeFactor.ToString();
        //    nodes[7].ChildNodes[0].InnerXml = tracking.zoomValue.ToString();
        //    nodes[8].ChildNodes[0].InnerXml = radioButton1.Checked.ToString();
        //    nodes[9].ChildNodes[0].InnerXml = tracking.playSound.ToString();
        //    nodes[10].ChildNodes[0].InnerXml = tracking.invertHorizontal.ToString();
        //    nodes[11].ChildNodes[0].InnerXml = tracking.invertVertical.ToString();

        //    nodes[12].ChildNodes[0].InnerXml = tracking.calibrationRegion.x1.ToString();
        //    nodes[13].ChildNodes[0].InnerXml = tracking.calibrationRegion.y1.ToString();

        //    nodes[14].ChildNodes[0].InnerXml = tracking.calibrationRegion.x2.ToString();
        //    nodes[15].ChildNodes[0].InnerXml = tracking.calibrationRegion.y2.ToString();

        //    nodes[16].ChildNodes[0].InnerXml = tracking.calibrationRegion.x3.ToString();
        //    nodes[17].ChildNodes[0].InnerXml = tracking.calibrationRegion.y3.ToString();

        //    nodes[18].ChildNodes[0].InnerXml = tracking.calibrationRegion.x4.ToString();
        //    nodes[19].ChildNodes[0].InnerXml = tracking.calibrationRegion.y4.ToString();
        //    doc.Save(path);           
        //}
        #endregion
    }    
}