// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using SW = Microsoft.Windows;

namespace System.Windows
{
    /// <summary>
    /// A collection of extension methods for the SW.IDataObject type.
    /// </summary>
    internal static class IDataObjectExtensions
    {
        /// <summary>
        /// Retrieves the data based using the first acceptable format.
        /// </summary>
        /// <param name="that">The data object.</param>
        /// <returns>The data retrieved from the data object.</returns>
        public static object GetData(this SW.IDataObject that)
        {
            return that.GetData(that.GetFormats()[0]);
        }
    }
}