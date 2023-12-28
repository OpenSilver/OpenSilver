// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows.Input;

namespace System.Windows.Controls
{
    /// <summary>
    /// Base class for all controls that provide value manipulation with a 
    /// Spinner and a text box.
    /// </summary>
    /// <typeparam name="T">Type of Value property.</typeparam>
    /// <QualityBand>Stable</QualityBand>
    [TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateMouseOver, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StatePressed, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateDisabled, GroupName = VisualStates.GroupCommon)]

    [TemplateVisualState(Name = VisualStates.StateFocused, GroupName = VisualStates.GroupFocus)]
    [TemplateVisualState(Name = VisualStates.StateUnfocused, GroupName = VisualStates.GroupFocus)]

    [TemplatePart(Name = UpDownBase.ElementTextName, Type = typeof(TextBox))]
    [TemplatePart(Name = UpDownBase.ElementSpinnerName, Type = typeof(Spinner))]
    [StyleTypedProperty(Property = UpDownBase.SpinnerStyleName, StyleTargetType = typeof(Spinner))]
    public abstract class UpDownBase<T> : UpDownBase
    {
#region Template Parts
        /// <summary>
        /// Private field for Text template part.
        /// </summary>
        private TextBox _textBox;

        /// <summary>
        /// Private field to hold previous value of TextBox.Text.
        /// </summary>
        /// <remarks>
        /// Because TextBox.TextChanged seems to fire randomly, 
        /// so we compensatethat by handling LostFocus,
        /// and comparing TextBox.Text with cached previous value.
        /// </remarks>
        private string _text;

        /// <summary>
        /// Gets the Text template part.
        /// </summary>
        internal TextBox Text
        {
            get { return _textBox; }
            private set
            {
                if (_textBox != null)
                {
                    _textBox.GotFocus -= OnTextGotFocus;
                    _textBox.LostFocus -= OnTextLostFocus;
                }

                _textBox = value;

                if (_textBox != null)
                {
                    _textBox.GotFocus += OnTextGotFocus;
                    _textBox.LostFocus += OnTextLostFocus;
                    _text = _textBox.Text;
                }
            }
        }

        /// <summary>
        /// Private field for Spinner template part.
        /// </summary>
        private Spinner _spinner;

        /// <summary>
        /// Gets the Spinner template part.
        /// </summary>
        internal Spinner Spinner
        {
            get { return _spinner; }
            private set
            {
                if (_spinner != null)
                {
                    _spinner.Spin -= OnSpinnerSpin;
                }

                _spinner = value;

                if (_spinner != null)
                {
                    _spinner.Spin += OnSpinnerSpin;
                }
            }
        }
#endregion

#region public T Value
        /// <summary>
        /// Gets or sets the Value property.
        /// </summary>
        public virtual T Value
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
                typeof(UpDownBase<T>),
                new PropertyMetadata(default(T), OnValuePropertyChanged));

        /// <summary>
        /// A value indicating whether a dependency property change handler
        /// should ignore the next change notification.  This is used to reset
        /// the value of properties without performing any of the actions in
        /// their change handlers.
        /// </summary>
        private bool _ignoreValueChange;

        /// <summary>
        /// ValueProperty property changed handler.
        /// </summary>
        /// <param name="d">UpDownBase whose Value changed.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UpDownBase<T> source = (UpDownBase<T>)d;

            // Ignore the change if requested
            if (source._ignoreValueChange)
            {
                return;
            }

            T oldValue = (T)e.OldValue;
            T newValue = (T)e.NewValue;

            // simulate pre and post events
            // The Value has already been changed when this function is called. 
            // So if user's chaning event handler check Value, it will be the changed value.
            // This is confusing, because we are simulating pre event on the platform that doesn't natively support it.
            RoutedPropertyChangingEventArgs<T> changingArgs = new RoutedPropertyChangingEventArgs<T>(e.Property, oldValue, newValue, true);
            source.OnValueChanging(changingArgs);

            // hack: work around the class hierarchy for value coercion in NumericUpDown
            if (changingArgs.InCoercion)
            {
            }
            else if (!changingArgs.Cancel)
            {
                newValue = (T)changingArgs.NewValue;
                ////if (!oldValue.Equals(newValue))
                ////{
                ////UpDownBaseAutomationPeer peer = nud.GetAutomationPeer() as UpDownBaseAutomationPeer;
                ////if (peer != null)
                ////{
                ////    peer.RaiseValuePropertyChangedEvent(oldValue, newValue);
                ////}
                RoutedPropertyChangedEventArgs<T> changedArgs = new RoutedPropertyChangedEventArgs<T>(oldValue, newValue);
                source.OnValueChanged(changedArgs);
                ////}
            }
            else
            {
                // revert back to old value if an event handler canceled the changing event.
                source._ignoreValueChange = true;
                source.Value = oldValue;
                source._ignoreValueChange = false;
            }
        }
