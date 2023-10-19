// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics;
using System.Globalization;
using System.Windows.Input;

namespace System.Windows.Controls.Primitives
{
    /// <summary>
    /// Represents the text input of a <see cref="DatePicker" />.
    /// </summary>
    /// <QualityBand>Mature</QualityBand>
    [TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateMouseOver, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateDisabled, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateUnfocused, GroupName = VisualStates.GroupFocus)]
    [TemplateVisualState(Name = VisualStates.StateFocused, GroupName = VisualStates.GroupFocus)]
    [TemplateVisualState(Name = VisualStates.StateUnwatermarked, GroupName = VisualStates.GroupWatermark)]
    [TemplateVisualState(Name = VisualStates.StateWatermarked, GroupName = VisualStates.GroupWatermark)]
    [TemplatePart(Name = DatePickerTextBox.ElementContentName, Type = typeof(ContentControl))]
    public sealed partial class DatePickerTextBox : TextBox
    {
        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private const string ElementContentName = "Watermark";

        /// <summary>
        /// Initializes a new instance of the <see cref="DatePickerTextBox" /> class.
        /// </summary>
        public DatePickerTextBox()
        {
            DefaultStyleKey = typeof(DatePickerTextBox);
            SetDefaults();

            this.MouseEnter += OnMouseEnter;
            this.MouseLeave += OnMouseLeave;
            this.Loaded += OnLoaded;
            this.LostFocus += OnLostFocus;
            this.GotFocus += OnGotFocus;
            this.TextChanged += OnTextChanged;
            this.IsEnabledChanged += new DependencyPropertyChangedEventHandler(OnIsEnabledChanged);
        }

        /// <summary>
        /// Gets or sets Inherited code: Requires comment.
        /// </summary>
        internal ContentControl ElementContent { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Inherited code: Requires comment.
        /// </summary>
        internal bool IsHovered { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Inherited code: Requires comment.
        /// </summary>
        internal bool HasFocusInternal { get; set; }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        internal void OnLoaded(object sender, RoutedEventArgs e)
        {
            ApplyTemplate();
            ChangeVisualState(false);
        }

        /// <summary>
        /// Change to the correct visual state for the textbox.
        /// </summary>
        internal void ChangeVisualState()
        {
            ChangeVisualState(true);
        }

        /// <summary>
        /// Change to the correct visual state for the textbox.
        /// </summary>
        /// <param name="useTransitions">
        /// True to use transitions when updating the visual state, false to
        /// snap directly to the new visual state.
        /// </param>
        internal void ChangeVisualState(bool useTransitions)
        {
            // Update the CommonStates group
            if (!IsEnabled)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateDisabled, VisualStates.StateNormal);
            }
            else if (IsHovered)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateMouseOver, VisualStates.StateNormal);
            }
            else
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateNormal);
            }

            // Update the FocusStates group
            if (HasFocusInternal && IsEnabled)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateFocused, VisualStates.StateUnfocused);
            }
            else
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateUnfocused);
            }

            // Update the WatermarkStates group
            if (this.Watermark != null && string.IsNullOrEmpty(this.Text))
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateWatermarked, VisualStates.StateUnwatermarked);
            }
            else
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateUnwatermarked);
            }
        }

        /// <summary>
        /// Builds the visual tree for the <see cref="DatePickerTextBox" /> when a new 
        /// template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ElementContent = ExtractTemplatePart<ContentControl>(ElementContentName);

            OnWatermarkChanged();

            ChangeVisualState(false);
        }

        #region Watermark
        /// <summary>
        /// Gets or sets the Watermark content.
        /// </summary>
        /// <value>The watermark.</value>
        public object Watermark
        {
            get { return GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }

        /// <summary>
        /// Watermark dependency property.
        /// </summary>
        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.Register(
                nameof(Watermark),
                typeof(object),
                typeof(DatePickerTextBox),
                new PropertyMetadata(OnWatermarkPropertyChanged));
        #endregion

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <typeparam name="T">Inherited code: Requires comment 1.</typeparam>
        /// <param name="partName">Inherited code: Requires comment 2.</param>
        /// <returns>Inherited code: Requires comment 3.</returns>
        private T ExtractTemplatePart<T>(string partName)
            where T : DependencyObject
        {
            DependencyObject obj = GetTemplateChild(partName);
            return ExtractTemplatePart<T>(partName, obj);
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <typeparam name="T">Inherited code: Requires comment 1.</typeparam>
        /// <param name="partName">Inherited code: Requires comment 2.</param>
        /// <param name="obj">Inherited code: Requires comment 3.</param>
        /// <returns>Inherited code: Requires comment 4.</returns>
        private static T ExtractTemplatePart<T>(string partName, DependencyObject obj)
            where T : DependencyObject
        {
            Debug.Assert(
                obj == null || typeof(T).IsInstanceOfType(obj),
                string.Format(CultureInfo.InvariantCulture, "The template part {0} is not an instance of {1}.", partName, typeof(T).Name));
            return obj as T;
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (IsEnabled)
            {
                HasFocusInternal = true;

                if (!string.IsNullOrEmpty(this.Text))
                {
                    Select(0, this.Text.Length);
                }

                ChangeVisualState();
            }
        }

        /// <summary>
        /// Called when the IsEnabled property changes.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Property changed args.</param>
        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Debug.Assert(e.NewValue is bool, "The new value should be a boolean!");
            bool isEnabled = (bool)e.NewValue;

            IsReadOnly = !isEnabled;
            if (!isEnabled)
            {
                IsHovered = false;
            }

            ChangeVisualState();
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            HasFocusInternal = false;
            ChangeVisualState();
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            IsHovered = true;

            if (!HasFocusInternal)
            {
                ChangeVisualState();
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            IsHovered = false;

            if (!HasFocusInternal)
            {
                ChangeVisualState();
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            ChangeVisualState();
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private void OnWatermarkChanged()
        {
            if (ElementContent != null)
            {
                Control watermarkControl = this.Watermark as Control;
                if (watermarkControl != null)
                {
                    watermarkControl.IsTabStop = false;
                    watermarkControl.IsHitTestVisible = false;
                }
            }
        }

        /// <summary>
        /// Called when watermark property is changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">
        /// The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.
        /// </param>
        private static void OnWatermarkPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            DatePickerTextBox datePickerTextBox = sender as DatePickerTextBox;
            Debug.Assert(datePickerTextBox != null, "The source is not an instance of a DatePickerTextBox!");
            datePickerTextBox.OnWatermarkChanged();
            datePickerTextBox.ChangeVisualState();
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private void SetDefaults()
        {
            IsEnabled = true;
            this.Watermark = "<enter text here>";
        }
    }
}