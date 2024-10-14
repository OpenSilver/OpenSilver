
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

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows.Automation.Peers;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace System.Windows.Controls
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

        private static void OnGroupNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadioButton radioButton = (RadioButton)d;
            string groupName = e.NewValue as string;

            string currentlyRegisteredGroupName;
            lock (_currentlyRegisteredGroupName)
            {
                _currentlyRegisteredGroupName.TryGetValue(radioButton, out currentlyRegisteredGroupName);
            }

            if (groupName != currentlyRegisteredGroupName)
            {
                // Unregister the old group name if set
                if (!string.IsNullOrEmpty(currentlyRegisteredGroupName))
                    Unregister(currentlyRegisteredGroupName, radioButton);

                // Register the new group name is set
                if (!string.IsNullOrEmpty(groupName))
                    Register(groupName, radioButton);
            }
        }

        private static void Register(string groupName, RadioButton radioButton)
        {
            _groupNameToElements ??= new();

            lock (_groupNameToElements)
            {
                if (!_groupNameToElements.TryGetValue(groupName, out List<WeakReference<RadioButton>> elements))
                {
                    elements = new List<WeakReference<RadioButton>>(1);
                    _groupNameToElements[groupName] = elements;
                }
                else
                {
                    // There were some elements there, remove dead ones
                    PurgeDead(elements, null);
                }

                elements.Add(new WeakReference<RadioButton>(radioButton));
            }
            lock (_currentlyRegisteredGroupName)
            {
                if (_currentlyRegisteredGroupName.TryGetValue(radioButton, out _))
                {
                    _currentlyRegisteredGroupName.Remove(radioButton);
                }
                _currentlyRegisteredGroupName.Add(radioButton, groupName);
            }
        }

        private static void Unregister(string groupName, RadioButton radioButton)
        {
            if (_groupNameToElements == null)
                return;

            lock (_groupNameToElements)
            {
                // Get all elements bound to this key and remove this element
                if (_groupNameToElements.TryGetValue(groupName, out List<WeakReference<RadioButton>> elements))
                {
                    PurgeDead(elements, radioButton);
                    if (elements.Count == 0)
                    {
                        _groupNameToElements.Remove(groupName);
                    }
                }
            }
            lock (_currentlyRegisteredGroupName)
            {
                _currentlyRegisteredGroupName.Remove(radioButton);
            }
        }

        private static void PurgeDead(List<WeakReference<RadioButton>> elements, RadioButton elementToRemove)
        {
            for (int i = 0; i < elements.Count;)
            {
                WeakReference<RadioButton> weakReference = elements[i];
                if (!weakReference.TryGetTarget(out RadioButton element) || element == elementToRemove)
                {
                    elements.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }

        private void UpdateRadioButtonGroup()
        {
            string groupName = GroupName;
            if (!string.IsNullOrEmpty(groupName))
            {
                DependencyObject rootScope = VisualTreeHelper.GetRoot(this);
                _groupNameToElements ??= new();

                lock (_groupNameToElements)
                {
                    // Get all elements bound to this key and remove this element
                    List<WeakReference<RadioButton>> elements = _groupNameToElements[groupName];
                    for (int i = 0; i < elements.Count;)
                    {
                        WeakReference<RadioButton> weakReference = elements[i];
                        if (!weakReference.TryGetTarget(out RadioButton rb))
                        {
                            // Remove dead instances
                            elements.RemoveAt(i);
                        }
                        else
                        {
                            // Uncheck all checked RadioButtons different from the current one
                            if (rb != this && (rb.IsChecked == true) && rootScope == VisualTreeHelper.GetRoot(rb))
                                rb.UncheckRadioButton();
                            i++;
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

        #region Properties and Events

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
            get { return (string)GetValue(GroupNameProperty); }
            set { SetValueInternal(GroupNameProperty, value); }
        }

        #endregion Properties and Events

        #region Override methods

        /// <summary>
        /// Returns a <see cref="RadioButtonAutomationPeer"/> for use by the Silverlight automation 
        /// infrastructure.
        /// </summary>
        /// <returns>
        /// An <see cref="RadioButtonAutomationPeer"/> for the <see cref="RadioButton"/> object.
        /// </returns>
        protected override AutomationPeer OnCreateAutomationPeer()
            => new RadioButtonAutomationPeer(this);

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

        /// <summary>
        /// Sets the <see cref="ToggleButton.IsChecked"/> property to
        /// <c>true</c>.
        /// </summary>
        protected internal override void OnToggle()
        {
            this.SetCurrentValue(IsCheckedProperty, true);
        }

        #endregion Override methods

        #region private data

        [ThreadStatic] private static Dictionary<string, List<WeakReference<RadioButton>>> _groupNameToElements;
        private static readonly ConditionalWeakTable<RadioButton, string> _currentlyRegisteredGroupName = new();

        #endregion private data
    }
}