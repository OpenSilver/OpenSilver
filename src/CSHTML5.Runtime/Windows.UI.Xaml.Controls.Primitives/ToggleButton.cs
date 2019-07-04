
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



using CSHTML5.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if MIGRATION
using System.Windows.Input;
#else
using Windows.UI.Xaml.Input;
#endif

#if MIGRATION
namespace System.Windows.Controls.Primitives
#else
namespace Windows.UI.Xaml.Controls.Primitives
#endif
{
    /// <summary>
    /// Base class for controls that can switch states, such as CheckBox and RadioButton.
    /// </summary>
    public class ToggleButton : ButtonBase
    {
        public ToggleButton()
        {
            _reactsToKeyboardEventsWhenFocused = true;


#if OLD_IMPLEMENTATION_OF_THE_CLICK_BASED_ON_HTML_CLICK_EVENT
            base._forceClickEventToBeTheLastEventRaised = true; // This is due to the fact that on Chrome and FireFox, the "click" event of the radio button happens before the "change" event, unlike in MS XAML.
#endif
            base.DisableBaseControlHandlingOfVisualStates = true; // This prevents the Control class from calling "GoToState" when the pointer is over/pressed, so that we can handle those in this class. This is required because the ToggleButton contains states such as "CheckedPressed", "CheckedPointerOver", etc.

            SetDefaultStyle();
        }

        internal bool INTERNAL_IsCodeProgrammaticallyChangingIsChecked = false; // Used by the CheckBox and the RadioButton derived classes.
        //internal dynamic INTERNAL_InnerDomElement = null; // This is the DOM element that will be like <input type="radio"> or <input type="checkbox">.
        bool? _isChecked = false;
        bool _isDisabled;

        protected virtual void SetDefaultStyle() // Overridden in CheckBox and RadioButton
        {
            // Set default style:
#if WORKINPROGRESS
            this.DefaultStyleKey = typeof(ToggleButton);
#else
            this.INTERNAL_SetDefaultStyle(INTERNAL_DefaultToggleButtonStyle.GetDefaultStyle());
#endif
        }

        /// <summary>
        /// Gets or sets whether the ToggleButton is checked.
        /// Returns:
        /// True if the ToggleButton is checked; false if the ToggleButton is unchecked;
        /// otherwise null. The default is false.  If you are programming using C# or
        /// Visual Basic, the type of this property is projected as bool? (a nullable
        /// Boolean).
        /// </summary>
        public bool? IsChecked
        {
            get { return (bool?)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }
        /// <summary>
        /// Identifies the IsChecked dependency property
        /// </summary>
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool?), typeof(ToggleButton), new PropertyMetadata(false, IsChecked_Changed));
        internal static void IsChecked_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ToggleButton toggleButton = (ToggleButton)d;

            bool? isChecked = (bool?)e.NewValue;

            // Update DOM appearance (if not using a ControlTemplate):
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(toggleButton) && !(toggleButton.INTERNAL_IsTemplated))
            {
                toggleButton.UpdateDomBasedOnCheckedState(isChecked);
            }

            // Raise user events if the new value is different from the old value:
            bool hasValueRemainedTheSame = (e.NewValue != null && e.NewValue.Equals(e.OldValue)) || (e.NewValue == null && e.OldValue == null);
            if (!hasValueRemainedTheSame) // note: We do not simply check "e.NewValue != e.OldValue" because it does not work as expected for boxed values, so example if both values are "true", it returns "false". //todo: do the same for other similar comparisons in the project.
            {
                if (isChecked == null)
                {
                    toggleButton.OnIndeterminate();
                }
                else if (isChecked == true)
                {
                    toggleButton.OnChecked();
                }
                else // false
                {
                    toggleButton.OnUnchecked();
                }
            }
        }


        /// <summary>
        /// Determines whether the control supports two or three states.
        /// </summary>
        public bool IsThreeState
        {
            get { return (bool)GetValue(IsThreeStateProperty); }
            set { SetValue(IsThreeStateProperty, value); }
        }

        /// <summary>
        /// Identifies the ToggleButton.IsThreeState dependency property.
        /// </summary>
        public static readonly DependencyProperty IsThreeStateProperty =
            DependencyProperty.Register("IsThreeState", typeof(bool), typeof(ToggleButton), new PropertyMetadata(false)); //I don't think we need a callback here.

#if MIGRATION
        internal override void OnKeyDownWhenFocused(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Space) || (e.Key == Key.Enter))
