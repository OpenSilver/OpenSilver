// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;

#if !MIGRATION
using ModifierKeys = Windows.System.VirtualKeyModifiers;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Inherited code: Requires comment.
    /// </summary>
    /// <QualityBand>Mature</QualityBand>
    internal static class CalendarExtensions
    {
        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private static Dictionary<DependencyObject, Dictionary<DependencyProperty, bool>> _suspendedHandlers = new Dictionary<DependencyObject, Dictionary<DependencyProperty, bool>>();

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="obj">Inherited code: Requires comment 1.</param>
        /// <param name="dependencyProperty">Inherited code: Requires comment 2.</param>
        /// <returns>Inherited code: Requires comment 3.</returns>
        public static bool IsHandlerSuspended(this DependencyObject obj, DependencyProperty dependencyProperty)
        {
            if (_suspendedHandlers.ContainsKey(obj))
            {
                return _suspendedHandlers[obj].ContainsKey(dependencyProperty);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="obj">Inherited code: Requires comment 1.</param>
        /// <param name="property">Inherited code: Requires comment 2.</param>
        /// <param name="value">Inherited code: Requires comment 3.</param>
        public static void SetValueNoCallback(this DependencyObject obj, DependencyProperty property, object value)
        {
            obj.SuspendHandler(property, true);
            try
            {
                obj.SetValue(property, value);
            }
            finally
            {
                obj.SuspendHandler(property, false);
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="obj">Inherited code: Requires comment 1.</param>
        /// <param name="dependencyProperty">Inherited code: Requires comment 2.</param>
        /// <param name="suspend">Inherited code: Requires comment 3.</param>
        private static void SuspendHandler(this DependencyObject obj, DependencyProperty dependencyProperty, bool suspend)
        {
            if (_suspendedHandlers.ContainsKey(obj))
            {
                Dictionary<DependencyProperty, bool> suspensions = _suspendedHandlers[obj];

                if (suspend)
                {
                    Debug.Assert(!suspensions.ContainsKey(dependencyProperty), "Suspensions should not contain the property!");

                    // true = dummy value
                    suspensions[dependencyProperty] = true;
                }
                else
                {
                    Debug.Assert(suspensions.ContainsKey(dependencyProperty), "Suspensions should contain the property!");
                    suspensions.Remove(dependencyProperty);
                    if (suspensions.Count == 0)
                    {
                        _suspendedHandlers.Remove(obj);
                    }
                }
            }
            else
            {
                Debug.Assert(suspend, "suspend should be true!");
                _suspendedHandlers[obj] = new Dictionary<DependencyProperty, bool>();
                _suspendedHandlers[obj][dependencyProperty] = true;
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="ctrl">Inherited code: Requires comment 1.</param>
        public static void GetMetaKeyState(out bool ctrl)
        {
            ctrl = (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;

            // The Apple key on a Mac is supposed to behave like the CTRL key on
            // a PC for things like multi-select, select-all, and grid
            // navigation.  To allow for this, we set the CTRL to true if the
            // Apple key is pressed.  One downside to this is that the Apple key
            // maps to the same value as the Windows key in the ModifierKeys
            // enum, which means the Windows key will also behave like CTRL.  If
            // this ever becomes an issue, we will have to add platform-specific
            // logic here.
            ctrl |= (Keyboard.Modifiers & ModifierKeys.Apple) == ModifierKeys.Apple;
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="ctrl">Inherited code: Requires comment 2.</param>
        /// <param name="shift">Inherited code: Requires comment 3.</param>
        public static void GetMetaKeyState(out bool ctrl, out bool shift)
        {
            ctrl = (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;

            // The Apple key on a Mac is supposed to behave like the CTRL key on
            // a PC for things like multi-select, select-all, and grid
            // navigation.  To allow for this, we set the CTRL to true if the
            // Apple key is pressed.  One downside to this is that the Apple key
            // maps to the same value as the Windows key in the ModifierKeys
            // enum, which means the Windows key will also behave like CTRL.  If
            // this ever becomes an issue, we will have to add platform-specific
            // logic here.
            ctrl |= (Keyboard.Modifiers & ModifierKeys.Apple) == ModifierKeys.Apple;
            shift = (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
        }
    }
}