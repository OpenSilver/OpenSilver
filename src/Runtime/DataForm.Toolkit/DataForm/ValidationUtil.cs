//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace System.Windows.Controls
{
    /// <summary>
    /// Utility class that contains static methods for DataForm validation purposes.
    /// </summary>
    internal static class ValidationUtil
    {
        /// <summary>
        /// Creates a new Binding that is a shallow copy of the source Binding.
        /// </summary>
        /// <param name="source">The Binding to copy.</param>
        /// <returns>The copied Binding.</returns>
        public static Binding CopyBinding(Binding source)
        {
            Binding copy = new Binding();

            if (source == null)
            {
                return copy;
            }

            copy.Converter = source.Converter;
            copy.ConverterCulture = source.ConverterCulture;
            copy.ConverterParameter = source.ConverterParameter;
            copy.FallbackValue = source.FallbackValue;
            copy.Mode = source.Mode;
            copy.NotifyOnValidationError = source.NotifyOnValidationError;
            copy.Path = source.Path;
            copy.StringFormat = source.StringFormat;
            copy.TargetNullValue = source.TargetNullValue;
            copy.UpdateSourceTrigger = source.UpdateSourceTrigger;
            copy.ValidatesOnExceptions = source.ValidatesOnExceptions;
            copy.BindsDirectlyToSource = source.BindsDirectlyToSource;
            copy.ValidatesOnDataErrors = source.ValidatesOnDataErrors;
            copy.ValidatesOnNotifyDataErrors = source.ValidatesOnNotifyDataErrors;

            // Binding keeps track of which of the three setters for
            // ElementName, RelativeSource, and Source have been called.
            // Calling any two of the setters, even if the value passed in is null,
            // will raise an exception.  For that reason, we must check for null
            // for these properties to ensure that we only call the setter when we should.
            if (source.ElementName != null)
            {
                copy.ElementName = source.ElementName;
            }
            else if (source.RelativeSource != null)
            {
                copy.RelativeSource = source.RelativeSource;
            }
            else if (source.Source != null)
            {
                copy.Source = source.Source;
            }


            return copy;
        }

        /// <summary>
        /// Returns whether or not a DependencyObject has errors.
        /// </summary>
        /// <param name="element">The element to test.</param>
        /// <returns>Whether or not it has errors.</returns>
        public static bool ElementHasErrors(DependencyObject element)
        {
            if (element == null)
            {
                return false;
            }

            if (Validation.GetHasError(element))
            {
                return true;
            }
            else
            {
                int childrenCount = VisualTreeHelper.GetChildrenCount(element);

                for (int i = 0; i < childrenCount; i++)
                {
                    DependencyObject childElement = VisualTreeHelper.GetChild(element, i);

                    DataForm childDataForm = childElement as DataForm;

                    // If we've found a child DataForm, validate it instead of continuing.
                    if (childDataForm != null)
                    {
                        childDataForm.ValidateItem();

                        if (!childDataForm.IsItemValid)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (ElementHasErrors(childElement))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Gets the list of binding expressions for the given element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The list of binding expressions.</returns>
        public static IList<BindingExpression> GetBindingExpressionsForElement(FrameworkElement element)
        {
            List<BindingExpression> bindingExpressions = new List<BindingExpression>();

            if (element == null)
            {
                return bindingExpressions;
            }

            List<DependencyProperty> dependencyProperties = GetDependencyPropertiesForElement(element);
            Debug.Assert(dependencyProperties != null, "GetDependencyPropertiesForElement() should never return null.");

            foreach (DependencyProperty dependencyProperty in dependencyProperties)
            {
                if (dependencyProperty != null)
                {
                    BindingExpression bindingExpression = element.GetBindingExpression(dependencyProperty);

                    if (bindingExpression != null &&
                        bindingExpression.ParentBinding != null &&
                        bindingExpression.ParentBinding.Path != null &&
                        !string.IsNullOrEmpty(bindingExpression.ParentBinding.Path.Path) &&
                        bindingExpression.ParentBinding.Mode == BindingMode.TwoWay)
                    {
                        bindingExpressions.Add(bindingExpression);
                    }
                }
            }

            int childrenCount = VisualTreeHelper.GetChildrenCount(element);

            for (int i = 0; i < childrenCount; i++)
            {
                FrameworkElement childElement = VisualTreeHelper.GetChild(element, i) as FrameworkElement;

                // Stop if we've found a child DataForm or DataField.
                if (childElement != null && childElement.GetType() != typeof(DataForm) && childElement.GetType() != typeof(DataField))
                {
                    bindingExpressions.AddRange(GetBindingExpressionsForElement(childElement));
                }
            }

            return bindingExpressions;
        }

        /// <summary>
        /// Searches through all Bindings on the specified element and returns a list of BindingInfo objects
        /// for each Binding that matches the specified criteria.
        /// </summary>
        /// <param name="element">FrameworkElement to search</param>
        /// <param name="dataItem">Only return Bindings with a context element equal to this object</param>
        /// <param name="twoWayOnly">If true, only returns TwoWay Bindings</param>
        /// <param name="searchChildren">If true, searches child elements for Bindings</param>
        /// <param name="excludedTypes">The Binding search will skip all of these Types</param>
        /// <returns>List of BindingInfo for every Binding found</returns>
        public static List<DataFormBindingInfo> GetDataFormBindingInfo(this FrameworkElement element, object dataItem, bool twoWayOnly, bool searchChildren, params Type[] excludedTypes)
        {
            List<DataFormBindingInfo> bindingData = new List<DataFormBindingInfo>();

            if (!searchChildren)
            {
                if (excludedTypes != null)
                {
                    foreach (Type excludedType in excludedTypes)
                    {
                        if (excludedType != null && excludedType.IsInstanceOfType(element))
                        {
                            return bindingData;
                        }
                    }
                }
                return element.GetDataFormBindingInfoOfSingleElement(element.DataContext ?? dataItem, dataItem, twoWayOnly);
            }

            Stack<DependencyObject> children = new Stack<DependencyObject>();
            Stack<object> dataContexts = new Stack<object>();
            children.Push(element);
            dataContexts.Push(element.DataContext ?? dataItem);

            while (children.Count != 0)
            {
                bool searchChild = true;
                DependencyObject child = children.Pop();
                object inheritedDataContext = dataContexts.Pop();
                object dataContext = inheritedDataContext;

                // Skip this particular child element if it is one of the excludedTypes
                if (excludedTypes != null)
                {
                    foreach (Type excludedType in excludedTypes)
                    {
                        if (excludedType != null && excludedType.IsInstanceOfType(child))
                        {
                            searchChild = false;
                            break;
                        }
                    }
                }

                // Add the bindings of the child element and push its children onto the stack of remaining elements to search
                if (searchChild)
                {
                    FrameworkElement childElement = child as FrameworkElement;
                    if (childElement != null)
                    {
                        dataContext = childElement.DataContext ?? inheritedDataContext;
                        bindingData.AddRange(childElement.GetDataFormBindingInfoOfSingleElement(inheritedDataContext, dataItem, twoWayOnly));
                    }

                    int childrenCount = VisualTreeHelper.GetChildrenCount(child);
                    for (int childIndex = 0; childIndex < childrenCount; childIndex++)
                    {
                        children.Push(VisualTreeHelper.GetChild(child, childIndex));
                        dataContexts.Push(dataContext);
                    }
                }
            }

            return bindingData;
        }

        /// <summary>
        /// Gets the list of dependency properties for the given element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The list of dependency properties.</returns>
        public static List<DependencyProperty> GetDependencyPropertiesForElement(FrameworkElement element)
        {
            List<DependencyProperty> dependencyProperties = new List<DependencyProperty>();

            if (element == null)
            {
                return dependencyProperties;
            }

            bool isBlocklisted =
                element is Panel || element is Button || element is Image || element is ScrollViewer || element is TextBlock ||
                element is Border || element is Shape || element is ContentPresenter || element is RangeBase;

            if (!isBlocklisted)
            {
                FieldInfo[] fields = element.GetType().GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);

                if (fields != null)
                {
                    foreach (FieldInfo field in fields)
                    {
                        if (field.FieldType == typeof(DependencyProperty))
                        {
                            dependencyProperties.Add((DependencyProperty)field.GetValue(null));
                        }
                    }
                }
            }

            return dependencyProperties;
        }

        /// <summary>
        /// Updates the source on the bindings for a given element.
        /// </summary>
        /// <param name="element">The element.</param>
        public static void UpdateSourceOnElementBindings(FrameworkElement element)
        {
            List<DependencyProperty> dependencyProperties = GetDependencyPropertiesForElement(element);
            Debug.Assert(dependencyProperties != null, "GetDependencyPropertiesForElement() should never return null.");

            foreach (DependencyProperty dependencyProperty in dependencyProperties)
            {
                if (dependencyProperty != null)
                {
                    BindingExpression bindingExpression = element.GetBindingExpression(dependencyProperty);

                    if (bindingExpression != null)
                    {
                        bindingExpression.UpdateSource();
                    }
                }
            }
        }

#region Private Methods

        /// <summary>
        /// Gets a list of active bindings on the specified FrameworkElement.  Bindings are gathered
        /// according to the same conditions BindingGroup uses to find bindings of descendent elements
        /// within the visual tree.
        /// </summary>
        /// <param name="element">Root FrameworkElement to search under</param>
        /// <param name="inheritedDataContext">DomainContext of the element's parent</param>
        /// <param name="dataItem">Target DomainContext</param>
        /// <param name="twoWayOnly">If true, only returns TwoWay Bindings</param>
        /// <returns>The list of active bindings.</returns>
        private static List<DataFormBindingInfo> GetDataFormBindingInfoOfSingleElement(this FrameworkElement element, object inheritedDataContext, object dataItem, bool twoWayOnly)
        {
            // Now see which of the possible dependency properties are being used
            List<DataFormBindingInfo> bindingData = new List<DataFormBindingInfo>();
            foreach (DependencyProperty bindingTarget in ValidationUtil.GetDependencyPropertiesForElement(element))
            {
                // We add bindings according to the same conditions as BindingGroups:
                //    Element.Binding.Mode == TwoWay
                //    Element.Binding.Source == null
                //    DataItem == ContextElement.DataContext where:
                //      If Element is ContentPresenter and TargetProperty is Content, ContextElement = Element.Parent
                //      Else if TargetProperty is DomainContext, ContextElement = Element.Parent
                //      Else ContextElement = Element
                BindingExpression bindingExpression = element.GetBindingExpression(bindingTarget);
                if (bindingExpression != null
                    && bindingExpression.ParentBinding != null
                    && (!twoWayOnly || bindingExpression.ParentBinding.Mode == BindingMode.TwoWay)
                    && bindingExpression.ParentBinding.Source == null)
                {
                    object dataContext;
                    if (bindingTarget == FrameworkElement.DataContextProperty
                        || (element is ContentPresenter && bindingTarget == ContentPresenter.ContentProperty))
                    {
                        dataContext = inheritedDataContext;
                    }
                    else
                    {
                        dataContext = element.DataContext ?? inheritedDataContext;
                    }
                    if (dataItem == dataContext)
                    {
                        bindingData.Add(new DataFormBindingInfo(bindingExpression, bindingTarget, element));
                    }
                }
            }
            return bindingData;
        }

#endregion Private Methods
    }
}
