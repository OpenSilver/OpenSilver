//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Resources;
using OpenSilver.Internal.Navigation;

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

#if SL_TOOLKIT
        private static readonly Regex XClassRegex = new Regex(".*x:Class=\"(.*?)\"", RegexOptions.CultureInvariant);
#endif

        #endregion

        #region Methods

        private static string GetEntryPointAssemblyPartSource()
        {
#if SL_TOOLKIT
            string assemblyPartSource = null;

            foreach (AssemblyPart ap in Deployment.Current.Parts)
            {
                if (ap.Source.Substring(0, ap.Source.Length - 4) == Deployment.Current.EntryPointAssembly)
                {
                    assemblyPartSource = ap.Source;
                    break;
                }
            }

            return assemblyPartSource.Substring(0, assemblyPartSource.Length - 4);
#else
            return Deployment.Current.EntryPointAssembly;
#endif
        }

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
                throw new InvalidOperationException(String.Format(Resource.PageResourceContentLoader_WrongIAsyncResult, "IAsyncResult", "PageResourceContentLoader.BeginLoad"));
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

#if SL_TOOLKIT
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We need to catch all exceptions to ensure we raise events in error cases instead, per design.")]
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

                string xaml = GetLocalXaml(pagePathAndName);

                if (String.IsNullOrEmpty(xaml))
                {
                    result.Exception = new InvalidOperationException(
                                        String.Format(
                                            CultureInfo.CurrentCulture,
                                            Resource.PageResourceContentLoader_NoXAMLWasFound,
                                            pagePathAndName));
                    return;
                }

                string classString = GetXClass(xaml);

                if (String.IsNullOrEmpty(classString))
                {
                    try
                    {
                        result.Content = XamlReader.Load(xaml);
                    }
                    catch (Exception ex)
                    {
                        result.Exception = new InvalidOperationException(
                                            String.Format(
                                                CultureInfo.CurrentCulture,
                                                Resource.PageResourceContentLoader_XAMLWasUnloadable,
                                                pagePathAndName),
                                            ex);
                        return;
                    }
                }
                else
                {
                    // If it does have an x:Class attribute, then it has a
                    // code-behind, so get the CLR type of the XAML instead.
                    Type t = GetTypeFromAnyLoadedAssembly(classString);

                    if (t == null)
                    {
                        result.Exception = new InvalidOperationException(String.Format(
                                            CultureInfo.CurrentCulture,
                                            Resource.PageResourceContentLoader_TheTypeSpecifiedInTheXClassCouldNotBeFound,
                                            classString,
                                            pagePathAndName));
                        return;
                    }

                    result.Content = Activator.CreateInstance(t);
                    return;
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

        /// <summary>
        /// Returns the XAML string of the page at the given path.
        /// If that page cannot be found (path does not exist, etc.)
        /// then it returns null.
        /// </summary>
        /// <param name="pagePathAndName">The path and name of the XAML (with the ".xaml" included)</param>
        /// <returns>See summary</returns>
        private static string GetLocalXaml(string pagePathAndName)
        {
            string assemblyPartSource = null;
            string pagePathAndNameWithoutAssembly = null;

            if (!pagePathAndName.Contains(UriParsingHelper.ComponentDelimiter))
            {
                assemblyPartSource = GetEntryPointAssemblyPartSource();
                pagePathAndNameWithoutAssembly = pagePathAndName;
            }
            else
            {
                string[] pagePathAndNameParts = pagePathAndName.Split(new string[] { UriParsingHelper.ComponentDelimiterWithoutSlash }, StringSplitOptions.RemoveEmptyEntries);
                if (pagePathAndNameParts.Length != 2)
                {
                    throw new InvalidOperationException(Resource.PageResourceContentLoader_InvalidComponentSyntax);
                }
                assemblyPartSource = pagePathAndNameParts[0];
                pagePathAndNameWithoutAssembly = pagePathAndNameParts[1];
            }

            if (String.IsNullOrEmpty(assemblyPartSource))
            {
                return null;
            }

            // In case the Uri contains international characters (ex: 完全采用统.xaml), we need to escape them.
            // because Application.GetResourceStream expects an escaped Uri.
            pagePathAndNameWithoutAssembly = Uri.EscapeUriString(pagePathAndNameWithoutAssembly);
            assemblyPartSource = Uri.EscapeUriString(assemblyPartSource);

            StreamResourceInfo sri = Application.GetResourceStream(new Uri(assemblyPartSource +
                                                                           UriParsingHelper.ComponentDelimiterWithoutSlash +
                                                                           pagePathAndNameWithoutAssembly,
                                                                           UriKind.Relative));
            if (sri == null)
            {
                return null;
            }

            using (StreamReader reader = new StreamReader(sri.Stream))
            {
                return reader.ReadToEnd();
            }
        }

        private static string GetXClass(string xaml)
        {
            Match m = XClassRegex.Match(xaml);

            if (m == Match.Empty)
            {
                return null;
            }
            else
            {
                return m.Groups[1].Value;
            }
        }

        private static Type GetTypeFromAnyLoadedAssembly(string typeName)
        {
            Type t = null;

            foreach (AssemblyPart ap in Deployment.Current.Parts)
            {
                StreamResourceInfo sri = Application.GetResourceStream(new Uri(ap.Source, UriKind.Relative));
                if (sri != null)
                {
                    Assembly theAssembly = new AssemblyPart().Load(sri.Stream);
                    if (theAssembly != null)
                    {
                        t = Type.GetType(
                                        typeName + "," + theAssembly,
                                        false /* don't throw on error, just return null */);
                    }
                }
                if (t != null)
                {
                    break;
                }
            }

            return t;
        }
#else
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

                Type factoryType = GetXamlPageFactoryType(pagePathAndName);

                if (factoryType == null)
                {
                    result.Exception = new InvalidOperationException(
                                        String.Format(
                                            CultureInfo.CurrentCulture,
                                            Resource.PageResourceContentLoader_NoXAMLWasFound,
                                            pagePathAndName));
                    return;
                }

                try
                {
                    result.Content = factoryType.GetMethod("Instantiate").Invoke(null, null);
                }
                catch (Exception ex)
                {
                    result.Exception = new InvalidOperationException(
                                            String.Format(
                                                CultureInfo.CurrentCulture,
                                                Resource.PageResourceContentLoader_XAMLWasUnloadable,
                                                pagePathAndName),
                                            ex);
                    return;
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

        private static Type GetXamlPageFactoryType(string pagePathAndName)
        {
            if (!GetXamlPagePath(pagePathAndName, out string assemblyPartSource, out string pagePathAndNameWithoutAssembly))
            {
                return null;
            }

            string factoryTypeName = GetXamlPageFactoryTypeName("/" + assemblyPartSource + UriParsingHelper.ComponentDelimiterWithoutSlash + pagePathAndNameWithoutAssembly);

            return Type.GetType(string.Concat(factoryTypeName, ", ", assemblyPartSource));
        }

        private static bool GetXamlPagePath(string pagePathAndName, out string assemblyPartSource, out string pagePathAndNameWithoutAssembly)
        {
            assemblyPartSource = null;
            pagePathAndNameWithoutAssembly = null;

            if (!pagePathAndName.Contains(UriParsingHelper.ComponentDelimiter))
            {
                assemblyPartSource = GetEntryPointAssemblyPartSource();
                pagePathAndNameWithoutAssembly = pagePathAndName;
            }
            else
            {
                string[] pagePathAndNameParts = pagePathAndName.Split(new string[] { UriParsingHelper.ComponentDelimiterWithoutSlash }, StringSplitOptions.RemoveEmptyEntries);
                if (pagePathAndNameParts.Length != 2)
                {
                    throw new InvalidOperationException(Resource.PageResourceContentLoader_InvalidComponentSyntax);
                }
                assemblyPartSource = pagePathAndNameParts[0];
                pagePathAndNameWithoutAssembly = pagePathAndNameParts[1];
            }

            if (String.IsNullOrEmpty(assemblyPartSource))
            {
                return false;
            }

            // In case the Uri contains international characters (ex: 完全采用统.xaml), we need to escape them.
            // because Application.GetResourceStream expects an escaped Uri.
            pagePathAndNameWithoutAssembly = Uri.EscapeUriString(pagePathAndNameWithoutAssembly);
            assemblyPartSource = Uri.EscapeUriString(assemblyPartSource);

            return true;
        }

        private static string GetXamlPageFactoryTypeName(string pagePath)
        {
            // Convert to TitleCase (so that when we remove the spaces, it is easily readable):
            string className = MakeTitleCase(pagePath);

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

        private static string MakeTitleCase(string str)
        {
            string result = "";
            string lowerStr = str.ToLower();
            int length = str.Length;
            bool makeUpper = true;
            int lastCopiedIndex = -1;
            /****************************
            * HOW THIS WORKS
            *
            * We go through all the characters of the string.
            * If any is not an alphanumerical character, we make the next alphanumerical character uppercase.
            * To do so, we copy the string (on which we call toLower) bit by bit into a new variable,
            * each bit being the part between two uppercase characters, and while inserting the
            * uppercase version of the character between each bit. then we add the end of the string.
            *****************************/

            for (int i = 0; i < length; ++i)
            {
                char ch = lowerStr[i];
                if ((ch >= 'a' && ch <= 'z') || (ch >= '0' && ch <= '0'))
                {
                    if (makeUpper && ch >= 'a' && ch <= 'z')
                    {
                        if (!(lastCopiedIndex == -1 && i == 0))
                        {
                            result += lowerStr.Substring(lastCopiedIndex + 1, i - lastCopiedIndex - 1);
                        }
                        result += (char)(ch - 32);
                        lastCopiedIndex = i;
                    }
                    makeUpper = false;
                }
                else
                {
                    makeUpper = true;
                }
            }

            if (lastCopiedIndex < length - 1)
            {
                result += str.Substring(lastCopiedIndex + 1);
            }
            return result;
        }
#endif

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
