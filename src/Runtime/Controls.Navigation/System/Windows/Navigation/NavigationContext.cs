//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

#if MIGRATION
namespace System.Windows.Navigation
#else
namespace Windows.UI.Xaml.Navigation
#endif
{
    /// <summary>
    /// Represents the state of a navigation operation.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public sealed class NavigationContext
    {
#region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="queryString">Dictionary of query string values.</param>
        internal NavigationContext(IDictionary<string, string> queryString)
        {
            this.QueryString = queryString;
        }

#endregion Constructors

#region Properties

        /// <summary>
        /// Gets a dictionary of query string values.
        /// </summary>
        public IDictionary<string, string> QueryString
        {
            get;
            private set;
        }

#endregion Properties
    }
}
