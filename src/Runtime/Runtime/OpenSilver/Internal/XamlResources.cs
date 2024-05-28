
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
using System.Reflection;
using OpenSilver.Internal.Xaml;
using System.Windows;
using System.Windows.Media;

namespace OpenSilver.Internal;

internal static class XamlResources
{
    [ThreadStatic]
    private static int _parsing;

    internal static bool IsSystemResourcesParsing
    {
        get => _parsing > 0;
        set
        {
            if (value)
            {
                _parsing++;
            }
            else
            {
                _parsing--;
            }
        }
    }

    private const string HIGH_CONSTRAST_RESOURCE_THEME_KEY = "HighContrast";
    private const string LIGHT_RESOURCE_THEME_KEY = "Light";
    private const string DARK_RESOURCE_THEME_KEY = "Dark";

    private static readonly Dictionary<Assembly, ResourceDictionary> _dictionaries = new();
    private static readonly Dictionary<Type, Style> _resourcesCache = new();
    private static ResourceDictionary _defaultResources;
    private static ResourceDictionary _defaultThemeResourcesDictionary;
    private static Dictionary<char, List<string>> _charToSimpleHighContrastNames; //this dictionary serves to link the first letter of the theme-dependent resource to the simple high contrast names that start with that letter.

    /// <summary>
    /// Tries to find the resourceKey in the Generic.xaml resources of the assembly. Note: the resource currently need to be defined in Project/Themes/generic.xaml
    /// </summary>
    /// <param name="typeKey">The resource to find in the Assembly's resources.</param>
    /// <returns>The resource associated with the given key in the given assembly's resources.</returns>
    internal static Style FindStyleResourceInGenericXaml(Type typeKey)
    {
        Debug.Assert(typeKey != null);

        if (!FindCachedResource(typeKey, out Style resource))
        {
            resource = (Style)FindDictionaryResource(typeKey);
            CacheResource(typeKey, resource);
        }

        return resource;
    }

    private static Type GetGenericXamlFactoryForAssembly(Assembly assembly)
    {
        Debug.Assert(assembly != null);

        string name = assembly.GetName().Name.Replace(" ", "ǀǀ").Replace(".", "ǀǀ");
        string factoryName = XamlResourcesHelper.MakeTitleCase("ǀǀ" + name + "ǀǀComponentǀǀThemesǀǀGenericǀǀXamlǀǀFactory");
        return assembly.GetType(factoryName);
    }

    private static bool FindCachedResource(Type typeKey, out Style resource) => _resourcesCache.TryGetValue(typeKey, out resource);

    private static void CacheResource(Type typeKey, Style resource) => _resourcesCache[typeKey] = resource;

    private static object FindDictionaryResource(Type typeKey)
    {
        Debug.Assert(typeKey != null);

        Assembly assembly = typeKey.Assembly;
        if (!_dictionaries.TryGetValue(assembly, out ResourceDictionary rd))
        {
            rd = LoadGenericResourceDictionary(assembly);
        }

        return rd?[typeKey];
    }

    private static ResourceDictionary LoadGenericResourceDictionary(Assembly assembly)
    {
        ResourceDictionary rd = null;

        Type factoryType = GetGenericXamlFactoryForAssembly(assembly);
        if (factoryType != null)
        {
            IsSystemResourcesParsing = true;

            try
            {
                if (Activator.CreateInstance(factoryType) is IXamlComponentLoader loader)
                {
                    rd = new ResourceDictionary();
                    loader.LoadComponent(rd);
                }
            }
            finally
            {
                IsSystemResourcesParsing = false;
            }
        }

        _dictionaries.Add(assembly, rd);

        return rd;
    }

    internal static object FindBuiltInResource(object key)
    {
        object resource;

        IsSystemResourcesParsing = true;

        try
        {
            EnsureDefaultThemeResources();
        }
        finally
        {
            IsSystemResourcesParsing = false;
        }

        //todo: find what the current theme is.
        //for now we assume the current theme is Light:
        ResourceDictionary lightResourceDictionary = _defaultThemeResourcesDictionary.ThemeDictionaries[LIGHT_RESOURCE_THEME_KEY];
        if (lightResourceDictionary.TryGetResource(key, out resource))
        {
            return resource;
        }

