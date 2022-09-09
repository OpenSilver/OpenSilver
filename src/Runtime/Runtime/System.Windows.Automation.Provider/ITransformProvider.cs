
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

#if MIGRATION
namespace System.Windows.Automation.Provider
#else
namespace Windows.UI.Xaml.Automation.Provider
#endif
{
    /// <summary>
    /// Exposes methods and properties to support access by a UI automation client to controls 
    /// or elements that can be moved, resized, or rotated within a two-dimensional space.
    /// </summary>
    public interface ITransformProvider
    {
        /// <summary>
        /// Gets a value that indicates whether the element can be moved.
        /// </summary>
        /// <returns>
        /// true if the element can be moved; otherwise, false.
        /// </returns>
        bool CanMove { get; }

        /// <summary>
        /// Gets a value that indicates whether the element can be resized.
        /// </summary>
        /// <returns>
        /// true if the element can be resized; otherwise, false.
        /// </returns>
        bool CanResize { get; }

        /// <summary>
        /// Gets a value that indicates whether the element can be rotated.
        /// </summary>
        /// <returns>
        /// true if the element can be rotated; otherwise, false.
        /// </returns>
        bool CanRotate { get; }

        /// <summary>
        /// Moves the control.
        /// </summary>
        /// <param name="x">
        /// The absolute screen coordinates of the left side of the control.
        /// </param>
        /// <param name="y">
        /// The absolute screen coordinates of the top of the control.
        /// </param>
        void Move(double x, double y);

        /// <summary>
        /// Resizes the control.
        /// </summary>
        /// <param name="width">
        /// The new width of the window, in pixels.
        /// </param>
        /// <param name="height">
        /// The new height of the window, in pixels.
        /// </param>
        void Resize(double width, double height);

        /// <summary>
        /// Rotates the control.
        /// </summary>
        /// <param name="degrees">
        /// The number of degrees to rotate the control. A positive number rotates 
        /// the control clockwise. A negative number rotates the control counterclockwise.
        /// </param>
        void Rotate(double degrees);
    }
}
