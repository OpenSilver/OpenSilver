

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

#if MIGRATION
namespace System.Windows.Navigation
#else
namespace Windows.UI.Xaml.Navigation
#endif
{
    /// <summary>
    /// Maps a URI into a new URI based on mapping rules defined in a concrete implementation.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public abstract class UriMapperBase
    {
        #region Methods

        /// <summary>
        /// Maps a given URI and returns a mapped URI.
        /// </summary>
        /// <param name="uri">Original URI value to be mapped to a new URI.</param>
        /// <returns>A URI derived from the <paramref name="uri"/> parameter.</returns>
        public abstract Uri MapUri(Uri uri);

        #endregion Methods
    }
}
