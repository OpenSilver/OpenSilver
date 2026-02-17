
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

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace OpenSilver.Internal.Controls.Primitives;

internal static class PopupService
{
    /// <summary>
    /// Place the Popup relative to this point 
    /// </summary>
    internal static Point MousePosition { get; private set; }

    internal static HashSet<PopupRoot> ActivePopups { get; } = [];

    internal static void UpdateMousePosition(MouseEventArgs e) => MousePosition = e.GetPosition(null);

    internal static void HandleMouseButton()
    {
        if (ActivePopups.Count == 0)
        {
            return;
        }

        // Note: If a popup has StayOpen=True, the value of "StayOpen" of its parents is ignored.
        // In other words, the parents of a popup that has StayOpen=True will always stay open
        // regardless of the value of their "StayOpen" property.

        var popupRoots = GetActivePopups();

        for (int i = 0; i < popupRoots.Length; i++)
        {
            ref var item = ref popupRoots[i];
            PopupRoot popupRoot = item.PopupRoot;

            if (popupRoot is null || popupRoot.Popup is not Popup popup)
            {
                item.PopupRoot = null;
                continue;
            }

            if (popup.StayOpen)
            {
                item.Close = false;

                for (popup = popup.ParentPopup; popup is not null; popup = popup.ParentPopup)
                {
                    int index = IndexOf(popupRoots, popupRoot);
                    if (index == -1)
                    {
                        break;
                    }

                    popupRoots[index].Close = false;
                }
            }
        }

        foreach ((PopupRoot popupRoot, bool close) in popupRoots)
        {
            if (popupRoot is null)
            {
                continue;
            }

            popupRoot.Popup.OnOutsideClick(close);
        }

        static (PopupRoot PopupRoot, bool Close)[] GetActivePopups()
        {
            var popupRoots = new (PopupRoot PopupRoot, bool Close)[ActivePopups.Count];

            int i = 0;
            foreach (var popupRoot in ActivePopups)
            {
                popupRoots[i] = (popupRoot, true);
                i++;
            }

            return popupRoots;
        }

        static int IndexOf((PopupRoot PopupRoot, bool Close)[] array, PopupRoot popupRoot)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].PopupRoot == popupRoot)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
