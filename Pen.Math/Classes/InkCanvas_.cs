using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace Pen.Math
{
    /// <summary>
    /// Ordinary Ink Canvas but with rulers drawn on it
    /// </summary>
    public class InkCanvas_ : InkCanvas
    {
        protected override void OnRender(System.Windows.Media.DrawingContext dc)
        {
            base.OnRender(dc);
            
            int spacing = 10;
            byte alpha1 = 160;
            byte alpha2 = 170;
            byte gray = 190;
            //FromArgb(255, 250, 220, 140))

            double thickness1 = 0.1; 
            double thickness2 = 0.1;
            SolidColorBrush color1 = new SolidColorBrush(Color.FromArgb(alpha1, 255, 70, 0));
            SolidColorBrush color2 = new SolidColorBrush(Color.FromArgb(alpha2, gray, gray, gray));
            System.Windows.Media.Pen thickPen = new System.Windows.Media.Pen(color1, thickness1);
            System.Windows.Media.Pen finePen = new System.Windows.Media.Pen(color2, thickness2);
           

            // draw vertical lines
            for (int x = spacing; x < this.ActualWidth; x += spacing)
                if (x % (spacing * 10) == 0)
                    dc.DrawLine(thickPen, new Point(x, 0), new Point(x, this.ActualHeight));
                else
                    dc.DrawLine(finePen, new Point(x, 0), new Point(x, this.ActualHeight));

            // draw horizonal lines
            for (int y = spacing; y < this.ActualHeight; y += spacing)
                if (y % (spacing * 10) == 0)
                    dc.DrawLine(thickPen, new Point(0, y), new Point(this.ActualWidth, y));
                else
                    dc.DrawLine(finePen, new Point(0, y), new Point(this.ActualWidth, y));
        }
    }
}
