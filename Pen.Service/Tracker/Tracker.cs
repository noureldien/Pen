// ### Documentation of how to define a region ###
// Region (Region of interest RIO) is the space on the user desktop
// in where the movement of the IR pen is translated to cursor movement.
// Any IR pen movement outside of this region, is neglected by the algorithm.
// Defining a region is done by using a term called: Region Defining Session.
// The defining session starts when the user palces his pen on the desktop ( whether 
// he begin the movement of the IR pen or not). If the user moves up his pen 
// from the desktop, this is considered the end of the region defining session.
// During this session, the movement of the IR pen is monitored and at the end 
// of this session the boundaries of this region is calculated. If, at calculating the region
// of the user, the width-to-height ratio of the region determined by user was far away
// from the width-to-height ratio of the computer screen, then the algorith will try to handle
// this conflict by increasing the number of pixels of the cursor movement to balance these
// two ratios.        

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using Emgu.CV;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Blob;
using VideoInputSharp;
using System.Speech.Synthesis;

namespace Pen.Service
{
    /// <summary>
    /// Responsible of Image Processing, Blob Detection, 
    /// Cursor Moving and Cursor Event Raising    
    /// </summary>
    class Tracker
    {
        #region Public member variables
        /// <summary>
        /// Maximum diagonal length of detected blob.
        /// </summary>
        public int blobCounterMaxSize = 45;
        /// <summary>
        /// Minimum diagonal length of detected blob.
        /// </summary>
        public int blobCounterMinSize = 10;
        /// <summary>
        /// Gray level value. 255 is white, 0 is black.
        /// </summary>
        public int grayLowValue = 100;
        /// <summary>
        /// Mask value of smooth gaussian filter.
        /// </summary>
        public int smoothGaussianValue = 5;
        /// <summary>
        /// Make the cursor moving fast, that's because every 1 pixel movement on 
        /// the frame is translated as n-pixel movement for the cursor on the screen.
        /// </summary>
        public int cursorMoveFactor = 1;
        /// <summary>
        /// If true, 3 different system sounds will be played at click, double click and end of drag-drop action.
        /// </summary>
        public bool playSound = true;
        /// <summary>
        /// Horizontally invert the captured frame from the camera.
        /// </summary>
        public bool invertHorizontal = false;
        /// <summary>
        /// Vertically invert the captured frame from the camera.
        /// </summary>
        public bool invertVertical = false;
        /// <summary>
        /// zoom value.
        /// </summary>
        public int zoomValue = 0;
        /// <summary>
        /// The user whose region is under defining (being defined).
        /// </summary>
        public int currentUser = 0;
        /// <summary>
        /// Determine the purpose of the tracking.
        /// </summary>
        public TrackingPurpose trackingPurpose = TrackingPurpose.CursorSimulation_TouchpadConcept;
        /// <summary>
        /// Indicates the region of calibration.
        /// </summary>
        public Region_ calibrationRegion;
        /// <summary>
        /// Indicates if the calibration process is done or not.
        /// </summary>
        public bool calibrated = false;
        #endregion

        #region Public Properties
        /// <summary>
        /// Set interval time of the main timer.
        /// </summary>
        public int TimerIntervalTime
        {
            set
            {
                timerIntervalTime = value;
                mainTimer.Interval = timerIntervalTime;
            }

            get
            {
                return timerIntervalTime;
            }
        }
        /// <summary>
        /// Property of the resize factor of the camera frame.
        /// </summary>
        public decimal ResizeFactor
        {
            get
            {
                return resizeFactor;
            }

            set
            {
                resizeFactor = value;
                FrameResize();
            }
        }
        #endregion

        #region Private member variables
        /// <summary>
        /// Factor of resizing the camera frame, i.e multiply the original size 
        /// of the camera frame with this value.
        /// </summary>
        private decimal resizeFactor = 1.0m;
        /// <summary>
        /// Interval time of main timer.
        /// </summary>
        private int timerIntervalTime = 30;
        /// <summary>
        /// Label to dispaly the frame rate on the windows form.
        /// </summary>
        private Label labelCounter;
        /// <summary>
        /// Frame rate counter of the camera.
        /// </summary>
        private int counter = 0;
        /// <summary>
        /// Number of the connected camera.
        /// </summary>
        private readonly int deviceID = 0;
        private readonly int screenHeight = SystemInformation.VirtualScreen.Height;
        private readonly int screenWidth = SystemInformation.VirtualScreen.Width;
        private MultitouchDriver touchDriver = new MultitouchDriver();
        #endregion

        #region mainTimer and frame-per-second Timer
        private bool timerInProgress = false;
        /// <summary>
        /// This timer to calculate frames pers seconds of the camera.
        /// </summary>
        private System.Windows.Forms.Timer fpsTimer;
        /// <summary>
        /// This timer to used to do image processing on very new frame captured form the camera.
        /// </summary>
        private System.Windows.Forms.Timer mainTimer;
        #endregion

        #region video stream capturing and image objects using OpenCvSharp
        /// <summary>
        /// Resizing Value of the image after prespective transform
        /// </summary>
        private int resizeVal;
        private IplImage frame;
        private IplImage resizedFrame;        
        private IplImage grayFrame;
        private IplImage transformFrame;
        private IplImage labelFrame;
        //private CvCapture capture;
        //private Capture capture_;
        private VideoInput vi;
        private CvWindow window;
        private CvBlobs blobs = new CvBlobs();
        private CvPoint textPosition = new CvPoint(); // plain, triplex, 
        private CvFont font = new CvFont(FontFace.HersheyPlain, 2.5, 2.5, 0, 3);
        #endregion

