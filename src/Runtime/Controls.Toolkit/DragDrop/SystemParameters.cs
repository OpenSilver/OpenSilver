// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace Microsoft.Windows
{
    /// <summary>
    /// Contains properties that you can use to query system settings.
    /// </summary>
    /// <QualityBand>Experimental</QualityBand>
    public static class SystemParameters
    {
        /// <summary>
        /// Gets the minimum amount of horizontal drag distance before a drag operation occurs.
        /// </summary>
        /// <returns>The minimum amount of horizontal drag distance before a drag operation occurs.</returns>
        public static double MinimumHorizontalDragDistance
        {
            get { return 4; }
        }

        /// <summary>
        /// Gets the minimum amount of vertical drag distance before a drag operation occurs.
        /// </summary>
        /// <returns>The minimum amount of vertical drag distance before a drag operation occurs.</returns>
        public static double MinimumVerticalDragDistance
        {
            get { return 4; }
        }
    }
}