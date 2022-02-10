// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

#if MIGRATION
using System.Windows.Data;
#else
using Windows.UI.Xaml.Data;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// A framework element that permits a binding to be evaluated in a new data
    /// context leaf node.
    /// </summary>
    /// <typeparam name="T">The type of dynamic binding to return.</typeparam>
    internal partial class BindingEvaluator<T> : FrameworkElement
    {
        /// <summary>
        /// Gets or sets the string value binding used by the control.
        /// </summary>
        private Binding _binding;

#region public T Value
        /// <summary>
        /// Gets or sets the data item string value.
        /// </summary>
        public T Value
        {
            get { return (T)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Identifies the Value dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value",
                typeof(T),
                typeof(BindingEvaluator<T>),
                new PropertyMetadata(default(T)));

#endregion public string Value

        /// <summary>
        /// Gets or sets the value binding.
        /// </summary>
        public Binding ValueBinding
        {
            get { return _binding; }
            set
            {
                _binding = value;
                SetBinding(ValueProperty, _binding);
            }
        }

        /// <summary>
        /// Initializes a new instance of the BindingEvaluator class.
        /// </summary>
        public BindingEvaluator()
        {
        }

        /// <summary>
        /// Initializes a new instance of the BindingEvaluator class,
        /// setting the initial binding to the provided parameter.
        /// </summary>
        /// <param name="binding">The initial string value binding.</param>
        public BindingEvaluator(Binding binding)
        {
            SetBinding(ValueProperty, binding);
        }

        /// <summary>
        /// Clears the data context so that the control does not keep a
        /// reference to the last-looked up item.
        /// </summary>
        public void ClearDataContext()
        {
            DataContext = null;
        }

        /// <summary>
        /// Updates the data context of the framework element and returns the 
        /// updated binding value.
        /// </summary>
        /// <param name="o">The object to use as the data context.</param>
        /// <param name="clearDataContext">If set to true, this parameter will
        /// clear the data context immediately after retrieving the value.</param>
        /// <returns>Returns the evaluated T value of the bound dependency
        /// property.</returns>
        public T GetDynamicValue(object o, bool clearDataContext)
        {
            DataContext = o;
            T value = Value;
            if (clearDataContext)
            {
                DataContext = null;
            }
            return value;
        }

        /// <summary>
        /// Updates the data context of the framework element and returns the 
        /// updated binding value.
        /// </summary>
        /// <param name="o">The object to use as the data context.</param>
        /// <returns>Returns the evaluated T value of the bound dependency
        /// property.</returns>
        public T GetDynamicValue(object o)
        {
            DataContext = o;
            return Value;
        }
    }
}