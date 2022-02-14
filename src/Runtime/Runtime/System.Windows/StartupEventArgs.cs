

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
using System.Security;
using System.Windows.Browser;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// Contains the event data for the Application.Startup event.
    /// </summary>
    public sealed partial class StartupEventArgs : EventArgs
    {
        #region Useful stuff to get the InitParams from the query string (Note: contains the static constructor for this class)
        private const string InitParamsKey = "InitParams";
        private static readonly IDictionary<string, string> Query;

        static StartupEventArgs()
        {
            Query = HtmlPage.Document.QueryString;
        }

        /// <summary>
        /// Gets the InitParams from the Query string. We do it this way as a workaround because I don't think there is a decent way of accessing the .aspx file.
        /// </summary>
        /// <returns>A Dictionary containing the keys and their values defined in the "InitParams" query parameter.</returns>
        private static Dictionary<string, string> GetInitParams()
        {
            //Getting the query parameters:
            Dictionary<string, string> queryParameters = Query.ToDictionary(entry => entry.Key,
                                                                            entry => HttpUtility.UrlDecode(entry.Value));

            Dictionary<string, string> initParameters = new Dictionary<string, string>();
            // If the query parameters contain a parameter with the key defined in InitParamsKey, it is the one that contains whet we want, so we add them to our return value:
            if (queryParameters.ContainsKey(InitParamsKey))
            {
                // Note: In the .aspx file in Silverlight, the parameters were written in the form: "key1=value1,key2=value2" so we keep that syntax in mind when retrieving the parameters.

                string initParamsAsString = queryParameters[InitParamsKey];
                string[] splittedInitParams = initParamsAsString.Split(',');
                foreach (string fullParam in splittedInitParams)
                {
                    string[] splittedParam = fullParam.Split('=');
                    if (splittedParam.Length == 2) //normal case
                    {
                        initParameters[splittedParam[0]] = splittedParam[1];
                    }
                    else if (splittedParam.Length == 1) //case of an empty string as a value:
                    {
                        initParameters[fullParam] = "";
                    }
                    //else I don't know what we should do?
                    //      One possibility would be to consider that it is unlikely for the key to contain '=' so we just take the first part as the key and the rest as the value
                    //      Another possibility would be to make them use '"' to differentiate the key from the value (i.e: someKey=stillKey=someValue=stillValue should be: "someKey=stillKey"="someValue=stillValue"
                }
            }
            return initParameters;
        }
        #endregion

        // Summary:
        //     Gets the initialization parameters that were passed as part of HTML initialization
        //     of a Silverlight plug-in.
        //
        // Returns:
        //     The set of initialization parameters, as a dictionary with key strings and
        //     value strings.
        /// <summary>
        /// BEHAVIOUR DIFFERS FROM SILVERLIGHT, SEE NOTE.
        /// Gets the initialization parameters that were passed in the query string.
        /// Note: This currently does not get the values from the .aspx file like in Silverlight.
        /// Instead, it gets the values from the parameters in the QueryString so in order to use this property, you need to add the parameter InitParams with the keys and values afterwards.
        /// For example, you would change your url from MyApp.com to MyApp.com?InitParams=key1%3Dvalue1,key2%3Dvalue2. %3D is the escaped value of the equal sign.
        /// </summary>
        public IDictionary<string, string> InitParams
        {
            get { return GetInitParams(); }
        }
    }
}