        //if we didn't find the key, try in the HighContrast resources.
        //todo: (Note:) I think the HighContrast resources can be changed outside of the app but that is probably specific to the machine so we'll ignore that for now and simply take the default high contrast values.
        if (_defaultThemeResourcesDictionary.ThemeDictionaries[HIGH_CONSTRAST_RESOURCE_THEME_KEY].TryGetResource(key, out resource))
        {
            return resource;
        }

        if (key is string keyString)
        {
            resource = FindThemeDependentBrush(keyString);
            if (resource != null)
            {
                return resource;
            }
        }
        //todo: find out whether this is a theme-dependent resource name. See:
        //https://docs.microsoft.com/en-us/windows/uwp/design/controls-and-patterns/xaml-theme-resources#theme-dependent-brushes
        //in this case, deal with it.

        IsSystemResourcesParsing = true;

        try
        {
            EnsureDefaultResources();
        }
        finally
        {
            IsSystemResourcesParsing = false;
        }

        if (_defaultResources.TryGetResource(key, out resource))
        {
            return resource;
        }
        return null;
    }

    private static object FindThemeDependentBrush(string resourceName)
    {
        // https://docs.microsoft.com/en-us/windows/uwp/design/controls-and-patterns/resourcedictionary-and-xaml-resource-references

        if (!(resourceName.StartsWith("SystemControl") && resourceName.EndsWith("Brush")))
        {
            return null; // we only support the pattern: SystemControl[Simple HighContrast name][Simple light/dark name]Brush
        }

        //list of the simple HighContrast names: Background, Foreground, Disabled, Highlight, HighlightAlt, Hyperlink, PageBackground, PageText

        //Note: since I don't think we can have themes as intended in UWP, we'll simply consider that we are in a Light theme for now, meaning that we will ignore the simple HighContrast name
        //todo: when we support themes, change this so that it's taken into consideration.

        //I currently only know of the following pattern: SystemControl[Simple HighContrast name][Simple light/dark name]Brush but there might be more (see note in https://docs.microsoft.com/en-us/windows/uwp/design/controls-and-patterns/xaml-theme-resources#theme-dependent-brushes)

        //WHAT WE DO:
        // - remove the "SystemControl" and "Brush" parts as they are useless.
        // - find which [Simple HighContrast name] is used and remove it.
        // - try to find the rest of the string in the Theme resources Light
        // - once found, make a SolidColorBrush out of the color it gives and return it.

        //Note: we cannot know if the [Simple HighContrast name] is "Highlight" or "HighlightAlt" because some [Simple light/dark name] start with "Alt"

        if (_charToSimpleHighContrastNames == null)
        {
            //fill this dictionary:
            _charToSimpleHighContrastNames = new Dictionary<char, List<string>>
            {
                { 'B', new List<string>() { { "Background" } } },
                { 'F', new List<string>() { { "Foreground" } } },
                { 'D', new List<string>() { { "Disabled" } } },
                { 'H', new List<string>() { { "Highlight" }, { "Hyperlink" } } }, //I didn't put HighLightAlt in it because some [Simple light/dark name] start with "Alt"
                { 'P', new List<string>() { { "PageBackground" }, { "PageText" } } }
            };
        }

        //Remove the useless parts:
        resourceName = resourceName.Substring(13, resourceName.Length - 18); //13 is the length of SystemControl and 18 is the length of SystemControl + Brush

        //find which High contrast name it uses:
        char firstLetter = resourceName[0];
        List<string> possibleNames = _charToSimpleHighContrastNames.ContainsKey(firstLetter) ? _charToSimpleHighContrastNames[firstLetter] : null;
        if (possibleNames == null)
        {
            return null; //the resource could not be found.
        }
        foreach (string possibleName in possibleNames)
        {
            if (resourceName.StartsWith(possibleName))
            {
                resourceName = resourceName.Substring(possibleName.Length); //we remove it so that we're left only with the [Simple light/dark name] part.
                break;
            }
        }