#endregion public T Value

#region public bool IsEditable
        /// <summary>
        /// Gets or sets a value indicating whether the value can be manually 
        /// edited by the end-user.
        /// </summary>
        public bool IsEditable
        {
            get { return (bool)GetValue(IsEditableProperty); }
            set { SetValue(IsEditableProperty, value); }
        }

        /// <summary>
        /// Identifies the IsEditable dependency property.
        /// </summary>
        public static readonly DependencyProperty IsEditableProperty =
            DependencyProperty.Register(
                "IsEditable",
                typeof(bool),
                typeof(UpDownBase<T>),
                new PropertyMetadata(true, OnIsEditablePropertyChanged));

        /// <summary>
        /// IsEditableProperty property changed handler.
        /// </summary>
        /// <param name="d">UpDownBase that changed its IsEditable.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnIsEditablePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UpDownBase<T> source = d as UpDownBase<T>;
            bool oldValue = (bool)e.OldValue;
            bool newValue = (bool)e.NewValue;
            source.OnIsEditableChanged(oldValue, newValue);
        }
#endregion public bool IsEditable

#region public events
        /// <summary>
        /// Occurs when Value property is changing.
        /// </summary>
        public event RoutedPropertyChangingEventHandler<T> ValueChanging;

        /// <summary>
        /// Occurs when Value property has changed.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<T> ValueChanged;

        /// <summary>
        /// Occurs when a value is being parsed and allows custom parsing.
        /// </summary>
        public event EventHandler<UpDownParsingEventArgs<T>> Parsing;

        /// <summary>
        /// Occurs when there is an error in parsing user input and allows adding parsing logic.
        /// </summary>
        public event EventHandler<UpDownParseErrorEventArgs> ParseError;
#endregion

        /// <summary>
        /// Initializes a new instance of the UpDownBase(of T) class.
        /// </summary>
        protected UpDownBase()
        {
        }

#region overrides
        /// <summary>
        /// GetValue override to return Value property as object type.
        /// </summary>
        /// <returns>The Value property as object type.</returns>
        ////[EditorBrowsable(EditorBrowsableState.Advanced)]
        public sealed override object GetValue()
        {
            return Value;
        }

        /// <summary>
        /// SetValue override to set value to Value property.
        /// </summary>
        /// <param name="value">New value.</param>
        ////[EditorBrowsable(EditorBrowsableState.Advanced)]
        public sealed override void SetValue(object value)
        {
            // ningz: throw argumentexception
            Value = (T)value;
        }

        /// <summary>
        /// Builds the visual tree for the UpDownBase(of T) control when a new 
        /// template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Text = GetTemplateChild(ElementTextName) as TextBox;
            Spinner = GetTemplateChild(ElementSpinnerName) as Spinner;

            // Bind properties of Text template part with those of UpDownBase<T>.
            SetTextBoxText();
            if (Text != null)
            {
                Text.IsReadOnly = !IsEditable;
            }

            Interaction.OnApplyTemplateBase();
        }

        /// <summary>
        /// Provides handling for the KeyDown event.
        /// </summary>
        /// <remarks>
        /// Only support up and down arrow keys.
        /// </remarks>
        /// <param name="e">Key event args.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Handled)
            {
                return;
            }

            switch (e.Key)
            {
                case Key.Up:
                    DoIncrement();
                    e.Handled = true;
                    break;

                case Key.Down:
                    DoDecrement();
                    e.Handled = true;
                    break;

                case Key.Enter:
                    ProcessUserInput();
                    e.Handled = true;
                    break;
            }
        }

        /// <summary>
        /// Provides handling for the MouseWheel event.
        /// </summary>
        /// <param name="e">Mouse wheel event args.</param>
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            if (!e.Handled)
            {
                if (e.Delta < 0)
                {
                    DoDecrement();
                }
                else if (0 < e.Delta)
                {
                    DoIncrement();
                }

                e.Handled = true;
            }
        }

