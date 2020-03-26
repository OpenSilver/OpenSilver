

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
using System.IO;
using System.Linq;
#if CSHTML5BLAZOR
using System.Net.Http;
#else
using System.Net;
#endif
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

#if !CSHTML5BLAZOR
namespace System
{
    /// <summary>
    /// Gives access to methods to help using WebRequests.
    /// </summary>
    internal class INTERNAL_WebRequestHelper_SimulatorOnly
    {
        /// <summary>
        /// Initializes a new instance of the INTERNAL_WebRequestHelper class.
        /// </summary>
        public INTERNAL_WebRequestHelper_SimulatorOnly() { }

        /// <summary>
        /// Makes a synchronous or asynchronous request at the specified uri, using the specified method, with the specified headers and body, and calls the callbackMethod.
        /// </summary>
        /// <param name="address">the uri that identifies the Internet resource.</param>
        /// <param name="headers">
        /// A dictionary containing the headers to put in the method.
        /// The pairs key/values in the dictionary correspond to the pairs key/value in the headers.
        /// </param>
        /// <param name="body">The body of the request.</param>
        /// <param name="callbackMethod">The method to be called after the request has been made.</param>
        /// <returns>The result of the request as a string.</returns>
        public string MakeRequest_CSharpVersion(Uri address, Dictionary<string, string> headers, string body)
        {
            WebClientWithCredentials wc = new WebClientWithCredentials(Application.Current.Host.Settings.DefaultSoapCredentialsMode);
            foreach (KeyValuePair<string, string> header in headers)
            {
                wc.Headers.Add(header.Key, header.Value);
            }

            try
            {
                return wc.UploadString(address, "POST", body);
            }
            catch (WebException ex)
            {
                if (ex.InnerException == null)
                {
                    // Get details about the error (the FaultException...):
                    string response = null;
                    try 
	                {	        
                        response = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
	                }
	                catch
	                {
	                }
                    if (response != null)
                    {
                        string exceptionMessage =
                            ex.Message.Replace("\r\n", "").Replace("\n", "").Replace("\r", "")
                            + " " + "ERROR DETAILS: " + response
                            + Environment.NewLine
                            + Environment.NewLine
                            + "SOAP ADDRESS:" + " " + address.ToString()
                            + Environment.NewLine
                            + Environment.NewLine
                            + "SOAP BODY:" + Environment.NewLine + body
                            + Environment.NewLine
                            + Environment.NewLine
                            + "Please refer to the InnerException for the original WebException.";
                        throw new WebException(exceptionMessage, ex);
                    }
                    else
                        throw;
                }
                else
                    throw;
            }
        }

        public void MakeRequestAsync_CSharpVersion(Uri address, Dictionary<string, string> headers, string body, UploadStringCompletedEventHandler callbackMethod)
        {
            WebClientWithCredentials wc = new WebClientWithCredentials(Application.Current.Host.Settings.DefaultSoapCredentialsMode);
            foreach (KeyValuePair<string, string> header in headers)
            {
                wc.Headers.Add(header.Key, header.Value);
            }

            wc.UploadStringCompleted -= callbackMethod;
            wc.UploadStringCompleted += callbackMethod;

            wc.UploadStringAsync(address, "POST", body);
        }
    }
}
#endif