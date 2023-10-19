// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics;
using System.Windows.Media;

namespace System.Windows.Controls
{
    /// <summary>
    /// The ScrollExtensions class provides utility methods for scrolling items
    /// ScrollViewers.
    /// </summary>
    // Note: This class should be internal, but due to conflicts with the
    // ScrollViewerExtensions class, this has been changed to public.
    // TODO: change back to internal when theses classes will be moved to
    // separate projects.
    public static class ScrollExtensions
    {
        /// <summary>
        /// The amount to scroll a ScrollViewer for a line change.
        /// </summary>
        private const double LineChange = 16.0;

        /// <summary>
        /// Scroll a ScrollViewer vertically by a given offset.
        /// </summary>
        /// <param name="viewer">The ScrollViewer.</param>
        /// <param name="offset">The vertical offset to scroll.</param>
        private static void ScrollByVerticalOffset(ScrollViewer viewer, double offset)
        {
            Debug.Assert(viewer != null, "viewer should not be null!");

            offset += viewer.VerticalOffset;
            offset = Math.Max(Math.Min(offset, viewer.ExtentHeight), 0);
            viewer.ScrollToVerticalOffset(offset);
        }

        /// <summary>
        /// Scroll a ScrollViewer horizontally by a given offset.
        /// </summary>
        /// <param name="viewer">The ScrollViewer.</param>
        /// <param name="offset">The horizontal offset to scroll.</param>
        private static void ScrollByHorizontalOffset(ScrollViewer viewer, double offset)
        {
            Debug.Assert(viewer != null, "viewer should not be null!");

            offset += viewer.HorizontalOffset;
            offset = Math.Max(Math.Min(offset, viewer.ExtentWidth), 0);
            viewer.ScrollToHorizontalOffset(offset);
        }

        /// <summary>
        /// Scroll the ScrollViewer up by a line.
        /// </summary>
        /// <param name="viewer">The ScrollViewer.</param>
        public static void LineUp(this ScrollViewer viewer)
        {
            Debug.Assert(viewer != null, "viewer should not be null!");
            ScrollByVerticalOffset(viewer, -LineChange);
        }

        /// <summary>
        /// Scroll the ScrollViewer down by a line.
        /// </summary>
        /// <param name="viewer">The ScrollViewer.</param>
        public static void LineDown(this ScrollViewer viewer)
        {
            Debug.Assert(viewer != null, "viewer should not be null!");
            ScrollByVerticalOffset(viewer, LineChange);
        }

        /// <summary>
        /// Scroll the ScrollViewer left by a line.
        /// </summary>
        /// <param name="viewer">The ScrollViewer.</param>
        public static void LineLeft(this ScrollViewer viewer)
        {
            Debug.Assert(viewer != null, "viewer should not be null!");
            ScrollByHorizontalOffset(viewer, -LineChange);
        }

        /// <summary>
        /// Scroll the ScrollViewer right by a line.
        /// </summary>
        /// <param name="viewer">The ScrollViewer.</param>
        public static void LineRight(this ScrollViewer viewer)
        {
            Debug.Assert(viewer != null, "viewer should not be null!");
            ScrollByHorizontalOffset(viewer, LineChange);
        }

        /// <summary>
        /// Scroll the ScrollViewer up by a page.
        /// </summary>
        /// <param name="viewer">The ScrollViewer.</param>
        public static void PageUp(this ScrollViewer viewer)
        {
            Debug.Assert(viewer != null, "viewer should not be null!");
            ScrollByVerticalOffset(viewer, -viewer.ViewportHeight);
        }

        /// <summary>
        /// Scroll the ScrollViewer down by a page.
        /// </summary>
        /// <param name="viewer">The ScrollViewer.</param>
        public static void PageDown(this ScrollViewer viewer)
        {
            Debug.Assert(viewer != null, "viewer should not be null!");
            ScrollByVerticalOffset(viewer, viewer.ViewportHeight);
        }

        /// <summary>
        /// Scroll the ScrollViewer left by a page.
        /// </summary>
        /// <param name="viewer">The ScrollViewer.</param>
        public static void PageLeft(this ScrollViewer viewer)
        {
            Debug.Assert(viewer != null, "viewer should not be null!");
            ScrollByHorizontalOffset(viewer, -viewer.ViewportWidth);
        }

        /// <summary>
        /// Scroll the ScrollViewer right by a page.
        /// </summary>
        /// <param name="viewer">The ScrollViewer.</param>
        public static void PageRight(this ScrollViewer viewer)
        {
            Debug.Assert(viewer != null, "viewer should not be null!");
            ScrollByHorizontalOffset(viewer, viewer.ViewportWidth);
        }

        /// <summary>
        /// Scroll the ScrollViewer to the top.
        /// </summary>
        /// <param name="viewer">The ScrollViewer.</param>
        public static void ScrollToTop(this ScrollViewer viewer)
        {
            Debug.Assert(viewer != null, "viewer should not be null!");
            viewer.ScrollToVerticalOffset(0);
        }

        /// <summary>
        /// Scroll the ScrollViewer to the bottom.
        /// </summary>
        /// <param name="viewer">The ScrollViewer.</param>
        public static void ScrollToBottom(this ScrollViewer viewer)
        {
            Debug.Assert(viewer != null, "viewer should not be null!");
            viewer.ScrollToVerticalOffset(viewer.ExtentHeight);
        }

        /// <summary>
        /// Get the top and bottom of an element with respect to its parent.
        /// </summary>
        /// <param name="element">The element to get the position of.</param>
        /// <param name="parent">The parent of the element.</param>
        /// <param name="top">Vertical offset to the top of the element.</param>
        /// <param name="bottom">
        /// Vertical offset to the bottom of the element.
        /// </param>
        public static void GetTopAndBottom(this FrameworkElement element, FrameworkElement parent, out double top, out double bottom)
        {
            Debug.Assert(element != null, "element should not be null!");
            Debug.Assert(parent != null, "parent should not be null!");

            GeneralTransform transform = element.TransformToVisual(parent);
            top = transform.Transform(new Point(0, 0)).Y;
            bottom = transform.Transform(new Point(0, element.ActualHeight)).Y;
        }
    }
}
