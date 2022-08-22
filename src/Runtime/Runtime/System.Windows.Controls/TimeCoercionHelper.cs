// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Helper class that centralizes the coercion logic across all 
    /// TimeInput controls.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    internal class TimeCoercionHelper
    {
        /// <summary>
        /// The TimeInput control that needs to be coerced.
        /// </summary>
        private ITimeInput _timeInputControl;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeCoercionHelper"/> class.
        /// </summary>
        /// <param name="timeInput">The time input that this helper will coerce.</param>
        /// <remarks>Lifetime of this helper class is determined by lifetime
        /// of control it is coercing.</remarks>
        public TimeCoercionHelper(ITimeInput timeInput)
        {
            _timeInputControl = timeInput;
        }

        /// <summary>
        /// Processes the minimum value being set.
        /// </summary>
        /// <param name="newMinimum">The new value.</param>
        internal void ProcessMinimumChange(DateTime? newMinimum)
        {
            DateTime? initialMaximum = _timeInputControl.Maximum;
            DateTime? newMaximum = initialMaximum;

            if (newMinimum.HasValue && initialMaximum.HasValue)
            {
                if (newMinimum.Value.TimeOfDay > initialMaximum.Value.TimeOfDay)
                {
                    // raise maximum to minimum
                    newMaximum = initialMaximum.Value.Date.Add(newMinimum.Value.TimeOfDay);
                }
            }

            // Possibly need to coerce value.
            CoerceValueOnRangeMove(newMinimum, newMaximum);

            if (initialMaximum != newMaximum)
            {
                _timeInputControl.Maximum = newMaximum;
            }
        }

        /// <summary>
        /// Processes the maximum value being set.
        /// </summary>
        /// <param name="newMaximum">The new value.</param>
        internal void ProcessMaximumChange(DateTime? newMaximum)
        {
            DateTime? initialMinimum = _timeInputControl.Minimum;
            DateTime? newMinimum = initialMinimum;

            if (newMaximum.HasValue && initialMinimum.HasValue)
            {
                if (newMaximum.Value.TimeOfDay < initialMinimum.Value.TimeOfDay)
                {
                    // lower minimum to maximum
                    newMinimum = initialMinimum.Value.Date.Add(newMaximum.Value.TimeOfDay);
                }
            }

            // Possibly need to coerce value.
            CoerceValueOnRangeMove(newMinimum, newMaximum);

            if (initialMinimum != newMinimum)
            {
                _timeInputControl.Minimum = newMinimum;
            }
        }

        /// <summary>
        /// Coerces the value.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        /// <returns>
        /// True if no coercion was needed and the value will not be
        /// modified, false if the coercion logic will set a different value.
        /// </returns>
        internal bool CoerceValue(DateTime? oldValue, DateTime? newValue)
        {
            if (newValue.HasValue == false)
            {
                return true;
            }

            if ((_timeInputControl.Minimum.HasValue && newValue.Value.TimeOfDay < _timeInputControl.Minimum.Value.TimeOfDay) ||
                (_timeInputControl.Maximum.HasValue && newValue.Value.TimeOfDay > _timeInputControl.Maximum.Value.TimeOfDay))
            {
                _timeInputControl.Value = oldValue;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Coerces the value.
        /// </summary>
        /// <param name="newMinimum">The new minimum.</param>
        /// <param name="newMaximum">The new maximum.</param>
        /// <returns>True if no coercion was needed and the value will not be
        /// modified, false if the coercion logic will set a different value.</returns>
        private bool CoerceValueOnRangeMove(DateTime? newMinimum, DateTime? newMaximum)
        {
            DateTime? newValue = _timeInputControl.Value;

            if (_timeInputControl.Value.HasValue)
            {
                if (newMinimum.HasValue && newMinimum.Value.TimeOfDay > _timeInputControl.Value.Value.TimeOfDay)
                {
                    newValue = _timeInputControl.Value.Value.Date.Add(newMinimum.Value.TimeOfDay);
                }
                if (newMaximum.HasValue && _timeInputControl.Value.Value.TimeOfDay > newMaximum.Value.TimeOfDay)
                {
                    newValue = _timeInputControl.Value.Value.Date.Add(newMaximum.Value.TimeOfDay);
                }
            }

            if (newValue != _timeInputControl.Value)
            {
                _timeInputControl.Value = newValue;
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
