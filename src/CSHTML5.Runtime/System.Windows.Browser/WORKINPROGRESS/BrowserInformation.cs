

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

#if WORKINPROGRESS

namespace System.Windows.Browser
{
    public sealed partial class BrowserInformation
    {
        //
        // Summary:
        //     Gets the version of the browser technology that the current browser is based
        //     on.
        //
        // Returns:
        //     The version of the underlying browser technology.
        public Version BrowserVersion { get; private set; }
        //
        // Summary:
        //     Gets a value that indicates whether the browser supports cookies.
        //
        // Returns:
        //     true if the browser supports cookies; otherwise, false.
        public bool CookiesEnabled { get; private set; }
        //
        // Summary:
        //     Gets the name of the browser technology that the current browser is based on.
        //
        // Returns:
        //     The name of the underlying browser technology.
        public string Name { get; private set; }
        //
        // Summary:
        //     Gets the name of the browser operating system.
        //
        // Returns:
        //     The name of the operating system that the browser is running on.
        public string Platform { get; private set; }
        //
        // Summary:
        //     Gets the product name of the browser.
        //
        // Returns:
        //     The product name of the browser.
        public string ProductName { get; private set; }
        //
        // Summary:
        //     Gets the product version number of the browser.
        //
        // Returns:
        //     The product version number of the browser.
        public string ProductVersion { get; private set; }
        //
        // Summary:
        //     Gets the user agent string of the browser.
        //
        // Returns:
        //     The user agent string that identifies the browser.
        public string UserAgent { get; private set; }

    }
}

#endif