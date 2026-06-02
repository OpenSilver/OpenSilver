
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

namespace System.Windows.Controls;

internal class AccessText : FrameworkElement
{
    internal const char AccessKeyMarker = '_';

    // Returns the index of _ marker.
    // _ can be escaped by double _
    private static int FindAccessKeyMarker(string text)
    {
        int lenght = text.Length;
        int startIndex = 0;
        while (startIndex < lenght)
        {
            int index = text.IndexOf(AccessKeyMarker, startIndex);
            if (index == -1)
            {
                return -1;
            }

            // If next char exist and different from _
            if (index + 1 < lenght && text[index + 1] != AccessKeyMarker)
            {
                return index;
            }

            startIndex = index + 2;
        }

        return -1;
    }

    internal static string RemoveAccessKeyMarker(string text)
    {
        if (!string.IsNullOrEmpty(text))
        {
            string accessKeyMarker = AccessKeyMarker.ToString();
            string doubleAccessKeyMarker = accessKeyMarker + accessKeyMarker;
            int index = FindAccessKeyMarker(text);
            if (index >= 0 && index < text.Length - 1)
            {
                text = text.Remove(index, 1);
            }

            // Replace double _ with single _
            text = text.Replace(doubleAccessKeyMarker, accessKeyMarker);
        }
        return text;
    }
}