        #region simulate Cursor using touch-pad concept
        //int cursorMoveError = 2;
        //int rectCenterX = 0, rectCenterY = 0;           // center of a detected blob = (x,y) + (with/2, height/2)
        /// <summary>
        /// represnets small cursor displacement
        /// </summary>
        private int cursorDx = 0, cursorDy = 0;
        /// <summary>
        /// Represents the coordination of the last detected blob.
        /// </summary>
        private int blobLastX = 0, blobLastY = 0;
        /// <summary>
        /// This indicates how fast double click should performend by user, time is in ms.
        /// </summary>
        private int cursorClickRepeatTime = 500;
        /// <summary>
        /// Indicates if blob is detected or not, i.e cursor needs movement or not.
        /// </summary>
        private bool blobDetected = false;
        /// <summary>
        /// Flags the begining of one movement session.
        /// </summary>
        private bool blobFirstMove = true;
        /// <summary>
        /// Indicates whether mouse down event is on|off, this flag is used in performing drag-drop action.
        /// </summary>
        private bool mouseDown = false;
        /// <summary>
        /// The point representing the position of the detected blob.
        /// </summary>
        private Point blobPosition = new Point();
        /// <summary>
        /// The point representing the last position of the touchpoint (cursor).
        /// </summary>
        private Point cursorPosition = new Point();
        /// <summary>
        /// Time Stamp.
        /// </summary>
        private DateTime cursorBeginMoveTime = DateTime.Now;
        /// <summary>
        /// Time Stamp.
        /// </summary>
        private DateTime cursorEndMoveTime = DateTime.Now;
        /// <summary>
        /// Required for handling double click event.
        /// </summary>
        private DateTime cursorLastCLickEvent = DateTime.Now;
        /// <summary>
        /// Contains methods to raise mouse Events like click, double click, drag-drop, PressDown, PressUp.
        /// </summary>
        private MouseEvent mouseEvent = new MouseEvent();
        /// <summary>
        /// Used to show a virtual cursor on the screen. A vistual cursor is simple a small image
        /// which is shown on the top.
        /// </summary>
        Dictionary<int, VirtualCursor> virtualCursors = new Dictionary<int, VirtualCursor>(4);
        #endregion

        #region Region Definition of touch-pad concept
        /// <summary>
        /// This where the regions of the users is saved
        /// </summary>
        private Dictionary<int, Region_> regions = new Dictionary<int, Region_>(4);
        /// <summary>
        /// The rectangles used to draw regions defined by user. used in touch-pad concept.
        /// </summary>
        private Dictionary<int, CvRect> rectangleRegions = new Dictionary<int, CvRect>(4);
        /// <summary>
        /// This is to save the drawings of the regions
        /// </summary>
        private Dictionary<int, Microsoft.Ink.Stroke> strokes = new Dictionary<int, Microsoft.Ink.Stroke>(4);
        /// <summary>
        /// The region that is under defining (being defined).
        /// </summary>
        private Region_ currentRegion = new Region_();
        /// <summary>
        /// The last point to be monitored in a defining session.
        /// </summary>
        private Point regionLastPoint = new Point();
        /// <summary>
        /// Flag the first movement in a new defining session of a certain region.
        /// </summary>
        private bool regionFirstMove = true;
        /// <summary>
        /// The picture which shows the user which corner (calibration point) is to calibrate.
        /// </summary>
        OpenCvSharp.UserInterface.PictureBoxIpl pictureBoxRegions;
        #endregion

        #region Calibration for touch-screen concept
        /// <summary>
        /// The x-axis ratio between the distance should be moved on the screen to the distance moved 
        /// on the camera frame. This is for cursor simulation using touch-screen concept.
        /// </summary>
        private double calibrationFactorX = 0;
        /// <summary>
        /// The Y-axis ratio between the distance should be moved on the screen to the distance moved 
        /// on the camera frame. This is for cursor simulation using touch-screen concept.
        /// </summary>
        private double calibrationFactorY = 0;
        /// <summary>
        /// The counter helps in changing the position of the calibration point 
        /// after a certain amount of time.
        /// </summary>
        private int calibrationCounter = 100;
        /// <summary>
        /// Indicates wich point is being calibrated now.
        /// </summary>
        private int calibrationPoint = 0;
        /// <summary>
        /// Rectangle Carrying info of ROI of calibration
        /// </summary>
        private CvRect calibrationROI;
        /// <summary>
        /// Calibration ROI of resized image
        /// </summary>
        private CvRect calibrationROIResized;
        /// <summary>
        /// Used in 2D prespective transformation of the frame used after calibration ends.
        /// </summary>
        private CvMat calibrationMatrix;
        /// <summary>
        /// The cross sign that moves on the corners of the calibration tab of the application.
        /// </summary>
        private PictureBox calibrationImg;
        CvPoint[][] polyline;
        #endregion

        // constructor
        public Tracker(ref Label lblCounter)
        {
            labelCounter = lblCounter;
            InitializeComponents();
        }

        // overloading constructor
        public Tracker(ref Label lblCounter, ref OpenCvSharp.UserInterface.PictureBoxIpl pictureBoxRegions, ref  PictureBox calibrationImg)
        {
            this.labelCounter = lblCounter;
            this.calibrationImg = calibrationImg;
            this.pictureBoxRegions = pictureBoxRegions;
            this.pictureBoxRegions.ImageIpl = new IplImage(pictureBoxRegions.Width, pictureBoxRegions.Height, BitDepth.U8, 3);
            InitializeComponents();
        }

