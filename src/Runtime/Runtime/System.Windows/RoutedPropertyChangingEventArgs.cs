// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace System.Windows.Controls
{
    /// <summary>
    /// Provides event data for various routed events that track property values
    /// changing.  Typically the events denote a cancellable action.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the value for the dependency property that is changing.
    /// </typeparam>
    /// <QualityBand>Preview</QualityBand>
    public class RoutedPropertyChangingEventArgs<T> : RoutedEventArgs
    {
        /// <summary>
        /// Gets the <see cref="T:System.Windows.DependencyProperty" />
        /// identifier for the property that is changing.
        /// </summary>
        /// <value>
        /// The <see cref="T:System.Windows.DependencyProperty" /> identifier
        /// for the property that is changing.
        /// </value>
        public DependencyProperty Property { get; private set; }

        /// <summary>
        /// Gets a value that reports the previous value of the changing
        /// property.
        /// </summary>
        /// <value>
        /// The previous value of the changing property.
        /// </value>
        public T OldValue { get; private set; }

        /// <summary>
        /// Gets or sets a value that reports the new value of the changing
        /// property, assuming that the property change is not cancelled.
        /// </summary>
        /// <value>
        /// The new value of the changing property.
        /// </value>
        public T NewValue { get; set; }

        /// <summary>
        /// Gets a value indicating whether the property change that originated
        /// the RoutedPropertyChanging event is cancellable.
        /// </summary>
        /// <value>
        /// True if the property change is cancellable. false if the property
        /// change is not cancellable.
        /// </value>
        public bool IsCancelable { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the property change that
        /// originated the RoutedPropertyChanging event should be cancelled.
        /// </summary>
        /// <value>
        /// True to cancel the property change; this resets the property to
        /// <see cref="P:System.Windows.Controls.RoutedPropertyChangingEventArgs`1.OldValue" />.
        /// false to not cancel the property change; the value changes to
        /// <see cref="P:System.Windows.Controls.RoutedPropertyChangingEventArgs`1.NewValue" />.
        /// </value>
        /// <exception cref="T:System.InvalidOperationException">
        /// Attempted to cancel in an instance where
        /// <see cref="P:System.Windows.Controls.RoutedPropertyChangingEventArgs`1.IsCancelable" />
        /// is false.
        /// </exception>
        public bool Cancel
        {
            get { return _cancel; }
            set
            {
                if (IsCancelable)
                {
                    _cancel = value;
                }
                else if (value)
                {
                    throw new InvalidOperationException("The RoutedPropertyChangingEvent cannot be canceled!");
                }
            }
        }

        /// <summary>
        /// Private member variable for Cancel property.
        /// </summary>
        private bool _cancel;

        /// <summary>
        /// Gets or sets a value indicating whether internal value coercion is
        /// acting on the property change that originated the
        /// RoutedPropertyChanging event.
        /// </summary>
        /// <value>
        /// True if coercion is active. false if coercion is not active.
        /// </value>
        /// <remarks>
        /// This is a total hack to work around the class hierarchy for Value
        /// coercion in NumericUpDown.
        /// </remarks>
        public bool InCoercion { get; set; }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:System.Windows.Controls.RoutedPropertyChangingEventArgs`1" />
        /// class.
        /// </summary>
        /// <param name="property">
        /// The <see cref="T:System.Windows.DependencyProperty" /> identifier
        /// for the property that is changing.
        /// </param>
        /// <param name="oldValue">The previous value of the property.</param>
        /// <param name="newValue">
        /// The new value of the property, assuming that the property change is
        /// not cancelled.
        /// </param>
        /// <param name="isCancelable">
        /// True if the property change is cancellable by setting
        /// <see cref="P:System.Windows.Controls.RoutedPropertyChangingEventArgs`1.Cancel" />
        /// to true in event handling. false if the property change is not
        /// cancellable.
        /// </param>
        public RoutedPropertyChangingEventArgs(
            DependencyProperty property,
            T oldValue,
            T newValue,
            bool isCancelable)
        {
            Property = property;
            OldValue = oldValue;
            NewValue = newValue;
            IsCancelable = isCancelable;
            Cancel = false;
        }
    }
}