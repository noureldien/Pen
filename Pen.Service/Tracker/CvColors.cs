using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenCvSharp;

namespace Pen.Service
{
    struct CvColors
    {
        static public CvScalar Red = new CvScalar(35, 30, 240);
        static public CvScalar Green = new CvScalar(75, 180,35);
        static public CvScalar Blue = new CvScalar(230, 160, 0);
        static public CvScalar White = new CvScalar(255, 255, 255);
        static public CvScalar Orange = new CvScalar(0, 120, 255);
        static public CvScalar Yellow = new CvScalar(0, 240, 255);
    }
}
