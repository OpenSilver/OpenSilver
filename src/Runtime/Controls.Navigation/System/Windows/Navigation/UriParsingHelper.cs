//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Windows.Browser;

namespace System.Windows.Navigation
{
    internal static class UriParsingHelper
    {
#region Static fields and constants

        internal const string QueryStringDelimiter = "?";
        private const string ValueDelimiter = "=";
        private const string StatePairDelimiter = "&";
        private const string FragmentDelimiter = "#";
        private const string PathSeparator = "/";
        private const string HttpLocalhost = "http://localhost";
        private const string XamlExtension = ".xaml";

        private const string ExternalFragmentDelimiter = "$";
        private const string ExternalFragmentDelimiterPercentEncoded = "%24";

        internal const string ComponentDelimiter = ";component/";
        internal const string ComponentDelimiterWithoutSlash = ";component";
        internal static readonly int ComponentDelimiterWithoutSlashLength = ComponentDelimiterWithoutSlash.Length;

#endregion

#region Methods

#region Methods acting on internal Uris

        private static Uri MakeAbsolute(Uri baseUri)
        {
            if (baseUri == null || baseUri.OriginalString.StartsWith(PathSeparator, StringComparison.Ordinal))
            {
                return new Uri(HttpLocalhost + baseUri, UriKind.Absolute);
            }
            else
            {
                return new Uri(HttpLocalhost + PathSeparator + baseUri, UriKind.Absolute);
            }
        }

        private static Uri MakeAbsolute(string originalString)
        {
            if (originalString == null || originalString.StartsWith(PathSeparator, StringComparison.Ordinal))
            {
                return new Uri(HttpLocalhost + originalString, UriKind.Absolute);
            }
            else
            {
                return new Uri(HttpLocalhost + PathSeparator + originalString, UriKind.Absolute);
            }
        }

        private static string GetUriComponents(Uri uri, UriComponents components)
        {
            if (uri != null && String.IsNullOrEmpty(uri.OriginalString))
            {
                return String.Empty;
            }

            if (uri != null && uri.OriginalString.StartsWith(PathSeparator, StringComparison.Ordinal))
            {
                components |= UriComponents.KeepDelimiter;
            }

            return MakeAbsolute(uri).GetComponents(components, UriFormat.SafeUnescaped);
        }

        internal static string InternalUriToExternalValue(Uri uri)
        {
            Uri absoluteUri = MakeAbsolute(uri);

            UriComponents pathComponents = UriComponents.Path;
            if (uri != null && uri.OriginalString.StartsWith(PathSeparator, StringComparison.Ordinal))
            {
                pathComponents |= UriComponents.KeepDelimiter;
            }

            string path = absoluteUri.GetComponents(pathComponents, UriFormat.UriEscaped)
                                     .Replace(ExternalFragmentDelimiter[0].ToString(), ExternalFragmentDelimiterPercentEncoded);

            string query = absoluteUri.GetComponents(UriComponents.Query, UriFormat.UriEscaped)
                                      .Replace(ExternalFragmentDelimiter[0].ToString(), ExternalFragmentDelimiterPercentEncoded);

            string fragment = absoluteUri.GetComponents(UriComponents.Fragment, UriFormat.UriEscaped)
                                         .Replace(ExternalFragmentDelimiter[0].ToString(), ExternalFragmentDelimiterPercentEncoded);

            string final = path;
            if (!String.IsNullOrEmpty(query))
            {
                final += QueryStringDelimiter + query;
            }
            if (!String.IsNullOrEmpty(fragment))
            {
                final += ExternalFragmentDelimiter + fragment;
            }

            return final;
        }

        internal static string InternalUriFromExternalValue(string externalVal)
        {
            Uri externalValCleansed = MakeAbsolute(externalVal.Replace(ExternalFragmentDelimiter, FragmentDelimiter)
                                                              .Replace(ExternalFragmentDelimiterPercentEncoded, ExternalFragmentDelimiter));

            string finalString = externalValCleansed.GetComponents(UriComponents.PathAndQuery | UriComponents.Fragment, UriFormat.SafeUnescaped);

            if (!externalVal.StartsWith(PathSeparator, StringComparison.Ordinal))
            {
                // Strip off the leading "/" if the external value did not have one to begin with
                finalString = finalString.Substring(1);
            }

            return finalString;
        }
    
