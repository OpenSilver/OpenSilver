

/*===================================================================================
*
*   Copyright (c) Userware/OpenSilver.net
*
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*
\*====================================================================================*/


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
