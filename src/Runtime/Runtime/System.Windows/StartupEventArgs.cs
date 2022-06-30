﻿

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
using System.Text.Json;
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
        private static Dictionary<string, string> InitParamsDict;
        private class InitParam
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        static StartupEventArgs()
        {
        }

        /// <summary>
        /// Gets the InitParams from the Query string. We do it this way as a workaround because I don't think there is a decent way of accessing the .aspx file.
        /// </summary>
        /// <returns>A Dictionary containing the keys and their values defined in the "InitParams" query parameter.</returns>
        private static Dictionary<string, string> GetInitParamsFromQuery(IDictionary<string, string> initQueryParameters)
        {
            //Getting the query parameters:
            Dictionary<string, string> queryParameters = initQueryParameters.ToDictionary(entry => entry.Key.ToLower(),
                                                                            entry => HttpUtility.UrlDecode(entry.Value));

            Dictionary<string, string> initParameters = new Dictionary<string, string>();
            // If the query parameters contain a parameter with the key defined in InitParamsKey, it is the one that contains whet we want, so we add them to our return value:
            if (queryParameters.ContainsKey(InitParamsKey.ToLower()))
            {
                // Note: In the .aspx file in Silverlight, the parameters were written in the form: "key1=value1,key2=value2" so we keep that syntax in mind when retrieving the parameters.

                string initParamsAsString = queryParameters[InitParamsKey];
                string[] splittedInitParams = initParamsAsString.Split(',');
                foreach (string fullParam in splittedInitParams)
                {
                    int index = fullParam.IndexOf('=');
                    if (index == -1)
                    {
                        initParameters[fullParam] = string.Empty;
                    }
                    else
                    {
                        initParameters[fullParam.Substring(0, index)] = fullParam.Substring(index + 1);
                    }
                }
            }
            return initParameters;
        }

        private static void ReadAllInitParams()
        {
            if (InitParamsDict != null) //Already read them
                return;

            var paramArrayJson = OpenSilver.Interop.ExecuteJavaScript("document.getInitParams()");
            var paramArry = JsonSerializer.Deserialize(paramArrayJson.ToString(), typeof(List<InitParam>)) as List<InitParam>;

            InitParamsDict = new Dictionary<string, string>();

            foreach (var param in paramArry)
            {
                InitParamsDict.Add(param.Name, param.Value);
            }

            var queryParams = HtmlPage.Document.QueryString;
            if (queryParams == null)
                return;

            // Query params will override ones with same name coming from the index page param tags
            queryParams = GetInitParamsFromQuery(queryParams);
            foreach (var param in queryParams)
            {
                if (InitParamsDict.ContainsKey(param.Key))
                    InitParamsDict[param.Key] = param.Value;
                else
                    InitParamsDict.Add(param.Key, param.Value);
            }
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
            get
            {
                ReadAllInitParams();
                return InitParamsDict;
            }
        }
    }
}
