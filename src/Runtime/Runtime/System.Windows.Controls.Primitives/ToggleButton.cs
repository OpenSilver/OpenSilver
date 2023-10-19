
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

using System.Windows.Automation.Peers;

namespace System.Windows.Controls.Primitives
{
    /// <summary>
    /// Base class for controls that can switch states, such as <see cref="CheckBox"/>
    /// and <see cref="RadioButton"/>.
    /// </summary>
    [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Pressed", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Unfocused", GroupName = "FocusStates")]
    [TemplateVisualState(Name = "Focused", GroupName = "FocusStates")]
    [TemplateVisualState(Name = "Checked", GroupName = "CheckStates")]
    [TemplateVisualState(Name = "Unchecked", GroupName = "CheckStates")]
    [TemplateVisualState(Name = "Indeterminate", GroupName = "CheckStates")]
    public class ToggleButton : ButtonBase
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ToggleButton"/> class.
        /// </summary>
        public ToggleButton() : base()
        {
            this.DefaultStyleKey = typeof(ToggleButton);
        }

        #endregion Constructor

        #region Dependency Properties

        /// <summary>
        /// Identifies the <see cref="ToggleButton.IsChecked"/> dependency
        /// property.
        /// </summary>
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register(
                "IsChecked",
                typeof(bool?),
                typeof(ToggleButton),
                new PropertyMetadata(false, OnIsCheckedChanged));

        /// <summary>
        /// Gets or sets whether the <see cref="ToggleButton"/> is checked.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the <see cref="ToggleButton"/> is checked; <c>false</c>
        /// if the <see cref="ToggleButton"/> is unchecked; otherwise
        /// null. The default is <c>false</c>.
        /// </returns>
        public bool? IsChecked
        {
            get { return (bool?)this.GetValue(IsCheckedProperty); }
            set { this.SetValue(IsCheckedProperty, value); }
        }

        private static void OnIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ToggleButton button = (ToggleButton)d;
            bool? oldValue = (bool?)e.OldValue;
            bool? newValue = (bool?)e.NewValue;

            if (newValue == true)
            {
                button.OnChecked(new RoutedEventArgs { OriginalSource = button });
            }
            else if (newValue == false)
            {
                button.OnUnchecked(new RoutedEventArgs { OriginalSource = button });
            }
            else
            {
                button.OnIndeterminate(new RoutedEventArgs { OriginalSource = button });
            }

            button.UpdateVisualStates();
        }

        /// <summary>
        /// Identifies the <see cref="ToggleButton.IsThreeState"/> dependency
        /// property.
        /// </summary>
        public static readonly DependencyProperty IsThreeStateProperty =
            DependencyProperty.Register(
                "IsThreeState",
                typeof(bool),
                typeof(ToggleButton),
                new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets whether the control supports two or three states.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the control supports three states; otherwise, <c>false</c>. 
        /// The default is <c>false</c>.
        /// </returns>
        public bool IsThreeState
        {
            get { return (bool)this.GetValue(IsThreeStateProperty); }
            set { this.SetValue(IsThreeStateProperty, value); }
        }

        #endregion Dependency Properties

        #region Public Events

        /// <summary>
        /// Occurs when a <see cref="ToggleButton"/> is checked.
        /// </summary>
        public event RoutedEventHandler Checked;

        /// <summary>
        /// Occurs when the state of a <see cref="ToggleButton"/> is
        /// switched to the indeterminate state.
        /// </summary>
        public event RoutedEventHandler Indeterminate;

        /// <summary>
        /// Occurs when a <see cref="ToggleButton"/> is unchecked.
        /// </summary>
        public event RoutedEventHandler Unchecked;

        #endregion Public Events

        #region Public Methods

        /// <summary>
        /// Returns the string representation of a <see cref="ToggleButton"/> object.
        /// </summary>
        public override string ToString()
        {
            bool? isChecked = this.IsChecked;
            return string.Format("{0} Content:{1} IsChecked:{2}",
                                 base.ToString(),
                                 this.Content,
                                 isChecked == null ? "null" : isChecked.Value.ToString());
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Called when the <see cref="ToggleButton"/> is clicked by
        /// the mouse or the keyboard.
        /// </summary>
        protected override void OnClick()
        {
            OnToggle();
            base.OnClick();
        }

        /// <summary>
        /// Called when the <see cref="ContentControl"/> property changes.
        /// </summary>
        /// <param name="oldContent">
        /// The content to be replaced.
        /// </param>
        /// <param name="newContent">
        /// The new content to display.
        /// </param>
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
        }

        /// <summary>
        /// Returns a <see cref="ToggleButtonAutomationPeer"/> for use by the Silverlight automation 
        /// infrastructure.
        /// </summary>
        /// <returns>
        /// A <see cref="ToggleButtonAutomationPeer"/> for the <see cref="ToggleButton"/> object.
        /// </returns>
        protected override AutomationPeer OnCreateAutomationPeer()
            => new ToggleButtonAutomationPeer(this);

        /// <summary>
        /// Called by the <see cref="ToggleButton.OnClick"/> method
        /// to implement toggle behavior.
        /// </summary>
        protected internal virtual void OnToggle()
        {
            // If IsChecked == true && IsThreeState == true   --->  IsChecked = null
            // If IsChecked == true && IsThreeState == false  --->  IsChecked = false
            // If IsChecked == false                          --->  IsChecked = true
            // If IsChecked == null                           --->  IsChecked = false
            bool? isChecked;
            if (IsChecked == true)
                isChecked = IsThreeState ? (bool?)null : (bool?)false;
            else // false or null
                isChecked = IsChecked.HasValue; // HasValue returns true if IsChecked==false
            SetCurrentValue(IsCheckedProperty, isChecked);
        }

        /// <summary>
        /// Raises the Checked event.
        /// </summary>
        protected virtual void OnChecked(RoutedEventArgs e)
        {
            if (Checked != null)
            {
                Checked(this, e);
            }
        }

        /// <summary>
        /// Raises the Indeterminate event.
        /// </summary>
        protected virtual void OnIndeterminate(RoutedEventArgs e)
        {
            if (Indeterminate != null)
            {
                Indeterminate(this, e);
            }
        }

        /// <summary>
        /// Raises the Unchecked event.
        /// </summary>
        protected virtual void OnUnchecked(RoutedEventArgs e)
        {
            if (Unchecked != null)
            {
                Unchecked(this, e);
            }
        }

        #endregion Protected Methods

        #region Internal Methods

        internal override void UpdateVisualStates()
        {
            base.UpdateVisualStates();
            // Update the Check state group
            var isChecked = IsChecked;
            if (isChecked == true)
            {
                VisualStateManager.GoToState(this, "Checked", true);
            }
            else if (isChecked == false)
            {
                VisualStateManager.GoToState(this, "Unchecked", true);
            }
            else
            {
                // isChecked is null
                VisualStates.GoToState(this, true, "Indeterminate", "Unchecked");
            }
        }

        #endregion Internal Methods
    }
}
