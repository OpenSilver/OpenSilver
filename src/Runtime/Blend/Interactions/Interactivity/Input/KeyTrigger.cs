// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------
namespace Microsoft.Expression.Interactivity.Input
{
    using System;
    using System.Windows.Input;
    using System.Windows.Interactivity;

#if MIGRATION
    using System.Windows;
    using System.Windows.Media;
#else
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Input;
    using Windows.UI.Xaml.Media;
    using Key = Windows.System.VirtualKey;
    using ModifierKeys = Windows.System.VirtualKeyModifiers;
#endif

    public enum KeyTriggerFiredOn
    {
        KeyDown,
        KeyUp
    }

    /// <summary>
    /// A Trigger that is triggered by a keyboard event.  If the target Key and Modifiers are detected, it fires.
    /// </summary>
    public class KeyTrigger : EventTriggerBase<UIElement>
    {
#if __WPF__
        public static readonly DependencyProperty KeyProperty = DependencyProperty.Register("Key", typeof(Key), typeof(KeyTrigger));

        public static readonly DependencyProperty ModifiersProperty = DependencyProperty.Register("Modifiers", typeof(ModifierKeys), typeof(KeyTrigger));

        public static readonly DependencyProperty ActiveOnFocusProperty = DependencyProperty.Register("ActiveOnFocus", typeof(bool), typeof(KeyTrigger));

        public static readonly DependencyProperty FiredOnProperty = DependencyProperty.Register("FiredOn", typeof(KeyTriggerFiredOn), typeof(KeyTrigger));
#else
        public static readonly DependencyProperty KeyProperty = DependencyProperty.Register("Key", typeof(Key), typeof(KeyTrigger), null);

        public static readonly DependencyProperty ModifiersProperty = DependencyProperty.Register("Modifiers", typeof(ModifierKeys), typeof(KeyTrigger), null);

        public static readonly DependencyProperty ActiveOnFocusProperty = DependencyProperty.Register("ActiveOnFocus", typeof(bool), typeof(KeyTrigger), null);

        public static readonly DependencyProperty FiredOnProperty = DependencyProperty.Register("FiredOn", typeof(KeyTriggerFiredOn), typeof(KeyTrigger), null);
#endif

        private UIElement targetElement;

        /// <summary>
        /// The key that must be pressed for the trigger to fire.
        /// </summary>
        public Key Key
        {
            get { return (Key)this.GetValue(KeyTrigger.KeyProperty); }
            set { this.SetValue(KeyTrigger.KeyProperty, value); }
        }

        /// <summary>
        /// The modifiers that must be active for the trigger to fire (the default is no modifiers pressed).
        /// </summary>
        public ModifierKeys Modifiers
        {
            get { return (ModifierKeys)this.GetValue(KeyTrigger.ModifiersProperty); }
            set { this.SetValue(KeyTrigger.ModifiersProperty, value); }
        }

        /// <summary>
        /// If true, the Trigger only listens to its trigger Source object, which means that element must have focus for the trigger to fire.
        /// If false, the Trigger listens at the root, so any unhandled KeyDown/Up messages will be caught.
        /// </summary>
        public bool ActiveOnFocus
        {
            get { return (bool)this.GetValue(KeyTrigger.ActiveOnFocusProperty); }
            set { this.SetValue(KeyTrigger.ActiveOnFocusProperty, value); }
        }

        /// <summary>
        /// Determines whether or not to listen to the KeyDown or KeyUp event.
        /// </summary>
        public KeyTriggerFiredOn FiredOn
        {
            get { return (KeyTriggerFiredOn)this.GetValue(KeyTrigger.FiredOnProperty); }
            set { this.SetValue(KeyTrigger.FiredOnProperty, value); }
        }

        protected override string GetEventName()
        {
            return "Loaded";
        }

#if MIGRATION
        private void OnKeyPress(object sender, KeyEventArgs e)
#else
        private void OnKeyPress(object sender, KeyRoutedEventArgs e)
#endif
        {
            if (e.Key == this.Key &&
                Keyboard.Modifiers == GetActualModifiers(e.Key, this.Modifiers))
            {
                this.InvokeActions(e);
            }
        }

        private static ModifierKeys GetActualModifiers(Key key, ModifierKeys modifiers)
        {
#if __WPF__
            if (key == Key.LeftCtrl || key == Key.RightCtrl)
            {
                modifiers |= ModifierKeys.Control;
            }
            else if (key == Key.LeftAlt || key == Key.RightAlt || key == Key.System)
            {
                modifiers |= ModifierKeys.Alt;
            }
            else if (key == Key.LeftShift || key == Key.RightShift)
            {
                modifiers |= ModifierKeys.Shift;
            }
            return modifiers;
#else
#if MIGRATION
            if (key == Key.Ctrl)
#else
            if (key == Key.Control)
#endif
            {
                modifiers |= ModifierKeys.Control;
            }
#if MIGRATION
            else if (key == Key.Alt)
            {
                modifiers |= ModifierKeys.Alt;
            }
#else
            else if (key == Key.Menu)
            {
                modifiers |= ModifierKeys.Menu;
            }
#endif
            else if (key == Key.Shift)
            {
                modifiers |= ModifierKeys.Shift;
            }
            return modifiers;
#endif
        }

#if MIGRATION
        protected override void OnEvent(EventArgs eventArgs)
#else
        protected override void OnEvent(RoutedEventArgs eventArgs)
#endif
        {
            // Listen to keyboard events.
            if (this.ActiveOnFocus)
            {
                this.targetElement = this.Source;
            }
            else
            {
                this.targetElement = KeyTrigger.GetRoot(this.Source);
            }

            if (this.FiredOn == KeyTriggerFiredOn.KeyDown)
            {
                this.targetElement.KeyDown += this.OnKeyPress;
            }
            else
            {
                this.targetElement.KeyUp += this.OnKeyPress;
            }
        }

        protected override void OnDetaching()
        {
            if (this.targetElement != null)
            {
                if (this.FiredOn == KeyTriggerFiredOn.KeyDown)
                {
                    this.targetElement.KeyDown -= this.OnKeyPress;
                }
                else
                {
                    this.targetElement.KeyUp -= this.OnKeyPress;
                }
            }

            base.OnDetaching();
        }

        private static UIElement GetRoot(DependencyObject current)
        {
            UIElement last = null;

            while (current != null)
            {
                last = current as UIElement;
                current = VisualTreeHelper.GetParent(current);
            }

            return last;
        }
    }
}
