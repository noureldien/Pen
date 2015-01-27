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
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Blob;
using VideoInputSharp;
using Pen.Service.Properties;

namespace Pen.Service
{
    /// <summary>
    /// Uses the camera and the touch driver to simulate a touch device.
    /// </summary>
    public class TouchTrackerImproved
    {
        // screen dimensions
        private readonly int screenHeight = SystemInformation.VirtualScreen.Height;
        private readonly int screenWidth = SystemInformation.VirtualScreen.Width;        
        /// <summary>
        /// Number of the connected camera
        /// </summary>
        private readonly int deviceID = 0;
        /// <summary>
        /// Sending touch messages to windows using virtual touch driver software.
        /// </summary>
        private MultitouchDriver touchDriver = new MultitouchDriver();

        #region Timer and its parameters
        /// <summary>
        /// Do image rpocesing on the new frame comming form the camera, every small amount of time (ms).
        /// </summary>
        private System.Windows.Forms.Timer mainTimer;
        /// <summary>
        /// Timer interval time
        /// </summary>
        public int timerIntervalTime = 30;
        /// <summary>
        /// Flags the time state, whether runnig or stopped.
        /// </summary>
        private bool timerInProgress = false;
        #endregion       

        #region Image Processing parameters
        /// <summary>
        /// Maximum diagonal length of detected blob.
        /// </summary>
        private int blobCounterMaxSize = 45;
        /// <summary>
        /// Minimum diagonal length of detected blob.
        /// </summary>
        private int blobCounterMinSize = 10;
        /// <summary>
        /// Gray level value. 255 is white, 0 is black.
        /// </summary>
        public int grayLowValue = 100;
        /// <summary>
        /// Mask value of smooth gaussian filter.
        /// </summary>
        public int smoothGaussianValue = 5;
        /// <summary>
        /// Horizontally invert the captured frame from the camera.
        /// </summary>
        private bool invertHorizontal = false;
        /// <summary>
        /// Vertically invert the captured frame from the camera.
        /// </summary>
        private bool invertVertical = false;       
        #endregion

        #region Video stream capturing using OpenCvSharp
        private IplImage frame;        
        private IplImage grayFrame;
        private IplImage transformFrame;
        private IplImage labelFrame;
        private IplImage resizedFrame;
        private VideoInput vi;       
        private CvBlobs blobs = new CvBlobs();
        #endregion

        #region Mouse simulation using Touch-screen concept
        /// <summary>
        /// Indicates if blob is detected or not, i.e cursor needs movement or not.
        /// </summary>
        private bool blobDetected = false;
        /// <summary>
        /// Flags thw begining of one movement session.
        /// </summary>
        private bool blobFirstMove = true;
        /// <summary>
        /// Indicates whether mouse down event is on|off, this flag is used in performing drag-drop action.
        /// </summary>
        private bool mouseDown = false;
        /// <summary>
        /// X-axis of last detected blob (cursorPosition)
        /// </summary>
        private int cursorLastX = 0;
        /// <summary>
        /// Y-axis of last detected blob (cursorPosition)
        /// </summary>
        private int cursorLastY = 0;
        /// <summary>
        /// The point representing cursor position.
        /// </summary>
        private Point blobPosition = new Point();
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
        /// Indicates the region of calibration.
        /// </summary>
        public Region_ calibrationRegion;
        #endregion

        //constructor
        public TouchTrackerImproved()
        {            
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
            mainTimer.Tick += new EventHandler(mainTimer_Tick);
            
            // Intialize camera
            try
            {
                vi = new VideoInput();                
            }
            catch (Exception exception)
            {
                Console.WriteLine("Failed to initialize the camera, the program will be closed." +
                    "\n\nThis is the internal error:\n" + exception.Message, "Notify", MessageBoxButtons.OK, MessageBoxIcon.Information);
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }

            vi.SetupDevice(deviceID, 640, 480);
            vi.SetIdealFramerate(deviceID, 30);

            CvSize size = new CvSize(640, 480);            
            frame = new IplImage(size, BitDepth.U8, 3);            
            grayFrame = new IplImage(size, BitDepth.U8, 1);
            transformFrame = new IplImage(size, BitDepth.U8, 1);         
            labelFrame = new IplImage(1280,960, BitDepth.F32, 1);
            resizedFrame = new IplImage(1280,960, BitDepth.U8, 1);
            
            // start touch driver
            touchDriver.Start();
        }

