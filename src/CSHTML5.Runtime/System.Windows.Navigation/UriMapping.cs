

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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
#if !MIGRATION
using Windows.UI.Xaml;
#endif

#if MIGRATION
namespace System.Windows.Navigation
#else
namespace Windows.UI.Xaml.Navigation
#endif
{
    /// <summary>
    /// Defines the pattern for converting a requested uniform resource identifier
    /// (URI) into a new URI.
    /// </summary>
    public sealed partial class UriMapping : DependencyObject
    {
        ///// <summary>
        ///// Initializes a new instance of the System.Windows.Navigation.UriMapping class.
        ///// </summary>
        //public UriMapping();

        ///// <summary>
        ///// Gets or sets the uniform resource identifier (URI) that is navigated to instead
        ///// of the originally requested URI.
        ///// </summary>
        //public Uri MappedUri
        //{
        //    get;
        //    set;
        //}


        /// <summary>
        /// Gets or sets the uniform resource identifier (URI) that is navigated to instead
        /// of the originally requested URI.
        /// </summary>
        public Uri MappedUri //Note: this wasn't originally a dependencyProperty
        {
            get { return (Uri)GetValue(MappedUriProperty); }
            set { SetValue(MappedUriProperty, value); }
        }

        /// <summary>
        /// Identifies the System.Windows.Navigation.UriMapping.MappedUri dependency property
        /// </summary>
        public static readonly DependencyProperty MappedUriProperty =
            DependencyProperty.Register("MappedUri", typeof(Uri), typeof(UriMapping), new PropertyMetadata(null)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });




        //Uri _uri;
        ///// <summary>
        ///// Gets or sets the pattern to match when determining if the requested uniform
        ///// resource identifier (URI) is converted to a mapped URI.
        ///// </summary>
        //public Uri Uri
        //{
        //    get { return _uri; }
        //    set
        //    {
        //        Regex r = new Regex("{[^}]*}", RegexOptions.ECMAScript); //this should be read as '{' followed by anything but '}', then '}'
        //        _groupNames = new List<string>();
        //        foreach(Match match in r.Matches(value.OriginalString))
        //        {
        //            _groupNames.Add(match.Value);
        //        }
        //        _regularExpressionToApplyOnUriToMap = r.Replace(value.OriginalString, "(.*)"); //whatever is needed to say "anything".
        //        _uri = value;
        //    }
        //}


        /// <summary>
        /// Gets or sets the pattern to match when determining if the requested uniform
        /// resource identifier (URI) is converted to a mapped URI.
        /// </summary>
        public Uri Uri //Note: this wasn't originally a dependencyProperty
        {
            get { return (Uri)GetValue(UriProperty); }
            set { SetValue(UriProperty, value); }
        }
        /// <summary>
        /// Identifies the System.Windows.Navigation.UriMapping.Uri dependency property
        /// </summary>
        public static readonly DependencyProperty UriProperty =
            DependencyProperty.Register("Uri", typeof(Uri), typeof(UriMapping), new PropertyMetadata(null, Uri_Changed)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        private static void Uri_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UriMapping mapping = (UriMapping)d;
            Uri newValue = (Uri)e.NewValue;
#if BRIDGE
            Regex r = new Regex("{[^}]*}"); //this should be read as '{' followed by anything but '}', then '}'
#else
            Regex r = new Regex("{[^}]*}", RegexOptions.ECMAScript); //this should be read as '{' followed by anything but '}', then '}'
#endif
            mapping._groupNames = new List<string>();
            foreach (Match match in r.Matches(newValue.OriginalString))
            {
                mapping._groupNames.Add(match.Value);
            }
            mapping._regularExpressionToApplyOnUriToMap = r.Replace(newValue.OriginalString, "(.*)"); //whatever is needed to say "anything".
        }



        string _regularExpressionToApplyOnUriToMap = "";
        List<string> _groupNames = null;

        // Exceptions:
        //   System.InvalidOperationException:
        //     System.Windows.Navigation.UriMapping.Uri is null.-or-System.Windows.Navigation.UriMapping.MappedUri
        //     is null.-or-System.Windows.Navigation.UriMapping.Uri includes a query string.-or-System.Windows.Navigation.UriMapping.Uri
        //     includes a content fragment.
        /// <summary>
        /// Converts the specified uniform resource identifier (URI) to a new URI, if
        /// the specified URI matches the defined template for converting.
        /// </summary>
        /// <param name="uri">The URI to convert.</param>
        /// <returns>The URI that has been converted or null if the URI cannot be converted.</returns>
        public Uri MapUri(Uri uri)
        {
            if (Uri == null || string.IsNullOrWhiteSpace(Uri.OriginalString))
            {
                throw new InvalidOperationException("this UriMapping's Uri is null");
            }
            if (MappedUri == null || string.IsNullOrWhiteSpace(MappedUri.OriginalString))
            {
                throw new InvalidOperationException("this UriMapping's MappedUri is null");
            }

#if BRIDGE
            Regex reg = new Regex(_regularExpressionToApplyOnUriToMap);
#else
            Regex reg = new Regex(_regularExpressionToApplyOnUriToMap, RegexOptions.ECMAScript);
#endif
            MatchCollection matches = reg.Matches(uri.OriginalString);
            Dictionary<string, string> groupNameToReplacement = new Dictionary<string,string>();
            string result = MappedUri.OriginalString;

            foreach (Match match in matches)
            {
                int i = 0;
                GroupCollection groups = match.Groups;
                if (groups.Count == _groupNames.Count + 1) //+1 because it adds one group with the whole match
                {
                    bool isFirst = true;
                    foreach (Group group in match.Groups)
                    {
                        if (!isFirst)
                        {
                            result = result.Replace(_groupNames.ElementAt(i), group.Value);
                            ++i;
                        }
                        else
                        {
                            isFirst = false;
                        }
                    }

                    return new Uri(result, UriKind.RelativeOrAbsolute);
                }
                else
                {
                    return null;
                }
            }
            return null;
            
            //if (uri != null)
            //{
            //    string uriString = uri.OriginalString;

            //    //check if the uri fits the pattern to map:
            //    //todo: use a regex instead of what I'm doing...

            //    Dictionary<string, string> groupNameToValue = new Dictionary<string, string>();
            //    for (int currentIndexInPatternUri = 0; currentIndexInPatternUri < _splittedUri.Length; ++currentIndexInPatternUri)
            //    {
            //        string str = _splittedUri[currentIndexInPatternUri];
            //        if (currentIndexInPatternUri % 2 == 0) //index is even --> we check if the string matches
            //        {
            //            if (uriString.StartsWith(str)) //it fits, we move on
            //            {
            //                uriString = uriString.Substring(str.Length);
            //            }
            //            else
            //            {
            //                return null;
            //            }
            //        }
            //        else
            //        {
            //            //we try to find the next part of the pattern uri

            //            //we should probably make that recursive so that if the part to put in the group contains the next part
            //        }
            //        ++currentIndexInPatternUri;
            //    }
            //}
            //else
            //{
            //    return null;
            //}
        }


        //Tuple<string, Dictionary<string, string>> FindUriPart(string uriRemainingPart, int splittedPatternCurrentIndex)
        //{
        //    Dictionary<string, string> groupNameToValue = new Dictionary<string, string>();

        //    string str = _splittedUri[splittedPatternCurrentIndex];
        //    if (uriRemainingPart.StartsWith(str)) //it fits, we move on
        //    {
        //        uriRemainingPart = uriRemainingPart.Substring(str.Length);
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //    ++splittedPatternCurrentIndex;
        //    if(splittedPatternCurrentIndex < _splittedUri.Length)
        //    {
        //        string currentGroupName = _splittedUri[splittedPatternCurrentIndex];
        //        ++splittedPatternCurrentIndex;
        //        if(splittedPatternCurrentIndex < _splittedUri.Length)
        //        {
        //            string nextFixedPart = _splittedUri[splittedPatternCurrentIndex];
        //            if(string.IsNullOrWhiteSpace(nextFixedPart))
        //            {
        //                groupNameToValue.Add(currentGroupName, uriRemainingPart); //todo: if splittedPatternCurrentIndex < splittedUri.Length
        //            }
        //        }
        //        else //I don't think it's possible to enter this else but I'd rather take one line to handle it than have problems afterwards:
        //        {
        //            groupNameToValue.Add(currentGroupName, uriRemainingPart);
        //        }
        //        //we try to find the next part of the pattern uri
        //        Tuple<string, Dictionary<string, string>> res = 
        //    }
        //}

    }
}
