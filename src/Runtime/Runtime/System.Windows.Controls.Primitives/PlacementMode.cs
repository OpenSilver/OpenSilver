
/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

namespace System.Windows.Controls.Primitives
{
    /// <summary>
    /// Specifies the preferred location for positioning a ToolTip relative to a visual element.
    /// </summary>
    public enum PlacementMode
    {
        /// <summary>
        /// A position of the <see cref="Popup"/> control where the control aligns its upper edge with
        /// the lower edge of the <see cref="Popup.PlacementTarget"/> and aligns its left edge with the 
        /// left edge of the <see cref="Popup.PlacementTarget"/>. If the lower screen-edge obscures the 
        /// <see cref="Popup"/>, the control repositions itself so that its lower edge aligns with the 
        /// upper edge of the <see cref="Popup.PlacementTarget"/>. If the upper screen-edge obscures the 
        /// <see cref="Popup"/>, the control then repositions itself so that its upper edge aligns with 
        /// the upper screen-edge.
        /// </summary>
        Bottom = 2,

        /// <summary>
        /// A position of the <see cref="Popup"/> control that aligns its left edge with the right edge 
        /// of the <see cref="Popup.PlacementTarget"/> and aligns its upper edge with the upper edge of 
        /// the <see cref="Popup.PlacementTarget"/>. If the right screen-edge obscures the <see cref="Popup"/>,
        /// the control repositions itself so that its left edge aligns with the left edge of the 
        /// <see cref="Popup.PlacementTarget"/>. If the left screen-edge obscures the <see cref="Popup"/>, 
        /// the control repositions itself so that its left edge aligns with the left screen-edge. If the 
        /// upper or lower screen-edge obscures the <see cref="Popup"/>, the control then repositions itself 
        /// to align with the obscuring screen edge.
        /// </summary>
        Right = 4,

        /// <summary>
        /// A position of the <see cref="Popup"/> control that aligns its upper edge with the lower edge 
        /// of the bounding box of the mouse and aligns its left edge with the left edge of the bounding 
        /// box of the mouse. If the lower screen-edge obscures the <see cref="Popup"/>, it repositions
        /// itself to align with the upper edge of the bounding box of the mouse. If the upper screen-edge 
        /// obscures the <see cref="Popup"/>, the control repositions itself to align with the upper screen-edge.
        /// </summary>
        Mouse = 7,

        /// <summary>
        /// A position of the <see cref="Popup"/> control relative to the tip of the mouse cursor and at 
        /// an offset that is defined by the <see cref="Popup.HorizontalOffset"/> and <see cref="Popup.VerticalOffset"/> 
        /// property values. If a horizontal or vertical screen edge obscures the <see cref="Popup"/>, it opens 
        /// in the opposite direction from the obscuring edge. If the opposite screen edge also obscures the 
        /// <see cref="Popup"/>, it then aligns with the obscuring screen edge.
        /// </summary>
        MousePoint = 8,

        /// <summary>
        /// A <see cref="Popup"/> control that aligns its right edge with the left edge of the <see cref="Popup.PlacementTarget"/>
        /// and aligns its upper edge with the upper edge of the <see cref="Popup.PlacementTarget"/>. If the left 
        /// screen-edge obscures the <see cref="Popup"/>, the <see cref="Popup"/> repositions itself so that its left
        /// edge aligns with the right edge of the <see cref="Popup.PlacementTarget"/>. If the right screen-edge obscures 
        /// the <see cref="Popup"/>, the right edge of the control aligns with the right screen-edge. If the upper or 
        /// lower screen-edge obscures the <see cref="Popup"/>, the control repositions itself to align with the obscuring 
        /// screen edge.
        /// </summary>
        Left = 9,

        /// <summary>
        /// A position of the <see cref="Popup"/> control that aligns its lower edge with the upper edge 
        /// of the <see cref="Popup.PlacementTarget"/> and aligns its left edge with the left edge of the 
        /// <see cref="Popup.PlacementTarget"/>. If the upper screen-edge obscures the <see cref="Popup"/>,
        /// the control repositions itself so that its upper edge aligns with the lower edge of the 
        /// <see cref="Popup.PlacementTarget"/>. If the lower screen-edge obscures the <see cref="Popup"/>,
        /// the lower edge of the control aligns with the lower screen-edge. If the left or right screen-edge
        /// obscures the <see cref="Popup"/>, it then repositions itself to align with the obscuring screen.
        /// </summary>
        Top = 10,
    }
}
