
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
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CSHTML5.Internal;

namespace OpenSilver.Internal.Media;

internal sealed class FontFace
{
    private static readonly Dictionary<string, FontFace> _fontFacesCache = new();
    private static readonly Dictionary<string, string> _fontSourceToFamilyCache = new();
    private static readonly ReferenceIDGenerator _idGenerator = new();
    private static string _defaultCssFontFamily;

    private TaskCompletionSource<bool> _loadOperation;
    private List<WeakReference<UIElement>> _measureList;

    internal static string DefaultCssFontFamily
    {
        get
        {
            if (_defaultCssFontFamily is null)
            {
                var rootDiv = Application.Current?.GetRootDiv();
                if (rootDiv is not null)
                {
                    string sDiv = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(rootDiv);
                    _defaultCssFontFamily = Interop.ExecuteJavaScriptString(
                        $"window.getComputedStyle({sDiv}).getPropertyValue('font-family');");
                }
            }

            return _defaultCssFontFamily ?? string.Empty;
        }
    }

    internal static FontFace GetFontFace(string source, UIElement relativeTo)
    {
        (string fontName, string fontUrl) = ParseFontSource(source, relativeTo);

        if (!_fontFacesCache.TryGetValue(fontName, out FontFace face))
        {
            face = new FontFace(fontName, fontUrl);
            _fontFacesCache.Add(fontName, face);
        }
        
        return face;
    }

    private FontFace(string fontName, string fontUrl)
    {
        CssFontName = fontName;
        CssFontUrl = fontUrl;
        IsLoaded = string.IsNullOrEmpty(fontUrl);
    }

    public string CssFontName { get; }

    public string CssFontUrl { get; }

    public bool IsLoaded { get; private set; }

    internal void RegisterForMeasure(UIElement uie)
    {
        if (IsLoaded) return;

        _measureList ??= new List<WeakReference<UIElement>>();
        _measureList.Add(new WeakReference<UIElement>(uie));
    }

    internal async Task<bool> LoadAsync()
    {
        if (IsLoaded) return true;

        if (_loadOperation is null)
        {
            _loadOperation = new TaskCompletionSource<bool>();

            var loadCallback = JavaScriptCallbackHelper.CreateSelfDisposedJavaScriptCallback<bool>(OnFontLoaded);

            string uriEscaped = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(CssFontUrl);
            string sCallback = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(loadCallback);
            Interop.ExecuteJavaScriptVoid(
                $"document.loadFont('{CssFontName}', 'url({uriEscaped})', {sCallback});");
        }

        return await _loadOperation.Task;
    }

    private void OnFontLoaded(bool success)
    {
        IsLoaded = true;
        _loadOperation?.TrySetResult(success);
        _loadOperation = null;

        if (success)
        {
            if (_measureList is null)
            {
                return;
            }

            foreach (var weakRef in _measureList)
            {
                if (!weakRef.TryGetTarget(out UIElement uie))
                {
                    continue;
                }

                switch (uie)
                {
                    case TextBlock tb:
                        tb.InvalidateCacheAndMeasure();
                        break;
                    default:
                        uie.InvalidateMeasure();
                        break;
                }
            }
        }

        _measureList = null;
    }

    private static (string CssFontName, string CssFontUrl) ParseFontSource(string fontSource, UIElement relativeTo)
    {
        string fontPath = fontSource.Trim().ToLower();

        // Note: if the path does not contain the character '.', then it means that there is no
        // specified file. It is therefore a default font or thet path to a folder containing
        // fonts, which we cannot handle so we simply return the font as is.
        if (fontPath.IndexOf('.') == -1)
        {
            if (fontPath == "portable user interface")
            {
                fontPath = DefaultCssFontFamily;
            }

            return (fontPath, null);
        }

        int index = fontPath.IndexOf('#');
        string fontUrl = index != -1 ? fontPath.Substring(0, index) : fontPath;

        // we try to make the path fit by considering that it is the startup assembly if not
        // specifically defined otherwise: (basically add a prefix ms-appx if there is none)
        // Note: we should not enter the "if" below if the path was defined in xaml. This cas
        // is already handled during compilation.
        if (!fontUrl.StartsWith(@"ms-appx:/") &&
            !fontUrl.StartsWith(@"http://") &&
            !fontUrl.StartsWith(@"https://") &&
            !fontUrl.Contains(@";component/"))
        {
            fontUrl = $"ms-appx:/{fontUrl}";
        }

        // Get a path that will lead to the position of the file
        string relativeFontPath = INTERNAL_UriHelper.ConvertToHtml5Path(fontUrl, relativeTo);
        relativeFontPath = relativeFontPath.Replace('\\', '/');
        if (relativeFontPath.StartsWith("/"))
        {
            relativeFontPath = relativeFontPath.Substring(1);
        }

        if (!_fontSourceToFamilyCache.TryGetValue(relativeFontPath, out string fontName))
        {
            fontName = GenerateFontName();
            _fontSourceToFamilyCache.Add(relativeFontPath, fontName);
        }

        return (fontName, relativeFontPath);
    }

    private static string GenerateFontName() => $"fontName{_idGenerator.NewId()}";
}
