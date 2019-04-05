
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



using CSHTML5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net
{

    // it is impossible to override the webclient methods from the c# version, so WebClientWithCredentials became
    // a class that can handle both behaviours, with and without credentials. just set CredentialsMode to Auto.
    public class WebClientWithCredentials : WebClient
    {


        private class CookiesContainer
        {
            // the last ASP.NET session id changes at each response from the server.
            // for security reasons and to respect the XHR request format, this ID is added in the cookies list in each request.
            public string LastSessionId { get; set; }

            HashSet2<string> _cookies = new HashSet2<string>();

            public HashSet2<string> Cookies
            {
                get { return _cookies; }
            }

            // get the asynchronous response for a specific address
            internal void INTERNAL_OnAsyncResponse(object sender, UploadStringCompletedEventArgs e)
            {
                WebClientWithCredentials wc = (WebClientWithCredentials)sender; // can't be another type

                wc.INTERNAL_AddNewCookiesInContainer_SimulatorOnly(this);
            }
        }



        // Note: most of the methods here are "Simulator Only" because in JavaScript all we do is set "withCredentials" to true on the Xhr.

        static Dictionary<string, CookiesContainer> containerDictionary_SimulatorOnly = new Dictionary<string, CookiesContainer>();

        static CookiesContainer GetContainer(string address)
        {
            if (!containerDictionary_SimulatorOnly.ContainsKey(address))
            {
                containerDictionary_SimulatorOnly.Add(address, new CookiesContainer());
            }

            return containerDictionary_SimulatorOnly[address];
        }


        CredentialsMode _credentialsMode;
        public CredentialsMode CredentialsMode
        {
            get { return _credentialsMode; }
            set
            {
                _credentialsMode = value;
            }
        }

        public WebClientWithCredentials(CredentialsMode credentialsMode = CredentialsMode.Auto)
        {
            CredentialsMode = credentialsMode;
        }


        void INTERNAL_AddNewCookiesInContainer_SimulatorOnly(CookiesContainer container)
        {
            if (Interop.IsRunningInTheSimulator)
            {
                // never execute this code if not in simulator

                WebHeaderCollection myWebHeaderCollection = base.ResponseHeaders;

                if (myWebHeaderCollection != null) // the headers can be null if the response has crashed for some reason
                {
                    string allowOrigin = myWebHeaderCollection.Get("Access-Control-Allow-Origin");
                    bool wrongAccessOrigin = (allowOrigin == "*" || allowOrigin == string.Empty || allowOrigin == null);

                    // if wrongAccessOrigin, we assume that credentials are not supported. Less restrictions in simulator than JS 
                    if (!wrongAccessOrigin)
                    {
                        // in javascript, this will always be null, but cookies are handled in another way
                        string Header = myWebHeaderCollection.Get("Set-Cookie");

                        if (Header != null)
                        {
                            string[] cookies = Header.Split(',');

                            foreach (string cookie in cookies)
                            {
                                // normally the container is an HashSet, so we don't need to check if the cookie is already in it.
                                if (cookie.Contains("ASP.NET")) // the .NET id cookie
                                    container.LastSessionId = cookie;
                                else
                                    container.Cookies.Add(cookie);
                            }
                        }
                    }
                    // else does not mean error, it just means that credentials are not supported on this response
                }
            }
        }


        // add an origin for the request in the simulator ONLY to simulate the browser behaviour
        void INTERNAL_addFakeOrigin_SimulatorOnly()
        {
            if (CSHTML5.Interop.IsRunningInTheSimulator)
            {
                base.Headers.Add("Origin", "http://cshtml5-Simulator");
            }
        }


        void INTERNAL_AddCookiesInRequest_SimulatorOnly(string address)
        {
            // in javascript, cookiesContainer will always be empty, but cookies are handled in another way
            CookiesContainer container = GetContainer(address);

            foreach (string cookie in container.Cookies)
            {
                base.Headers.Add("cookie", cookie);
            }

            // as XHR, we put the last session ID in the cookies list
            if (container.Cookies.Count > 0 && container.LastSessionId != null && container.LastSessionId != string.Empty)
                base.Headers.Add("cookie", container.LastSessionId);

        }

        void INTERNAL_AddHeaders_SimulatorOnly(string address)
        {
            INTERNAL_addFakeOrigin_SimulatorOnly();
            INTERNAL_AddCookiesInRequest_SimulatorOnly(address);
        }


        public new void UploadStringAsync(Uri address, string data)
        {
            UploadStringAsync(address, null, data, null);
        }


        public new void UploadStringAsync(Uri address, string method, string data)
        {
            UploadStringAsync(address, method, data, null);
        }

        public new void UploadStringAsync(Uri address, string method, string data, object userToken)
        {
            if (CredentialsMode != CredentialsMode.Disabled)
                INTERNAL_AddHeaders_SimulatorOnly(address.OriginalString);

            if (Interop.IsRunningInTheSimulator)
                base.UploadStringCompleted += GetContainer(address.OriginalString).INTERNAL_OnAsyncResponse;

            base.UploadStringAsync(address, method, data);
        }

        public new string UploadString(string address, string data)
        {
            return UploadString(new Uri(address), null, data);
        }

        public new string UploadString(string address, string method, string data)
        {
            return UploadString(new Uri(address), method, data);
        }

        public new string UploadString(Uri address, string data)
        {
            return UploadString(address, null, data);
        }

        public new string UploadString(Uri address, string method, string data)
        {
            if (CredentialsMode != CredentialsMode.Disabled)
                INTERNAL_AddHeaders_SimulatorOnly(address.OriginalString);

            string response = base.UploadString(address, method, data);

            if (CredentialsMode != CredentialsMode.Disabled)
                INTERNAL_AddNewCookiesInContainer_SimulatorOnly(GetContainer(address.OriginalString));

            return response;
        }


        // For the javascript version, check WebClient.cs to really see the implemented features

    }
}
