//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Windows.Controls
{
    /// <summary>
    /// Wrapper class used to sort paths based on the order derived from a DisplayAttribute.
    /// </summary>
    internal class PathOrderPair
    {
        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        internal string Path
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        internal int Order
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the order from a PathOrderPair.
        /// </summary>
        /// <param name="pathOrderPair">The PathOrderPair.</param>
        /// <returns>The order from the PathOrderPair.</returns>
        internal static int GetOrder(PathOrderPair pathOrderPair)
        {
            return pathOrderPair.Order;
        }
    }
}
