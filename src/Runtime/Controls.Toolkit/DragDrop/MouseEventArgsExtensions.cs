// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace System.Windows.Input
{
    /// <summary>
    /// Contains extension methods for the MouseEventArgs class.
    /// </summary>
    public static class MouseEventArgsExtensions
    {
        /// <summary>
        /// Returns the position of mouse relative to an object.
        /// </summary>
        /// <param name="mouseEventArgs">Information about a mouse event.</param>
        /// <param name="relativeTo">The element relative to which the position
        /// is returned.</param>
        /// <returns>The position of the mouse relative to the object.</returns>
        public static Point GetSafePosition(this MouseEventArgs mouseEventArgs, UIElement relativeTo)
        {
            Point returnPoint = new Point(0, 0);

            try
            {
                returnPoint = mouseEventArgs.GetPosition(relativeTo);
            }
            catch
            {
                // Should never throw but unfortunately it does.
            }

            return returnPoint;
        }
    }
}