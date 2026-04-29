
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

using OpenSilver.Internal;
using OpenSilver.Internal.Data;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace System.Windows
{
    internal static class StyleHelper
    {
        internal const string SelfName = "~Self";

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
            CleanupStyleTriggerStorage(fe);

            UpdateInstanceData(fe, oldStyle, effectiveStyle, _setLocalStyleValueDelegate);

            // Initialize triggers for the new style
            if (effectiveStyle is not null && effectiveStyle.HasTriggers)
            {
                InitializeStyleTriggerStorage(fe, effectiveStyle);
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
            if (newStyle is not null && newStyle.HasTriggers)
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
                CleanupStyleTriggerStorage(fe);

                UpdateInstanceData(fe, oldStyle, newStyle, _setLocalStyleValueDelegate);

                // Initialize triggers for the new implicit style
                if (newStyle is not null && newStyle.HasTriggers)
                {
                    InitializeStyleTriggerStorage(fe, newStyle);
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
                    object value = ResolveSetterValue(fe, dp, pValue.Value);
                    setValue(fe, dp, value);
                }
            }
        }

        internal static object ResolveSetterValue(DependencyObject target, DependencyProperty dp, object value)
        {
            return value switch
            {
                BindingBase bindingBase => bindingBase.CreateBindingExpression(target, dp, null),
                DynamicResourceExtension dynamicResource => new ResourceReferenceExpression(dynamicResource.ResourceKey ??
                    throw new InvalidOperationException(Strings.MarkupExtensionResourceKey)),
                ResponsiveExtension responsive => responsive.CreateResponsiveExpression(target, dp),
                _ => value,
            };
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
            if (themeStyleKey is not null && !overridesDefaultStyle)
            {
                // Regular lookup based on the DefaultStyleKey. Involves locking and Hashtable lookup
                if (XamlResources.FindResourceInGenericXaml(themeStyleKey) is object styleLookup)
                {
                    if (styleLookup is not Style style)
                    {
                        throw new InvalidOperationException(string.Format(Strings.SystemResourceForTypeIsNotStyle, themeStyleKey));
                    }

                    newThemeStyle = style;
                }

                if (newThemeStyle is null)
                {
                    // No style in system resources, try to retrieve the default
                    // style for the target type.

                    if (themeStyleKey is Type typeKey)
                    {
                        if (FrameworkElement.StyleProperty.GetMetadata(typeKey) is PropertyMetadata styleMetadata)
                        {
                            // Have a metadata object, get the default style (if any)
                            newThemeStyle = styleMetadata.DefaultValue as Style;
                        }
                    }
                }
            }

            return newThemeStyle;
        }

        //
        //  This method
        //  1. Updates the template cache for the given fe
        //
        internal static void UpdateTemplateCache(
            FrameworkElement fe,
            FrameworkTemplate oldTemplate,
            FrameworkTemplate newTemplate,
            DependencyProperty templateProperty)
        {
            newTemplate?.Seal();

            // Update the template cache
            fe.TemplateCache = newTemplate;

            FrameworkTemplate.TemplateNameScopeField.ClearValue(fe);
            CleanupTemplateTriggerStorage(fe);

            fe.TemplateChild = null;
        }

        internal static bool ApplyTemplateContent(FrameworkElement container, FrameworkTemplate template)
        {
            Debug.Assert(container is not null, "Must have a non-null TemplatedParent.");
            Debug.Assert(template is not null);

            bool visualsCreated;

            if (template.Template is not null)
            {
                FrameworkElement visualTree = (FrameworkElement)template.Template.LoadContent(container);
                container.TemplateChild = visualTree;

                visualsCreated = visualTree is not null;
            }
            else
            {
                visualsCreated = template.BuildVisualTree(container);
            }

            if (visualsCreated)
            {
                InitializeTemplateTriggerStorage(container, template);
            }

            return visualsCreated;
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

        private static readonly UncommonField<TriggerStorage> TriggerStorageField = new();

        private static void InitializeTemplateTriggerStorage(FrameworkElement fe, FrameworkTemplate template)
        {
            if (template.TriggersInternal is TriggerCollection triggers && triggers.InternalCount > 0)
            {
                var storage = GetOrCreateTriggerStorage(fe, InternalFlags.HasTemplateTriggers);
                storage.TemplateTriggers = new TemplateTriggerStorage(fe, template.TriggersInternal);
                storage.TemplateTriggers.Initialize();
            }
        }

        private static void CleanupTemplateTriggerStorage(FrameworkElement fe)
        {
            if (fe.HasTemplateTriggers)
            {
                var storage = GetOrClearTriggerStorage(fe, InternalFlags.HasTemplateTriggers);
                storage.TemplateTriggers.Cleanup();
                storage.TemplateTriggers = null;
            }
        }

        private static void InitializeStyleTriggerStorage(FrameworkElement fe, Style style)
        {
            var storage = GetOrCreateTriggerStorage(fe, InternalFlags.HasStyleTriggers);
            storage.StyleTriggers = new StyleTriggerStorage(fe, style, false);
            storage.StyleTriggers.Initialize();
        }

        private static void CleanupStyleTriggerStorage(FrameworkElement fe)
        {
            if (fe.HasStyleTriggers)
            {
                var storage = GetOrClearTriggerStorage(fe, InternalFlags.HasStyleTriggers);
                storage.StyleTriggers.Cleanup();
                storage.StyleTriggers = null;
            }
        }

        private static void InitializeThemeStyleTriggerStorage(FrameworkElement fe, Style style)
        {
            var storage = GetOrCreateTriggerStorage(fe, InternalFlags.HasThemeStyleTriggers);
            storage.ThemeStyleTriggers = new StyleTriggerStorage(fe, style, true);
            storage.ThemeStyleTriggers.Initialize();
        }

        private static void CleanupThemeStyleTriggerStorage(FrameworkElement fe)
        {
            if (fe.HasThemeStyleTriggers)
            {
                var storage = GetOrClearTriggerStorage(fe, InternalFlags.HasThemeStyleTriggers);
                storage.ThemeStyleTriggers.Cleanup();
                storage.ThemeStyleTriggers = null;
            }
        }

        private static TriggerStorage GetOrCreateTriggerStorage(FrameworkElement fe, InternalFlags flag)
        {
            TriggerStorage storage;

            if (fe.HasTriggers)
            {
                storage = TriggerStorageField.GetValue(fe);
            }
            else
            {
                storage = new TriggerStorage();
                TriggerStorageField.SetValue(fe, storage);
            }

            fe.WriteInternalFlag(flag, true);

            return storage;
        }

        private static TriggerStorage GetOrClearTriggerStorage(FrameworkElement fe, InternalFlags flag)
        {
            fe.WriteInternalFlag(flag, false);

            TriggerStorage storage;

            if (fe.HasTriggers)
            {
                storage = TriggerStorageField.GetValue(fe);
            }
            else
            {
                TriggerStorageField.ClearValue(fe, out storage);
            }

            return storage;
        }

        internal static void OnPropertyChanged(FrameworkElement fe, DependencyProperty dp)
        {
            // Notify the templated parent's template trigger storage about property changes
            // (for SourceName triggers that reference this element)
            if (fe.TemplatedParent is FrameworkElement templatedParent)
            {
                if (templatedParent.HasTemplateTriggers)
                {
                    TriggerStorageField.GetValue(templatedParent).TemplateTriggers.OnTriggerSourcePropertyInvalidated(fe, dp);
                }
            }

            if (fe.HasTriggers)
            {
                TriggerStorageField.GetValue(fe).OnPropertyChanged(dp);
            }
        }

        internal static bool Match(object state, object triggerValue) => Equals(state, triggerValue);

        internal abstract class DataTriggerBindingHelperBase
        {
            private readonly object _referenceValue;
            private readonly BindingExpressionBase _bindingExpr;
            private ReferenceValueCache _referenceValueCache;
            private object _bindingValue = DependencyProperty.UnsetValue;

            protected DataTriggerBindingHelperBase(object referenceValue, BindingBase bindingBase, FrameworkElement container)
            {
                _referenceValue = referenceValue;
                _referenceValueCache = new ReferenceValueCache(null, null);
                _bindingExpr = BindingExpressionBase.CreateUntargetedBindingExpression(container, bindingBase);
                _bindingExpr.Attach(container);
                _bindingExpr.ValueChanged += OnValueChanged;
                UpdateValue();
            }

            internal void Detach()
            {
                _bindingExpr.ValueChanged -= OnValueChanged;
                _bindingExpr.Detach();
            }

            private void UpdateValue() => _bindingValue = _bindingExpr.GetValue(_bindingExpr.Target, _bindingExpr.TargetProperty);

            private void OnValueChanged(object sender, EventArgs e)
            {
                UpdateValue();
                OnValueChanged();
            }

            internal abstract void OnValueChanged();

            internal bool IsMatch()
            {
                object state = _bindingValue;
                object referenceValue = _referenceValue;
                Type stateType = state?.GetType();

                if (referenceValue is string referenceString && stateType is not null && stateType != typeof(string))
                {
                    Type cachedType = _referenceValueCache.BindingValueType;
                    object cachedValue = _referenceValueCache.ValueAsBindingValueType;

                    if (stateType != cachedType)
                    {
                        // the cached type isn't the current type - refresh the cache

                        cachedValue = referenceValue; // in case of failure

                        if (DefaultValueConverter.GetConverter(stateType) is TypeConverter typeConverter && typeConverter.CanConvertFrom(typeof(string)))
                        {
                            try
                            {
                                cachedValue = typeConverter.ConvertFromString(null, CultureInfo.InvariantCulture, referenceString);
                            }
                            catch (Exception ex)
                            {
                                if (CriticalExceptions.IsCriticalApplicationException(ex))
                                {
                                    throw;
                                }

                                // if the conversion failed, just use the unconverted value
                            }
                        }

                        _referenceValueCache = new ReferenceValueCache(stateType, cachedValue);
                    }

                    referenceValue = cachedValue;
                }

                return Match(state, referenceValue);
            }

            private readonly struct ReferenceValueCache
            {
                public ReferenceValueCache(Type bindingValueType, object valueAsBindingValueType)
                {
                    BindingValueType = bindingValueType;
                    ValueAsBindingValueType = valueAsBindingValueType;
                }

                internal readonly Type BindingValueType;
                internal readonly object ValueAsBindingValueType;
            }
        }
    }
}