#endregion

#region private methods
        /// <summary>
        /// Performs an increment if conditions allow it.
        /// </summary>
        private void DoDecrement()
        {
            if (Spinner == null || (Spinner.ValidSpinDirection & ValidSpinDirections.Decrease) == ValidSpinDirections.Decrease)
            {
                OnDecrement();
            }
        }

        /// <summary>
        /// Performs a decrement if conditions allow it.
        /// </summary>
        private void DoIncrement()
        {
            if (Spinner == null || (Spinner.ValidSpinDirection & ValidSpinDirections.Increase) == ValidSpinDirections.Increase)
            {
                OnIncrement();
            }
        }
#endregion

#region private event handlers
        /// <summary>
        /// Event handler for Text template part's LostFocus event.
        /// We use this event to compare current TextBox.Text with cached previous 
        /// value to decide whether user has typed in a new value. 
        /// </summary>
        /// <param name="sender">The Text template part.</param>
        /// <param name="e">Event args.</param>
        private void OnTextLostFocus(object sender, RoutedEventArgs e)
        {
            ProcessUserInput();
        }

        /// <summary>
        /// Called OnLostFocus and OnKeyDown(Key.Enter) to process user input.
        /// </summary>
        private void ProcessUserInput()
        {
            if (Text != null)
            {
                if (string.Compare(_text, Text.Text, StringComparison.CurrentCulture) != 0)
                {
                    // retain caretposition
                    int caretPosition = Text.SelectionStart;

                    _text = Text.Text;
                    ApplyValue(_text);

                    if (caretPosition < Text.Text.Length)
                    {
                        // set back caretposition.
                        Text.SelectionStart = caretPosition;
                    }
                }
            }
        }

        /// <summary>
        /// Event handler for Text template part's GotFocus event.
        /// This event handler selects the whole text on GotFocus when nothing is selected.
        /// </summary>
        /// <param name="sender">The Text template part.</param>
        /// <param name="e">Event args.</param>
        private void OnTextGotFocus(object sender, RoutedEventArgs e)
        {
            SelectAllText();
        }

        /// <summary>
        /// Display current value in Text template part.
        /// </summary>
        internal void SetTextBoxText()
        {
            if (Text != null)
            {
                _text = FormatValue() ?? string.Empty;
                Text.Text = _text;

                // always move cursor to the right.
                Text.SelectionStart = _text.Length;
            }
        }

        /// <summary>
        /// Event handler for Spinner template part's Spin event.
        /// </summary>
        /// <param name="sender">The Spinner template part.</param>
        /// <param name="e">Event args.</param>
        private void OnSpinnerSpin(object sender, SpinEventArgs e)
        {
            // Force LostFocus event handling to get latest user input because 
            // ButtonSpinner's Button.Click happens before Text's TextBox.LostFocus.
            if (Text != null)
            {
                OnTextLostFocus(Text, new RoutedEventArgs());
            }

            OnSpin(e);
        }
#endregion

