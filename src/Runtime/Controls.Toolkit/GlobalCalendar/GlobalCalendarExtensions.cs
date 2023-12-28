// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace System.Windows.Controls
{
    /// <summary>
    /// Inherited code: Requires comment.
    /// </summary>
    /// <QualityBand>Experimental</QualityBand>
    internal static class GlobalCalendarExtensions
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
        public static bool IsHandlerSuspended(DependencyObject obj, DependencyProperty dependencyProperty)
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
        public static void SetValueNoCallback(DependencyObject obj, DependencyProperty property, object value)
        {
            SuspendHandler(obj, property, true);
            try
            {
                obj.SetValue(property, value);
            }
            finally
            {
                SuspendHandler(obj, property, false);
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="obj">Inherited code: Requires comment 1.</param>
        /// <param name="dependencyProperty">Inherited code: Requires comment 2.</param>
        /// <param name="suspend">Inherited code: Requires comment 3.</param>
        private static void SuspendHandler(DependencyObject obj, DependencyProperty dependencyProperty, bool suspend)
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
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Inherited mature control.")]
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

        /// <summary>
        /// Returns true if any day in the given DateTime range is contained in
        /// the current CalendarDateRange.
        /// </summary>
        /// <param name="value">The current range.</param>
        /// <param name="range">The range to compare.</param>
        /// <param name="info">The CalendarInfo.</param>
        /// <returns>
        /// A value indicating whether any day in the given DateTime range is\
        /// contained in the current CalendarDateRange.
        /// </returns>
        public static bool ContainsAny(CalendarDateRange value, CalendarDateRange range, CalendarInfo info)
        {
            Debug.Assert(value != null, "value should not be null!");
            Debug.Assert(range != null, "range should not be null!");
            Debug.Assert(info != null, "info should not be null!");

            int start = info.Compare(value.Start, range.Start);

            // Check if any part of the supplied range is contained by this
            // range or if the supplied range completely covers this range.
            return (start <= 0 && info.Compare(value.End, range.Start) >= 0) ||
                (start >= 0 && info.Compare(value.Start, range.End) <= 0);
        }

        /// <summary>
        /// Get the date of a GlobalCalendarDayButton.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <returns>The date of the button.</returns>
        public static DateTime GetDate(GlobalCalendarDayButton button)
        {
            Debug.Assert(button != null, "button should not be null!");
            return (DateTime)button.DataContext;
        }

        /// <summary>
        /// Get the date of a GlobalCalendarDayButton.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <returns>The date of the button.</returns>
        public static DateTime? GetDateNullable(GlobalCalendarDayButton button)
        {
            Debug.Assert(button != null, "button should not be null!");
            return button.DataContext as DateTime?;
        }

        /// <summary>
        /// Set the date of a GlobalCalendarDayButton.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <param name="date">The date of the button.</param>
        public static void SetDate(GlobalCalendarDayButton button, DateTime date)
        {
            Debug.Assert(button != null, "button should not be null!");
            button.DataContext = date;
        }
    }
}