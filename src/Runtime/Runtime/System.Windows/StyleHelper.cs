
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
using System.Windows.Data;
using OpenSilver.Internal;

namespace System.Windows
{
    internal static class StyleHelper
    {
        private delegate void SetStyleValueDelegate(FrameworkElement fe, DependencyProperty dp, object value);

        private static readonly SetStyleValueDelegate _setLocalStyleValueDelegate =
            new SetStyleValueDelegate((fe, dp, v) => fe.SetLocalStyleValue(dp, v));

        private static readonly SetStyleValueDelegate _setThemeStyleValueDelegate =
            new SetStyleValueDelegate((fe, dp, v) => fe.SetThemeStyleValue(dp, v));

        //
        //  This method
        //  1. Updates the style cache for the given fe
        //
        internal static void UpdateStyleCache(FrameworkElement fe, Style oldStyle, Style newStyle, ref Style styleCache)
        {
            SealStyle(fe, newStyle);

            styleCache = newStyle;

            UpdateInstanceData(fe, oldStyle, fe.HasLocalStyle ? newStyle : fe.ImplicitStyle, _setLocalStyleValueDelegate);
        }

        //
        //  This method
        //  1. Updates the theme style cache for the given fe
        //
        internal static void UpdateThemeStyleCache(FrameworkElement fe, Style oldStyle, Style newStyle, ref Style themeStyleCache)
        {
            SealStyle(fe, newStyle);

            themeStyleCache = newStyle;

            UpdateInstanceData(fe, oldStyle, newStyle, _setThemeStyleValueDelegate);
        }

        internal static void UpdateImplicitStyleCache(FrameworkElement fe, Style oldStyle, Style newStyle, ref Style implicitStyleCache)
        {
            SealStyle(fe, newStyle);

            implicitStyleCache = newStyle;

            fe.HasLocalStyle = fe.ReadLocalValueInternal(FrameworkElement.StyleProperty) != DependencyProperty.UnsetValue;

            // Local style takes priority over an implicit style.
            if (!fe.HasLocalStyle)
            {
                UpdateInstanceData(fe, oldStyle, newStyle, _setLocalStyleValueDelegate);
            }
        }

        //
        //  This method
        //  1. Is called whenever a Style is [un]applied to an FE
        //  2. It updates the per-instance style data
        //
        private static void UpdateInstanceData(FrameworkElement fe, Style oldStyle, Style newStyle, SetStyleValueDelegate setValue)
        {
            Dictionary<int, object> newStyleValues = newStyle?.EffectiveValues;

            if (oldStyle != null)
            {
                // Clear old theme style values
                // if a property is about to be set again in the new theme style
                // we don't unset the value directly to prevent from potientially
                // firing the DependencyPropertyChanged callback twice.
                foreach (int propertyIndex in oldStyle.EffectiveValues.Keys)
                {
                    if (newStyleValues?.ContainsKey(propertyIndex) ?? false)
                    {
                        continue;
                    }

                    DependencyProperty dp = DependencyProperty.RegisteredPropertyList[propertyIndex];
                    setValue(fe, dp, DependencyProperty.UnsetValue);
                }
            }

            if (newStyle != null)
            {
                foreach (var pValue in newStyleValues)
                {
                    DependencyProperty dp = DependencyProperty.RegisteredPropertyList[pValue.Key];
                    object value = pValue.Value is Binding binding ?
                        binding.CreateBindingExpression(fe, dp) :
                        pValue.Value;

                    setValue(fe, dp, value);
                }
            }
        }

        internal static Style GetThemeStyle(FrameworkElement fe)
        {
            // If this is the first time that the ThemeStyleProperty
            // is being fetched then mark it such
            fe.HasThemeStyleEverBeenFetched = true;

            // Fetch the DefaultStyleKey and the self Style for
            // the given FrameworkElement
            object themeStyleKey = fe.GetValue(FrameworkElement.DefaultStyleKeyProperty);
            //Style selfStyle = fe.Style;
            Style oldThemeStyle = fe.ThemeStyle;
            Style newThemeStyle = null;

            // Don't lookup properties from the themes if user has specified OverridesDefaultStyle
            // or DefaultStyleKey = null
            if (themeStyleKey != null)
            {
                // First look for an applicable style in system resources
                object styleLookup;
                // Regular lookup based on the DefaultStyleKey. Involves locking and Hashtable lookup

                if (themeStyleKey is Type typeKey)
                {
                    styleLookup = XamlResources.FindStyleResourceInGenericXaml(typeKey);
                }
                else
                {
                    styleLookup = null;
                }

                if (styleLookup != null)
                {
                    if (styleLookup is Style)
                    {
                        // We have found an applicable Style in system resources
                        //  let's us use that as second stop to find property values.
                        newThemeStyle = (Style)styleLookup;
                    }
                    else
                    {
                        // We found something keyed to the ThemeStyleKey, but it's not
                        //  a style.  This is a problem, throw an exception here.
                        throw new InvalidOperationException(string.Format("System resource for type '{0}' is not a Style object.", themeStyleKey));
                    }
                }

                if (newThemeStyle == null)
                {
                    // No style in system resources, try to retrieve the default
                    // style for the target type.
                    Type themeStyleTypeKey = themeStyleKey as Type;
                    if (themeStyleTypeKey != null)
                    {
                        PropertyMetadata styleMetadata = FrameworkElement.StyleProperty.GetMetadata(themeStyleTypeKey);

                        if (styleMetadata != null)
                        {
                            // Have a metadata object, get the default style (if any)
                            newThemeStyle = styleMetadata.DefaultValue as Style;
                        }
                    }
                }
            }

            // Propagate change notification
            if (oldThemeStyle != newThemeStyle)
            {
                FrameworkElement.OnThemeStyleChanged(fe, oldThemeStyle, newThemeStyle);
            }

            return newThemeStyle;
        }

        internal static void SealStyle(FrameworkElement fe, Style style)
        {
            if (style != null)
            {
                // We have a new style.  Make sure it's targeting the right
                // type, and then seal it.

                style.CheckTargetType(fe);
                style.Seal();
            }
        }
    }
}