        /// <summary>
        /// Initialize Camera, timer and some objects
        /// </summary>
        private void InitializeComponents()
        {
            // initialize mainTimer
            mainTimer = new System.Windows.Forms.Timer();
            mainTimer.Interval = timerIntervalTime;
            mainTimer.Tick += new EventHandler(timer_Tick);

            // initialize timer used to count frames per seconds of the camera
            fpsTimer = new System.Windows.Forms.Timer();
            fpsTimer.Interval = 1000;
            fpsTimer.Tick += new EventHandler((object obj, EventArgs eventArgs) =>
            {
                labelCounter.Text = counter.ToString();
                counter = 0;
            });

            // intialize and show virtual cursor
            virtualCursors.Add(1, new VirtualCursor());
            virtualCursors.Add(2, new VirtualCursor());
            virtualCursors.Add(3, new VirtualCursor());
            virtualCursors.Add(4, new VirtualCursor());

            // initial position of the touchPoints and virtualCursors
            cursorPosition.X = screenWidth / 2;
            cursorPosition.Y = screenHeight / 2;

            //int min, max = 0, SteppingDelta, currentValue, flags, defaultValue;
            //vi.GetVideoSettingCamera(deviceID, vi.PropZoom, out min, ref max, out SteppingDelta, out currentValue, out flags, out defaultValue);
            //MessageBox.Show("min" + min.ToString() + " max" + max.ToString() + " steppingDelta" + SteppingDelta + " currentValue" + currentValue + " flags" + flags + " defaultValue" + defaultValue);            
        }
        
        /// <summary>
        /// Initialize camera input, frame window and other image objects required.
        /// This is done after getting the settings of the tracker object of this class.
        /// </summary>
        public void InitilizeCamera()
        {
            // Intialize camera
            try
            {
                //capture_ = new Capture(1);
                vi = new VideoInput();
            }
            catch (Exception exception)
            {
                MessageBox.Show("Failed to initialize the camera, the program will be closed." +
                    "\n\nThis is the internal error:\n" + exception.Message, "Notify", MessageBoxButtons.OK, MessageBoxIcon.Information);
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }

            vi.SetupDevice(deviceID, 640, 480);
            vi.SetIdealFramerate(deviceID, 30);
            resizeVal = 2;

            //CvSize size = new CvSize(vi.GetWidth(deviceID), vi.GetHeight(deviceID));
            CvSize size = new CvSize(640, 480);
            frame = new IplImage(size, BitDepth.U8, 3);
            grayFrame = new IplImage(size, BitDepth.U8, 1);
            transformFrame = new IplImage(size, BitDepth.U8, 1);
            labelFrame = new IplImage(size, BitDepth.F32, 1);
            resizedFrame = new IplImage(size, BitDepth.U8, 1);                           

            // window to view what's going on
            window = new CvWindow("Pen Camera", WindowMode.KeepRatio);
            window.Resize(320, 240);
            window.Move(screenWidth - 614, 55);
            if (calibrated) CalculateCalibrationParameters();
            touchDriver.Start();
        }