#else
        internal override void OnKeyDownWhenFocused(object sender, KeyRoutedEventArgs e)
        {
            if ((e.Key == Windows.System.VirtualKey.Space) || (e.Key == Windows.System.VirtualKey.Enter))
#endif
            {
                ToggleButton_Click(this, null);
            }
        }

#if MIGRATION
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();

            Click += ToggleButton_Click;
            var visualStateGroups = base.INTERNAL_GetVisualStateGroups();

            // Apply the current state:
            UpdateVisualStates();

            if (visualStateGroups != null
                &&
                (visualStateGroups.ContainsVisualState("PointerOver") || visualStateGroups.ContainsVisualState("CheckedPointerOver") || visualStateGroups.ContainsVisualState("IndeterminatePointerOver")))
            {
                // Note: We unregster the event before registering it because, in case the user removes the control from the visual tree and puts it back, the "OnApplyTemplate" is called again.
#if MIGRATION
                this.MouseEnter -= Control_MouseEnter;
                this.MouseEnter += Control_MouseEnter;
                this.MouseLeave -= Control_MouseLeave;
                this.MouseLeave += Control_MouseLeave;
#else
                this.PointerEntered -= Control_PointerEntered;
                this.PointerEntered += Control_PointerEntered;
                this.PointerExited -= Control_PointerExited;
                this.PointerExited += Control_PointerExited;
#endif
            }

            if (visualStateGroups != null
                &&
                (visualStateGroups.ContainsVisualState("Pressed") || visualStateGroups.ContainsVisualState("CheckedPressed") || visualStateGroups.ContainsVisualState("IndeterminatePressed")))
            {
                // Note: We unregster the event before registering it because, in case the user removes the control from the visual tree and puts it back, the "OnApplyTemplate" is called again.
#if MIGRATION
                this.MouseLeftButtonDown -= Control_MouseLeftButtonDown;
                this.MouseLeftButtonDown += Control_MouseLeftButtonDown;
                this.MouseLeftButtonUp -= Control_MouseLeftButtonUp;
                this.MouseLeftButtonUp += Control_MouseLeftButtonUp;
#else
                this.PointerPressed -= Control_PointerPressed;
                this.PointerPressed += Control_PointerPressed;
                this.PointerReleased -= Control_PointerReleased;
                this.PointerReleased += Control_PointerReleased;
#endif
            }
        }

        bool _isPointerOver = false;
        bool _isPressed = false;
#if MIGRATION
        void Control_MouseEnter(object sender, Input.MouseEventArgs e)
#else
        void Control_PointerEntered(object sender, Input.PointerRoutedEventArgs e)
#endif
        {
            _isPointerOver = true;
            UpdateVisualStates();
        }

#if MIGRATION
        void Control_MouseLeave(object sender, Input.MouseEventArgs e)
#else
        void Control_PointerExited(object sender, Input.PointerRoutedEventArgs e)
#endif
        {
            _isPointerOver = false;
            UpdateVisualStates();
        }

#if MIGRATION
        void Control_MouseLeftButtonDown(object sender, Input.MouseButtonEventArgs e)
#else
        void Control_PointerPressed(object sender, Input.PointerRoutedEventArgs e)
#endif
        {
            _isPressed = true;
            UpdateVisualStates();
        }

#if MIGRATION
        void Control_MouseLeftButtonUp(object sender, Input.MouseButtonEventArgs e)
#else
        void Control_PointerReleased(object sender, Input.PointerRoutedEventArgs e)
