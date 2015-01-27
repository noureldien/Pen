using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace Pen.Language
{
    /// <summary>
    /// Ordinary Grid but with rulers drawn on it
    /// </summary>
    public class Grid_ : Grid
    {
        protected override void OnRender(System.Windows.Media.DrawingContext dc)
        {
            base.OnRender(dc);
            int spacing = 40;
            double thickness = 0.1;
            Color color = Colors.ForestGreen;

            System.Windows.Media.Pen pen = new System.Windows.Media.Pen(new SolidColorBrush(color), thickness);

            for (int y = 40; y < this.ActualHeight; y += spacing)
                dc.DrawLine(pen, new Point(0, y), new Point(this.ActualWidth, y));
        }
    }
}
