

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

#if MIGRATION
namespace System.Windows.Input
#else
namespace Windows.UI.Xaml.Input
#endif
{
    // Summary:
    //     Specifies the tabbing behavior across tab stops for a Silverlight tabbing
    //     sequence within a container.
    public enum KeyboardNavigationMode
    {
        // Summary:
        //     Tab indexes are considered on the local subtree only inside this container.
        Local = 0,
        //
        // Summary:
        //     Focus returns to the first or the last keyboard navigation stop inside of
        //     a container when the first or last keyboard navigation stop is reached.
        Cycle = 1,
        //
        // Summary:
        //     The container and all of its child elements as a whole receive focus only
        //     once.
        Once = 2,
    }
}