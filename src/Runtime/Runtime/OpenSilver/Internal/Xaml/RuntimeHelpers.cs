
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
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using OpenSilver.Internal.Xaml.Context;

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
        public static T ConvertFromInvariantString<T>(string value)
        {
            Debug.Assert(value is not null);

            if (TypeConverterHelper.GetConverter(typeof(T)) is TypeConverter converter)
            {
                return (T)converter.ConvertFromInvariantString(value);
            }

            if (value is T t)
            {
                return t;
            }

            throw new XamlParseException($"Failed to create a '{typeof(T)}' from the text '{value}'.");
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static TypeConverter GetTypeConverter(Type forType)
        {
            Debug.Assert(forType is not null);

            return TypeConverterHelper.GetConverter(forType);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static DependencyProperty DependencyPropertyFromName(string name, Type type)
        {
            Debug.Assert(name is not null);
            Debug.Assert(type is not null);

            return DependencyProperty.FromName(name, type);
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
        public static void RegisterEventHandler(string handlerName, string eventName, object target, object firstArgument)
        {
            try
            {
                var methodInfo = firstArgument.GetType().GetMethod(handlerName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var eventInfo = target.GetType().GetEvent(eventName);
                Delegate handler = Delegate.CreateDelegate(eventInfo.EventHandlerType,
                                             firstArgument,
                                             methodInfo);
                eventInfo.AddEventHandler(target, handler);
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException($"{handlerName}: {ex.Message}");
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
        public static void XamlContext_SetTemplatedParent(XamlContext context, FrameworkElement element)
        {
            Debug.Assert(context is not null);
            Debug.Assert(element is not null);

            element.SetTemplatedParent(context.TemplateOwnerReference);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void XamlContext_SetTemplatedParent(XamlContext context, IFrameworkElement element)
        {
            Debug.Assert(context is not null);
            Debug.Assert(element is not null);

            // We do not want to share the weak reference here, because IFrameworkElements can be defined
            // outside of OpenSilver, and we cannot ensure that it will not modify the target of the weak
            // reference.
            ((IInternalFrameworkElement)element).SetTemplatedParent(new(context.GetTemplateOwner()));
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void SetTemplatedParent(FrameworkElement element, DependencyObject templatedParent)
        {
            Debug.Assert(element is not null);

            element.SetTemplatedParent(new(templatedParent));
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void SetTemplatedParent(IFrameworkElement element, DependencyObject templatedParent)
        {
            Debug.Assert(element is not null);

            ((IInternalFrameworkElement)element).SetTemplatedParent(new(templatedParent));
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(Helper.ObsoleteMemberMessage + " Use RuntimeHelpers.SetTemplatedParent(IFrameworkElement, DependencyObject) instead.", true)]
        public static void SetTemplatedParent(IFrameworkElement element, IFrameworkElement templatedParent)
        {
            Debug.Assert(element is not null);

            ((IInternalFrameworkElement)element).SetTemplatedParent(new((DependencyObject)templatedParent));
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

            template.Template = new TemplateContent(xamlContext, factory);
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
            Debug.Assert(context is not null);
            Debug.Assert(markupExtension is not null);

            return markupExtension.ProvideValue(context.ServiceProvider);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool TrySetMarkupExtension(object target, DependencyProperty dp, IMarkupExtension<object> markupExtension, out object value)
        {
            Debug.Assert(target is not null);
            Debug.Assert(dp is not null);
            Debug.Assert(markupExtension is not null);

            var serviceProvider = new ServiceProvider(target, dp);

            value = markupExtension.ProvideValue(serviceProvider);

            if (value is BindingBase binding)
            {
                value = binding.ProvideValue(serviceProvider);
            }

            if (value is Expression expression && target is DependencyObject d)
            {
                d.SetValue(dp, expression);
                return true;
            }

            return false;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void XamlContext_SetAnimationContext(XamlContext context, Timeline timeline)
        {
            Debug.Assert(context != null);
            Debug.Assert(timeline != null);

            if (timeline is not Storyboard)
            {
                timeline.NameResolver = context.NameResolver;
            }
        }
    }
}
