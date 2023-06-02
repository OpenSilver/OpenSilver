﻿
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
using System.ComponentModel;
using System.Diagnostics;
using System.Xaml;
using OpenSilver.Internal.Xaml.Context;

#if MIGRATION
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Animation;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media.Animation;
#endif

namespace OpenSilver.Internal.Xaml
{
    /// <summary>
    /// DO NOT USE THIS CLASS IN YOUR CODE. 
    /// This class is publicly exposed because we need to give access to some 
    /// internal features of OpenSilver when converting xaml files to C#.
    /// We reserve ourselves the right to change this class or delete it in
    /// later releases.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class RuntimeHelpers
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static T GetPropertyValue<T>(Type xamlType, string propertyName, string xamlValue, Func<T> fallbackValueFunc)
        {
            ReflectedPropertyData property = TypeConverterHelper.GetProperties(xamlType)[propertyName];
            if (property == null)
            {
                throw new InvalidOperationException(
                    $"Property '{propertyName}' was not found in type '{xamlType}'."
                );
            }

            Debug.Assert(property.PropertyType == typeof(T));

            // Note: here we do not want to use the Converter property as it would return a non-null
            // converter for several known types for which object instantiation can be optimized at
            // compilation and we would waste time parsing strings.
            TypeConverter converter = property.InternalConverter;
            if (converter != null)
            {
                return (T)converter.ConvertFromInvariantString(xamlValue);
            }

            if (fallbackValueFunc != null)
            {
                return fallbackValueFunc();
            }

            throw new InvalidOperationException(
                $"Failed to create a '{typeof(T)}' from the text '{xamlValue}'."
            );
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static object GetPropertyValue(Type xamlType, string propertyName, string xamlValue, Func<object> fallbackValueFunc)
        {
            ReflectedPropertyData property = TypeConverterHelper.GetProperties(xamlType)[propertyName];
            if (property == null)
            {
                throw new InvalidOperationException(
                    $"Property '{propertyName}' was not found in type '{xamlType}'."
                );
            }

            // Note: here we do not want to use the Converter property as it would return a non-null
            // converter for several known types for which object instantiation can be optimized at
            // compilation and we would waste time parsing strings.
            TypeConverter converter = property.InternalConverter;
            if (converter != null)
            {
                return converter.ConvertFromInvariantString(xamlValue);
            }

            if (fallbackValueFunc != null)
            {
                return fallbackValueFunc();
            }

            throw new InvalidOperationException(
                $"Failed to create a '{property.PropertyType}' from the text '{xamlValue}'."
            );
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void InitializeNameScope(DependencyObject dependencyObject)
        {
            Debug.Assert(dependencyObject is IFrameworkElement);

            NameScope.SetNameScope(dependencyObject, new NameScope());
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void RegisterName(DependencyObject dependencyObject, string name, object scopedElement)
        {
            if (scopedElement is DependencyObject)
            {
                INameScope nameScope = NameScope.GetNameScope(dependencyObject);
                if (nameScope != null)
                {
                    nameScope.RegisterName(name, scopedElement);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void XamlContext_RegisterName(XamlContext context, string name, object scopedElement)
        {
            Debug.Assert(context != null && context.ExternalNameScope != null);

            if (scopedElement is DependencyObject)
            {
                context.ExternalNameScope.RegisterName(name, scopedElement);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void SetTemplatedParent(FrameworkElement element, DependencyObject templatedParent)
        {
            element.TemplatedParent = templatedParent;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void SetTemplatedParent(IFrameworkElement element, IFrameworkElement templatedParent)
        {
            ((IInternalFrameworkElement)element).TemplatedParent = (DependencyObject)templatedParent;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static XamlContext Create_XamlContext()
        {
            return new XamlContext();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void SetTemplateContent(FrameworkTemplate template, XamlContext xamlContext, Func<FrameworkElement, XamlContext, FrameworkElement> factory)
        {
            Debug.Assert(template != null);
            Debug.Assert(xamlContext != null);
            Debug.Assert(factory != null);

            template.Template = new TemplateContent(xamlContext, (e, c) => factory((FrameworkElement)e, c));
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void SetTemplateContent(FrameworkTemplate template, XamlContext xamlContext, Func<IFrameworkElement, XamlContext, IFrameworkElement> factory)
        {
            Debug.Assert(template != null);
            Debug.Assert(xamlContext != null);
            Debug.Assert(factory != null);

            template.Template = new TemplateContent(xamlContext, (e, c) => (IInternalFrameworkElement)factory(e, c));
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static T XamlContext_WriteStartObject<T>(XamlContext context, T instance)
        {
            Debug.Assert(context != null);

            context.PushScope();
            context.CurrentInstance = instance;

            // Silverlight does not call BeginInit/EndInit on the root instance of the xaml page.
            if (context.Depth > 1 && instance is ISupportInitialize init)
            {
                init.BeginInit();
            }

            return instance;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void XamlContext_WriteEndObject(XamlContext context)
        {
            Debug.Assert(context != null);

            object currentInstance = context.CurrentInstance;

            // Silverlight does not call BeginInit/EndInit on the root instance of the xaml page.
            if (context.Depth > 1 && currentInstance is ISupportInitialize init)
            {
                init.EndInit();
            }

            context.PopScope();
        }

        [Obsolete("Use RuntimeHelpers.XamlContext_WriteStartObject instead.", true)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static T XamlContext_PushScope<T>(XamlContext context, T instance)
        {
            Debug.Assert(context != null);

            context.PushScope();
            context.CurrentInstance = instance;

            return instance;
        }

        [Obsolete("Use RuntimeHelpers.XamlContext_WriteEndObject instead.", true)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void XamlContext_PopScope(XamlContext context)
        {
            Debug.Assert(context != null);

            context.PopScope();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void XamlContext_SetConnectionId(XamlContext context, int connectionId, object instance)
        {
            Debug.Assert(context != null);

            if (context.RootInstance is IComponentConnector connector)
            {
                connector.Connect(connectionId, instance);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static T CallProvideValue<T>(XamlContext context, IMarkupExtension<T> markupExtension) where T : class
        {
            Debug.Assert(context != null);
            Debug.Assert(markupExtension != null);

            return markupExtension.ProvideValue(new ServiceProviderContext(context));
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void XamlContext_SetAnimationContext(XamlContext context, Timeline timeline)
        {
            Debug.Assert(context != null);
            Debug.Assert(timeline != null);

            if (!(timeline is Storyboard))
            {
                timeline.NameResolver = context.NameResolver;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void UIElement_SetKeepHiddenInFirstRender(UIElement uie, bool value)
        {
            Debug.Assert(uie != null);
            uie.KeepHiddenInFirstRender = value;
        }
    }
}
