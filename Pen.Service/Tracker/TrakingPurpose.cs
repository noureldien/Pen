using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pen.Service
{
    /// <summary>
    /// Indicates the purpose of traking the blobs (pen) whether it is for region detection
    /// / calibration / cursor simluation (with its two concepts: touchpad and touch-screen)
    /// </summary>
    enum TrackingPurpose
    {
        /// <summary>
        /// The detected blobs act as cursors with the touch-pad concept.
        /// </summary>
        CursorSimulation_TouchpadConcept,
        /// <summary>
        /// The detected blob is used to determine region of each user. This is used in
        /// touch-pad concept.
        /// </summary>
        RegionDetection,
        /// <summary>
        /// The detected blob acts as a cursor with the touch-screen concept.
        /// </summary>            
        CursorSimulation_TouchScreenConcept,
        /// <summary>
        /// The detected blob is used to calibrate the screen for the touch-screen concept.
        /// </summary>
        Calibration,
        /// <summary>
        /// Used when modifying parameters of the camera and filters.
        /// No need to do things with the detected bolb.
        /// </summary>
        None
    }
}