#region protected virtual methods
        /// <summary>
        /// Processes user input when the TextBox.TextChanged event occurs.
        /// </summary>
        /// <param name="text">User input.</param>
        protected internal virtual void ApplyValue(string text)
        {
            UpDownParsingEventArgs<T> parsingArgs = new UpDownParsingEventArgs<T>(text);
            Exception ParsingException = null;
            T regularParsedValue = default(T);
            try
            {
                // parse value might throw an exception.
                regularParsedValue = ParseValue(text);
                parsingArgs.Value = regularParsedValue;
            }
            catch (Exception error)
            {
                ParsingException = error;
            }

            try
            {
                OnParsing(parsingArgs);
            }
            catch (Exception)
            {
                // swallow any exception during custom parsing.
            }

            if (ParsingException == null)
            {
                // if no error, regularParsedValue must be initialized.
                T newValue = parsingArgs.Handled ? parsingArgs.Value : regularParsedValue;
                if (Value.Equals(newValue))
                {
                    // although value might be the same, still format the value.
                    SetTextBoxText();
                }
                Value = newValue;
            }
            else
            {
                if (parsingArgs.Handled)
                {
                    if (Value.Equals(parsingArgs.Value))
                    {
                        // although value might be the same, still format the value.
                        SetTextBoxText();
                    }
                    Value = parsingArgs.Value;
                }
                else
                {
                    UpDownParseErrorEventArgs args = new UpDownParseErrorEventArgs(text, ParsingException);
                    OnParseError(args);

                    if (!args.Handled)
                    {
                        // default error handling is to discard user input and revert to the old text
                        SetTextBoxText();
                    }
                }
            }
        }

        /// <summary>
        /// Raises the ParserError event when there is an error in parsing user input.
        /// </summary>
        /// <param name="e">Event args.</param>
        protected virtual void OnParseError(UpDownParseErrorEventArgs e)
        {
            if (ParseError != null)
            {
                ParseError(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:Parsing"/> event, to allow easily hooking
        /// into the parse logic.
        /// </summary>
        /// <param name="e">The <see cref="UpDownParsingEventArgs&lt;T&gt;"/> 
        /// instance containing the event data.</param>
        protected virtual void OnParsing(UpDownParsingEventArgs<T> e)
        {
            EventHandler<UpDownParsingEventArgs<T>> handler = Parsing;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Occurs when the spinner spins.
        /// </summary>
        /// <param name="e">Event args.</param>
        protected virtual void OnSpin(SpinEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            if (e.Direction == SpinDirection.Increase)
            {
                DoIncrement();
            }
            else
            {
                DoDecrement();
            }
        }

        /// <summary>
        /// Raises the ValueChanging event when Value property is changing.
        /// </summary>
        /// <param name="e">Event args.</param>
        protected virtual void OnValueChanging(RoutedPropertyChangingEventArgs<T> e)
        {
            if (ValueChanging != null)
            {
                ValueChanging(this, e);
            }
        }

        /// <summary>
        /// Raises the ValueChanged event when Value property has changed.
        /// </summary>
        /// <param name="e">Event args.</param>
        protected virtual void OnValueChanged(RoutedPropertyChangedEventArgs<T> e)
        {
            if (ValueChanged != null)
            {
                ValueChanged(this, e);
            }

            SetTextBoxText();
        }

        /// <summary>
        /// Called when IsEditable property value changed.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected virtual void OnIsEditableChanged(bool oldValue, bool newValue)
        {
            if (Text != null)
            {
                Text.IsReadOnly = !IsEditable;
            }
        }

        /// <summary>
        /// Selects all text.
        /// </summary>
        protected virtual void SelectAllText()
        {
            if (Text != null)
            {
                if (Text.SelectionLength == 0 && Text.Text != null)
                {
                    Text.Select(0, Text.Text.Length);
                }
            }
        }
#endregion

#region abstract methods
        /// <summary>
        /// Called by ApplyValue to parse user input.
        /// </summary>
        /// <param name="text">User input.</param>
        /// <returns>Value parsed from user input.</returns>
        protected abstract T ParseValue(string text);

        /// <summary>
        /// Renders the value property into the textbox text.
        /// </summary>
        /// <returns>Formatted Value.</returns>
        protected internal abstract string FormatValue();

        /// <summary>
        /// Called by OnSpin when the spin direction is SpinDirection.Increase.
        /// </summary>
        protected abstract void OnIncrement();

        /// <summary>
        /// Called by OnSpin when the spin direction is SpinDirection.Increase.
        /// </summary>
        protected abstract void OnDecrement();
#endregion
    }
}