#endif
        {
            _isPressed = false;
            UpdateVisualStates();
        }

        void UpdateVisualStates()
        {
            if (_isChecked == false)
            {
                if (_isDisabled)
                    VisualStateManager.GoToState(this, "Disabled", true);
                else if (_isPressed)
                    VisualStateManager.GoToState(this, "Pressed", true);
                else if (_isPointerOver)
#if MIGRATION
                    VisualStateManager.GoToState(this, "MouseOver", true);
#else
                    VisualStateManager.GoToState(this, "PointerOver", true);
#endif
                else
                    VisualStateManager.GoToState(this, "Normal", true);
            }
            else if (_isChecked == true)
            {
                if (_isDisabled)
                    VisualStateManager.GoToState(this, "CheckedDisabled", true);
                else if (_isPressed)
                    VisualStateManager.GoToState(this, "CheckedPressed", true);
                else if (_isPointerOver)
                    VisualStateManager.GoToState(this, "CheckedPointerOver", true);
                else
                    VisualStateManager.GoToState(this, "Checked", true);
            }
            else if (_isChecked == null)
            {
                if (_isDisabled)
                    VisualStateManager.GoToState(this, "IndeterminateDisabled", true);
                else if (_isPressed)
                    VisualStateManager.GoToState(this, "IndeterminatePressed", true);
                else if (_isPointerOver)
                    VisualStateManager.GoToState(this, "IndeterminatePointerOver", true);
                else
                    VisualStateManager.GoToState(this, "Indeterminate", true);
            }
        }

        internal void ToggleButton_Click(object sender, RoutedEventArgs e) //note: IsThreeState only has an effect when clicking when IsChecked is true, which makes the checkbox go to the Indeterminate state.
        {
            if (IsChecked == true)
            {
                if (IsThreeState)
                {
                    SetLocalValue(IsCheckedProperty, null);
                }
                else
                {
                    SetLocalValue(IsCheckedProperty, false);
                }
            }
            else if (IsChecked == null)
            {
                SetLocalValue(IsCheckedProperty, false);
            }
            else
            {
                SetLocalValue(IsCheckedProperty, true);
            }
        }

        internal void UnregisterFromDefaultClickEvent()
        {
            Click -= ToggleButton_Click;
        }

        protected virtual void UpdateDomBasedOnCheckedState(bool? isChecked)
        {
            // (Virtual method)
        }

        ///// <summary>
        ///// Gets or sets a value that indicates whether the control supports three states.
        ///// Returns true if the control supports three states; otherwise, false. The default is false.
        ///// </summary>
        //public bool IsThreeState
        //{
        //    get { return (bool)GetValue(IsThreeStateProperty); }
        //    private set { SetValue(IsThreeStateProperty, value); }
        //}
        //public static readonly DependencyProperty IsThreeStateProperty =
        //    DependencyProperty.Register("IsThreeState", typeof(bool), typeof(ToggleButton), new PropertyMetadata(false, IsThreeState_Changed));

        //static void IsThreeState_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    //todo: fill this method
        //}

        /// <summary>
        /// This method is used to subscribe to the click event on the child container so that clicking on it toggles it.
        /// </summary>
        /// <param name="divWhereToPlaceChild"></param>
        /// <param name="toggleDomElement"></param>
        internal virtual void SubscribeToClickEventForChildContainerDiv(dynamic divWhereToPlaceChild, dynamic toggleDomElement)
        {
            //we do nothing, it is supposed to be overriden.
        }

        /// <summary>
        /// Occurs when a ToggleButton is checked.
        /// </summary>
        public event RoutedEventHandler Checked;
        /// <summary>
        /// Raises the Checked event.
        /// </summary>
        protected void OnChecked()
        {
            if (Checked != null)
            {
                Checked(this, new RoutedEventArgs()
                {
                    OriginalSource = this
                });
            }
            _isChecked = true;
            UpdateVisualStates();
        }

        /// <summary>
        /// Occurs when the state of a ToggleButton is switched to the indeterminate state.
        /// </summary>
        public event RoutedEventHandler Indeterminate;
        /// <summary>
        /// Raises the Indeterminate event.
        /// </summary>
        protected void OnIndeterminate()
        {
            if (Indeterminate != null)
            {
                Indeterminate(this, new RoutedEventArgs()
                {
                    OriginalSource = this
                });
            }
            _isChecked = null;
            UpdateVisualStates();
        }

        /// <summary>
        /// Occurs when a ToggleButton is unchecked.
        /// </summary>
        public event RoutedEventHandler Unchecked;
        /// <summary>
        /// Raises the Unchecked event.
        /// </summary>
        protected void OnUnchecked()
        {
            if (Unchecked != null)
            {
                Unchecked(this, new RoutedEventArgs()
                {
                    OriginalSource = this
                });
            }
            _isChecked = false;
            UpdateVisualStates();

        }

        ///// <summary>
        ///// Called when the ToggleButton receives toggle stimulus.
        ///// </summary>
        //protected virtual void OnToggle() //todo: see what it is supposed to do and implement it
        //{
        //}

        //-----------------------
        // ISENABLED (OVERRIDE)
        //-----------------------

        protected internal override void ManageIsEnabled(bool isEnabled)
        {
            base.ManageIsEnabled(isEnabled); // Useful for setting the "disabled" attribute on the DOM element.

            _isDisabled = !isEnabled; // We remember the value so that when we update the visual states, we know whether we should go to the "Disabled" state or not.
            UpdateVisualStates();
        }
    }
}
