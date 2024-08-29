
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

using System.Collections;
using System.Windows;
using System.Windows.Controls.Primitives;

//
// Important: do not rename this class without updating the Simulator as well!
// The class is called via reflection from the Simulator.
//

namespace DotNetForHtml5.Core;

internal static class PopupsManager
{
    // IMPORTANT: This is called via reflection from the "Visual Tree Inspector" of the Simulator.
    // If you rename or remove it, be sure to update the Simulator accordingly!
    public static IEnumerable GetAllRootUIElements()
    {
        // Include the main window:
        yield return Window.Current;

        // And all the popups:
        foreach (PopupRoot popupRoot in PopupRoot.GetActivePopupRoots())
        {
            yield return popupRoot;
        }
    }
}
