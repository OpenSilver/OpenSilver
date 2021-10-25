//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;

#if MIGRATION
namespace System.Windows.Navigation
#else
namespace Windows.UI.Xaml.Navigation
#endif
{
    /// <summary>
    /// Default implementation of INavigationContentLoader that is capable of resolving URI values to XAML types located in the application deployment XAP.
    /// </summary>
    public sealed class PageResourceContentLoader : INavigationContentLoader
    {
        #region Static fields and constants

        private static readonly Regex XClassRegex = new Regex(".*x:Class=\"(.*?)\"", RegexOptions.CultureInvariant);

        #endregion

        #region Methods

        #region INavigationContentLoader implementation

        /// <summary>
        /// Begins asynchronous loading of the provided <paramref name="targetUri"/>.
        /// </summary>
        /// <param name="targetUri">A URI value to resolve and begin loading.</param>
        /// <param name="currentUri">The URI that is currently loaded.</param>
        /// <param name="userCallback">A callback function that will be called when this asynchronous request is ready to have <see cref="EndLoad"/> called on it.</param>
        /// <param name="asyncState">A custom state object that will be returned in <see cref="IAsyncResult.AsyncState"/>, to correlate between multiple async calls.</param>
        /// <returns>An <see cref="IAsyncResult"/> that can be passed to <see cref="CancelLoad(IAsyncResult)"/> at any time, or <see cref="EndLoad(IAsyncResult)"/> after the <paramref name="userCallback"/> has been called.</returns>
        public IAsyncResult BeginLoad(Uri targetUri, Uri currentUri, AsyncCallback userCallback, object asyncState)
        {
            PageResourceContentLoaderAsyncResult result = new PageResourceContentLoaderAsyncResult(targetUri, asyncState);

            if (targetUri == null)
            {
                result.Exception = new ArgumentNullException("targetUri");
            }

            if (SynchronizationContext.Current != null)
            {
                SynchronizationContext.Current.Post((args) => BeginLoad_OnUIThread(userCallback, result), null);
            }
            else
            {
                Deployment.Current.Dispatcher.BeginInvoke(() => BeginLoad_OnUIThread(userCallback, result));
            }

            return result;
        }

        /// <summary>
        /// Attempts to cancel a pending load operation.
        /// </summary>
        /// <param name="asyncResult">The <see cref="IAsyncResult"/> returned from <see cref="BeginLoad(Uri,Uri,AsyncCallback,object)"/> for the operation you wish to cancel.</param>
        /// <remarks>Cancellation is not guaranteed.  Check the result from EndLoad to determine if cancellation was successful.</remarks>
        public void CancelLoad(IAsyncResult asyncResult)
        {
            // No-opt here, we could save that this asyncResult was cancelled in order to throw if someone calls
            // EndLoad on it, but that isn't worth the work since the NavigationService should never do that
            return;
        }

        /// <summary>
        /// Completes the asynchronous loading of content
        /// </summary>
        /// <param name="asyncResult">The result returned from <see cref="BeginLoad(Uri,Uri,AsyncCallback,object)"/>, and passed in to the callback function.</param>
        /// <returns>The content loaded, or null if content was not loaded</returns>
        public LoadResult EndLoad(IAsyncResult asyncResult)
        {
            Guard.ArgumentNotNull(asyncResult, "asyncResult");
            PageResourceContentLoaderAsyncResult result = asyncResult as PageResourceContentLoaderAsyncResult;
            if (result == null)
            {
                throw new InvalidOperationException(String.Format("Wrong kind of {0} passed in.  The {0} passed in should only come from {1}.", "IAsyncResult", "PageResourceContentLoader.BeginLoad"));
            }
            if (result.Exception != null)
            {
                throw result.Exception;
            }

            return new LoadResult(result.Content);
        }

        /// <summary>
        /// Tells whether or not the target Uri can be loaded
        /// </summary>
        /// <param name="targetUri">A URI to load</param>
        /// <param name="currentUri">The current URI</param>
        /// <returns>True if the targetURI can be loaded</returns>
        public bool CanLoad(Uri targetUri, Uri currentUri)
        {
            return UriParsingHelper.InternalUriIsNavigable(targetUri);
        }

        #endregion INavigationContentLoader implementation

        static void GetXamlFileAssociatedClassInstancierName(string pagePathAndName, out string assemblyName, out string instancierName)
        {
            string uriAsString = pagePathAndName;
            //we make sure we have an absolute uri:
            string absoluteSourceUri = "";
            assemblyName = "";
            string[] splittedUri = uriAsString.Split(';');
            if (splittedUri.Length == 1)
            {
                assemblyName = CSHTML5.Internal.StartupAssemblyInfo.StartupAssemblyShortName;
                if (uriAsString.StartsWith("/"))
                {
                    absoluteSourceUri = "/" + assemblyName + ";component" + uriAsString;
                }
                else
                {
                    absoluteSourceUri = "/" + assemblyName + ";component/" + uriAsString;
                }
            }
            else if (splittedUri.Length == 2)
            {
                assemblyName = splittedUri[0];
                if (assemblyName.StartsWith("/"))
                {
                    assemblyName = assemblyName.Substring(1);
                }
                absoluteSourceUri = uriAsString;
            }
            else
            {
                throw new Exception("Class name badly formatted.");
            }
            instancierName = GenerateClassNameFromAbsoluteUri_ForRuntimeAccess(absoluteSourceUri);
        }

        static string GenerateClassNameFromAbsoluteUri_ForRuntimeAccess(string absoluteSourceUri)
        {
            // Convert to TitleCase (so that when we remove the spaces, it is easily readable):
            string className = MakeTitleCase(absoluteSourceUri);

            // If file name contains invalid chars, remove them:
            className = Regex.Replace(className, @"\W", "ǀǀ"); //Note: this is not a pipe (the thing we get with ctrl+alt+6), it is U+01C0

            // If class name doesn't begin with a letter, insert an underscore:
            if (char.IsDigit(className, 0))
            {
                className = className.Insert(0, "_");
            }

            // Remove white space:
            className = className.Replace(" ", string.Empty);

            className += "ǀǀFactory"; //Note: this is not a pipe (the thing we get with ctrl+alt+6), it is U+01C0

            return className;
        }

        static string MakeTitleCase(string str)
        {
            string result = "";
            string lowerStr = str.ToLower();
            int length = str.Length;
            bool makeUpper = true;
            int lastCopiedIndex = -1;
            //****************************
            //HOW THIS WORKS:
            //
            //  We go through all the characters of the string.
            //  If any is not an alphanumerical character, we make the next alphanumerical character uppercase.
            //  To do so, we copy the string (on which we call toLower) bit by bit into a new variable,
            //  each bit being the part between two uppercase characters, and while inserting the uppercase version of the character between each bit.
            //  then we add the end of the string.
            //****************************

            for (int i = 0; i < length; ++i)
            {
                char ch = lowerStr[i];
                if (ch >= 'a' && ch <= 'z' || ch >= '0' && ch <= '0')
                {
                    if (makeUpper && ch >= 'a' && ch <= 'z') //if we have a letter, we make it uppercase. otherwise, it is a number so we let it as is.
                    {
                        if (!(lastCopiedIndex == -1 && i == 0)) //except this very specific case, we should never have makeUpper at true while i = lastCopiedindex + 1 (since we made lowerStr[lastCopiedindex] into an uppercase letter.
                        {
                            result += lowerStr.Substring(lastCopiedIndex + 1, i - lastCopiedIndex - 1); //i - lastCopied - 1 because we do not want to copy the current index since we want to make it uppercase:
                        }
                        result += (char)(ch - 32); //32 is the difference between the lower case and the upper case, meaning that (char)('a' - 32) --> 'A'.
                        lastCopiedIndex = i;
                    }
                    makeUpper = false;
                }
                else
                {
                    makeUpper = true;
                }
            }
            //we copy the rest of the string:
            if (lastCopiedIndex < length - 1)
            {
                result += str.Substring(lastCopiedIndex + 1);
            }
            return result;


            //bool isFirst = true;
            //string[] spaceSplittedString = str.Split(' ');
            //foreach (string s in spaceSplittedString)
            //{
            //    if (isFirst)
            //    {
            //        isFirst = false;
            //    }
            //    else
            //    {
            //        result += " ";
            //    }
            //    result += MakeFirstCharUpperAndRestLower(s);
            //}
            //return result;
        }

#if SL_TOOLKIT
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We need to catch all exceptions to ensure we raise events in error cases instead, per design.")]
#endif
        private static void BeginLoad_OnUIThread(AsyncCallback userCallback, PageResourceContentLoaderAsyncResult result)
        {
            if (result.Exception != null)
            {
                result.IsCompleted = true;
                userCallback(result);
                return;
            }

            try
            {
                string pagePathAndName = UriParsingHelper.InternalUriGetBaseValue(result.Uri);

                string assemblyName;
                string instancierName;
                GetXamlFileAssociatedClassInstancierName(pagePathAndName, out assemblyName, out instancierName); //this method should only accept .xaml files (imo), it returns the name of the class we generated during the compilation, built from the assembly(gotten from either the uri, or the startingAssembly) and the file name.

                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (assembly.GetName().Name == assemblyName)
                    {
                        Type type = assembly.GetType(instancierName);
                        if (type != null)
                        {
                            MethodInfo methodInfo = type.GetMethod("Instantiate");
                            result.Content = methodInfo.Invoke(null, null);
                            return;
                        }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }
            finally
            {
                result.IsCompleted = true;
                if (userCallback != null)
                {
                    userCallback(result);
                }
            }
        }

#endregion Methods

#region Nested Classes

        private class PageResourceContentLoaderAsyncResult : IAsyncResult
        {
            private object _asyncState;
            private bool _isCompleted;

#region Constructors

            /// <summary>
            /// Constructs an instance of the <see cref="PageResourceContentLoaderAsyncResult"/>
            /// </summary>
            /// <param name="uri">The Uri that the <see cref="PageResourceContentLoader"/> is loading.</param>
            /// <param name="asyncState">The state object the user passed in to <see cref="PageResourceContentLoader.BeginLoad"/></param>
            internal PageResourceContentLoaderAsyncResult(Uri uri, object asyncState)
            {
                this._asyncState = asyncState;
                this.Uri = uri;
            }

#endregion

#region IAsyncResult Members

            public object AsyncState
            {
                get { return this._asyncState; }
            }

            public WaitHandle AsyncWaitHandle
            {
                get { return null; }
            }

            public bool CompletedSynchronously
            {
                get { return false; }
            }

            public bool IsCompleted
            {
                get { return this._isCompleted; }
                internal set { this._isCompleted = value; }
            }

#endregion

#region Properties

            internal Uri Uri { get; set; }

            internal Exception Exception { get; set; }

            internal object Content { get; set; }

#endregion
        }

#endregion
    }
}
