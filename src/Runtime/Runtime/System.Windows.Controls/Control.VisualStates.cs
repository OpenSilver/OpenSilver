
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

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using OpenSilver.Internal;

namespace System.Windows.Controls
{
    public partial class Control : FrameworkElement
    {
        private VisualStateUpdater _visualStatesUpdater;
        private bool _handleCommonVisualStates = false;
        private bool _isInvalid;
        private bool _isFocused;

        [Obsolete(Helper.ObsoleteMemberMessage + " Use EnableBaseControlHandlingOfVisualStates instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected bool DisableBaseControlHandlingOfVisualStates
        {
            get => !EnableBaseControlHandlingOfVisualStates;
            set => EnableBaseControlHandlingOfVisualStates = !value;
        }

        /// <summary>
        /// Derived classes can set this flag to True in their constructor in order to 
        /// disable the "GoToState" calls of this class related to PointerOver/Pressed/Disabled, 
        /// and handle them by themselves. An example is the ToggleButton control, which 
        /// contains states such as "CheckedPressed", "CheckedPointerOver", etc.
        /// The default value is false.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected bool EnableBaseControlHandlingOfVisualStates
        {
            get => _handleCommonVisualStates;
            set
            {
                if (_handleCommonVisualStates != value)
                {
                    _handleCommonVisualStates = value;
                    if (!value)
                    {
                        _visualStatesUpdater?.Dispose();
                        _visualStatesUpdater = null;
                    }
                }
            }
        }

        internal void ShowValidationError()
        {
            _isInvalid = true;
            UpdateValidationState();
        }

        internal void HideValidationError()
        {
            _isInvalid = false;
            UpdateValidationState();
        }

        private void UpdateValidationState()
        {
            if (_isInvalid)
            {
                VisualStateManager.GoToState(this, _isFocused ? VisualStates.StateInvalidFocused : VisualStates.StateInvalidUnfocused, true);
            }
            else
            {
                VisualStateManager.GoToState(this, VisualStates.StateValid, true);
            }
        }

        internal void GoToState(string state) => VisualStateManager.GoToState(this, state, true);

        internal virtual void UpdateVisualStates() => _visualStatesUpdater?.UpdateVisualStates(true);

        private sealed class VisualStateUpdater
        {
            private readonly Control _owner;
            private bool _isMouseOver = false;
            private bool _isPressed = false;
            private bool _isFocused = false;

            public VisualStateUpdater(Control owner)
            {
                Debug.Assert(owner != null);
                _owner = owner;

                ConnectToOwner();
            }

            private void ConnectToOwner()
            {              
                _owner.IsEnabledChanged += new DependencyPropertyChangedEventHandler(OnIsEnabledChanged);

                var groups = (Collection<VisualStateGroup>)_owner.StateGroupsRoot?.GetValue(VisualStateManager.VisualStateGroupsProperty);
                if (groups != null)
                {
                    bool hasMouseOverState = false;
                    bool hasPressedState = false;
                    bool hasFocusedState = false;

                    foreach (VisualStateGroup group in groups)
                    {
                        foreach (VisualState state in group.States)
                        {
                            if (state.Name == VisualStates.StateMouseOver)
                            {
                                hasMouseOverState = true;
                            }
                            else if (state.Name == VisualStates.StatePressed)
                            {
                                hasPressedState = true;
                            }
                            else if (state.Name == VisualStates.StateFocused)
                            {
                                hasFocusedState = true;
                            }
                        }
                    }

                    if (hasMouseOverState)
                    {
                        _owner.MouseEnter += new MouseEventHandler(OnMouseEnter);
                        _owner.MouseLeave += new MouseEventHandler(OnMouseLeave);
                    }

                    if (hasPressedState)
                    {
                        _owner.MouseLeftButtonDown += new MouseButtonEventHandler(OnMouseLeftButtonDown);
                        _owner.MouseLeftButtonUp += new MouseButtonEventHandler(OnMouseLeftButtonUp);
                    }

                    if (hasFocusedState)
                    {
                        _owner.GotFocus += new RoutedEventHandler(OnGotFocus);
                        _owner.LostFocus += new RoutedEventHandler(OnLostFocus);
                    }
                }
            }

            private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
            {
                UpdateVisualStates(false);
            }

            private void OnMouseEnter(object sender, MouseEventArgs e)
            {
                _isMouseOver = true;
                UpdateVisualStates(false);
            }

            private void OnMouseLeave(object sender, MouseEventArgs e)
            {
                _isMouseOver = false;
                UpdateVisualStates(false);
            }

            private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            {
                _isPressed = true;
                UpdateVisualStates(false);
            }

            private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
            {
                _isPressed = false;
                UpdateVisualStates(false);
            }

            private void OnGotFocus(object sender, RoutedEventArgs e)
            {
                _isFocused = true;
                UpdateFocusState(false);
            }

            private void OnLostFocus(object sender, RoutedEventArgs e)
            {
                _isFocused = false;
                UpdateFocusState(false);
            }

            public void UpdateVisualStates(bool useTransitions)
            {
                if (!_owner.IsEnabled)
                {
                    VisualStateManager.GoToState(_owner, VisualStates.StateDisabled, useTransitions);
                }
                else if (_isPressed)
                {
                    VisualStateManager.GoToState(_owner, VisualStates.StatePressed, useTransitions);
                }
                else if (_isMouseOver)
                {
                    VisualStateManager.GoToState(_owner, VisualStates.StateMouseOver, useTransitions);
                }
                else
                {
                    VisualStateManager.GoToState(_owner, VisualStates.StateNormal, useTransitions);
                }
            }

            private void UpdateFocusState(bool useTransitions)
            {
                if (_isFocused)
                {
                    VisualStateManager.GoToState(_owner, VisualStates.StateFocused, useTransitions);
                }
                else
                {
                    VisualStateManager.GoToState(_owner, VisualStates.StateUnfocused, useTransitions);
                }
            }

            public void Dispose()
            {
                _owner.MouseEnter -= new MouseEventHandler(OnMouseEnter);
                _owner.MouseLeave -= new MouseEventHandler(OnMouseLeave);
                _owner.MouseLeftButtonDown -= new MouseButtonEventHandler(OnMouseLeftButtonDown);
                _owner.MouseLeftButtonUp -= new MouseButtonEventHandler(OnMouseLeftButtonUp);
                _owner.IsEnabledChanged -= new DependencyPropertyChangedEventHandler(OnIsEnabledChanged);
                _owner.GotFocus -= new RoutedEventHandler(OnGotFocus);
                _owner.LostFocus -= new RoutedEventHandler(OnLostFocus);
            }
        }
    }
}
