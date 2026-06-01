
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

using OpenSilver;
using System.Windows.Markup;

namespace System.Windows.Controls;

/// <summary>
/// Specifies with an underscore the character that is used as the access key.
/// </summary>
[ContentProperty(nameof(Text))]
[NotImplemented]
public class AccessText : TextBlock // FrameworkElement, IAddChild
{
    // Defines the character to be used in front of the access key
    internal const char AccessKeyMarker = '_';

    // Cached character that immediately followed the (first non-escaped) underscore in the
    // most recently assigned value, before stripping.
    private char _accessKey;

    static AccessText()
    {
        TextProperty.OverrideMetadata(
            typeof(AccessText),
            new FrameworkPropertyMetadata(
                string.Empty,
                FrameworkPropertyMetadataOptions.AffectsMeasure,
                null,
                CoerceAccessText));
    }

    /// <summary>
    /// Provides read-only access to the character that follows the first underline character.
    /// </summary>
    public char AccessKey => _accessKey;

    private static object CoerceAccessText(DependencyObject d, object baseValue)
    {
        var accessText = (AccessText)d;
        return accessText.ParseAccessKey((string)baseValue);
    }

    private string ParseAccessKey(string text)
    {
        _accessKey = '\0';

        if (string.IsNullOrEmpty(text))
            return string.Empty;

        int markerIndex = FindAccessKeyMarker(text);
        if (markerIndex >= 0 && markerIndex + 1 < text.Length)
        {
            _accessKey = text[markerIndex + 1];
        }

        return RemoveAccessKeyMarker(text);
    }

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
