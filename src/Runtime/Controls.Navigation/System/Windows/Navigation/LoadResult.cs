

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


#if MIGRATION
namespace System.Windows.Navigation
#else
namespace Windows.UI.Xaml.Navigation
#endif
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
