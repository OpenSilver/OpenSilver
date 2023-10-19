//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Windows.Navigation
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