        // timer tick event handler
        private void timer_Tick(object sender, EventArgs e)
        {
            Track();
            switch (trackingPurpose)
            {
                case TrackingPurpose.CursorSimulation_TouchpadConcept:
                    Trigger_TouchPadConcept_TouchEvents();
                    break;

                case TrackingPurpose.RegionDetection:
                    DefineRegion();
                    break;

                case TrackingPurpose.CursorSimulation_TouchScreenConcept:
                    Trigger_TouchScreenConcept();
                    break;

                case TrackingPurpose.Calibration:
                    Calibrate();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// used to dispose any object created from this class
        /// </summary>
        public void Dispose()
        {
            if (timerInProgress)
                mainTimer.Stop();

            if (mainTimer != null)
                mainTimer.Dispose();

            if (window != null)
            {
                window.Close();
                window.Dispose();
            }

            //if (capture_ != null)
            //    capture_.Dispose();

            if (vi != null)
            {
                vi.StopDevice(deviceID);
                vi.Dispose();
            }
        }

        #region Start/stop capturing frames form the camera
        /// <summary>
        /// Start mainThread, that starts tracking
        /// </summary>
        public void StartProcessing()
        {
            mainTimer.Start();
            fpsTimer.Start();
            timerInProgress = true;
        }

        /// <summary>
        /// Stop mainThread, that stops tracking
        /// </summary>
        public void StopProcessing()
        {
            mainTimer.Stop();
            fpsTimer.Stop();
            timerInProgress = false;
        }
        #endregion

        #region Frame zoom and resize
        /// <summary>
        /// Resize the camera frame
        /// </summary>
        private void FrameResize()
        {
            //if (timerInProgress)
            //    StopProcessing();

            //CvSize size = capture.QueryFrame().Size;            
            //Size size_ = capture_.QueryFrame().Size;
            //CvSize size = new CvSize(size_.Width, size_.Height);           

            //CvSize size = new CvSize(vi.GetWidth(deviceID), vi.GetHeight(deviceID));
            //size.Width = (int)(size.Width * resizeFactor);
            //size.Height = (int)(size.Height * resizeFactor);            
            //vi.SetupDevice(deviceID, size.Width, size.Height);

            //frame = new IplImage(size, BitDepth.U8, 3);
            //grayFrame = new IplImage(size, BitDepth.U8, 1);
            //labelFrame = new IplImage(size, BitDepth.F32, 1);
            //window.Resize(size.Width, size.Height);
            //if (!timerInProgress)
            //    StartProcessing();
        }

        /// <summary>
        /// zoom the image while manitiaing its size
        /// </summary>        
        private void Zoom(int zoomValue)
        {

        }
        #endregion

        /// <summary>
        /// Image Processing (blobs tracking). It is done using OpenCVSharp Library.
        /// </summary>
        private void Track()
        {
            // increment counter
            counter++;
            vi.GetPixels(deviceID, frame.ImageData, false, !invertVertical);
            if (invertHorizontal) frame.Flip(frame, FlipMode.Y);
            Cv.CvtColor(frame, grayFrame, ColorConversion.BgrToGray);

            if (calibrated)
            {
                Cv.Threshold(grayFrame, grayFrame, grayLowValue, 255, ThresholdType.Binary);
                Cv.WarpPerspective(grayFrame, transformFrame, calibrationMatrix, Interpolation.Linear | Interpolation.FillOutliers, CvScalar.ScalarAll(100));
                //Cv.Threshold(transformFrame, transformFrame, grayLowValue, 255, ThresholdType.Binary);
                
                Cv.Resize(transformFrame, resizedFrame);
                CvBlobLib.Label(resizedFrame, labelFrame, blobs);
                //blobs.RenderBlobs(labelFrame, doubleFrame, frame, RenderBlobsMode.Color);

                // draw the region od interest before and after prespective transform
                resizedFrame.DrawRect(calibrationROIResized, CvColors.Red);
                resizedFrame.DrawPolyLine(polyline, true, CvColors.Blue);
                window.Image = resizedFrame;
            }
            else
            {
                Cv.Threshold(grayFrame, grayFrame, grayLowValue, 255, ThresholdType.Binary);
                CvBlobLib.Label(grayFrame, labelFrame, blobs);
                blobs.RenderBlobs(labelFrame, frame, frame, RenderBlobsMode.Color);
                //Cv.CvtColor(grayFrame, frame, ColorConversion.GrayToBgr);
                //Cv.Smooth(grayFrame, grayFrame, SmoothType.Gaussian, smoothGaussianValue);
                //blobs.FilterByArea((uint)(blobCounterMinSize^2), (uint)(blobCounterMaxSize^2));
                //blobs.RenderBlobs(labelFrame, frame, frame, RenderBlobsMode.Color);

                // label the blobs with numbers
                for (uint i = 1; i <= blobs.Values.Count; i++)
                {
                    textPosition.X = (int)blobs[i].Centroid.X;
                    textPosition.Y = (int)blobs[i].Centroid.Y;
                    frame.PutText(i.ToString(), textPosition, font, CvColors.White);
                }
                // show image on the separate window
                window.Image = frame;
            }            

            // get the position of the detected blob (if exists)
            if (blobs.Count > 0)
            {
                // indicates that a blob was detected
                blobDetected = true;
                // get position of detected blob
                blobPosition.X = (int)blobs[1].Centroid.X;
                blobPosition.Y = (int)blobs[1].Centroid.Y;
                //cursorPosition.X = (int)blobs[blobs.GreaterBlob()].Centroid.X;
                //cursorPosition.Y = (int)blobs[blobs.GreaterBlob()].Centroid.Y;                
            }
            else
            {
                blobDetected = false;
            }             
        }

        #region Touch-pad concept and region definition
        /// <summary>
        /// Implements the strategy of cursor events: move, click, double click and drag-drop.
        /// This is done under the touch-pad concpet. Triggered events are typical mouse events.
        /// </summary>
        private void Trigger_TouchPadConcept_ClickEvents()
        {
            // check if a blob was detected, i.e. a cursor needs movement
            if (blobDetected)
            {
                // check if it is the first point for a new cursor movement
                if (blobFirstMove)
                {
                    blobFirstMove = false;
                    cursorDx = cursorDy = 0;  // no delta movement

                    // check for sufficient condition required for drag-and-drop action
                    // i.e. to call mouse down while there was a one-click event fired recently
                    int spanLastClickEvent = (int)(DateTime.Now - cursorLastCLickEvent).TotalMilliseconds;
                    if (spanLastClickEvent < cursorClickRepeatTime)
                    {
                        mouseDown = true;
                        mouseEvent.PressDown();
                    }
                    // register time for mouse BeginMove of new movement session
                    cursorBeginMoveTime = DateTime.Now;
                }
                else
                {
                    cursorDx = (blobPosition.X - blobLastX) * cursorMoveFactor;
                    cursorDy = (blobPosition.Y - blobLastY) * cursorMoveFactor;
                }

                blobLastX = blobPosition.X;
                blobLastY = blobPosition.Y;
                // move cursor only if dx is less than the error
                // that is in case of first move only                
                mouseEvent.CursorPosition(Cursor.Position.X + cursorDx, Cursor.Position.Y + cursorDy);
            }

            else
            {
                // register the timer
                cursorEndMoveTime = DateTime.Now;

                // the problem is when there is no blob detected [i.e cursorMove=false]
                // this part in called several times, so we involve this
                // condition cursorFirstMove=False which means cursorFirstStop=true
                // to call MouseHook only one time during the mouse stop session                
                if (blobFirstMove == false)
                {
                    // this is to perform PressUp at the end of the DragDrop Action [if exists]
                    // to remove the effect of Mouse PressDown event fired at the begining
                    // of DragDrop [if exists]
                    if (mouseDown)
                    {
                        mouseDown = false;
                        mouseEvent.PressUp();
                        if (playSound)
                            System.Media.SystemSounds.Exclamation.Play();
                    }

                    int upperBound = 500;
                    int lowerBound = 100;
                    // span and compare one complete cursor movement
                    int span = (int)(cursorEndMoveTime - cursorBeginMoveTime).TotalMilliseconds;
                    int compare = DateTime.Compare(cursorEndMoveTime, cursorBeginMoveTime);
                    // span to calculate time between two successive mouse clickEvents
                    int spanLastClick = (int)(DateTime.Now - cursorLastCLickEvent).TotalMilliseconds;

                    // illustration return value of DateTime.Compare() method
                    // t1 is earlier than t2 [Less than zero]
                    // t1 is the same as  t2 [Zero]
                    // t1 is later than   t2 [Greater than zero]

                    // check for conditions sufficient to fire a mouse one-click event
                    if (span > lowerBound && span < upperBound && compare > 0)
                    {
                        // check for conditions sufficient to fire a mouse double-click event
                        if (spanLastClick < cursorClickRepeatTime)
                        {
                            mouseEvent.DoubleClick();
                            if (playSound)
                                System.Media.SystemSounds.Beep.Play();
                        }
                        // fire a one-click event
                        else
                        {
                            mouseEvent.Click();
                            if (playSound)
                                System.Media.SystemSounds.Asterisk.Play();
                        }

                        cursorLastCLickEvent = DateTime.Now;
                    }

                    blobFirstMove = true;
                }
            }
        }

        /// <summary>
        /// Implements the strategy of cursor events: move, click, double click and drag-drop.
        /// This is done under the touch-pad concept. The triggered events are touch events
        /// not the typical mouse events.
        /// </summary>
        private void Trigger_TouchPadConcept_TouchEvents()
        {
            // check if a blob was detected, i.e. a cursor needs movement
            if (blobDetected)
            {
                // check if it is the first point for a new cursor movement
                if (blobFirstMove)
                {
                    blobFirstMove = false;
                    // no delta movement (displacement)
                    cursorDx = cursorDy = 0;

                    // check for sufficient condition required for drag-and-drop action
                    // i.e. to call touch down while there was a one-click event fired recently
                    int spanLastClickEvent = (int)(DateTime.Now - cursorLastCLickEvent).TotalMilliseconds;
                    if (spanLastClickEvent < cursorClickRepeatTime)
                    {
                        //mouseEvent.PressDown();
                        mouseDown = true;
                        touchDriver.SendContact(1, virtualCursors[1].Location.X, virtualCursors[1].Location.Y, HidContactState.Adding);
                        Console.WriteLine("New     " + virtualCursors[1].Location.X + ", " + virtualCursors[1].Location.Y);
                    }
                    // register time for mouse BeginMove of new movement session
                    cursorBeginMoveTime = DateTime.Now;
                }
                else
                {
                    // calculate the cursor delta dispalcement
                    cursorDx = (blobPosition.X - blobLastX) * cursorMoveFactor;
                    cursorDy = (blobPosition.Y - blobLastY) * cursorMoveFactor;
                }

                // save coordinates of last blob
                blobLastX = blobPosition.X;
                blobLastY = blobPosition.Y;

                // check screen-boundary limitation of new position

                if ((cursorPosition.X + cursorDx) < 0 ||
                    (cursorPosition.X + cursorDx) > screenWidth ||
                    (cursorPosition.Y + cursorDy) < 0 ||
                    (cursorPosition.Y + cursorDy) > screenHeight)
                {
                    Console.WriteLine("Out:    " + (cursorPosition.X + cursorDx) + ", " + (cursorPosition.Y + cursorDy));
                    return;
                }

                // check also if there was no new delta movement
                if (cursorDx == 0 && cursorDx == 0)
                {
                    // point is out of region of interest
                    Console.WriteLine("No Movement:    " + (cursorPosition.X + cursorDx) + ", " + (cursorPosition.Y + cursorDy));
                    return;
                }

                // update cursor position and move virtualCursor
                cursorPosition.X += cursorDx;
                cursorPosition.Y += cursorDy;
                if (mouseDown)
                {
                    touchDriver.SendContact(1, cursorPosition.X, cursorPosition.Y, HidContactState.Updated);
                }
                virtualCursors[1].Location_ = cursorPosition;
                Console.WriteLine("Move:   " + cursorPosition.X + ", " + cursorPosition.Y);
            }

            else
            {
                // register the timer
                cursorEndMoveTime = DateTime.Now;

                // the problem is when there is no blob detected [i.e cursorMove=false]
                // this part in called several times, so we involve this
                // condition cursorFirstMove=False which means cursorFirstStop=true
                // to call MouseHook only one time during the mouse stop session                
                if (blobFirstMove == false)
                {
                    // this is to perform PressUp at the end of the DragDrop Action [if exists]
                    // to remove the effect of Mouse PressDown event fired at the begining
                    // of DragDrop [if exists]
                    if (mouseDown)
                    {
                        //mouseEvent.PressUp();
                        mouseDown = false;
                        touchDriver.SendContact(1, virtualCursors[1].Location.X, virtualCursors[1].Location.Y, HidContactState.Removing);
                        Console.WriteLine("Remove     " + virtualCursors[1].Location.X + ", " + virtualCursors[1].Location.Y);
                        if (playSound)
                            System.Media.SystemSounds.Exclamation.Play();
                    }

                    int upperBound = 500;
                    int lowerBound = 100;
                    // span and compare one complete cursor movement
                    int span = (int)(cursorEndMoveTime - cursorBeginMoveTime).TotalMilliseconds;
                    int compare = DateTime.Compare(cursorEndMoveTime, cursorBeginMoveTime);
                    // span to calculate time between two successive mouse clickEvents
                    int spanLastClick = (int)(DateTime.Now - cursorLastCLickEvent).TotalMilliseconds;

                    // illustration return value of DateTime.Compare() method
                    // t1 is earlier than t2 [Less than zero]
                    // t1 is the same as  t2 [Zero]
                    // t1 is later than   t2 [Greater than zero]

                    // check for conditions sufficient to fire a mouse one-click event
                    if (span > lowerBound && span < upperBound && compare > 0)
                    {

                        if (spanLastClick < cursorClickRepeatTime)
                        {
                            //mouseEvent.DoubleClick();
                            touchDriver.SendContact(1, virtualCursors[1].Location.X, virtualCursors[1].Location.Y, HidContactState.Adding);
                            Console.WriteLine("Add DC     " + virtualCursors[1].Location.X + ", " + virtualCursors[1].Location.Y);

                            touchDriver.SendContact(1, virtualCursors[1].Location.X, virtualCursors[1].Location.Y, HidContactState.Removing);
                            Console.WriteLine("Remove DC     " + virtualCursors[1].Location.X + ", " + virtualCursors[1].Location.Y);

                        }

                        cursorLastCLickEvent = DateTime.Now;
                    }
                    blobFirstMove = true;
                }
            }
        }

        /// <summary>
        /// Define the region of interest that user will use it to use his pen
        /// </summary>
        private void DefineRegion()
        {
            //check if user is selected or not            
            if (currentUser <= 0)
                return;

            //check if blob is detected or not
            if (blobDetected)
            {
                // this is the first move of the defining session
                // save the first point
                if (regionFirstMove)
                {
                    regionFirstMove = false;
                    // check if the selected user has a previous region or not
                    // if he has no region before, then add new one. Also add recatangleRegion
                    if (!regions.TryGetValue(currentUser, out currentRegion))
                    {
                        regions.Add(currentUser, new Region_());
                        rectangleRegions.Add(currentUser, new CvRect());
                    }
                    // save the first point
                    currentRegion.x1 = (int)blobs[1].Centroid.X;
                    currentRegion.y1 = (int)blobs[1].Centroid.Y;
                    regions[currentUser] = currentRegion;
                }
                // update last point
                else
                {
                    regionLastPoint.X = (int)blobs[1].Centroid.X;
                    regionLastPoint.Y = (int)blobs[1].Centroid.Y;
                }

            }
            else if (!blobDetected && !regionFirstMove)
            {
                // this is the end of the region-defining session
                // save the last point, calulate the region and draw it

                regionFirstMove = true;
                // save last point
                currentRegion = regions[currentUser];
                currentRegion.x3 = regionLastPoint.X;
                currentRegion.y3 = regionLastPoint.Y;
                // calculate the region (i.e calculate (x2,y2), (x4,y4))
                currentRegion.x2 = currentRegion.x3;
                currentRegion.y2 = currentRegion.y1;
                currentRegion.x4 = currentRegion.x1;
                currentRegion.y4 = currentRegion.y3;
                regions[currentUser] = currentRegion;
                // draw the region
                DrawRegion(currentUser);
            }
        }

        /// <summary>
        /// Used to paint/draw a region
        /// </summary>        
        private void DrawRegion(int regionNumber)
        {
            // height and width ratios between frame and pictureBoxRegions
            int hRatio = frame.Height / pictureBoxRegions.Height;
            int wRatio = frame.Width / pictureBoxRegions.Width;

            CvPoint[] pt = new CvPoint[4];
            pt[0].X = regions[regionNumber].x1 / wRatio;
            pt[1].X = regions[regionNumber].x2 / wRatio;
            pt[2].X = regions[regionNumber].x3 / wRatio;
            pt[3].X = regions[regionNumber].x4 / wRatio;

            pt[0].Y = regions[regionNumber].y1 / hRatio;
            pt[1].Y = regions[regionNumber].y2 / hRatio;
            pt[2].Y = regions[regionNumber].y3 / hRatio;
            pt[3].Y = regions[regionNumber].y4 / hRatio;

            // update x,y, height and width of drawing rectangle of region
            rectangleRegions[regionNumber] = new CvRect(pt[0].X, pt[0].Y, pt[1].X - pt[0].X, pt[2].Y - pt[1].Y);

            //pictureBoxRegions.ImageIpl.DrawPolyLine(new CvPoint[][] { pt }, true, CvColors.Orange, 1);
            //pictureBoxRegions.ImageIpl.DrawRect(pt[0], pt[2], CvColors.Blue, 1);

            //pictureBoxRegions.ImageIpl.DeleteMoire();
            pictureBoxRegions.ImageIpl.DrawRect(rectangleRegions[regionNumber], CvColors.Blue, 1);
            pictureBoxRegions.RefreshIplImage(pictureBoxRegions.ImageIpl);
        }

        /// <summary>
        /// Clear data of region for a certain user in order to define a new region.
        /// </summary>
        public void ClearRegion(int userNumber)
        {

        }

        /// <summary>
        /// Show or hide all the virual cursors according to the sent flag.
        /// </summary>       
        public void ShowHideCursors(bool show)
        {
            if (show)
                foreach (var item in virtualCursors)
                    item.Value.Show();
            else
                foreach (var item in virtualCursors)
                    item.Value.Hide();
        }
        #endregion

        #region Touch-screen concept and its calibration
        /// <summary>
        /// Implements the strategy of simulating cursor events according to the touch-screen concept.
        /// </summary>
        private void Trigger_TouchScreenConcept()
        {
            if (!calibrated)
                return;

            // check if a blob was detected, i.e. a cursor needs movement
            if (blobDetected)
            {
                // multiply coordinates of setected blob by the calibration factor               
                blobPosition.X = (int)(blobPosition.X * calibrationFactorX);
                blobPosition.Y = (int)(blobPosition.Y * calibrationFactorY);
                                
                Console.WriteLine("Position:    " + blobPosition.X + ", " + blobPosition.Y);            

                // check limitations of detected position
                if (blobPosition.X < 0 || blobPosition.X > screenWidth ||
                    blobPosition.Y < 0 || blobPosition.Y > screenHeight)
                {
                    // point is out of region of interest
                    Console.WriteLine("Out:    " + blobPosition.X + ", " + blobPosition.Y);
                    return;
                }

                if (!mouseDown)
                {
                    mouseDown = true;
                    // add new contact
                    touchDriver.SendContact(1, blobPosition.X, blobPosition.Y, HidContactState.Adding);
                    Console.WriteLine("New     " + blobPosition.X + ", " + blobPosition.Y);
                    // record the position of this point as the last point
                    blobLastX = blobPosition.X;
                    blobLastY = blobPosition.Y;
                }
                else if (blobPosition.X != blobLastX && blobPosition.Y != blobLastY)
                {
                    // move contact
                    //MoveTouchpoint(blobLastX, blobLastY, blobPosition.X, blobPosition.Y);
                    touchDriver.SendContact(1, blobPosition.X, blobPosition.Y, HidContactState.Updated);
                    Console.WriteLine("Move:   " + blobPosition.X + ", " + blobPosition.Y);
                    blobLastX = blobPosition.X;
                    blobLastY = blobPosition.Y;
                }
            }
            else if (mouseDown)
            {
                mouseDown = false;
                // remove contact
                touchDriver.SendContact(1, blobPosition.X, blobPosition.Y, HidContactState.Removing);
                Console.WriteLine("Remove: " + blobPosition.X + ", " + blobPosition.Y);
            }
        }

        /// <summary>
        /// Move the cursor form point 1 to point 2. This is to make the cursor movement
        /// more smoothly in the concept of TouchScreen
        /// </summary>
        private void MoveTouchpoint(int x1, int y1, int x2, int y2)
        {
            //touchDriver.SendContact(1, x2, y2, HidContactState.Updated);
            int dx = x2 - x1;
            int dy = y2 - y1;
            int dxAbs = Math.Abs(dx);
            int dyAbs = Math.Abs(dy);

            int xInc = 0, yInc = 0;
            int xIncAbs = 0, yIncAbs = 0;

            while (xIncAbs < dxAbs || yIncAbs < dyAbs)
            {
                xInc = xIncAbs < dxAbs ? (dx >= 0 ? xInc + 1 : xInc - 1) : xInc;
                yInc = yIncAbs < dyAbs ? (dy >= 0 ? yInc + 1 : yInc - 1) : yInc;
                xIncAbs = Math.Abs(xInc);
                yIncAbs = Math.Abs(yInc);

                touchDriver.SendContact(1, x1 + xInc, y1 + yInc, HidContactState.Updated);
                Console.WriteLine("Move:   " + x1 + xInc + ", " + y1 + yInc);
            }
        }

        /// <summary>
        /// This is to calculate the calibration ROI and matrix using the currently saved calibration Region.
        /// </summary>
        private void CalculateCalibrationParameters()
        {
            // calculate the roi to include the maximimum points is one rectangle
            // this is happened when there is a small slope in the calibratedRegion
            int x, y, width, height;
            x = Math.Min(calibrationRegion.x1, calibrationRegion.x4);
            width = Math.Max(calibrationRegion.x2, calibrationRegion.x3) - x;
            y = Math.Min(calibrationRegion.y1, calibrationRegion.y2);
            height = Math.Max(calibrationRegion.y3, calibrationRegion.y4) - y;

            // set regions of interest
            calibrationROI = new CvRect(x, y, width, height);
            calibrationROIResized = new CvRect(
                  calibrationROI.X * resizeVal, calibrationROI.Y * resizeVal
                , calibrationROI.Width * resizeVal, calibrationROI.Height * resizeVal);
                        
            // calculate the calibration x,y factors
            calibrationFactorX = screenWidth / ((double)(width * resizeVal));
            calibrationFactorY = screenHeight / ((double)(height * resizeVal));

            // resize images
            CvSize size = new CvSize(1280, 960);
            resizedFrame = new IplImage(size, BitDepth.U8, 1);
            labelFrame = new IplImage(size, BitDepth.F32, 1);           

            // set calibration ROI for the images
            frame.SetROI(calibrationROI);            
            grayFrame.SetROI(calibrationROI);
            transformFrame.SetROI(calibrationROI);
            resizedFrame.SetROI(calibrationROIResized);
            labelFrame.SetROI(calibrationROIResized);

            // shifting the calibration region and shifting the ROI
            calibrationROI = new CvRect(0, 0, width, height);
            Region_ reg = new Region_();
            reg.x1 = calibrationRegion.x1 - x;
            reg.x2 = calibrationRegion.x2 - x;
            reg.x3 = calibrationRegion.x3 - x;
            reg.x4 = calibrationRegion.x4 - x;
            reg.y1 = calibrationRegion.y1 - y;
            reg.y2 = calibrationRegion.y2 - y;
            reg.y3 = calibrationRegion.y3 - y;
            reg.y4 = calibrationRegion.y4 - y;

            // now, the final point of calibration process is perspective transform
            CvPoint2D32f[] src = new CvPoint2D32f[4];
            CvPoint2D32f[] dst = new CvPoint2D32f[4];
            src[0] = new CvPoint2D32f(reg.x1, reg.y1);
            src[1] = new CvPoint2D32f(reg.x2, reg.y2);
            src[2] = new CvPoint2D32f(reg.x3, reg.y3);
            src[3] = new CvPoint2D32f(reg.x4, reg.y4);

            dst[0] = new CvPoint2D32f(0, 0);
            dst[1] = new CvPoint2D32f(width, 0);
            dst[2] = new CvPoint2D32f(width, height);
            dst[3] = new CvPoint2D32f(0, height);
            calibrationMatrix = Cv.GetPerspectiveTransform(src, dst);

            // this is a polyline used to draw the original calibrationRegion on the frame
            polyline = new CvPoint[][] { new CvPoint[] {
                       new CvPoint(reg.x1*resizeVal, reg.y1*resizeVal),
                       new CvPoint(reg.x2*resizeVal, reg.y2*resizeVal),
                       new CvPoint(reg.x3*resizeVal, reg.y3*resizeVal),
                       new CvPoint(reg.x4*resizeVal, reg.y4*resizeVal),
                       new CvPoint(reg.x1*resizeVal, reg.y1*resizeVal) }};
        }

        /// <summary>
        /// Prepare and calibrate the screen for the touch-screen concept.
        /// </summary>
        private void Calibrate()
        {
            // this condition is to stop ging deeper in this method if the calibration process
            // has finished. When the user leaves the calibration tab and returns to it again,
            // calibrated is set to false to start new calibration process.
            if (calibrated)
                return;

            calibrationCounter++;
            // 1. Move Picture Box at the corners of the calibration region
            if (calibrationCounter > 100)
            {
                calibrationCounter = 1;
                calibrationPoint = (calibrationPoint == 4) ? 1 : ++calibrationPoint;

                // size of container of calibration pictureBox: 255, 389
                switch (calibrationPoint)
                {
                    case 1:
                        calibrationImg.Location = new Point(0, 0);
                        break;
                    case 2:
                        calibrationImg.Location = new Point(207, 0);
                        break;
                    case 3:
                        calibrationImg.Location = new Point(207, 341);
                        break;
                    case 4:
                        calibrationImg.Location = new Point(0, 341);
                        break;
                    default:
                        break;
                }

                // speak the number of the calibration point to improve the user experience
                Thread speechThread = new Thread(() =>
                {
                    SpeechSynthesizer speechSynth = new SpeechSynthesizer();
                    speechSynth.Speak(calibrationPoint.ToString());
                    speechSynth.Dispose();
                });
                speechThread.Start();
            }

            // 2. register the corner points
            if (blobDetected)
            {
                switch (calibrationPoint)
                {
                    case 1:
                        calibrationRegion.x1 = blobPosition.X;
                        calibrationRegion.y1 = blobPosition.Y;
                        break;
                    case 2:
                        calibrationRegion.x2 = blobPosition.X;
                        calibrationRegion.y2 = blobPosition.Y;
                        break;
                    case 3:
                        calibrationRegion.x3 = blobPosition.X;
                        calibrationRegion.y3 = blobPosition.Y;
                        break;
                    case 4:
                        calibrationRegion.x4 = blobPosition.X;
                        calibrationRegion.y4 = blobPosition.Y;
                        break;
                    default:
                        break;
                }

                // calibration points collected, now calculate the ROI
                if (calibrationRegion.x1 != 0 && calibrationRegion.x2 != 0
                    && calibrationRegion.x3 != 0 && calibrationRegion.x4 != 0)
                {
                    CalculateCalibrationParameters();
                    calibrated = true;
                    System.Media.SystemSounds.Asterisk.Play();

                    //else if (calibrated == false)
                    //{
                    //    calibrated = true;
                    //    System.Media.SystemSounds.Exclamation.Play();
                    //    //calibrationFactorX = screenWidth / (calibrationRegion.x2 - calibrationRegion.x1);
                    //    //calibrationFactorY = screenHeight / (calibrationRegion.y3 - calibrationRegion.y2);
                    //    ////MessageBox.Show("Slopes:\n" + (calibrationRegion.x1 - calibrationRegion.x4) +
                    //    ////    ", " + (calibrationRegion.x2 - calibrationRegion.x3) +
                    //    ////    "\n" + (calibrationRegion.y1 - calibrationRegion.y2) +
                    //    ////    ", " + (calibrationRegion.y4 - calibrationRegion.y3));
                    //}

                }
            }
        }

        /// <summary>
        /// Restart calibration Process.
        /// </summary>
        public void RestartCalibrate()
        {
            calibrated = false;
            calibrationCounter = 100;
            calibrationPoint = 0;
            calibrationImg.Location = new Point(0, 0);
            calibrationRegion = new Region_();
            frame.ResetROI();
            grayFrame.ResetROI();
            transformFrame.ResetROI();
            labelFrame.ResetROI();
        }
        #endregion

        #region Trials

        ///// <summary>
        ///// move cursor with those displacements dx and dy
        ///// </summary>        
        //private void MoveCursor(int dx, int dy)
        //{
        //    while (dx != 0 || dy != 0)
        //    {
        //        if (dx != 0)
        //            mouseEvent.CursorPosition(Cursor.Position.X + dx, Cursor.Position.Y);
        //        if (dy != 0)
        //            mouseEvent.CursorPosition(Cursor.Position.X, Cursor.Position.Y + dy);

        //        if (dx > 0)
        //            dx--;
        //        else if (dx < 0)
        //            dx++;

        //        if (dy > 0)
        //            dy--;
        //        else if (dy < 0)
        //            dy++;

        //        Application.DoEvents();
        //    }
        //}

        #endregion
    }
}