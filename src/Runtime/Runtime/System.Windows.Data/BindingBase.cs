
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

using System.Diagnostics;
using System.Reflection;
using System.Windows.Markup;
using System.Xaml.Markup;
using System.Xaml;

namespace System.Windows.Data
{
    /// <summary>
    /// Provides an abstract base class for the Binding class.
    /// </summary>
    public abstract class BindingBase : MarkupExtension
    {
        private bool _isSealed;
        private object _fallbackValue;
        private object _targetNullValue;
        protected string _stringFormat;

        /// <summary>
        /// Initializes a new instance of the BindingBase class.
        /// </summary>
        protected BindingBase() { }

        /// <summary>
        /// Gets or sets the value to use when the binding is unable to return a value.
        /// </summary>
        public object FallbackValue
        {
            get { return _fallbackValue; }
            set { CheckSealed(); _fallbackValue = value; }
        }

        /// <summary>
        /// Gets or sets the value that is used in the target when the value of the source
        /// is null.
        /// </summary>
        public object TargetNullValue
        {
            get { return _targetNullValue; }
            set { CheckSealed(); _targetNullValue = value; }
        }

        /// <summary>
        /// Gets or sets a string that specifies how to format the binding if it displays 
        /// the bound value as a string.
        /// </summary>
        public string StringFormat
        {
            get { return _stringFormat; }
            set { CheckSealed(); _stringFormat = value; }
        }

        protected internal void CheckSealed()
        {
            if (_isSealed)
            {
                throw new InvalidOperationException("Binding cannot be changed after it has been used.");
            }
        }

        internal void Seal()
        {
            _isSealed = true;
        }

        /// <summary>
        /// Returns an object that should be set on the property where this binding and extension
        /// are applied.
        /// </summary>
        /// <param name="serviceProvider">
        /// The object that can provide services for the markup extension. May be null; see
        /// the Remarks section for more information.
        /// </param>
        /// <returns>
        /// The value to set on the binding target property.
        /// </returns>
        public sealed override object ProvideValue(IServiceProvider serviceProvider)
        {
            // Binding a property value only works on DependencyObject and DependencyProperties.
            // For all other cases, just return this Binding object as the value.

            if (serviceProvider == null)
            {
                return this;
            }

            // Bindings are not allowed On CLR props except for Setter

            CheckCanReceiveMarkupExtension(this,
                serviceProvider,
                out DependencyObject targetDependencyObject,
                out DependencyProperty targetDependencyProperty);

            if (targetDependencyObject == null || targetDependencyProperty == null)
            {
                return this;
            }

            // delegate real work to subclass
            return CreateBindingExpression(targetDependencyObject, targetDependencyProperty);
        }

        /// <summary>
        /// Create an appropriate expression for this Binding, to be attached
        /// to the given DependencyProperty on the given DependencyObject.
        /// </summary>
        internal abstract BindingExpressionBase CreateBindingExpressionOverride(DependencyObject targetObject, DependencyProperty targetProperty);

        /// <summary>
        /// Create an appropriate expression for this Binding, to be attached
        /// to the given DependencyProperty on the given DependencyObject.
        /// </summary>
        internal BindingExpressionBase CreateBindingExpression(DependencyObject targetObject, DependencyProperty targetProperty)
        {
            _isSealed = true;
            return CreateBindingExpressionOverride(targetObject, targetProperty);
        }