        internal static Uri InternalUriMerge(Uri baseUri, Uri newUri)
        {
            Guard.ArgumentNotNull(newUri, "newUri");

            if (baseUri == null)
            {
                baseUri = new Uri(String.Empty, UriKind.Relative);
            }

            Debug.Assert(!InternalUriIsFragment(baseUri), "Cannot merge URIs when the base Uri is only a fragment");

            // If the newUri is just a fragment, this is easy
            if (InternalUriIsFragment(newUri))
            {
                if (baseUri.OriginalString.StartsWith(PathSeparator, StringComparison.Ordinal))
                {
                    return new Uri(InternalUriGetAllButFragment(baseUri) + newUri.OriginalString, UriKind.Relative);
                }
                else
                {
                    // Account for the case when baseUri.OriginalString == String.Empty, which can happen
                    // when a Frame is initially loaded
                    string baseAllButFragment = InternalUriGetAllButFragment(baseUri);
                    if (!String.IsNullOrEmpty(baseAllButFragment))
                    {
                        baseAllButFragment = baseAllButFragment.Substring(1);
                    }

                    return new Uri(baseAllButFragment + newUri.OriginalString, UriKind.Relative);
                }
            }

            return newUri;
        }

        internal static bool InternalUriIsNavigable(Uri uri)
        {
            return uri != null &&
                   (InternalUriIsFragment(uri) ||  // Fragment uri or non-fragment uri with a xaml extension
                       ((InternalUriIsRelativeToAppRoot(uri) || InternalUriIsRelativeWithComponent(uri) || String.IsNullOrEmpty(uri.OriginalString)) &&
                        InternalUriHasXamlExtension(uri)));
        }

        internal static bool InternalUriHasXamlExtension(Uri uri)
        {
            string path = InternalUriGetPath(uri);
            if (path != null)
            {
                return path.EndsWith(XamlExtension, StringComparison.Ordinal);
            }
            return false;
        }

        internal static bool InternalUriIsRelativeToAppRoot(Uri uri)
        {
            return !uri.IsAbsoluteUri &&
                   uri.OriginalString.StartsWith(PathSeparator, StringComparison.Ordinal) && // If the OriginalString does not start with "/" then it is not relative to the app *root*
                   !uri.OriginalString.Contains(ComponentDelimiter);  // If the OriginalString contains ";component/" then it is not relative to the app root - it is relative to another assembly
        }

