// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows.Data;

namespace System.Windows.Controls
{
    /// <summary>
    /// A framework element that permits a binding to be evaluated in a new data
    /// context leaf node. Uses the BindingSource to do the evaluation.
    /// </summary>
    /// <typeparam name="T">The type of dynamic binding to return.</typeparam>
    /// <remarks>Mimicks the class BindingEvaluator that uses DataContext
    /// to do evaluation.</remarks>
    internal class BindingSourceEvaluator<T> : FrameworkElement
    {
        #region public T Value
        /// <summary>
        /// Gets the data item string value.
        /// </summary>
        private T Value
        {
            get { return (T)GetValue(ValueProperty); }
        }

        /// <summary>
        /// Identifies the Value dependency property.
        /// </summary>
        private static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value",
                typeof(T),
                typeof(BindingSourceEvaluator<T>),
                new PropertyMetadata(default(T)));
        #endregion public T Value

        /// <summary>
        /// Gets the value binding that is used as a template
        /// for the actual evaluation.
        /// </summary>
        public Binding ValueBinding { get; private set; }

        /// <summary>
        /// Initializes a new instance of the BindingSourceEvaluator class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        public BindingSourceEvaluator(Binding binding)
        {
            ValueBinding = binding;
        }

        /// <summary>
        /// Evaluates the specified source.
        /// </summary>
        /// <param name="source">The object used as a source for the
        /// evaluation.</param>
        /// <returns>The evaluated binding.</returns>
        /// <remarks>Only uses Path, Converter, ConverterCulture and
        /// ConverterParameter.</remarks>
        public T GetDynamicValue(object source)
        {
            // a binding cannot be altered after it has been used.
            Binding copy = new Binding()
            {
                BindsDirectlyToSource = ValueBinding.BindsDirectlyToSource,
                Converter = ValueBinding.Converter,
                ConverterCulture = ValueBinding.ConverterCulture,
                ConverterParameter = ValueBinding.ConverterParameter,
                FallbackValue = ValueBinding.FallbackValue,
                Mode = ValueBinding.Mode,
                NotifyOnValidationError = ValueBinding.NotifyOnValidationError,
                Path = ValueBinding.Path,
                StringFormat = ValueBinding.StringFormat,
                TargetNullValue = ValueBinding.TargetNullValue,
                UpdateSourceTrigger = ValueBinding.UpdateSourceTrigger,
                ValidatesOnDataErrors = ValueBinding.ValidatesOnDataErrors,
                ValidatesOnExceptions = ValueBinding.ValidatesOnExceptions,
                ValidatesOnNotifyDataErrors = ValueBinding.ValidatesOnNotifyDataErrors,
            };
            copy.Source = source;

            SetBinding(ValueProperty, copy);
            T evaluatedValue = Value;
            ClearValue(ValueProperty);
            return evaluatedValue;
        }
    }
}