        /// <summary>
        /// Checks if the given IProvideValueTarget can receive
        /// a DynamicResource or Binding MarkupExtension.
        /// </summary>
        internal static void CheckCanReceiveMarkupExtension(
            MarkupExtension markupExtension,
            IServiceProvider serviceProvider,
            out DependencyObject targetDependencyObject,
            out DependencyProperty targetDependencyProperty)
        {
            targetDependencyObject = null;
            targetDependencyProperty = null;

            if (serviceProvider.GetService(typeof(IProvideValueTarget)) is not IProvideValueTarget provideValueTarget)
            {
                return;
            }

            if (provideValueTarget.TargetObject is not object targetObject)
            {
                return;
            }

            if (provideValueTarget.TargetProperty is object targetProperty)
            {
                targetDependencyProperty = targetProperty as DependencyProperty;
                if (targetDependencyProperty is not null)
                {
                    // This is the DependencyProperty case

                    targetDependencyObject = targetObject as DependencyObject;
                    Debug.Assert(targetDependencyObject is not null, "DependencyProperties can only be set on DependencyObjects");
                }
                else
                {
                    if (targetProperty is MemberInfo targetMember)
                    {
                        // This is the Clr Property case
                        PropertyInfo propertyInfo = targetMember as PropertyInfo;
                        Type targetType = targetObject.GetType();

                        // Setters, Triggers, DataTriggers & Conditions are the special cases of
                        // Clr properties where DynamicResource & Bindings are allowed. Normally
                        // these cases are handled by the parser calling the appropriate
                        // ReceiveMarkupExtension method.  But a custom MarkupExtension
                        // that delegates ProvideValue will end up here (see Dev11 117372).
                        // So we handle it similarly to how the parser does it.

                        EventHandler<XamlSetMarkupExtensionEventArgs> setMarkupExtension
                            = LookupSetMarkupExtensionHandler(targetType);

                        if (setMarkupExtension is not null && propertyInfo is not null)
                        {
                            if (serviceProvider.GetService(typeof(IXamlSchemaContextProvider)) is IXamlSchemaContextProvider scp)
                            {
                                XamlSchemaContext sc = scp.SchemaContext;
                                if (sc.GetXamlType(targetType) is XamlType xt && xt.GetMember(propertyInfo.Name) is XamlMember member)
                                {
                                    var eventArgs = new XamlSetMarkupExtensionEventArgs(member, markupExtension, serviceProvider);

                                    // ask the target object whether it accepts MarkupExtension
                                    setMarkupExtension(targetObject, eventArgs);
                                    if (eventArgs.Handled)
                                    {
                                        return;     // if so, all is well
                                    }
                                }
                            }
                        }

                        // Find the MemberType

                        Debug.Assert(targetMember is PropertyInfo || targetMember is MethodInfo,
                            "TargetMember is either a Clr property or an attached static settor method");

                        Type memberType;

                        if (propertyInfo is not null)
                        {
                            memberType = propertyInfo.PropertyType;
                        }
                        else
                        {
                            MethodInfo methodInfo = (MethodInfo)targetMember;
                            ParameterInfo[] parameterInfos = methodInfo.GetParameters();
                            Debug.Assert(parameterInfos.Length == 2, "The signature of a static settor must contain two parameters");
                            memberType = parameterInfos[1].ParameterType;
                        }

                        // Check if the MarkupExtensionType is assignable to the given MemberType
                        // This check is to allow properties such as the following
                        // - DataTrigger.Binding
                        // - Condition.Binding
                        // - HierarchicalDataTemplate.ItemsSource
                        // - GridViewColumn.DisplayMemberBinding

                        if (!typeof(MarkupExtension).IsAssignableFrom(memberType) ||
                             !memberType.IsAssignableFrom(markupExtension.GetType()))
                        {
                            throw new XamlParseException(
                                $"A '{markupExtension.GetType().Name}' cannot be set on the '{targetMember.Name}' property of type '{targetType.Name}'. A '{markupExtension.GetType().Name}' can only be set on a DependencyProperty of a DependencyObject.");
                        }
                    }
                }
            }
        }

        private static EventHandler<XamlSetMarkupExtensionEventArgs> LookupSetMarkupExtensionHandler(Type type)
        {
            if (typeof(Setter) == type)
            {
                return Setter.ReceiveMarkupExtension;
            }
            return null;
        }
    }
}
