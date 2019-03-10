
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
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
    }
}
