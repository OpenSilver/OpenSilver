// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace Microsoft.Windows
{
    /// <summary>
    /// Contains arguments relevant to all drag-and-drop events (System.Windows.DragDrop.DragEnter,
    /// System.Windows.DragDrop.DragLeave, System.Windows.DragDrop.DragOver, and
    /// System.Windows.DragDrop.Drop).
    /// </summary>
    /// <QualityBand>Experimental</QualityBand>
    public sealed class DragEventArgs : ExtendedRoutedEventArgs
    {
        /// <summary>
        /// Gets a member of the System.Windows.DragDropEffects enumeration that specifies
        /// which operations are allowed by the originator of the drag event.
        /// </summary>
        public DragDropEffects AllowedEffects { get; internal set; }

        /// <summary>
        /// Gets a data object that contains the data associated with the corresponding
        /// drag event.
        /// </summary>
        public IDataObject Data { get; internal set; }

        /// <summary>
        /// Gets or sets the target drop-and-drop operation.
        /// </summary>
        public DragDropEffects Effects { get; set; }

        /// <summary>
        /// Gets or sets the MouseEventArgs related to the DragEventArgs.
        /// </summary>
        internal MouseEventArgs MouseEventArgs { get; set; }

        /// <summary>
        /// Initializes a new instance of the DragEventArgs class.
        /// </summary>
        internal DragEventArgs()
        {
        }

        /// <summary>
        /// Initializes a new instance of the DragEventArgs class.
        /// </summary>
        /// <param name="args">The DragEventArgs object to use as the base for
        /// this DragEventArgs.</param>
        internal DragEventArgs(DragEventArgs args)
        {
            Debug.Assert(args != null, "args must not be null.");

            this.AllowedEffects = args.AllowedEffects;
            this.MouseEventArgs = args.MouseEventArgs;
            this.Effects = args.Effects;
            this.Data = args.Data;
            this.OriginalSource = args.OriginalSource;
        }

        /// <summary>
        /// Returns a drop point that is relative to a specified System.Windows.UIElement.
        /// </summary>
        /// <param name="relativeTo">A UIElement object for which to get a relative drop point.</param>
        /// <returns>A drop point that is relative to the element specified in relativeTo.</returns>
        public Point GetPosition(UIElement relativeTo)
        {
            if (relativeTo == null)
            {
                throw new ArgumentNullException(nameof(relativeTo));
            }

            if (MouseEventArgs != null)
            {
                 return MouseEventArgs.GetSafePosition(relativeTo);
            }
            return new Point(double.NaN, double.NaN);
        }
    }
}