// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

#if DEBUG

using System.Windows.Controls.Primitives;

namespace System.Windows.Controls
{
    /// <summary>
    /// Represents the control that redistributes space between columns or rows
    /// of a Grid control.
    /// </summary>
    /// <QualityBand>Mature</QualityBand>
    public partial class GridSplitter : Control
    {
        /// <summary>
        /// Exposes test hooks to unit tests with internal access.
        /// </summary>
        private InternalTestHook _testHook;

        /// <summary>
        /// Gets a test hook for unit tests with internal access.
        /// </summary>
        internal InternalTestHook TestHook
        {
            get
            {
                if (_testHook == null)
                {
                    _testHook = new InternalTestHook(this);
                }
                return _testHook;
            }
        }

        /// <summary>
        /// Expose test hooks for internal and private members of the
        /// GridSplitter.
        /// </summary>
        internal class InternalTestHook
        {
            /// <summary>
            /// Reference to the outer 'parent' GridSplitter.
            /// </summary>
            private GridSplitter _gridSplitter;

            /// <summary>
            /// Initializes a new instance of the InternalTestHook class.
            /// </summary>
            /// <param name="gridSplitter">The grid splitter to hook.</param>
            internal InternalTestHook(GridSplitter gridSplitter)
            {
                _gridSplitter = gridSplitter;
            }

            /// <summary>
            /// Gets the GridSplitter's GridResizeDirection.
            /// </summary>
            internal GridResizeDirection GridResizeDirection
            {
                get { return _gridSplitter._currentGridResizeDirection; }
            }

            /// <summary>
            /// Gets the GridSplitter's PreviewLayer.
            /// </summary>
            internal Canvas PreviewLayer
            {
                get { return _gridSplitter._previewLayer; }
            }

            /// <summary>
            /// Gets the GridSplitter's ResizeData.
            /// </summary>
            internal ResizeData ResizeData
            {
                get { return _gridSplitter.ResizeDataInternal; }
            }

            /// <summary>
            /// Simulate the DragValidator's DragCompleted event.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="e">Event arguments.</param>
            internal void DragValidator_DragCompletedEvent(object sender, DragCompletedEventArgs e)
            {
                _gridSplitter.DragValidator_DragCompletedEvent(sender, e);
            }

            /// <summary>
            /// Simulate the DragValidator's DragDelta event.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="e">Event arguments.</param>
            internal void DragValidator_DragDeltaEvent(object sender, DragDeltaEventArgs e)
            {
                _gridSplitter.DragValidator_DragDeltaEvent(sender, e);
            }

            /// <summary>
            /// Simulate the DragValidator's DragStarted event.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="e">Event arguments.</param>
            internal void DragValidator_DragStartedEvent(object sender, DragStartedEventArgs e)
            {
                _gridSplitter.DragValidator_DragStartedEvent(sender, e);
            }

            /// <summary>
            /// Simulate using the keyboard to move the splitter.
            /// </summary>
            /// <param name="horizontalChange">Horizontal change.</param>
            /// <param name="verticalChange">Vertical change.</param>
            /// <returns>
            /// A value indicating whether the splitter was moved.
            /// </returns>
            internal bool KeyboardMoveSplitter(double horizontalChange, double verticalChange)
            {
                return _gridSplitter.KeyboardMoveSplitter(horizontalChange, verticalChange);
            }
        }
    }
}

#endif