        /// <summary>
        /// timer tick event handler
        /// </summary>        
        void mainTimer_Tick(object sender, EventArgs e)
        {
            Track();
            Trigger_TouchScreenConcept();
        }

        /// <summary>
        /// Start mainThread, that starts tracking
        /// </summary>
        public void StartProcessing()
        {
            mainTimer.Start();            
            timerInProgress = true;
        }

        /// <summary>
        /// Stop mainThread, that stops tracking
        /// </summary>
        public void StopProcessing()
        {
            mainTimer.Stop();            
            timerInProgress = false;
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

            if (vi != null)
            {
                vi.StopDevice(deviceID);
                vi.Dispose();
            }

            if (touchDriver != null)
            {
                touchDriver.Dispose();                
            }
        }

        /// <summary>
        /// Image Processing (blobs tracking). It is done using OpenCVSharp Library.
        /// </summary>
        public void Track()
        {
            vi.GetPixels(deviceID, frame.ImageData, false, true);
            Cv.CvtColor(frame, grayFrame, ColorConversion.BgrToGray);
            //Cv.Threshold(grayFrame, grayFrame, grayLowValue, 255, ThresholdType.Binary);           
            Cv.WarpPerspective(grayFrame, transformFrame, calibrationMatrix, Interpolation.Linear | Interpolation.FillOutliers, CvScalar.ScalarAll(100));
            Cv.Threshold(transformFrame, transformFrame, grayLowValue, 255, ThresholdType.Binary);
            Cv.Resize(transformFrame, resizedFrame);
            CvBlobLib.Label(resizedFrame, labelFrame, blobs);
            
            // get the position of the detected blob (if exists)
            if (blobs.Count > 0)
            {
                // indicates that a blob was detected
                blobDetected = true;
                // get position of detected blob                
                blobPosition.X = (int)blobs[1].Centroid.X;
                blobPosition.Y = (int)blobs[1].Centroid.Y;
            }
            else
            {
                blobDetected = false;
            }           
        }

        /// <summary>
        /// Implements the strategy of simulating cursor events according to the touch-screen concept.
        /// </summary>
        public void Trigger_TouchScreenConcept()
        {
            // check if a blob was detected, i.e. a cursor needs movement
            if (blobDetected)
            {
                // multiply coordinates of setected blob by the calibration factor               
                blobPosition.X = (int)(blobPosition.X * calibrationFactorX);
                blobPosition.Y = (int)(blobPosition.Y * calibrationFactorY);
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
                }
                else if (blobPosition.X != cursorLastX && blobPosition.Y != cursorLastY)
                {
                    // move contact
                    touchDriver.SendContact(1, blobPosition.X, blobPosition.Y, HidContactState.Updated);
                    Console.WriteLine("Move:   " + blobPosition.X + ", " + blobPosition.Y);
                    cursorLastX = blobPosition.X;
                    cursorLastY = blobPosition.Y;
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
        /// This is to calculate the calibration ROI and matrix using calibration Region.
        /// This is done  before start tracking, i.e only one time at the start of the program.
        /// </summary>
        public void CalculateCalibrationParameters()
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
                  calibrationROI.X * 2, calibrationROI.Y * 2
                , calibrationROI.Width * 2, calibrationROI.Height * 2);
            // calculate the calibration x,y factors
            calibrationFactorX = screenWidth / ((double)(width * 2));
            calibrationFactorY = screenHeight / ((double)(height * 2));

            // set calibration ROI for the images
            frame.SetROI(calibrationROI);
            grayFrame.SetROI(calibrationROI);            
            transformFrame.SetROI(calibrationROI);
            labelFrame.SetROI(calibrationROIResized);
            resizedFrame.SetROI(calibrationROIResized);

            // shifting the calibration region and shifting the ROI
            calibrationROI = new CvRect(0, 0, width, height);
            calibrationRegion.x1 -= x;
            calibrationRegion.x2 -= x;
            calibrationRegion.x3 -= x;
            calibrationRegion.x4 -= x;
            calibrationRegion.y1 -= y;
            calibrationRegion.y2 -= y;
            calibrationRegion.y3 -= y;
            calibrationRegion.y4 -= y;            

            // now, the final point of calibration process is perspective transform
            CvPoint2D32f[] src = new CvPoint2D32f[4];
            CvPoint2D32f[] dst = new CvPoint2D32f[4];
            src[0] = new CvPoint2D32f(calibrationRegion.x1, calibrationRegion.y1);
            src[1] = new CvPoint2D32f(calibrationRegion.x2, calibrationRegion.y2);
            src[2] = new CvPoint2D32f(calibrationRegion.x3, calibrationRegion.y3);
            src[3] = new CvPoint2D32f(calibrationRegion.x4, calibrationRegion.y4);

            dst[0] = new CvPoint2D32f(0, 0);
            dst[1] = new CvPoint2D32f(width, 0);
            dst[2] = new CvPoint2D32f(width, height);
            dst[3] = new CvPoint2D32f(0, height);
            calibrationMatrix = Cv.GetPerspectiveTransform(src, dst);
        }

