
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



#if MIGRATION
namespace System.Windows.Controls.Primitives
#else
namespace Windows.UI.Xaml.Controls.Primitives
#endif
{
    /// <summary>
    /// Specifies the type of Scroll event that occurred.
    /// </summary>
    public enum ScrollEventType
    {
        /// <summary>
        /// Specifies that the Thumb moved a specified distance, as determined by the
        /// value of SmallChange. The Thumb moved to the left for a horizontal ScrollBar
        /// or upward for a vertical ScrollBar.
        /// </summary>
        SmallDecrement = 0,

        /// <summary>
        /// Specifies that the Thumb moved a specified distance, as determined by the
        /// value of SmallChange. The Thumb moved to the right for a horizontal ScrollBar
        /// or downward for a vertical ScrollBar.
        /// </summary>
        SmallIncrement = 1,

        /// <summary>
        /// Specifies that the Thumb moved a specified distance, as determined by the
        /// value of LargeChange. The Thumb moved to the left for a horizontal ScrollBar
        /// or upward for a vertical ScrollBar.
        /// </summary>
        LargeDecrement = 2,

        /// <summary>
        /// Specifies that the Thumb moved a specified distance, as determined by the
        /// value of LargeChange. The Thumb moved to the right for a horizontal ScrollBar
        /// or downward for a vertical ScrollBar.
        /// </summary>
        LargeIncrement = 3,

        ///// <summary>
        ///// Specifies that the Thumb moved to a new position because the user selected
        ///// Scroll Here in the shortcut menu of the ScrollBar.
        ///// </summary>
        //ThumbPosition = 4,

        /// <summary>
        /// The Thumb was dragged and caused a PointerMoved event. A Scroll event of
        /// this ScrollEventType may occur more than one time when the Thumb is dragged
        /// in the ScrollBar.
        /// </summary>
        ThumbTrack = 5,

        ///// <summary>
        ///// Specifies that the Thumb moved to the Minimum position of the ScrollBar.
        ///// </summary>
        //First = 6,

        ///// <summary>
        ///// Specifies that the Thumb moved to the Minimum position of the ScrollBar.
        ///// </summary>
        //Last = 7,

        /// <summary>
        /// Specifies that the Thumb was dragged to a new position and is now no longer
        /// being dragged by the user.
        /// </summary>
        EndScroll = 8,

#if WORKINPROGRESS
        /// <summary>
        /// Specifies that the Thumb moved to a new position because the user selected
        /// Scroll Here in the shortcut menu of the ScrollBar.
        /// </summary>
        ThumbPosition = 4,
        /// <summary>
        /// Specifies that the Thumb moved to the Minimum position of the ScrollBar.
        /// </summary>
        First = 6,
        /// <summary>
        /// Specifies that the Thumb moved to the Minimum position of the ScrollBar.
        /// </summary>
        Last = 7,
#endif
    }
}