        if (_defaultThemeResourcesDictionary.ThemeDictionaries[LIGHT_RESOURCE_THEME_KEY].TryGetResource(resourceName, out object resource))
        {
            Color color = (Color)resource;
            return new SolidColorBrush(color);
        }
        else
        {
            if (resourceName.StartsWith("Alt")) //if the [Simple HighContrast name] was "HighlightAlt", there is an "Alt" that was not removed here.
            {
                resourceName = resourceName.Substring(3);
                if (_defaultThemeResourcesDictionary.ThemeDictionaries[LIGHT_RESOURCE_THEME_KEY].TryGetResource(resourceName, out resource))
                {
                    Color color = (Color)resource;
                    return new SolidColorBrush(color);
                }
            }
            else if (resourceName == "Transparent") //todo: see if this should be added to _defaultThemeResourcesDictionary.ThemeDictionaries[LIGHT_RESOURCE_THEME_KEY] and such or not. 
            {
                return new SolidColorBrush(Colors.Transparent);
            }

        }
        return null;
    }

    private static void EnsureDefaultResources()
    {
        if (_defaultResources != null)
        {
            return;
        }

        _defaultResources = new ResourceDictionary
        {
            ["ContentControlThemeFontFamily"] = new FontFamily("Segoe UI"),
            ["SymbolThemeFontFamily"] = new FontFamily("Segoe UI Symbol"),
            ["ControlContentThemeFontSize"] = 14.667,
            ["ThumbBorderThemeBrush"] = new SolidColorBrush(Color.FromUInt32(0x3B555555)),
            ["SystemAccentColor"] = Color.FromUInt32(0xFF0078D7),// this is the default blue value but it can be changed in windows so yeah...
            ["SystemColorButtonFaceColor"] = Color.FromUInt32(0xFF000000),// todo: find what the value is supposed to be by default, coudln't find it so I put black and we'll see if it's ok.
            ["ScrollBarThumbBackgroundColor"] = Color.FromUInt32(0xFF333333),// todo: find what the value is supposed to be by default, coudln't find it so I put black and we'll see if it's ok.
            ["ScrollBarPanningThumbBackgroundColor"] = Color.FromUInt32(0x00333333)// todo: find what the value is supposed to be by default, coudln't find it so I put black and we'll see if it's ok.
        };
    }