        /// <summary>
        /// Retreive some setting values from setting.setting.
        /// </summary>
        private void GetSettings()
        {
            //this.timerIntervalTime = Settings.Default.timerIntervalTime;
            //this.blobCounterMaxSize = Settings.Default.blobCounterMaxSize;
            //this.blobCounterMinSize = Settings.Default.blobCounterMinSize;
            //this.grayLowValue = Settings.Default.grayLowValue; 
            //this.smoothGaussianValue = Settings.Default.smoothGaussianValue;            
            //this.invertHorizontal = Settings.Default.invertHorizontal;
            //this.invertVertical = Settings.Default.invertVertical;

            //this.calibrationRegion.x1 = Settings.Default.x1;
            //this.calibrationRegion.x2 = Settings.Default.x2;
            //this.calibrationRegion.x3 = Settings.Default.x3;
            //this.calibrationRegion.x4 = Settings.Default.x4;

            //this.calibrationRegion.y1 = Settings.Default.y1;
            //this.calibrationRegion.y2 = Settings.Default.y2;
            //this.calibrationRegion.y3 = Settings.Default.y3;
            //this.calibrationRegion.y4 = Settings.Default.y4;          
        }

        #region Trial Code
        ///// <summary>
        ///// Save the send text to the application settings class.
        ///// </summary>
        //public static void SaveSettings(int timerIntervalTime_, int grayLowValue_, int smoothGaussianValue_, 
        //    int blobCounterMaxSize_, int blobCounterMinSize_, int cursorMoveFactor_, int zoomValue_,
        //    int x1_, int x2_, int x3_, int x4_, int y1_, int y2_, int y3_, int y4_)
        //{
        //    Settings.Default.timerIntervalTime = timerIntervalTime_;
        //    Settings.Default.grayLowValue = grayLowValue_;
        //    Settings.Default.smoothGaussianValue = smoothGaussianValue_;
        //    Settings.Default.blobCounterMaxSize = blobCounterMaxSize_;
        //    Settings.Default.blobCounterMinSize = blobCounterMinSize_;
        //    Settings.Default.cursorMoveFactor = cursorMoveFactor_;
        //    Settings.Default.zoomValue = zoomValue_;

        //    //Properties.Settings.Default.cursorSimulationConcept = radioButton1.Checked;
        //    //Properties.Settings.Default.invertHorizontal = tracking.invertHorizontal;
        //    //Properties.Settings.Default.invertVertical = tracking.invertVertical;
        //    //Properties.Settings.Default.playSound = tracking.playSound;

        //    Settings.Default.x1 = x1_;
        //    Settings.Default.x2 = x2_;
        //    Settings.Default.x3 = x3_;
        //    Settings.Default.x4 = x4_;

        //    Settings.Default.y1 = y1_;
        //    Settings.Default.y2 = y2_;
        //    Settings.Default.y3 = y3_;
        //    Settings.Default.y4 = y4_;
        //    Settings.Default.Save();
        //}
        #endregion
    }
}