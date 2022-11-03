

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
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using OpenSilver.Internal;
#if MIGRATION
using System.Windows;
using System.Windows.Media;
#else
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#endif

namespace CSHTML5.Internal
{
    internal class INTERNAL_XamlResourcesHandler
    {
        [ThreadStatic]
        private static int _parsing;
        internal static bool IsSystemResourcesParsing
        {
            get
            {
                return _parsing > 0;
            }
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

        const string HIGH_CONSTRAST_RESOURCE_THEME_KEY = "HighContrast";
        const string LIGHT_RESOURCE_THEME_KEY = "Light";
        const string DARK_RESOURCE_THEME_KEY = "Dark";

        Dictionary<Assembly, ResourceDictionary> _assemblyToResourceDictionary = new Dictionary<Assembly, ResourceDictionary>();
        ResourceDictionary _defaultResources;
        ResourceDictionary _defaultThemeResourcesDictionary;
        Dictionary<char, List<string>> _charToSimpleHighContrastNames; //this dictionary serves to link the first letter of the theme-dependent resource to the simple high contrast names that start with that letter.
        //The objective is to reduce the amount of strings we try to find in the resource (probably a minor improvement in performance but it's something)

        /// <summary>
        /// Tries to find the resourceKey in the Generic.xaml resources of the assembly. Note: the resource currently need to be defined in Project/Themes/generic.xaml
        /// </summary>
        /// <param name="assemblyWhereGenericXamlIsLocated">The assembly where we will look into.</param>
        /// <param name="resourceKey">The resource to find in the Assembly's resources.</param>
        /// <returns>The resource associated with the given key in the given assembly's resources.</returns>
        internal object TryFindResourceInGenericXaml(Assembly assemblyWhereGenericXamlIsLocated, object resourceKey)
        {
            ResourceDictionary resourceDictionary = null;
            if (_assemblyToResourceDictionary.ContainsKey(assemblyWhereGenericXamlIsLocated))
            {
                resourceDictionary = _assemblyToResourceDictionary[assemblyWhereGenericXamlIsLocated];
            }
            else
            {
#if NETSTANDARD
                string assemblyName = assemblyWhereGenericXamlIsLocated.GetName().Name;
#else // BRIDGE
                string assemblyName = INTERNAL_BridgeWorkarounds.GetAssemblyNameWithoutCallingGetNameMethod(assemblyWhereGenericXamlIsLocated);
#endif
                assemblyName = assemblyName.Replace(" ", "ǀǀ").Replace(".", "ǀǀ");
                string factoryTypeName = XamlResourcesHelper.MakeTitleCase("ǀǀ" + assemblyName + "ǀǀComponentǀǀThemesǀǀGenericǀǀXamlǀǀFactory");
                Type resourceDictionaryFactoryType = assemblyWhereGenericXamlIsLocated.GetType(factoryTypeName);

                if (resourceDictionaryFactoryType != null)
                {
                    IsSystemResourcesParsing = true;

                    try
                    {
                        resourceDictionary = (ResourceDictionary)resourceDictionaryFactoryType.GetMethod("Instantiate").Invoke(null, null);
                    }
                    finally
                    {
                        IsSystemResourcesParsing = false;
                    }

                    _assemblyToResourceDictionary.Add(assemblyWhereGenericXamlIsLocated, resourceDictionary);
                }
            }

            if (resourceDictionary != null && resourceDictionary.Contains(resourceKey))
            {
                return resourceDictionary[resourceKey];
            }

            return null;
        }

        /// <summary>
        /// Searches for the specified resource.
        /// </summary>
        /// <param name="resourceKey">The name of the resource to find.</param>
        /// <returns>
        /// The requested resource object. If the requested resource is not found, a
        /// null reference is returned.
        /// </returns>
        internal object TryFindResource(object resourceKey)
        {
            if (resourceKey is Type)
            {
                Type resourceKeyAsType = resourceKey as Type;
                object result = TryFindResourceInGenericXaml(resourceKeyAsType.Assembly, resourceKeyAsType);
                if (result != null)
                {
                    return result;
                }
            }

            if (Application.Current.Resources.Contains(resourceKey))
            {
                return Application.Current.Resources[resourceKey]; //I guess we try to find in the local resources first?
            }

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
            if (lightResourceDictionary.Contains(resourceKey))
            {
                return lightResourceDictionary[resourceKey];
            }

            //if we didn't find the key, try in the HighContrast resources.
            //todo: (Note:) I think the HighContrast resources can be changed outside of the app but that is probably specific to the machine so we'll ignore that for now and simply take the default high contrast values.
            if (_defaultThemeResourcesDictionary.ThemeDictionaries[HIGH_CONSTRAST_RESOURCE_THEME_KEY].Contains(resourceKey))
            {
                return _defaultThemeResourcesDictionary.ThemeDictionaries[HIGH_CONSTRAST_RESOURCE_THEME_KEY][resourceKey];
            }

            if (resourceKey is string)
            {
                object result = null;
                result = FindThemeDependentBrush(resourceKey as string);
                if (result != null)
                {
                    return result;
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

            if (_defaultResources.Contains(resourceKey))
            {
                return _defaultResources[resourceKey];
            }
            return null;
        }

        object FindThemeDependentBrush(string resourceName)
        {
            // https://docs.microsoft.com/en-us/windows/uwp/design/controls-and-patterns/resourcedictionary-and-xaml-resource-references

            if (!(resourceName.StartsWith("SystemControl") && resourceName.EndsWith("Brush")))
            {
                return null; // we only support the pattern: SystemControl[Simple HighContrast name][Simple light/dark name]Brush
            }

            #region explanatory comments
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
            #endregion

            if (_charToSimpleHighContrastNames == null)
            {
                //fill this dictionary:
                _charToSimpleHighContrastNames = new Dictionary<char, List<string>>();
                _charToSimpleHighContrastNames.Add('B', new List<string>() { { "Background" } });
                _charToSimpleHighContrastNames.Add('F', new List<string>() { { "Foreground" } });
                _charToSimpleHighContrastNames.Add('D', new List<string>() { { "Disabled" } });
                _charToSimpleHighContrastNames.Add('H', new List<string>() { { "Highlight" }, { "Hyperlink" } }); //I didn't put HighLightAlt in it because some [Simple light/dark name] start with "Alt"
                _charToSimpleHighContrastNames.Add('P', new List<string>() { { "PageBackground" }, { "PageText" } });
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

            if (_defaultThemeResourcesDictionary.ThemeDictionaries[LIGHT_RESOURCE_THEME_KEY].Contains(resourceName))
            {
                Color color = (Color)_defaultThemeResourcesDictionary.ThemeDictionaries[LIGHT_RESOURCE_THEME_KEY][resourceName];
                return new SolidColorBrush(color);
            }
            else
            {
                if (resourceName.StartsWith("Alt")) //if the [Simple HighContrast name] was "HighlightAlt", there is an "Alt" that was not removed here.
                {
                    resourceName = resourceName.Substring(3);
                    if (_defaultThemeResourcesDictionary.ThemeDictionaries[LIGHT_RESOURCE_THEME_KEY].Contains(resourceName))
                    {
                        Color color = (Color)_defaultThemeResourcesDictionary.ThemeDictionaries[LIGHT_RESOURCE_THEME_KEY][resourceName];
                        return new SolidColorBrush(color);
                    }
                }
                if (resourceName == "Transparent") //todo: see if this should be added to _defaultThemeResourcesDictionary.ThemeDictionaries[LIGHT_RESOURCE_THEME_KEY] and such or not. 
                {
                    return new SolidColorBrush(Colors.Transparent);
                }

            }
            return null;
        }

        void EnsureDefaultResources()
        {
            if (_defaultResources != null)
            {
                return;
            }

            _defaultResources = new ResourceDictionary();
            _defaultResources["ContentControlThemeFontFamily"] = new FontFamily("Segoe UI");
            _defaultResources["SymbolThemeFontFamily"] = new FontFamily("Segoe UI Symbol");
            _defaultResources["ControlContentThemeFontSize"] = 14.667;
            _defaultResources["ThumbBorderThemeBrush"] = new SolidColorBrush(Color.INTERNAL_ConvertFromString("#3B555555"));
            _defaultResources["SystemAccentColor"] = Color.INTERNAL_ConvertFromString("#FF0078D7");// this is the default blue value but it can be changed in windows so yeah...
            _defaultResources["SystemColorButtonFaceColor"] = Color.INTERNAL_ConvertFromString("#FF000000");// todo: find what the value is supposed to be by default, coudln't find it so I put black and we'll see if it's ok.
            _defaultResources["ScrollBarThumbBackgroundColor"] = Color.INTERNAL_ConvertFromString("#FF333333");// todo: find what the value is supposed to be by default, coudln't find it so I put black and we'll see if it's ok.
            _defaultResources["ScrollBarPanningThumbBackgroundColor"] = Color.INTERNAL_ConvertFromString("#00333333");// todo: find what the value is supposed to be by default, coudln't find it so I put black and we'll see if it's ok.
        }

        void EnsureDefaultThemeResources()
        {
            if (_defaultThemeResourcesDictionary != null)
            {
                return;
            }

            //todo-perfs: replace "INTERNAL_ConvertFromString" with Color.FromArgb(...)

            //to finish this list:
            //https://docs.microsoft.com/en-us/windows/uwp/design/controls-and-patterns/xaml-theme-resources

            _defaultThemeResourcesDictionary = new ResourceDictionary();

            //todo: SystemAccentColor?
            #region colors as colors
            // Add Light default values:
            ResourceDictionary resourceDictionary = new ResourceDictionary();

            #region setting light default values
            #region AltHigh Light
            Color color = Color.INTERNAL_ConvertFromString("#FFFFFFFF");
            resourceDictionary.Add("SystemAltHighColor", color);
            #endregion

            #region AltLow Light
            color = Color.INTERNAL_ConvertFromString("#33FFFFFF");
            resourceDictionary.Add("SystemAltLowColor", color);
            #endregion

            #region AltMedium Light
            color = Color.INTERNAL_ConvertFromString("#99FFFFFF");
            resourceDictionary.Add("SystemAltMediumColor", color);
            #endregion

            #region AltMediumHigh Light
            color = Color.INTERNAL_ConvertFromString("#CCFFFFFF");
            resourceDictionary.Add("SystemAltMediumHighColor", color);
            #endregion

            #region AltMediumLow Light
            color = Color.INTERNAL_ConvertFromString("#66FFFFFF");
            resourceDictionary.Add("SystemAltMediumLowColor", color);
            #endregion

            #region BaseHigh Light
            color = Color.INTERNAL_ConvertFromString("#FF000000");
            resourceDictionary.Add("SystemBaseHighColor", color);
            #endregion

            #region BaseLow Light
            color = Color.INTERNAL_ConvertFromString("#33000000");
            resourceDictionary.Add("SystemBaseLowColor", color);
            #endregion

            #region BaseMedium Light
            color = Color.INTERNAL_ConvertFromString("#99000000");
            resourceDictionary.Add("SystemBaseMediumColor", color);
            #endregion

            #region BaseMediumHigh Light
            color = Color.INTERNAL_ConvertFromString("#CC000000");
            resourceDictionary.Add("SystemBaseMediumHighColor", color);
            #endregion

            #region BaseMediumLow Light
            color = Color.INTERNAL_ConvertFromString("#66000000");
            resourceDictionary.Add("SystemBaseMediumLowColor", color);
            #endregion

            #region ChromeAltLow Light
            color = Color.INTERNAL_ConvertFromString("#FF171717");
            resourceDictionary.Add("SystemChromeAltLowColor", color);
            #endregion

            #region ChromeBlackHigh Light
            color = Color.INTERNAL_ConvertFromString("#FF000000");
            resourceDictionary.Add("SystemChromeBlackHighColor", color);
            #endregion

            #region ChromeBlackLow Light
            color = Color.INTERNAL_ConvertFromString("#33000000");
            resourceDictionary.Add("SystemChromeBlackLowColor", color);
            #endregion

            #region ChromeBlackMediumLow Light
            color = Color.INTERNAL_ConvertFromString("#66000000");
            resourceDictionary.Add("SystemChromeBlackMediumLowColor", color);
            #endregion

            #region ChromeBlackMedium Light
            color = Color.INTERNAL_ConvertFromString("#CC000000");
            resourceDictionary.Add("SystemChromeBlackMediumColor", color);
            #endregion

            #region ChromeDisabledHigh Light
            color = Color.INTERNAL_ConvertFromString("#FFCCCCCC");
            resourceDictionary.Add("SystemChromeDisabledHighColor", color);
            #endregion

            #region ChromeDisabledLow Light
            color = Color.INTERNAL_ConvertFromString("#FF7A7A7A");
            resourceDictionary.Add("SystemChromeDisabledLowColor", color);
            #endregion

            #region ChromeHigh Light
            color = Color.INTERNAL_ConvertFromString("#FFCCCCCC");
            resourceDictionary.Add("SystemChromeHighColor", color);
            #endregion

            #region ChromeLow Light
            color = Color.INTERNAL_ConvertFromString("#FFF2F2F2");
            resourceDictionary.Add("SystemChromeLowColor", color);
            #endregion

            #region ChromeMedium Light
            color = Color.INTERNAL_ConvertFromString("#FFE6E6E6");
            resourceDictionary.Add("SystemChromeMediumColor", color);
            #endregion

            #region ChromeMediumLow Light
            color = Color.INTERNAL_ConvertFromString("#FFF2F2F2");
            resourceDictionary.Add("SystemChromeMediumLowColor", color);
            #endregion

            #region ChromeWhite Light
            color = Color.INTERNAL_ConvertFromString("#FFFFFFFF");
            resourceDictionary.Add("SystemChromeWhiteColor", color);
            #endregion

            #region ListLow Light
            color = Color.INTERNAL_ConvertFromString("#19000000");
            resourceDictionary.Add("SystemListLowColor", color);
            #endregion

            #region ListMedium Light
            color = Color.INTERNAL_ConvertFromString("#33000000");
            resourceDictionary.Add("SystemListMediumColor", color);
            #endregion

            #endregion
            _defaultThemeResourcesDictionary.ThemeDictionaries.Add(LIGHT_RESOURCE_THEME_KEY, resourceDictionary);

            // Add Dark default values:
            resourceDictionary = new ResourceDictionary();

            #region setting dark default values

            #region AltHigh Dark
            color = Color.INTERNAL_ConvertFromString("#FF000000");
            resourceDictionary.Add("SystemAltHighColor", color);
            #endregion

            #region AltLow Dark
            color = Color.INTERNAL_ConvertFromString("#33000000");
            resourceDictionary.Add("SystemAltLowColor", color);
            #endregion

            #region AltMedium Dark
            color = Color.INTERNAL_ConvertFromString("#99000000");
            resourceDictionary.Add("SystemAltMediumColor", color);
            #endregion

            #region AltMediumHigh Dark
            color = Color.INTERNAL_ConvertFromString("#CC000000");
            resourceDictionary.Add("SystemAltMediumHighColor", color);
            #endregion

            #region AltMediumLow Dark
            color = Color.INTERNAL_ConvertFromString("#66000000");
            resourceDictionary.Add("SystemAltMediumLowColor", color);
            #endregion

            #region BaseHigh Dark
            color = Color.INTERNAL_ConvertFromString("#FFFFFFFF");
            resourceDictionary.Add("SystemBaseHighColor", color);
            #endregion

            #region BaseLow Dark
            color = Color.INTERNAL_ConvertFromString("#33FFFFFF");
            resourceDictionary.Add("SystemBaseLowColor", color);
            #endregion

            #region BaseMedium Dark
            color = Color.INTERNAL_ConvertFromString("#99FFFFFF");
            resourceDictionary.Add("SystemBaseMediumColor", color);
            #endregion

            #region BaseMediumHigh Dark
            color = Color.INTERNAL_ConvertFromString("#CCFFFFFF");
            resourceDictionary.Add("SystemBaseMediumHighColor", color);
            #endregion

            #region BaseMediumLow Dark
            color = Color.INTERNAL_ConvertFromString("#66FFFFFF");
            resourceDictionary.Add("SystemBaseMediumLowColor", color);
            #endregion

            #region ChromeAltLow Dark
            color = Color.INTERNAL_ConvertFromString("#FFF2F2F2");
            resourceDictionary.Add("SystemChromeAltLowColor", color);
            #endregion

            #region ChromeBlackHigh Dark
            color = Color.INTERNAL_ConvertFromString("#FF000000");
            resourceDictionary.Add("SystemChromeBlackHighColor", color);
            #endregion

            #region ChromeBlackLow Dark
            color = Color.INTERNAL_ConvertFromString("#33000000");
            resourceDictionary.Add("SystemChromeBlackLowColor", color);
            #endregion

            #region ChromeBlackMediumLow Dark
            color = Color.INTERNAL_ConvertFromString("#66000000");
            resourceDictionary.Add("SystemChromeBlackMediumLowColor", color);
            #endregion

            #region ChromeBlackMedium Dark
            color = Color.INTERNAL_ConvertFromString("#CC000000");
            resourceDictionary.Add("SystemChromeBlackMediumColor", color);
            #endregion

            #region ChromeDisabledHigh Dark
            color = Color.INTERNAL_ConvertFromString("#FF333333");
            resourceDictionary.Add("SystemChromeDisabledHighColor", color);
            #endregion

            #region ChromeDisabledLow Dark
            color = Color.INTERNAL_ConvertFromString("#FF858585");
            resourceDictionary.Add("SystemChromeDisabledLowColor", color);
            #endregion

            #region ChromeHigh Dark
            color = Color.INTERNAL_ConvertFromString("#FF767676");
            resourceDictionary.Add("SystemChromeHighColor", color);
            #endregion

            #region ChromeLow Dark
            color = Color.INTERNAL_ConvertFromString("#FF171717");
            resourceDictionary.Add("SystemChromeLowColor", color);
            #endregion

            #region ChromeMedium Dark
            color = Color.INTERNAL_ConvertFromString("#FF1F1F1F");
            resourceDictionary.Add("SystemChromeMediumColor", color);
            #endregion

            #region ChromeMediumLow Dark
            color = Color.INTERNAL_ConvertFromString("#FF2B2B2B");
            resourceDictionary.Add("SystemChromeMediumLowColor", color);
            #endregion

            #region ChromeWhite Dark
            color = Color.INTERNAL_ConvertFromString("#FFFFFFFF");
            resourceDictionary.Add("SystemChromeWhiteColor", color);
            #endregion

            #region ListLow Dark
            color = Color.INTERNAL_ConvertFromString("#19FFFFFF");
            resourceDictionary.Add("SystemListLowColor", color);
            #endregion

            #region ListMedium Dark
            color = Color.INTERNAL_ConvertFromString("#33FFFFFF");
            resourceDictionary.Add("SystemListMediumColor", color);
            #endregion

            #endregion

            _defaultThemeResourcesDictionary.ThemeDictionaries.Add(DARK_RESOURCE_THEME_KEY, resourceDictionary);

            //Add HighContrast default values:
            resourceDictionary = new ResourceDictionary();

            #region high contrast values

            #region Button Text (background) HighContrast
            //simple name: Background
            color = Color.INTERNAL_ConvertFromString("#FFF0F0F0");
            resourceDictionary.Add("SystemColorButtonFaceColor", color);
            #endregion

            #region Button Text (foreground) HighContrast
            //simple name: Foreground
            color = Color.INTERNAL_ConvertFromString("#FF000000");
            resourceDictionary.Add("SystemColorButtonTextColor", color);
            #endregion

            #region Disabled Text HighContrast
            //simple name: Disabled
            color = Color.INTERNAL_ConvertFromString("#FF6D6D6D");
            resourceDictionary.Add("SystemColorGrayTextColor", color);
            #endregion

            #region Selected Text (background) HighContrast
            //simple name: Highlight
            color = Color.INTERNAL_ConvertFromString("#FF3399FF");
            resourceDictionary.Add("SystemColorHighlightColor", color);
            #endregion

            #region Selected Text (foreground) HighContrast
            //simple name: HighlightAlt
            color = Color.INTERNAL_ConvertFromString("#FFFFFFFF");
            resourceDictionary.Add("SystemColorHighlightTextColor", color);
            #endregion

            #region Hyperlinks HighContrast
            //simple name: Hyperlink
            color = Color.INTERNAL_ConvertFromString("#FF0066CC");
            resourceDictionary.Add("SystemColorHotlightColor", color);
            #endregion

            #region Background HighContrast
            //simple name: PageBackground
            color = Color.INTERNAL_ConvertFromString("#FFFFFFFF");
            resourceDictionary.Add("SystemColorWindowColor", color);
            #endregion

            #region Text HighContrast
            //simple name: PageText
            color = Color.INTERNAL_ConvertFromString("#FF000000");
            resourceDictionary.Add("SystemColorWindowTextColor", color);
            #endregion

            #endregion

            _defaultThemeResourcesDictionary.ThemeDictionaries.Add(HIGH_CONSTRAST_RESOURCE_THEME_KEY, resourceDictionary);
            #endregion
            #region colors in brushes (not what we wanted)

            #endregion

            //todo: Add others ?:

        }
    }
}
