// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows.Input;
using System.Windows.Media;
using SW = Microsoft.Windows;

namespace System.Windows
{
    /// <summary>
    /// A collection of extension methods for the UIElement class.
    /// </summary>
    /// <QualityBand>Experimental</QualityBand>
    internal static partial class UIElementExtensions
    {
        /// <summary>
        /// Returns the size of an element.
        /// </summary>
        /// <remarks>
        /// Note that this method only exists because there is a Silverlight bug that
        /// causes elements inside of a Canvas to return (0,0) for their RenderSize.
        /// This is a workaround that attempts to downcast to a FrameworkElement and
        /// if the cast is successful it uses the ActualWidth and ActualHeight 
        /// properties.
        /// </remarks>
        /// <param name="that">The element for which to retrieve the size.</param>
        /// <returns>The size of the element.</returns>
        internal static Size GetSize(this UIElement that)
        {
            FrameworkElement frameworkElement = that as FrameworkElement;
            if (frameworkElement != null)
            {
                return new Size(frameworkElement.ActualWidth, frameworkElement.ActualHeight);
            }
            else
            {
                return that.RenderSize;
            }
        }

        /// <summary>
        /// Converts the key enumeration into the appropriate DragDropKeyStates
        /// value.
        /// </summary>
        /// <param name="key">The key value.</param>
        /// <returns>The appropriate SW.DragDropKeyStates value.</returns>
        internal static SW.DragDropKeyStates ToDragDropKeyStates(Key key)
        {
            switch (key)
            {
                case Key.Ctrl:
                    return SW.DragDropKeyStates.ControlKey;
                case Key.Alt:
                    return SW.DragDropKeyStates.AltKey;
                case Key.Shift:
                    return SW.DragDropKeyStates.ShiftKey;
            }
            return SW.DragDropKeyStates.None;
        }

        /// <summary>
        /// This method performs a transform to visual operation but traps exceptions that occur.
        /// </summary>
        /// <param name="that">The element to transform to.</param>
        /// <param name="element">The element to transform from.</param>
        /// <returns>A general transform.</returns>
        internal static GeneralTransform SafeTransformToVisual(this UIElement that, UIElement element)
        {
            try
            {
                return that.TransformToVisual(element);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }
    }
}