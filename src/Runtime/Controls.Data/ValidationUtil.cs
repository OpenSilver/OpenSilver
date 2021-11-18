// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Threading;

#if MIGRATION
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
#else
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endif


#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Static class with methods to help with validation.
    /// </summary>
    internal static class ValidationUtil
    {
        #region Public Methods

        /// <summary>
        /// Adds a new ValidationResult to the collection if an equivalent does not exist.
        /// </summary>
        /// <param name="collection">ValidationResults to search through</param>
        /// <param name="value">ValidationResult to add</param>
        public static void AddIfNew(this ICollection<ValidationResult> collection, ValidationResult value)
        {
            if (!collection.ContainsEqualValidationResult(value))
            {
                collection.Add(value);
            }
        }

        /// <summary>
        /// Performs an action and catches any non-critical exceptions.
        /// </summary>
        /// <param name="action">Action to perform</param>
        public static void CatchNonCriticalExceptions(Action action)
        {
            try
            {
                action();
            }
            catch (Exception exception)
            {
                if (IsCriticalException(exception))
                {
                    throw;
                }
                // Catch any non-critical exceptions
            }
        }

        /// <summary>
        /// Creates a new Binding that is a shallow copy of the source Binding.
        /// </summary>
        /// <param name="source">The Binding to clone</param>
        /// <returns>The cloned Binding</returns>
        public static Binding CloneBinding(Binding source)
        {
            Binding clone = new Binding();

            if (source == null)
            {
                return clone;
            }

            clone.BindsDirectlyToSource = source.BindsDirectlyToSource;
            clone.Converter = source.Converter;
            clone.ConverterCulture = source.ConverterCulture;
            clone.ConverterParameter = source.ConverterParameter;
            clone.FallbackValue = source.FallbackValue;
            clone.Mode = source.Mode;
            clone.NotifyOnValidationError = source.NotifyOnValidationError;
            clone.Path = source.Path;
            clone.StringFormat = source.StringFormat;
            clone.TargetNullValue = source.TargetNullValue;
            clone.UpdateSourceTrigger = source.UpdateSourceTrigger;
            clone.ValidatesOnExceptions = source.ValidatesOnExceptions;
            clone.ValidatesOnDataErrors = source.ValidatesOnDataErrors;
            clone.ValidatesOnNotifyDataErrors = source.ValidatesOnNotifyDataErrors;

            // Binding keeps track of which of the three setters for
            // ElementName, RelativeSource, and Source have been called.
            // Calling any two of the setters, even if the value passed in is null,
            // will raise an exception.  For that reason, we must check for null
            // for these properties to ensure that we only call the setter when we should.
            if (source.ElementName != null)
            {
                clone.ElementName = source.ElementName;
            }
            else if (source.RelativeSource != null)
            {
                clone.RelativeSource = source.RelativeSource;
            }
            else if (source.Source != null)
            {
                clone.Source = source.Source;
            }

            return clone;
        }

        /// <summary>
        /// Determines whether the collection contains an equivalent ValidationResult
        /// </summary>
        /// <param name="collection">ValidationResults to search through</param>
        /// <param name="target">ValidationResult to search for</param>
        /// <returns></returns>
        public static bool ContainsEqualValidationResult(this ICollection<ValidationResult> collection, ValidationResult target)
        {
            return (collection.FindEqualValidationResult(target) != null);
        }

        /// <summary>
        /// Searches a ValidationResult for the specified target member name.  If the target is null
        /// or empty, this method will return true if there are no member names at all.
        /// </summary>
        /// <param name="validationResult">ValidationResult to search.</param>
        /// <param name="target">Member name to search for.</param>
        /// <returns>True if found.</returns>
        public static bool ContainsMemberName(this ValidationResult validationResult, string target)
        {
            int memberNameCount = 0;
            foreach (string memberName in validationResult.MemberNames)
            {
                if (string.Equals(target, memberName))
                {
                    return true;
                }
                memberNameCount++;
            }
            return (memberNameCount == 0 && string.IsNullOrEmpty(target));
        }

        /// <summary>
        /// Finds an equivalent ValidationResult if one exists.
        /// </summary>
        /// <param name="collection">ValidationResults to search through.</param>
        /// <param name="target">ValidationResult to find.</param>
        /// <returns>Equal ValidationResult if found, null otherwise.</returns>
        public static ValidationResult FindEqualValidationResult(this ICollection<ValidationResult> collection, ValidationResult target)
        {
            foreach (ValidationResult oldValidationResult in collection)
            {
                if (oldValidationResult.ErrorMessage == target.ErrorMessage)
                {
                    bool movedOld = true;
                    bool movedTarget = true;
                    IEnumerator<string> oldEnumerator = oldValidationResult.MemberNames.GetEnumerator();
                    IEnumerator<string> targetEnumerator = target.MemberNames.GetEnumerator();
                    while (movedOld && movedTarget)
                    {
                        movedOld = oldEnumerator.MoveNext();
                        movedTarget = targetEnumerator.MoveNext();

                        if (!movedOld && !movedTarget)
                        {
                            return oldValidationResult;
                        }
                        if (movedOld != movedTarget || oldEnumerator.Current != targetEnumerator.Current)
                        {
                            break;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Searches through all Bindings on the specified element and returns a list of BindingInfo objects
        /// for each Binding that matches the specified criteria.
        /// </summary>
        /// <param name="element">FrameworkElement to search</param>
        /// <param name="dataItem">Only return Bindings with a context element equal to this object</param>
        /// <param name="twoWayOnly">If true, only returns TwoWay Bindings</param>
        /// <param name="useBlockList">If true, ignores elements not typically used for input</param>
        /// <param name="searchChildren">If true, only returns TwoWay Bindings</param>
        /// <param name="excludedTypes">The Binding search will skip all of these Types</param>
        /// <returns>List of BindingInfo for every Binding found</returns>
        public static List<BindingInfo> GetBindingInfo(this FrameworkElement element, object dataItem, bool twoWayOnly, bool useBlockList, bool searchChildren, params Type[] excludedTypes)
        {
            List<BindingInfo> bindingData = new List<BindingInfo>();

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
                return element.GetBindingInfoOfSingleElement(element.DataContext ?? dataItem, dataItem, twoWayOnly, useBlockList);
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
                        bindingData.AddRange(childElement.GetBindingInfoOfSingleElement(inheritedDataContext, dataItem, twoWayOnly, useBlockList));
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
        /// Gets a list of the specified FrameworkElement's DependencyProperties. This method will return all
        /// DependencyProperties of the element unless 'useBlockList' is true, in which case all bindings on elements
        /// that are typically not used as input controls will be ignored.
        /// </summary>
        /// <param name="element">FrameworkElement of interest</param>
        /// <param name="useBlockList">If true, ignores elements not typically used for input</param>
        /// <returns>List of DependencyProperties</returns>
        public static List<DependencyProperty> GetDependencyProperties(this FrameworkElement element, bool useBlockList)
        {
            List<DependencyProperty> dependencyProperties = new List<DependencyProperty>();

            bool isBlocklisted = useBlockList &&
                (element is Panel || element is Button || element is Image || element is ScrollViewer || element is TextBlock ||
                 element is Border || element is Shape || element is ContentPresenter);

            if (!isBlocklisted)
            {
                Type type = element.GetType();
                FieldInfo[] fields = type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                foreach (FieldInfo field in fields)
                {
                    if (field.FieldType == typeof(DependencyProperty))
                    {
                        dependencyProperties.Add((DependencyProperty)field.GetValue(null));
                    }
                }
            }

            return dependencyProperties;
        }

        /// <summary>
        /// Determines if the specified exception is un-recoverable.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>True if the process cannot be recovered from the exception.</returns>
        public static bool IsCriticalException(Exception exception)
        {
            return (exception is OutOfMemoryException) ||
                (exception is StackOverflowException) ||
                (exception is AccessViolationException) ||
                (exception is ThreadAbortException);
        }

        #endregion Public Methods

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
        /// <param name="useBlockList">If true, ignores elements not typically used for input</param>
        /// <returns></returns>
        private static List<BindingInfo> GetBindingInfoOfSingleElement(this FrameworkElement element, object inheritedDataContext, object dataItem, bool twoWayOnly, bool useBlockList)
        {
            // Now see which of the possible dependency properties are being used
            List<BindingInfo> bindingData = new List<BindingInfo>();
            foreach (DependencyProperty bindingTarget in element.GetDependencyProperties(useBlockList))
            {
                // We add bindings according to the same conditions as BindingGroups:
                //    Element.Binding.Mode == TwoWay
                //    Element.Binding.Source == null
                //    DataItem == ContextElement.DataContext where:
                //      If Element is ContentPresenter and TargetProperty is Content, ContextElement = Element.Parent
                //      Else if TargetProperty is DomainContext, ContextElement = Element.Parent
                //      Else ContextElement = Element
                //
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
                        bindingData.Add(new BindingInfo(bindingExpression, bindingTarget, element));
                    }
                }
            }
            return bindingData;
        }

        #endregion Private Methods
    }

    /// <summary>
    /// Stores information about a Binding, including the BindingExpression, BindingTarget and associated Element.
    /// </summary>
    internal class BindingInfo
    {
        /// <summary>
        /// Creates a new BindingInfo.
        /// </summary>
        public BindingInfo()
        {
        }

        /// <summary>
        /// Creates a new BindingInfo with the specified BindingExpression, BindingTarget and Element.
        /// </summary>
        /// <param name="bindingExpression">BindingExpression</param>
        /// <param name="bindingTarget">BindingTarget</param>
        /// <param name="element">Element</param>
        public BindingInfo(BindingExpression bindingExpression, DependencyProperty bindingTarget, FrameworkElement element)
        {
            this.BindingExpression = bindingExpression;
            this.BindingTarget = bindingTarget;
            this.Element = element;
        }

        /// <summary>
        /// Gets or sets the BindingExpression.
        /// </summary>
        public BindingExpression BindingExpression { get; set; }

        /// <summary>
        /// Gets or sets the BindingTarget.
        /// </summary>
        public DependencyProperty BindingTarget { get; set; }

        /// <summary>
        /// Gets or sets the Element.
        /// </summary>
        public FrameworkElement Element { get; set; }
    }
}
