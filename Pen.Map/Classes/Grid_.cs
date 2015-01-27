using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace Pen.Map
{
    /// <summary>
    /// Ordinary Ink Canvas but with rulers drawn on it
    /// </summary>
    public class Grid_ : Grid
    {
        protected override void OnRender(System.Windows.Media.DrawingContext dc)
        {
            base.OnRender(dc);
            int spacing = 31;
            double thickness = 0.1;

            System.Windows.Media.Pen pen = new System.Windows.Media.Pen(new SolidColorBrush(Colors.Gray), thickness);

            //for (int x = spacing; x < this.ActualWidth; x += spacing)
            //    dc.DrawLine(pen, new Point(x, 0), new Point(x, this.ActualHeight));

            for (int y = 21; y < this.ActualHeight; y += spacing)
                dc.DrawLine(pen, new Point(0, y), new Point(this.ActualWidth, y));
        }
    }
}
