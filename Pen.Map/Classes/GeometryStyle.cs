using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MapPoint.Rendering3D.Utility;

namespace Pen.Map
{
    /// <summary>
    /// Provides styles for GeometryShapes of the map (pushpin, label, polygon and polyline)
    /// </summary>
    class GeometryStyle
    {
        static string blue = "http://i528.photobucket.com/albums/dd329/onemanengine/pin_1.png";
        static string orange = "http://i528.photobucket.com/albums/dd329/onemanengine/pin_2.png";
        static string yellow = "http://i528.photobucket.com/albums/dd329/onemanengine/pin_3.png";
        static string red = "http://i528.photobucket.com/albums/dd329/onemanengine/pin_4.png";
        static string green = "http://i528.photobucket.com/albums/dd329/onemanengine/pin_5.png";
        static string empty = "http://i528.photobucket.com/albums/dd329/onemanengine/empty.png";

        /// <summary>
        /// takes the label of the pushpin Geometry and get the style of a pushpin
        /// </summary>        
        public static PushpinInfo GetPushPinStyle(string label)
        {
            PushpinInfo style = PushpinInfo.Default;
            style.AltitudeMode = AltitudeMode.FromGround;
            style.Label = label;
            style.Resource = orange;
            style.HighlightResource = blue;
            style.FontSize = 16;
            style.FontStyle = System.Drawing.FontStyle.Bold;
            style.FadeInAndOut = true;
            style.LabelOffsetPercent.Y = 40;
            style.FontName = "Sakkal Majalla";
            style.HitDetect = HitDetectMode.HighlightOnPointer;
            style.OrientationMode = PushpinOrientationMode.UseProvided;
            return style;
        }

        /// <summary>
        /// takes the text/label of the labelGeometry and get the style of a labelGeometry
        /// </summary>           
        public static LabelInfo GetLabelStyle(string labelText)
        {
            LabelInfo style = LabelInfo.Default;
            style.AltitudeMode = AltitudeMode.FromGround;
            style.FontSize = 16;
            style.FontStyle = System.Drawing.FontStyle.Bold;
            style.HorizontalAlignment = System.Drawing.StringAlignment.Near;
            style.FadeInAndOut = true;
            style.GlowColor = System.Drawing.Color.OrangeRed;
            style.GlowSize = 1;
            style.FontName = "Sakkal Majalla";
            style.HitDetect = HitDetectMode.HighlightOnPointer;
            style.Label = labelText;
            return style;
        }

        /// <summary>
        /// takes the color that a polygonGeometry is filling with 
        /// and returns the style of a polygonGeometry    
        /// </summary>        
        public static PolyInfo GetPolygonStyle(System.Drawing.Color fillColor)
        {
            PolyInfo style = PolyInfo.DefaultPolygon;
            style.AltitudeMode = AltitudeMode.FromGround;
            style.HitDetect = HitDetectMode.None;
            // colors
            style.Filled = true;
            style.FillColor = fillColor;
            style.FillHighlightColor = System.Drawing.Color.FromArgb(130, 0, 150, 255);
            // line style
            style.CapStyle = CapStyle.Round;
            style.JoinStyle = JoinStyle.Bevel;
            style.DashStyle = Microsoft.MapPoint.Rendering3D.Utility.DashStyle.Solid;
            style.LineColor = System.Drawing.Color.White;
            style.LineHighlightColor = System.Drawing.Color.White;
            style.LineStyle = LineStyle.Single;
            style.LineWidthUnits = LineWidthUnits.Pixels;
            style.LineWidth = 2;
            style.Outlined = true;

            return style;
        }
        
        /// <summary>
        /// takes the color that a polylineGeometry is filling with 
        /// and returns the style of a polylineGeometry    
        /// </summary>        
        public static PolyInfo GetPolylineStyle(System.Drawing.Color fillColor)
        {
            PolyInfo style = PolyInfo.DefaultPolyline;
            style.AltitudeMode = AltitudeMode.FromGround;
            style.HitDetect = HitDetectMode.None;
            // line style
            style.CapStyle = CapStyle.None;
            style.JoinStyle = JoinStyle.Bevel;
            style.DashStyle = Microsoft.MapPoint.Rendering3D.Utility.DashStyle.Solid;
            style.LineColor = fillColor;
            style.LineHighlightColor = System.Drawing.Color.OrangeRed;
            style.LineStyle = LineStyle.Single;
            style.LineWidthUnits = LineWidthUnits.Pixels;
            style.LineWidth = 7;
            style.Outlined = true;

            return style;
        }
    }
}
