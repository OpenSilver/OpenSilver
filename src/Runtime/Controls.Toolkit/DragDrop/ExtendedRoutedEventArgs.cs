// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace Microsoft.Windows
{
    /// <summary>
    /// Contains state information and event data associated with a routed event.
    /// </summary>
    /// <QualityBand>Experimental</QualityBand>
    public abstract class ExtendedRoutedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets a value indicating whether the present state of the 
        /// event handling for a routed event as it travels the route.
        /// </summary>
        public bool Handled { get; set; }

        /// <summary>
        /// Gets the original reporting source as determined by pure hit testing, before
        /// any possible System.Windows.RoutedEventArgs.Source adjustment by a parent
        /// class.
        /// </summary>
        public object OriginalSource { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the ExtendedRoutedEventArgs class.
        /// </summary>
        internal ExtendedRoutedEventArgs()
        {
        }
    }
}