    private static void EnsureDefaultThemeResources()
    {
        if (_defaultThemeResourcesDictionary != null)
        {
            return;
        }

        //to finish this list:
        //https://docs.microsoft.com/en-us/windows/uwp/design/controls-and-patterns/xaml-theme-resources

        _defaultThemeResourcesDictionary = new ResourceDictionary();

        // Light default values
        _defaultThemeResourcesDictionary.ThemeDictionaries.Add(
            LIGHT_RESOURCE_THEME_KEY,
            new ResourceDictionary
            {
                { "SystemAltHighColor", Color.FromUInt32(0xFFFFFFFF) },
                { "SystemAltLowColor", Color.FromUInt32(0x33FFFFFF) },
                { "SystemAltMediumColor", Color.FromUInt32(0x99FFFFFF) },
                { "SystemAltMediumHighColor", Color.FromUInt32(0xCCFFFFFF) },
                { "SystemAltMediumLowColor", Color.FromUInt32(0x66FFFFFF) },
                { "SystemBaseHighColor", Color.FromUInt32(0xFF000000) },
                { "SystemBaseLowColor", Color.FromUInt32(0x33000000) },
                { "SystemBaseMediumColor", Color.FromUInt32(0x99000000) },
                { "SystemBaseMediumHighColor", Color.FromUInt32(0xCC000000) },
                { "SystemBaseMediumLowColor", Color.FromUInt32(0x66000000) },
                { "SystemChromeAltLowColor", Color.FromUInt32(0xFF171717) },
                { "SystemChromeBlackHighColor", Color.FromUInt32(0xFF000000) },
                { "SystemChromeBlackLowColor", Color.FromUInt32(0x33000000) },
                { "SystemChromeBlackMediumLowColor", Color.FromUInt32(0x66000000) },
                { "SystemChromeBlackMediumColor", Color.FromUInt32(0xCC000000) },
                { "SystemChromeDisabledHighColor", Color.FromUInt32(0xFFCCCCCC) },
                { "SystemChromeDisabledLowColor", Color.FromUInt32(0xFF7A7A7A) },
                { "SystemChromeHighColor", Color.FromUInt32(0xFFCCCCCC) },
                { "SystemChromeLowColor", Color.FromUInt32(0xFFF2F2F2) },
                { "SystemChromeMediumColor", Color.FromUInt32(0xFFE6E6E6) },
                { "SystemChromeMediumLowColor", Color.FromUInt32(0xFFF2F2F2) },
                { "SystemChromeWhiteColor", Color.FromUInt32(0xFFFFFFFF) },
                { "SystemListLowColor", Color.FromUInt32(0x19000000) },
                { "SystemListMediumColor", Color.FromUInt32(0x33000000) }
            });

        // Dark default values
        _defaultThemeResourcesDictionary.ThemeDictionaries.Add(
            DARK_RESOURCE_THEME_KEY,
            new ResourceDictionary
            {
                { "SystemAltHighColor", Color.FromUInt32(0xFF000000) },
                { "SystemAltLowColor", Color.FromUInt32(0x33000000) },
                { "SystemAltMediumColor", Color.FromUInt32(0x99000000) },
                { "SystemAltMediumHighColor", Color.FromUInt32(0xCC000000) },
                { "SystemAltMediumLowColor", Color.FromUInt32(0x66000000) },
                { "SystemBaseHighColor", Color.FromUInt32(0xFFFFFFFF) },
                { "SystemBaseLowColor", Color.FromUInt32(0x33FFFFFF) },
                { "SystemBaseMediumColor", Color.FromUInt32(0x99FFFFFF) },
                { "SystemBaseMediumHighColor", Color.FromUInt32(0xCCFFFFFF) },
                { "SystemBaseMediumLowColor", Color.FromUInt32(0x66FFFFFF) },
                { "SystemChromeAltLowColor", Color.FromUInt32(0xFFF2F2F2) },
                { "SystemChromeBlackHighColor", Color.FromUInt32(0xFF000000) },
                { "SystemChromeBlackLowColor", Color.FromUInt32(0x33000000) },
                { "SystemChromeBlackMediumLowColor", Color.FromUInt32(0x66000000) },
                { "SystemChromeBlackMediumColor", Color.FromUInt32(0xCC000000) },
                { "SystemChromeDisabledHighColor", Color.FromUInt32(0xFF333333) },
                { "SystemChromeDisabledLowColor", Color.FromUInt32(0xFF858585) },
                { "SystemChromeHighColor", Color.FromUInt32(0xFF767676) },
                { "SystemChromeLowColor", Color.FromUInt32(0xFF171717) },
                { "SystemChromeMediumColor", Color.FromUInt32(0xFF1F1F1F) },
                { "SystemChromeMediumLowColor", Color.FromUInt32(0xFF2B2B2B) },
                { "SystemChromeWhiteColor", Color.FromUInt32(0xFFFFFFFF) },
                { "SystemListLowColor", Color.FromUInt32(0x19FFFFFF) },
                { "SystemListMediumColor", Color.FromUInt32(0x33FFFFFF) }
            });

        // HighContrast default values
        _defaultThemeResourcesDictionary.ThemeDictionaries.Add(
            HIGH_CONSTRAST_RESOURCE_THEME_KEY,
            new ResourceDictionary
            {
                { "SystemColorButtonFaceColor", Color.FromUInt32(0xFFF0F0F0) },
                { "SystemColorButtonTextColor", Color.FromUInt32(0xFF000000) },
                { "SystemColorGrayTextColor", Color.FromUInt32(0xFF6D6D6D) },
                { "SystemColorHighlightColor", Color.FromUInt32(0xFF3399FF) },
                { "SystemColorHighlightTextColor", Color.FromUInt32(0xFFFFFFFF) },
                { "SystemColorHotlightColor", Color.FromUInt32(0xFF0066CC) },
                { "SystemColorWindowColor", Color.FromUInt32(0xFFFFFFFF) },
                { "SystemColorWindowTextColor", Color.FromUInt32(0xFF000000) }
            });
    }
}
