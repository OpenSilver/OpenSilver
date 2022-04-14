﻿// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------
namespace Microsoft.Expression.Interactivity.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.Windows.Interactivity;

#if MIGRATION
    using System.Windows;
    using System.Windows.Data;
#else
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Data;
#endif

    /// <summary>
    /// Represents a trigger that performs actions when the bound data meets a specified condition.
    /// </summary>
    public class DataTrigger : PropertyChangedTrigger
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(DataTrigger), new PropertyMetadata(OnValueChanged));
        public static readonly DependencyProperty ComparisonProperty = DependencyProperty.Register("Comparison", typeof(ComparisonConditionType), typeof(DataTrigger), new PropertyMetadata(OnComparisonChanged));

        /// <summary>
        /// Gets or sets the value to be compared with the property value of the data object. This is a dependency property.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "This matches the structure of the WPF DataTrigger.")]
        public object Value
        {
            get { return this.GetValue(ValueProperty); }
            set { this.SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Gets or sets the type of comparison to be performed between the specified values. This is a dependency property.
        /// </summary>
        public ComparisonConditionType Comparison
        {
            get { return (ComparisonConditionType)this.GetValue(ComparisonProperty); }
            set { this.SetValue(ComparisonProperty, value); }
        }

        public DataTrigger()
        {
        }

        /// <summary>
        /// Called when the binding property has changed. 
        /// UA_REVIEW:chabiss
        /// </summary>
        /// <param name="args"><see cref="T:System.Windows.DependencyPropertyChangedEventArgs"/> argument.</param>
        protected override void EvaluateBindingChange(object args)
        {
            if (this.Compare())
            {
                // Fire the actions when the binding data has changed
                this.InvokeActions(args);
            }
        }

        private static void OnValueChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            DataTrigger trigger = (DataTrigger)sender;
            trigger.EvaluateBindingChange(args);
        }

        private static void OnComparisonChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            DataTrigger trigger = (DataTrigger)sender;
            trigger.EvaluateBindingChange(args);
        }

        private bool Compare()
        {
            if (this.AssociatedObject != null)
            {
                return ComparisonLogic.EvaluateImpl(this.Binding, this.Comparison, this.Value);
            }
            return false;
        }
    }
}
