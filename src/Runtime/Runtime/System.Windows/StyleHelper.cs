
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
using System.Windows.Controls;
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
            if (newStyle is not null)
            {
                // We have a new style.  Make sure it's targeting the right
                // type, and then seal it.

                newStyle.CheckTargetType(fe);
                newStyle.Seal();
            }

            styleCache = newStyle;

            Style effectiveStyle = fe.HasLocalStyle ? newStyle : fe.ImplicitStyle;
            
            // Cleanup old trigger storage (always try, as it might have been from implicit style)
            CleanupTriggerStorage(fe);

            UpdateInstanceData(fe, oldStyle, effectiveStyle, _setLocalStyleValueDelegate);

            // Initialize triggers for the new style
            if (effectiveStyle?.HasTriggers == true)
            {
                InitializeTriggerStorage(fe, effectiveStyle);
            }
        }

        //
        //  This method
        //  1. Updates the theme style cache for the given fe
        //
        internal static void UpdateThemeStyleCache(FrameworkElement fe, Style oldStyle, Style newStyle, ref Style themeStyleCache)
        {
            if (newStyle is not null)
            {
                // We have a new style.  Make sure it's targeting the right
                // type, and then seal it.

                newStyle.CheckTargetType(fe);
                newStyle.Seal();

                // Check if the theme style has EventHandlers set on the target tag or in its setter collection.
                // We do not support EventHandlers in a ThemeStyle
                if (newStyle.HasEventSetters)
                {
                    throw new InvalidOperationException(Strings.CannotHaveEventHandlersInThemeStyle);
                }
            }

            themeStyleCache = newStyle;

            // Cleanup old theme style trigger storage
            CleanupThemeStyleTriggerStorage(fe);

            UpdateInstanceData(fe, oldStyle, newStyle, _setThemeStyleValueDelegate);

            // Initialize triggers for the new theme style
            if (newStyle?.HasTriggers == true)
            {
                InitializeThemeStyleTriggerStorage(fe, newStyle);
            }
        }

        internal static void UpdateImplicitStyleCache(FrameworkElement fe, Style oldStyle, Style newStyle, ref Style implicitStyleCache)
        {
            if (newStyle is not null)
            {
                // We have a new style.  Make sure it's targeting the right
                // type, and then seal it.

                newStyle.CheckTargetType(fe);
                newStyle.Seal();
            }

            implicitStyleCache = newStyle;

            fe.HasLocalStyle = fe.ReadLocalValue(FrameworkElement.StyleProperty) != DependencyProperty.UnsetValue;

            // Local style takes priority over an implicit style.
            if (!fe.HasLocalStyle)
            {
                // Cleanup old trigger storage (from old implicit style)
                CleanupTriggerStorage(fe);

                UpdateInstanceData(fe, oldStyle, newStyle, _setLocalStyleValueDelegate);

                // Initialize triggers for the new implicit style
                if (newStyle?.HasTriggers == true)
                {
                    InitializeTriggerStorage(fe, newStyle);
                }
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
                    object value = pValue.Value switch
                    {
                        BindingBase bindingBase => bindingBase.CreateBindingExpression(fe, dp, null),
                        DynamicResourceExtension dynamicResource => new ResourceReferenceExpression(dynamicResource.ResourceKey ??
                            throw new InvalidOperationException(Strings.MarkupExtensionResourceKey)),
                        ResponsiveExtension responsive => responsive.CreateResponsiveExpression(fe, dp),
                        _ => pValue.Value,
                    };

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
            object themeStyleKey = fe.DefaultStyleKey;
            bool overridesDefaultStyle = fe.OverridesDefaultStyle;
            Style newThemeStyle = null;

            // Don't lookup properties from the themes if user has specified OverridesDefaultStyle or
            // DefaultStyleKey is null.
            if (!overridesDefaultStyle && themeStyleKey is Type typeKey)
            {
                // Regular lookup based on the DefaultStyleKey. Involves locking and Hashtable lookup
                newThemeStyle = XamlResources.FindStyleResourceInGenericXaml(typeKey);

                if (newThemeStyle is null)
                {
                    // No style in system resources, try to retrieve the default
                    // style for the target type.

                    if (FrameworkElement.StyleProperty.GetMetadata(typeKey) is PropertyMetadata styleMetadata)
                    {
                        // Have a metadata object, get the default style (if any)
                        newThemeStyle = styleMetadata.DefaultValue as Style;
                    }
                }
            }

            return newThemeStyle;
        }

        //
        //  This method
        //  1. If the value is an ISealable and it is not sealed
        //     and can be sealed, seal it now.
        //  2. Else it returns the value as is.
        //
        internal static void SealIfSealable(object value)
        {
            // If the value is an ISealable and it is not sealed
            // and can be sealed, seal it now.
            if (value is ISealable sealable && !sealable.IsSealed && sealable.CanSeal)
            {
                sealable.Seal();
            }
        }

        internal static bool ShouldGetValueFromStyle(DependencyProperty dp) => dp != FrameworkElement.StyleProperty;

        internal static bool ShouldGetValueFromThemeStyle(DependencyProperty dp) =>
            dp != FrameworkElement.StyleProperty &&
            dp != FrameworkElement.DefaultStyleKeyProperty &&
            dp != FrameworkElement.OverridesDefaultStyleProperty;

        internal static bool ShouldGetValueFromTemplate(DependencyProperty dp) =>
            dp != FrameworkElement.StyleProperty &&
            dp != FrameworkElement.DefaultStyleKeyProperty &&
            dp != FrameworkElement.OverridesDefaultStyleProperty &&
            dp != Control.TemplateProperty &&
            dp != ContentPresenter.TemplateProperty;

        // Trigger storage management for explicit/implicit styles
        internal static readonly UncommonField<TriggerStorage> TriggerStorageField = new();

        // Trigger storage management for theme (default) styles
        internal static readonly UncommonField<TriggerStorage> ThemeStyleTriggerStorageField = new();

        private static void InitializeTriggerStorage(FrameworkElement fe, Style style)
        {
            var storage = new TriggerStorage(fe, style);
            TriggerStorageField.SetValue(fe, storage);
            storage.Initialize();
        }

        private static void CleanupTriggerStorage(FrameworkElement fe)
        {
            var storage = TriggerStorageField.GetValue(fe);
            if (storage is not null)
            {
                storage.Cleanup();
                TriggerStorageField.ClearValue(fe);
            }
        }

        private static void InitializeThemeStyleTriggerStorage(FrameworkElement fe, Style style)
        {
            var storage = new TriggerStorage(fe, style, isThemeStyle: true);
            ThemeStyleTriggerStorageField.SetValue(fe, storage);
            storage.Initialize();
        }

        private static void CleanupThemeStyleTriggerStorage(FrameworkElement fe)
        {
            var storage = ThemeStyleTriggerStorageField.GetValue(fe);
            if (storage is not null)
            {
                storage.Cleanup();
                ThemeStyleTriggerStorageField.ClearValue(fe);
            }
        }

        /// <summary>
        /// Called when a dependency property value changes on an element that may have style triggers.
        /// </summary>
        internal static void OnPropertyChanged(FrameworkElement fe, DependencyProperty dp)
        {
            TriggerStorageField.GetValue(fe)?.OnPropertyChanged(dp);
            ThemeStyleTriggerStorageField.GetValue(fe)?.OnPropertyChanged(dp);
        }
    }
}
