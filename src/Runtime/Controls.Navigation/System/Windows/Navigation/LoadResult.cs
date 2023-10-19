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
    /// Result of a Load operation from an <see cref="INavigationContentLoader"/>
    /// </summary>
    public class LoadResult
    {
        /// <summary>
        /// Creates a LoadResult
        /// </summary>
        /// <param name="loadedContent">Content loaded from the load operation</param>
        public LoadResult(object loadedContent)
        {
            this.LoadedContent = loadedContent;
        }

        /// <summary>
        /// Creates a LoadResult
        /// </summary>
        /// <param name="redirectUri">Uri used for redirection by the <see cref="NavigationService"/></param>
        public LoadResult(Uri redirectUri)
        {
            this.RedirectUri = redirectUri;
        }

        /// <summary>
        /// Content loaded from the load operation
        /// </summary>
        public object LoadedContent
        {
            get;
            private set;
        }

        /// <summary>
        /// Uri used for redirection by the <see cref="NavigationService"/>
        /// </summary>
        public Uri RedirectUri
        {
            get;
            private set;
        }
    }
}
