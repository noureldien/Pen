using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Blob;
using System.Media;

namespace Pen.Service
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // create imagesand window
            CvSize size = new CvSize(400, 400);
            CvBlobs blobs = new CvBlobs();
            IplImage frame = new IplImage(size, BitDepth.U8, 1);
            IplImage labelFrame = new IplImage(size, BitDepth.F32, 1);
            IplImage blobsFrame = new IplImage(size, BitDepth.U8, 3);            
            CvWindow window = new CvWindow("Transform", WindowMode.KeepRatio);
            window.Resize(400, 400);

            // set the main frame and the black dot
            frame = new IplImage(Application.StartupPath + "\\img.png", LoadMode.GrayScale);
            blobsFrame = new IplImage(Application.StartupPath + "\\img.png", LoadMode.Color);
           
            CvColor black = new CvColor(0, 0, 0);
            CvColor white = new CvColor(255, 255, 255);


            for (int y = 0; y < 400; y++)
            {
                for (int x = 0; x < 400; x++)
                {                    
                    // set black dot in the main frame frame[y,x]
                    frame[y, x] = white;
                    // blob detection and rendering blobs
                    CvBlobLib.Label(frame, labelFrame, blobs);
                    //CvBlobLib.RenderBlobs(labelFrame, blobs, blobsFrame, blobsFrame, RenderBlobsMode.BoundingBox);
                    // show image on the window
                    //window.Image = blobsFrame;
                    
                    //CvBlob blob = blobs[blobs.GreaterBlob()];
                    //label1.Text = "(" + blob.Centroid.X + ", " + blob.Centroid.Y + ")";
                    //Application.DoEvents();
                    frame[y, x] = black;
                }

                frame[y, 399] = black;
                label1.Text = y.ToString();
                Application.DoEvents();
            }

            SystemSounds.Asterisk.Play();

            //CvBlob blob = blobs[blobs.GreaterBlob()];
            //label1.Text = "(" + blob.Centroid.X + ", " + blob.Centroid.Y + ")";
            
        }

        private void button2_Click(object sender, EventArgs e)
        {            
            DateTime t1 = DateTime.Now;
            Point p = PrespectiveTransform(10, 10, 390, 5, 380, 390, 0, 400, (int)numericUpDown1.Value, (int)numericUpDown2.Value,400,400);
            label1.Text = "(" + p.X + ", " + p.Y + ")";
            label2.Text = (DateTime.Now - t1).TotalMilliseconds.ToString();
        }

        #region Calculate Prespective Transform of a point
        /// <summary>
        /// Calculate the prespective transform (projection) of the given point (x,y)
        /// over the deformed rectangle (transformed rectangle) with corners (x1, ...., y4) (clockwise).
        /// Also takes width and height of the regular rectangle. Returns the point
        /// after being transformed to the deformed (transformed rectangle).
        /// </summary>        
        private Point PrespectiveTransform(int x1, int y1, int x2, int y2,
                                           int x3, int y3, int x4, int y4, int x, int y, int w, int h)
        {
            double a1, b1, a2, b2, a3, b3, a4, b4;

            a1 = ((h - y) * x1 + y * x4) / h;
            a2 = ((h - y) * x2 + y * x3) / h;
            a3 = ((w - x) * x1 + x * x2) / w;
            a4 = ((w - x) * x4 + x * x3) / w;

            b1 = ((h - y) * y1 + y * y4) / h;
            b2 = ((h - y) * y2 + y * y3) / h;
            b3 = ((w - x) * y1 + x * y2) / w;
            b4 = ((w - x) * y4 + x * y3) / w;

            double d = Det(a1 - a2, b1 - b2, a3 - a4, b3 - b4);
            d = (d == 0) ? 1 : d;

            // transformed point
            int xT = (int)(Det(Det(a1, b1, a2, b2), a1 - a2, Det(a3, b3, a4, b4), a3 - a4) / d);
            int yT = (int)(Det(Det(a1, b1, a2, b2), b1 - b2, Det(a3, b3, a4, b4), b3 - b4) / d);

            return new Point(xT, yT);
        }
        
        /// <summary>
        /// Calculate determinant of 2x2 matrix.
        /// </summary>        
        private double Det(double a, double b, double c, double d)
        {
            double result = a * d - b * c;
            return result;
        }
        #endregion

        private void OldCode()
        {
            // create images
            CvSize size = new CvSize(400, 400);
            CvBlobs blobs = new CvBlobs();
            IplImage frame = new IplImage(size, BitDepth.U8, 1);
            IplImage transformFrame = new IplImage(size, BitDepth.U8, 1);
            IplImage labelFrame = new IplImage(size, BitDepth.F32, 1);
            IplImage blobsFrame = new IplImage(size, BitDepth.U8, 3);

            // create window
            CvWindow window = new CvWindow("Transform", WindowMode.KeepRatio);
            window.Resize(400, 400);

            // set the main frame and the black dot
            frame = new IplImage(Application.StartupPath + "\\img.png", LoadMode.GrayScale);

            // matrix of the prespective transform
            CvPoint2D32f[] src = new CvPoint2D32f[4];
            CvPoint2D32f[] dst = new CvPoint2D32f[4];

            src[0] = new CvPoint2D32f(0, 0);
            src[1] = new CvPoint2D32f(frame.Width, 0);
            src[2] = new CvPoint2D32f(frame.Width, frame.Height);
            src[3] = new CvPoint2D32f(0, frame.Height);

            dst[0] = new CvPoint2D32f(10, 10);
            dst[1] = new CvPoint2D32f(390, 5);
            dst[2] = new CvPoint2D32f(380, 390);
            dst[3] = new CvPoint2D32f(0, 400);

            CvMat mapMatrix = Cv.GetPerspectiveTransform(src, dst);

            CvColor black = new CvColor(0, 0, 0);
            CvColor white = new CvColor(255, 255, 255);

            // set black dot in the main frame frame[y,x]
            frame[(int)numericUpDown2.Value, (int)numericUpDown1.Value] = white;

            DateTime t1 = DateTime.Now;
            Cv.WarpPerspective(frame, transformFrame, mapMatrix, Interpolation.Linear | Interpolation.FillOutliers, CvScalar.ScalarAll(100));
            label2.Text = (DateTime.Now - t1).TotalMilliseconds.ToString();
            //CvInvoke.cvWarpPerspective(frame.CvPtr, transformFrame.CvPtr, mapMatrix.CvPtr, Interpolation.Linear | Interpolation.FillOutliers, CvScalar.ScalarAll(100));

            // blob detection and rendering blobs
            CvBlobLib.Label(transformFrame, labelFrame, blobs);
            blobs.FilterByArea(1, 10);
            blobs.RenderBlobs(labelFrame, blobsFrame, blobsFrame, RenderBlobsMode.Centroid);
            // show image on the windows
            window.Image = blobsFrame;

            CvBlob blob = blobs[blobs.GreaterBlob()];
            label1.Text = "(" + blob.Centroid.X + ", " + blob.Centroid.Y + ")";
        }
    }
}
