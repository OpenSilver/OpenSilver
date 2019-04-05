
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
using System.Windows;
#else
using Windows.UI.Xaml;
#endif


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
