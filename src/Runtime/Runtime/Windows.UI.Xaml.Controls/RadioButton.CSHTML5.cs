

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

using System;
using System.Collections.Generic;

#if MIGRATION
using System.Windows.Controls.Primitives;
using System.Windows.Media;
#else
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents a button that allows a user to select a single option from a group
    /// of options.
    /// </summary>
    [TemplateVisualState(Name = "InvalidFocused", GroupName = "ValidationStates")]
    [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Pressed", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Unfocused", GroupName = "FocusStates")]
    [TemplateVisualState(Name = "Focused", GroupName = "FocusStates")]
    [TemplateVisualState(Name = "Checked", GroupName = "CheckStates")]
    [TemplateVisualState(Name = "Unchecked", GroupName = "CheckStates")]
    [TemplateVisualState(Name = "Valid", GroupName = "ValidationStates")]
    [TemplateVisualState(Name = "InvalidUnfocused", GroupName = "ValidationStates")]
    public class RadioButton : ToggleButton
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RadioButton"/> class.
        /// </summary>
        public RadioButton()
        {
            this.DefaultStyleKey = typeof(RadioButton);
        }

        #endregion Constructor

        #region Dependency Properties

        /// <summary>
        /// Identifies the <see cref="RadioButton.GroupName"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GroupNameProperty =
            DependencyProperty.Register(
                "GroupName",
                typeof(string),
                typeof(RadioButton),
                new PropertyMetadata(null, OnGroupNameChanged));

        /// <summary>
        /// Gets or sets the name that specifies which <see cref="RadioButton"/>
        /// controls are mutually exclusive.
        /// </summary>
        /// <returns>
        /// The name that specifies which <see cref="RadioButton"/> controls are
        /// mutually exclusive. The default is null.
        /// </returns>
        public string GroupName
        {
            get { return (string)this.GetValue(GroupNameProperty); }
            set { this.SetValue(GroupNameProperty, value); }
        }

        private static void OnGroupNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadioButton radioButton = (RadioButton)d;
            string groupName = e.NewValue as string;
            string currentlyRegisteredGroupName = e.OldValue as string;

            if(groupName != currentlyRegisteredGroupName)
            {
                if (!string.IsNullOrEmpty(currentlyRegisteredGroupName))
                {
                    TryUnregister(currentlyRegisteredGroupName, radioButton);
                }

                if (!string.IsNullOrEmpty(groupName))
                {
                    TryRegister(groupName, radioButton);
                }
            }
        }

        private static void TryRegister(string groupName, RadioButton radioButton)
        {
            if (radioButton.IsLoaded)
            {
                Register(groupName, radioButton);
            }
            else
            {
                radioButton.Loaded -= new RoutedEventHandler(OnLoaded);
                radioButton.Loaded += new RoutedEventHandler(OnLoaded);
            }
        }

        private static void Register(string groupName, RadioButton radioButton)
        {
            if (_groupNameToElements == null)
            {
                _groupNameToElements = new Dictionary<string, List<RadioButton>>();
            }

            lock (_groupNameToElements)
            {
                List<RadioButton> elements;
                if (!_groupNameToElements.TryGetValue(groupName, out elements))
                {
                    elements = new List<RadioButton>(1);
                    _groupNameToElements.Add(groupName, elements);
                }

                elements.Add(radioButton);

                radioButton.Unloaded += new RoutedEventHandler(OnUnloaded);
            }
        }

        private static void OnLoaded(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            Register(radioButton.GroupName, radioButton);
        }

        private static void OnUnloaded(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            radioButton.Unloaded -= new RoutedEventHandler(OnUnloaded);
            Unregister(radioButton.GroupName, radioButton);
        }

        private static void TryUnregister(string groupName, RadioButton radioButton)
        {
            if (radioButton.IsLoaded)
            {
                Unregister(groupName, radioButton);
            }
            else
            {
                radioButton.Loaded -= new RoutedEventHandler(OnLoaded);
            }
        }

        private static void Unregister(string groupName, RadioButton radioButton)
        {
            if (_groupNameToElements == null)
            {
                return;
            }

            lock (_groupNameToElements)
            {
                List<RadioButton> elements;
                if (_groupNameToElements.TryGetValue(groupName, out elements))
                {
                    if (elements.Remove(radioButton))
                    {
                        radioButton.Unloaded -= new RoutedEventHandler(OnUnloaded);

                        if (elements.Count == 0)
                        {
                            _groupNameToElements.Remove(groupName);
                        }
                    }
                }
            }
        }

        // true if same group, false otherwise.
        private static bool CompareGroups(string group1, string group2)
        {
            if (string.IsNullOrEmpty(group1))
            {
                return string.IsNullOrEmpty(group2);
            }

            if (string.IsNullOrEmpty(group2))
            {
                return string.IsNullOrEmpty(group1);
            }

            return group1 == group2;
        }

        #endregion Dependency Properties

        #region Protected Methods

        /// <summary>
        /// Sets the <see cref="ToggleButton.IsChecked"/> property to
        /// <c>true</c>.
        /// </summary>
        protected internal override void OnToggle()
        {
            this.SetCurrentValue(IsCheckedProperty, true);
        }

        /// <summary>
        ///     This method is invoked when the IsChecked becomes true.
        /// </summary>
        /// <param name="e">RoutedEventArgs.</param>
        protected override void OnChecked(RoutedEventArgs e)
        {
            // If RadioButton is checked we should uncheck the others in the same group
            UpdateRadioButtonGroup();
            base.OnChecked(e);
        }

        private void UpdateRadioButtonGroup()
        {
            string groupName = GroupName;
            if (!string.IsNullOrEmpty(groupName))
            {
                DependencyObject rootScope = VisualTreeHelper.GetRoot(this);
                if (_groupNameToElements == null)
                {
                    _groupNameToElements = new Dictionary<string, List<RadioButton>>();
                }
                lock (_groupNameToElements)
                {
                    List<RadioButton> elements;
                    if (!_groupNameToElements.TryGetValue(groupName, out elements))
                    {
                        elements = new List<RadioButton>(1);
                    }
                    for (int i = 0; i < elements.Count; ++i)
                    {
                        RadioButton rb = elements[i];
                        if (rb != this && (rb.IsChecked == true) && object.ReferenceEquals(rootScope, VisualTreeHelper.GetRoot(rb)))
                        {
                            rb.UncheckRadioButton();
                        }
                    }
                }
            }
            else // Logical parent should be the group
            {
                DependencyObject parent = this.Parent;
                if (parent != null)
                {
                    // Traverse logical children
                    int count = VisualTreeHelper.GetChildrenCount(parent);
                    for (int i = 0; i < count; ++i)
                    {
                        RadioButton rb = VisualTreeHelper.GetChild(parent, i) as RadioButton;
                        if (rb != null && rb != this && string.IsNullOrEmpty(rb.GroupName) && (rb.IsChecked == true))
                        {
                            rb.UncheckRadioButton();
                        }
                    }
                }
            }
        }

        private void UncheckRadioButton()
        {
            SetCurrentValue(IsCheckedProperty, false);
        }

#if false
        /// <summary>
        /// Returns a <see cref="RadioButtonAutomationPeer"/> for use by
        /// the Silverlight automation infrastructure.
        /// </summary>
        /// <returns>
        /// An <see cref="RadioButtonAutomationPeer"/> for the radio button
        /// object.
        /// </returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadioButtonAutomationPeer(this);
        }
#endif

        #endregion Protected Methods

        #region private data

        private static Dictionary<string, List<RadioButton>> _groupNameToElements;

        #endregion private data
    }
}