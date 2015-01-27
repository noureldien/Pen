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
using System.Text;

namespace Pen.Service
{
    /// <summary>
    /// Defines the region of interest for a user. This is done using 4 defining points.
    /// </summary>
    public struct Region_
    {
        /// <summary>
        /// X value a point of the 4 defining points of the region.
        /// </summary> 
        public int x1, x2, x3, x4;

        /// <summary>
        /// Y value a point of the 4 defining points of the region.
        /// </summary> 
        public int y1, y2, y3, y4;

        ///// <summary>
        ///// Defines the last defined point of the 3 defining points of this region.
        ///// </summary>
        //public int lastDefinedPoint;

        ///// <summary>
        ///// Is true when the region is defined, i.e 4 points are defined.
        ///// </summary>
        //public bool finished;
    }   
}