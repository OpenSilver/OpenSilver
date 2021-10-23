

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
    /// Navigation interface for ContentLoader instances
    /// </summary>
    public interface INavigationContentLoader
    {
        /// <summary>
        /// Begins asynchronous loading of the provided <paramref name="targetUri"/>.
        /// </summary>
        /// <param name="targetUri">A URI value to resolve and begin loading.</param>
        /// <param name="currentUri">The current URI.</param>
        /// <param name="userCallback">A callback function that will be called when this asynchronous request is ready to have <see cref="EndLoad"/> called on it.</param>
        /// <param name="asyncState">A custom state object that will be returned in <see cref="IAsyncResult.AsyncState"/>, to correlate between multiple async calls.</param>
        /// <returns>An <see cref="IAsyncResult"/> that can be passed to <see cref="CancelLoad(IAsyncResult)"/> at any time, or <see cref="EndLoad(IAsyncResult)"/> after the <paramref name="userCallback"/> has been called.</returns>
        IAsyncResult BeginLoad(Uri targetUri, Uri currentUri, AsyncCallback userCallback, object asyncState);

        /// <summary>
        /// Attempts to cancel a pending load operation.
        /// </summary>
        /// <param name="asyncResult">The <see cref="IAsyncResult"/> returned from <see cref="BeginLoad(Uri,Uri,AsyncCallback,object)"/> for the operation you wish to cancel.</param>
        /// <remarks>Cancellation is not guaranteed.  Check the result from EndLoad to determine if cancellation was successful.</remarks>
        void CancelLoad(IAsyncResult asyncResult);

        /// <summary>
        /// Completes the asynchronous loading of content
        /// </summary>
        /// <param name="asyncResult">The result returned from <see cref="BeginLoad(Uri,Uri,AsyncCallback,object)"/>, and passed in to the callback function.</param>
        /// <returns>A <see cref="LoadResult"/> containing the content loaded, null content, or a redirect Uri.</returns>
        LoadResult EndLoad(IAsyncResult asyncResult);

        /// <summary>
        /// Tells whether or not the targetUri is of the correct format for <see cref="BeginLoad(Uri,Uri,AsyncCallback,object)"/>.
        /// </summary>
        /// <param name="targetUri">A URI to load</param>
        /// <param name="currentUri">The current URI</param>
        /// <returns>True if the targetUri can be loaded</returns>
        bool CanLoad(Uri targetUri, Uri currentUri);
    }
}