        internal static bool InternalUriIsRelativeWithComponent(Uri uri)
        {
            if (uri.OriginalString.Length < 1 ||
                uri.IsAbsoluteUri ||
                uri.OriginalString.StartsWith(PathSeparator, StringComparison.OrdinalIgnoreCase) == false) // If the Uri doesn't start with "/" then it's not relative in a manner we can use
            {
                return false;
            }

            // Copied directly from System.Windows.Application.IsComponentUri(Uri xamlUri)
            string str = uri.ToString();
            int startIndex = 0;

            if (str[0] == PathSeparator[0])
            {
                startIndex = 1;
            }

            int index = str.IndexOf(PathSeparator[0], startIndex);

            if ((index > 0) && str.Substring(startIndex, index - startIndex).EndsWith(ComponentDelimiterWithoutSlash, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Parses the Uri to determine if it is a fragment
        /// </summary>
        /// <param name="uri">The uri to parse</param>
        /// <returns>True if this Uri is a fragment, false if it is not</returns>
        internal static bool InternalUriIsFragment(Uri uri)
        {
            return uri != null &&
                   !uri.IsAbsoluteUri &&
                   !String.IsNullOrEmpty(uri.OriginalString) &&
                   uri.OriginalString.StartsWith(FragmentDelimiter, StringComparison.Ordinal);
        }

        /// <summary>
        /// Parses the Uri to retrieve the fragment, if present
        /// </summary>
        /// <param name="uri">The uri to parse</param>
        /// <returns>The fragment, or null if there is not one</returns>
        internal static string InternalUriGetFragment(Uri uri)
        {
            return MakeAbsolute(uri).GetComponents(UriComponents.Fragment, UriFormat.Unescaped);
        }

        /// <summary>
        /// Parses the Uri to strip off the fragment
        /// </summary>
        /// <param name="uri">The uri to parse</param>
        /// <returns>The uri without the fragment</returns>
        internal static string InternalUriGetAllButFragment(Uri uri)
        {
            return GetUriComponents(uri, UriComponents.PathAndQuery);
        }

        /// <summary>
        /// Parses the Uri to strip off the query string and the fragment
        /// </summary>
        /// <param name="uri">The uri to parse</param>
        /// <returns>The uri without the query string or the fragment</returns>
        internal static string InternalUriGetPath(Uri uri)
        {
            return GetUriComponents(uri, UriComponents.Path);
        }

        /// <summary>
        /// Parse the query string out of a Uri (the part following the '?')
        /// </summary>
        /// <param name="uri">The uri to parse for a query string</param>
        /// <returns>The query string, without a leading '?'.  Empty string in the case of no query string present.</returns>
        internal static string InternalUriGetQueryString(Uri uri)
        {
            return MakeAbsolute(uri).GetComponents(UriComponents.Query, UriFormat.SafeUnescaped);
        }

        /// <summary>
        /// Cut the query string off a given Uri, to process only the part before the '?', and strips off the fragment
        /// </summary>
        /// <param name="uri">The uri to parse</param>
        /// <returns>The uri without its query string, and without its fragment</returns>
        internal static string InternalUriGetBaseValue(Uri uri)
        {
            UriComponents components = UriComponents.Path;

            if (uri.OriginalString.StartsWith("/", StringComparison.Ordinal))
            {
                components |= UriComponents.KeepDelimiter;
            }

            return MakeAbsolute(uri).GetComponents(components, UriFormat.SafeUnescaped);
        }

        /// <summary>
        /// Parses the query string into name/value pairs
        /// </summary>
        /// <param name="uri">The Uri to parse the query string from</param>
        /// <param name="decodeResults">True if the resulting dictionary should contain decoded values, false if not</param>
        /// <returns>A dictionary containing one entry for each name/value pair in the query string</returns>
        internal static IDictionary<string, string> InternalUriParseQueryStringToDictionary(Uri uri, bool decodeResults)
        {
            IDictionary<string, string> dict = new Dictionary<string, string>(StringComparer.Ordinal);

            string[] kvps = InternalUriGetQueryString(uri).Split(StatePairDelimiter.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (string kvp in kvps)
            {
                int delimiterIndex = kvp.IndexOf(ValueDelimiter, StringComparison.Ordinal);
                if (delimiterIndex == -1)
                {
                    dict.Add(
                        decodeResults ? HttpUtility.UrlDecode(kvp)
                                      : kvp,
                        String.Empty);
                }
                else
                {
                    dict.Add(
                        decodeResults ? HttpUtility.UrlDecode(kvp.Substring(0, delimiterIndex))
                                      : kvp.Substring(0, delimiterIndex),
                        decodeResults ? HttpUtility.UrlDecode(kvp.Substring(delimiterIndex + 1))
                                      : kvp.Substring(delimiterIndex + 1));
                }
            }

            return dict;
        }

        internal static Uri InternalUriCreateWithQueryStringValues(string uriBase, IDictionary<string, string> queryStringValues, string fragment)
        {
            StringBuilder sb = new StringBuilder(200);
            sb = sb.Append(uriBase);

            if (queryStringValues.Count > 0)
            {
                sb = sb.Append(QueryStringDelimiter);

                foreach (string key in queryStringValues.Keys)
                {
                    sb = sb.AppendFormat(CultureInfo.InvariantCulture,
                                         "{0}{1}{2}{3}",
                                         key,
                                         ValueDelimiter[0],
                                         queryStringValues[key],
                                         StatePairDelimiter[0]);
                }

                // Strip off the last delimiter between internal state pairs
                sb = sb.Remove(sb.Length - 1, 1);
            }

            if (!String.IsNullOrEmpty(fragment))
            {
                sb.AppendFormat(CultureInfo.InvariantCulture,
                                "{0}{1}",
                                FragmentDelimiter,
                                fragment);
            }

            return new Uri(sb.ToString(), UriKind.Relative);
        }

#endregion

#endregion
    }
}
