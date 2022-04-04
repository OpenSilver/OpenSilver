

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
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows.Automation;
using System.Windows.Automation.Peers;

#if MIGRATION
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
#else
using Windows.UI.Xaml.Controls.Primitives;
 using Windows.UI.Xaml.Input;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    internal interface ITimeInput
    {
        /// <summary>Gets or sets the current time.</summary>
        /// <value>The current time.</value>
        DateTime? Value { get; set; }

        /// <summary>Gets or sets the minimum time.</summary>
        /// <value>The minimum time.</value>
        DateTime? Minimum { get; set; }

        /// <summary>Gets or sets the maximum time.</summary>
        /// <value>The maximum time.</value>
        DateTime? Maximum { get; set; }
    }

    internal class TimeCoercionHelper
    {
        /// <summary>The TimeInput control that needs to be coerced.</summary>
        private ITimeInput _timeInputControl;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Windows.Controls.TimeCoercionHelper" /> class.
        /// </summary>
        /// <param name="timeInput">The time input that this helper will coerce.</param>
        /// <remarks>Lifetime of this helper class is determined by lifetime
        /// of control it is coercing.</remarks>
        public TimeCoercionHelper(ITimeInput timeInput)
        {
            this._timeInputControl = timeInput;
        }

        /// <summary>Processes the minimum value being set.</summary>
        /// <param name="newMinimum">The new value.</param>
        internal void ProcessMinimumChange(DateTime? newMinimum)
        {
            DateTime? maximum = this._timeInputControl.Maximum;
            DateTime? newMaximum = maximum;
            if (newMinimum.HasValue && maximum.HasValue)
            {
                DateTime date = newMinimum.Value;
                TimeSpan timeOfDay1 = date.TimeOfDay;
                date = maximum.Value;
                TimeSpan timeOfDay2 = date.TimeOfDay;
                if (timeOfDay1 > timeOfDay2)
                {
                    ref DateTime? local = ref newMaximum;
                    date = maximum.Value;
                    date = date.Date;
                    DateTime dateTime = date.Add(newMinimum.Value.TimeOfDay);
                    local = new DateTime?(dateTime);
                }
            }
            this.CoerceValueOnRangeMove(newMinimum, newMaximum);
            DateTime? nullable1 = maximum;
            DateTime? nullable2 = newMaximum;
            if ((nullable1.HasValue != nullable2.HasValue ? 1 : (!nullable1.HasValue ? 0 : (nullable1.GetValueOrDefault() != nullable2.GetValueOrDefault() ? 1 : 0))) == 0)
                return;
            this._timeInputControl.Maximum = newMaximum;
        }

        /// <summary>Processes the maximum value being set.</summary>
        /// <param name="newMaximum">The new value.</param>
        internal void ProcessMaximumChange(DateTime? newMaximum)
        {
            DateTime? minimum = this._timeInputControl.Minimum;
            DateTime? newMinimum = minimum;
            if (newMaximum.HasValue && minimum.HasValue)
            {
                DateTime date = newMaximum.Value;
                TimeSpan timeOfDay1 = date.TimeOfDay;
                date = minimum.Value;
                TimeSpan timeOfDay2 = date.TimeOfDay;
                if (timeOfDay1 < timeOfDay2)
                {
                    ref DateTime? local = ref newMinimum;
                    date = minimum.Value;
                    date = date.Date;
                    DateTime dateTime = date.Add(newMaximum.Value.TimeOfDay);
                    local = new DateTime?(dateTime);
                }
            }
            this.CoerceValueOnRangeMove(newMinimum, newMaximum);
            DateTime? nullable1 = minimum;
            DateTime? nullable2 = newMinimum;
            if ((nullable1.HasValue != nullable2.HasValue ? 1 : (!nullable1.HasValue ? 0 : (nullable1.GetValueOrDefault() != nullable2.GetValueOrDefault() ? 1 : 0))) == 0)
                return;
            this._timeInputControl.Minimum = newMinimum;
        }

        /// <summary>Coerces the value.</summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        /// <returns>
        /// True if no coercion was needed and the value will not be
        /// modified, false if the coercion logic will set a different value.
        /// </returns>
        internal bool CoerceValue(DateTime? oldValue, DateTime? newValue)
        {
            if (!newValue.HasValue)
                return true;
            DateTime dateTime;
            DateTime? nullable;
            int num;
            if (this._timeInputControl.Minimum.HasValue)
            {
                dateTime = newValue.Value;
                TimeSpan timeOfDay1 = dateTime.TimeOfDay;
                nullable = this._timeInputControl.Minimum;
                dateTime = nullable.Value;
                TimeSpan timeOfDay2 = dateTime.TimeOfDay;
                if (timeOfDay1 < timeOfDay2)
                {
                    num = 0;
                    goto label_8;
                }
            }
            nullable = this._timeInputControl.Maximum;
            if (nullable.HasValue)
            {
                dateTime = newValue.Value;
                TimeSpan timeOfDay1 = dateTime.TimeOfDay;
                nullable = this._timeInputControl.Maximum;
                dateTime = nullable.Value;
                TimeSpan timeOfDay2 = dateTime.TimeOfDay;
                num = !(timeOfDay1 > timeOfDay2) ? 1 : 0;
            }
            else
                num = 1;
            label_8:
            if (num != 0)
                return true;
            this._timeInputControl.Value = oldValue;
            return false;
        }

        /// <summary>Coerces the value.</summary>
        /// <param name="newMinimum">The new minimum.</param>
        /// <param name="newMaximum">The new maximum.</param>
        /// <returns>True if no coercion was needed and the value will not be
        /// modified, false if the coercion logic will set a different value.</returns>
        private bool CoerceValueOnRangeMove(DateTime? newMinimum, DateTime? newMaximum)
        {
            DateTime? nullable1 = this._timeInputControl.Value;
            DateTime? nullable2;
            if (this._timeInputControl.Value.HasValue)
            {
                DateTime dateTime1;
                if (newMinimum.HasValue && newMinimum.Value.TimeOfDay > this._timeInputControl.Value.Value.TimeOfDay)
                {
                    ref DateTime? local1 = ref nullable1;
                    DateTime date = this._timeInputControl.Value.Value;
                    date = date.Date;
                    ref DateTime local2 = ref date;
                    dateTime1 = newMinimum.Value;
                    TimeSpan timeOfDay = dateTime1.TimeOfDay;
                    DateTime dateTime2 = local2.Add(timeOfDay);
                    local1 = new DateTime?(dateTime2);
                }
                int num;
                if (newMaximum.HasValue)
                {
                    nullable2 = this._timeInputControl.Value;
                    num = !(nullable2.Value.TimeOfDay > newMaximum.Value.TimeOfDay) ? 1 : 0;
                }
                else
                    num = 1;
                if (num == 0)
                {
                    ref DateTime? local1 = ref nullable1;
                    nullable2 = this._timeInputControl.Value;
                    DateTime date = nullable2.Value;
                    date = date.Date;
                    ref DateTime local2 = ref date;
                    dateTime1 = newMaximum.Value;
                    TimeSpan timeOfDay = dateTime1.TimeOfDay;
                    DateTime dateTime2 = local2.Add(timeOfDay);
                    local1 = new DateTime?(dateTime2);
                }
            }
            nullable2 = nullable1;
            DateTime? nullable3 = this._timeInputControl.Value;
            if ((nullable2.HasValue != nullable3.HasValue ? 1 : (!nullable2.HasValue ? 0 : (nullable2.GetValueOrDefault() != nullable3.GetValueOrDefault() ? 1 : 0))) == 0)
                return true;
            this._timeInputControl.Value = nullable1;
            return false;
        }
    }

    internal static class TypeConverters
    {
        /// <summary>
        /// Returns a value indicating whether this converter can convert an
        /// object of the given type to an instance of the expected type.
        /// </summary>
        /// <typeparam name="T">Expected type of the converter.</typeparam>
        /// <param name="sourceType">
        /// The type of the source that is being evaluated for conversion.
        /// </param>
        /// <returns>
        /// A value indicating whether the converter can convert the provided
        /// type.
        /// </returns>
        internal static bool CanConvertFrom<T>(Type sourceType)
        {
            if (sourceType == null)
                throw new ArgumentNullException(nameof(sourceType));
            return sourceType == typeof(string) || typeof(T).IsAssignableFrom(sourceType);
        }

        /// <summary>
        /// Attempts to convert a specified object to an instance of the
        /// expected type.
        /// </summary>
        /// <typeparam name="T">Expected type of the converter.</typeparam>
        /// <param name="converter">TypeConverter instance.</param>
        /// <param name="value">The object being converted.</param>
        /// <returns>
        /// The instance of the expected type created from the converted object.
        /// </returns>
        internal static object ConvertFrom<T>(TypeConverter converter, object value)
        {
            Debug.Assert(converter != null, "converter should not be null!");
            if (value is T)
                return value;
            throw new NotSupportedException(string.Format((IFormatProvider)CultureInfo.CurrentCulture, "{0} {1} {2}", (object)converter.GetType().Name, value != null ? (object)value.GetType().FullName : (object)"(null)"));
        }

        /// <summary>
        /// Determines whether conversion is possible to a specified type.
        /// </summary>
        /// <typeparam name="T">Expected type of the converter.</typeparam>
        /// <param name="destinationType">
        /// Identifies the data type to evaluate for conversion.
        /// </param>
        /// <returns>A value indicating whether conversion is possible.</returns>
        internal static bool CanConvertTo<T>(Type destinationType)
        {
            if (destinationType == null)
                throw new ArgumentNullException(nameof(destinationType));
            return destinationType == typeof(string) || destinationType.IsAssignableFrom(typeof(T));
        }

        /// <summary>
        /// Attempts to convert a specified object to an instance of the
        /// desired type.
        /// </summary>
        /// <param name="converter">TypeConverter instance.</param>
        /// <param name="value">The object being converted.</param>
        /// <param name="destinationType">The type to convert the value to.</param>
        /// <returns>The value of the conversion to the specified type.</returns>
        internal static object ConvertTo(TypeConverter converter, object value, Type destinationType)
        {
            Debug.Assert(converter != null, "converter should not be null!");
            if (destinationType == null)
                throw new ArgumentNullException(nameof(destinationType));
            if (value == null && !destinationType.IsValueType)
                return (object)null;
            if (value != null && destinationType.IsAssignableFrom(value.GetType()))
                return value;
            throw new NotSupportedException(string.Format((IFormatProvider)CultureInfo.CurrentCulture, "{0} {1} {2}", (object)converter.GetType().Name, value != null ? (object)value.GetType().FullName : (object)"(null)", (object)destinationType.GetType().Name));
        }
    }
    public class TimeTypeConverter : TypeConverter
    {
        /// <summary>BackingField for the TimeFormats being used.</summary>
        private static readonly string[] _timeFormats = new string[6]
        {
      "h:mm tt",
      "h:mm:ss tt",
      "HH:mm",
      "HH:mm:ss",
      "H:mm",
      "H:mm:ss"
        };
        /// <summary>BackingField for the DateFormats being used.</summary>
        private static readonly string[] _dateFormats = new string[1]
        {
      "M/d/yyyy"
        };

        /// <summary>
        /// Determines whether this instance can convert from
        /// the specified type descriptor context.
        /// </summary>
        /// <param name="typeDescriptorContext">The type descriptor context.</param>
        /// <param name="sourceType">Type of the source.</param>
        /// <returns>
        /// 	<c>True</c> if this instance can convert from the specified type
        /// descriptor context; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvertFrom(
          ITypeDescriptorContext typeDescriptorContext,
          Type sourceType)
        {
            return Type.GetTypeCode(sourceType) == TypeCode.String;
        }

        /// <summary>
        /// Determines whether this instance can convert to the specified
        /// type descriptor context.
        /// </summary>
        /// <param name="typeDescriptorContext">The type descriptor context.</param>
        /// <param name="destinationType">Type of the destination.</param>
        /// <returns>
        /// 	<c>True</c> if this instance can convert to the specified type
        /// descriptor context; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvertTo(
          ITypeDescriptorContext typeDescriptorContext,
          Type destinationType)
        {
            return Type.GetTypeCode(destinationType) == TypeCode.String || TypeConverters.CanConvertTo<DateTime?>(destinationType);
        }

        /// <summary>
        /// Converts instances of other data types into instances of DateTime that
        /// represent a time.
        /// </summary>
        /// <param name="typeDescriptorContext">The type descriptor context.</param>
        /// <param name="cultureInfo">The culture used to convert. This culture
        /// is not used during conversion, but a specific set of formats is used.</param>
        /// <param name="source">The string being converted to the DateTime.</param>
        /// <returns>A DateTime that is the value of the conversion.</returns>
        public override object ConvertFrom(
          ITypeDescriptorContext typeDescriptorContext,
          CultureInfo cultureInfo,
          object source)
        {
            if (source == null)
                return (object)null;
            if (!(source is string s))
                throw new InvalidCastException(string.Format((IFormatProvider)CultureInfo.CurrentCulture, "{0} {1} {2}", (object)this.GetType().Name, source, (object)typeof(DateTime).Name));
            if (string.IsNullOrEmpty(s))
                return (object)null;
            DateTime result;
            foreach (string timeFormat in TimeTypeConverter._timeFormats)
            {
                if (DateTime.TryParseExact(s, timeFormat, (IFormatProvider)CultureInfo.InvariantCulture, DateTimeStyles.NoCurrentDateDefault, out result))
                {
                    DateTime dateTime = DateTime.Now;
                    dateTime = dateTime.Date;
                    return (object)dateTime.Add(result.TimeOfDay);
                }
            }
            foreach (string dateFormat in TimeTypeConverter._dateFormats)
            {
                foreach (string timeFormat in TimeTypeConverter._timeFormats)
                {
                    if (DateTime.TryParseExact(s, string.Format((IFormatProvider)CultureInfo.InvariantCulture, "{0} {1}", (object)dateFormat, (object)timeFormat), (IFormatProvider)CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
                        return (object)result;
                }
            }
            foreach (string dateFormat in TimeTypeConverter._dateFormats)
            {
                if (DateTime.TryParseExact(s, dateFormat, (IFormatProvider)CultureInfo.InvariantCulture, DateTimeStyles.NoCurrentDateDefault, out result))
                    return (object)result;
            }
            throw new FormatException(string.Format((IFormatProvider)CultureInfo.CurrentCulture, "{0} {1} {2}", (object)this.GetType().Name, (object)s, (object)typeof(DateTime).Name));
        }

        /// <summary>Converts a DateTime into a string.</summary>
        /// <param name="typeDescriptorContext">The type descriptor context.</param>
        /// <param name="cultureInfo">The culture used to convert.</param>
        /// <param name="value">
        /// The value that is being converted to a specified type.
        /// </param>
        /// <param name="destinationType">The type to convert the value to.</param>
        /// <returns>The value of the conversion to the specified type.</returns>
        public override object ConvertTo(
          ITypeDescriptorContext typeDescriptorContext,
          CultureInfo cultureInfo,
          object value,
          Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (value == null)
                    return (object)string.Empty;
                if (value is DateTime dateTime)
                    return (object)dateTime.ToString("HH:mm:ss", (IFormatProvider)new CultureInfo("en-US"));
            }
            return TypeConverters.ConvertTo((TypeConverter)this, value, destinationType);
        }
    }

    public class TimeFormatConverter : TypeConverter
    {
        /// <summary>
        /// Determines whether this instance can convert from the specified type
        /// descriptor context.
        /// </summary>
        /// <param name="typeDescriptorContext">The type descriptor context.</param>
        /// <param name="sourceType">Type of the source.</param>
        /// <returns>
        /// 	<c>True</c> if this instance can convert from the specified
        /// type descriptor context; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvertFrom(
          ITypeDescriptorContext typeDescriptorContext,
          Type sourceType)
        {
            return Type.GetTypeCode(sourceType) == TypeCode.String;
        }

        /// <summary>
        /// Determines whether this instance can convert to the specified type
        /// descriptor context.
        /// </summary>
        /// <param name="typeDescriptorContext">The type descriptor context.</param>
        /// <param name="destinationType">Type of the destination.</param>
        /// <returns>
        /// 	<c>True</c> if this instance can convert to the specified type
        /// descriptor context; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvertTo(
          ITypeDescriptorContext typeDescriptorContext,
          Type destinationType)
        {
            return Type.GetTypeCode(destinationType) == TypeCode.String || TypeConverters.CanConvertTo<DateTime?>(destinationType);
        }

        /// <summary>
        /// Converts instances of type string to an instance of type ITimeFormat.
        /// </summary>
        /// <param name="typeDescriptorContext">The type descriptor context.</param>
        /// <param name="cultureInfo">The culture used to convert.</param>
        /// <param name="source">The string that is converted.</param>
        /// <returns>
        /// An instance of ITimeFormat that is the value of the conversion.
        /// </returns>
        public override object ConvertFrom(
          ITypeDescriptorContext typeDescriptorContext,
          CultureInfo cultureInfo,
          object source)
        {
            if (source == null)
                throw new NotSupportedException(string.Format((IFormatProvider)CultureInfo.CurrentCulture, "{0} {1}", (object)this.GetType().Name, (object)"null"));
            if (source is string str)
            {
                if (string.Compare(str, "short", StringComparison.OrdinalIgnoreCase) == 0)
                    return (object)new ShortTimeFormat();
                if (string.Compare(str, "long", StringComparison.OrdinalIgnoreCase) == 0)
                    return (object)new LongTimeFormat();
                if (str.IndexOfAny(new char[3] { 'h', 'm', 's' }) < 0)
                    throw new FormatException(string.Format((IFormatProvider)CultureInfo.CurrentCulture, "{0} {1} {2}", (object)this.GetType().Name, (object)str, (object)typeof(ITimeFormat).Name));
                return (object)new CustomTimeFormat(str);
            }
            throw new InvalidCastException(string.Format((IFormatProvider)CultureInfo.CurrentCulture, "{0} {1} {2}", (object)this.GetType().Name, source, (object)typeof(ITimeFormat).Name));
        }

        /// <summary>
        /// Converts an known instance of type ITimeFormat to a string.
        /// </summary>
        /// <param name="typeDescriptorContext">The type descriptor context.</param>
        /// <param name="cultureInfo">The culture used to convert.</param>
        /// <param name="value">
        /// The value that is being converted to a specified type.
        /// </param>
        /// <param name="destinationType">The type to convert the value to.</param>
        /// <returns>The value of the conversion to the specified type.</returns>
        public override object ConvertTo(
          ITypeDescriptorContext typeDescriptorContext,
          CultureInfo cultureInfo,
          object value,
          Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                switch (value)
                {
                    case ShortTimeFormat _:
                        return (object)"Short";
                    case LongTimeFormat _:
                        return (object)"Long";
                    default:
                        CustomTimeFormat customTimeFormat = value as CustomTimeFormat;
                        if (customTimeFormat != (CustomTimeFormat)null)
                            return (object)customTimeFormat.Format;
                        break;
                }
            }
            return TypeConverters.ConvertTo((TypeConverter)this, value, destinationType);
        }
    }

    /// <summary>
    /// Represents a control that uses a spinner and textbox to allow a user to
    /// input time.
    /// </summary>
    /// <remarks>TimeInput supports only the following formatting characters:
    /// 'h', 'm', 's', 'H', 't'. All other characters are filtered out:
    /// 'd', 'f', 'F', 'g', 'K', 'M', 'y', 'z'.</remarks>
    /// <QualityBand>Preview</QualityBand>
    [StyleTypedProperty(Property = "SpinnerStyle", StyleTargetType = typeof(ButtonSpinner))]
    [TemplateVisualState(GroupName = "TimeHintStates", Name = "TimeHintOpenedUp")]
    [TemplatePart(Name = "Spinner", Type = typeof(Spinner))]
    [TemplateVisualState(GroupName = "CommonStates", Name = "Normal")]
    [TemplateVisualState(GroupName = "CommonStates", Name = "MouseOver")]
    [TemplateVisualState(GroupName = "CommonStates", Name = "Pressed")]
    [TemplateVisualState(GroupName = "CommonStates", Name = "Disabled")]
    [TemplateVisualState(GroupName = "FocusStates", Name = "Focused")]
    [TemplateVisualState(GroupName = "FocusStates", Name = "Unfocused")]
    [TemplatePart(Name = "TimeHintPopup", Type = typeof(Popup))]
    [TemplateVisualState(GroupName = "TimeHintStates", Name = "TimeHintOpenedDown")]
    [TemplateVisualState(GroupName = "TimeHintStates", Name = "TimeHintClosed")]
    [TemplateVisualState(GroupName = "ParsingStates", Name = "ValidTime")]
    [TemplateVisualState(GroupName = "ParsingStates", Name = "InvalidTime")]
    [TemplateVisualState(GroupName = "ParsingStates", Name = "EmptyTime")]
    [TemplatePart(Name = "Text", Type = typeof(TextBox))]
    public class TimeUpDown : UpDownBase<DateTime?>, IUpdateVisualState, ITimeInput
    {
        /// <summary>Identifies the Minimum dependency property.</summary>
        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(nameof(Minimum), typeof(DateTime?), typeof(TimeUpDown), new PropertyMetadata((object)null, new PropertyChangedCallback(TimeUpDown.OnMinimumPropertyChanged)));
        /// <summary>Identifies the Maximum dependency property.</summary>
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(nameof(Maximum), typeof(DateTime?), typeof(TimeUpDown), new PropertyMetadata((object)null, new PropertyChangedCallback(TimeUpDown.OnMaximumPropertyChanged)));
        /// <summary>Identifies the TimeParsers dependency property.</summary>
        public static readonly DependencyProperty TimeParsersProperty = DependencyProperty.Register(nameof(TimeParsers), typeof(TimeParserCollection), typeof(TimeUpDown), new PropertyMetadata(new PropertyChangedCallback(TimeUpDown.OnTimeParsersPropertyChanged)));
        /// <summary>Identifies the Format dependency property.</summary>
        public static readonly DependencyProperty FormatProperty = DependencyProperty.Register(nameof(Format), typeof(ITimeFormat), typeof(TimeUpDown), new PropertyMetadata(new PropertyChangedCallback(TimeUpDown.OnFormatPropertyChanged)));
        /// <summary>Identifies the Culture dependency property.</summary>
        public static readonly DependencyProperty CultureProperty = DependencyProperty.Register(nameof(Culture), typeof(CultureInfo), typeof(TimeUpDown), new PropertyMetadata((object)null, new PropertyChangedCallback(TimeUpDown.OnCulturePropertyChanged)));
        /// <summary>
        /// Identifies the TimeGlobalizationInfo dependency property.
        /// </summary>
        public static readonly DependencyProperty TimeGlobalizationInfoProperty = DependencyProperty.Register(nameof(TimeGlobalizationInfo), typeof(TimeGlobalizationInfo), typeof(TimeUpDown), new PropertyMetadata(new PropertyChangedCallback(TimeUpDown.OnTimeGlobalizationInfoPropertyChanged)));
        /// <summary>Identifies the IsCyclic dependency property.</summary>
        public static readonly DependencyProperty IsCyclicProperty = DependencyProperty.Register(nameof(IsCyclic), typeof(bool), typeof(TimeUpDown), new PropertyMetadata((object)true, new PropertyChangedCallback(TimeUpDown.OnIsCyclicPropertyChanged)));
        /// <summary>Identifies the TimeHintContent dependency property.</summary>
        public static readonly DependencyProperty TimeHintContentProperty = DependencyProperty.Register(nameof(TimeHintContent), typeof(object), typeof(TimeUpDown), new PropertyMetadata((object)string.Empty, new PropertyChangedCallback(TimeUpDown.OnTimeHintContentPropertyChanged)));
        /// <summary>StringFormat used in the TimeHint.</summary>
        private const string TimeHintFormat = "<{0}>";
        /// <summary>The name for the TimeHint element.</summary>
        private const string ElementTimeHintPopupName = "TimeHintPopup";
        /// <summary>The group name "TimeHintStates".</summary>
        internal const string GroupTimeHint = "TimeHintStates";
        /// <summary>The group name "ParsingStates".</summary>
        internal const string GroupTimeParsingStates = "ParsingStates";
        /// <summary>
        /// The state name "TimeHintOpenedUp" indicates that the hint is being
        /// shown on the top of the control.
        /// </summary>
        internal const string TimeHintOpenedUpStateName = "TimeHintOpenedUp";
        /// <summary>
        /// The state name "TimeHintOpenedDown" indicates that the hint is
        /// being shown at the bottom of the control.
        /// </summary>
        internal const string TimeHintOpenedDownStateName = "TimeHintOpenedDown";
        /// <summary>
        /// The state name "TimeHintClosed" indicates that no hint is being
        /// shown.
        /// </summary>
        internal const string TimeHintClosedStateName = "TimeHintClosed";
        /// <summary>
        /// The state name "ValidTime" that indicates that currently the textbox
        /// text parses to a valid Time.
        /// </summary>
        internal const string ValidTimeStateName = "ValidTime";
        /// <summary>
        /// The state name "InvalidTime" that indicates that currently the textbox
        /// text does not allow parsing.
        /// </summary>
        internal const string InvalidTimeStateName = "InvalidTime";
        /// <summary>
        /// The state name "EmptyTime" that indicates that currently the textbox
        /// text would parse to a Null.
        /// </summary>
        internal const string EmptyTimeStateName = "EmptyTime";
        /// <summary>BackingField for TimeHintPopupPart.</summary>
        private Popup _timeHintPopupPart;
        /// <summary>
        /// Helper class that centralizes the coercion logic across all
        /// TimeInput controls.
        /// </summary>
        private readonly TimeCoercionHelper _timeCoercionHelper;
        /// <summary>The text that was last parsed. Used in comparisons.</summary>
        private string _lastParsedText;
        /// <summary>The direction in which the TimeHint will expand.</summary>
        private ExpandDirection? _timeHintExpandDirection;
        /// <summary>BackingField for IsShowTimeHint.</summary>
        private bool _isShowTimeHint;
        /// <summary>
        /// A value indicating whether a dependency property change handler
        /// should ignore the next change notification.  This is used to reset
        /// the value of properties without performing any of the actions in
        /// their change handlers.
        /// </summary>
        private bool _ignoreValueChange;
        /// <summary>BackingField for ActualFormat.</summary>
        private ITimeFormat _actualFormat;
        /// <summary>BackingField for ActualTimeGlobalizationInfo.</summary>
        private TimeGlobalizationInfo _actualTimeGlobalizationInfo;
        /// <summary>
        /// Represents the formatted DateTime that is used in the TimeHint hint.
        /// </summary>
        private string _timeHintDate;
        /// <summary>
        /// Indicates whether the control should not proceed with selecting all
        /// text.
        /// </summary>
        private bool _isIgnoreSelectionOfAllText;
        /// <summary>BackingField for AllowHintContentChange.</summary>
        private bool _allowHintContentChange;

        /// <summary>Gets or sets the time hint popup part.</summary>
        /// <value>The time hint popup part.</value>
        private Popup TimeHintPopupPart
        {
            get
            {
                return this._timeHintPopupPart;
            }
            set
            {
                if (this._timeHintPopupPart != null && this._timeHintPopupPart.Child != null)
                    this._timeHintPopupPart.Child.MouseLeftButtonDown -= new MouseButtonEventHandler(this.OnTimeHintMouseLeftButtonDown);
                this._timeHintPopupPart = value;
                if (this._timeHintPopupPart == null)
                    return;
                if (this._timeHintPopupPart.Child != null)
                    this._timeHintPopupPart.Child.MouseLeftButtonDown += new MouseButtonEventHandler(this.OnTimeHintMouseLeftButtonDown);
                this.Dispatcher.BeginInvoke((Action)(() => this._timeHintPopupPart.IsOpen = true));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is showing a
        /// TimeHint visual.
        /// </summary>
        /// <value><c>True</c> if this instance is showing the TimeHint; otherwise, <c>false</c>.</value>
        private bool IsShowTimeHint
        {
            get
            {
                return this._isShowTimeHint;
            }
            set
            {
                if (value && (this.Text != null && this.TimeHintPopupPart != null && this.TimeHintPopupPart.Child != null && this.TimeHintPopupPart.Child is FrameworkElement child))
                {
                    child.Width = this.Text.ActualWidth;
                    this._timeHintExpandDirection = new ExpandDirection?();
                    MatrixTransform visual;
                    try
                    {
                        visual = this.TransformToVisual((UIElement)null) as MatrixTransform;
                    }
                    catch
                    {
                        this.IsShowTimeHint = false;
                        return;
                    }
                    child.Height = double.NaN;
                    child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    if (visual != null)
                    {
                        Matrix matrix = visual.Matrix;
                        if (matrix.OffsetY > child.DesiredSize.Height)
                        {
                            this._timeHintExpandDirection = new ExpandDirection?(ExpandDirection.Up);
                            this.TimeHintPopupPart.VerticalAlignment = VerticalAlignment.Top;
                        }
                        else
                        {
                            double actualHeight = Application.Current.Host.Content.ActualHeight;
                            matrix = visual.Matrix;
                            double num = matrix.OffsetY + this.ActualHeight;
                            if (actualHeight - num > child.DesiredSize.Height)
                            {
                                this._timeHintExpandDirection = new ExpandDirection?(ExpandDirection.Down);
                                this.TimeHintPopupPart.VerticalAlignment = VerticalAlignment.Bottom;
                            }
                            else
                            {
                                this.IsShowTimeHint = false;
                                return;
                            }
                        }
                    }
                }
                this._isShowTimeHint = value;
                this.UpdateVisualState(true);
            }
        }

        /// <summary>
        /// Gets the actual minimum. If a Minimum is set, use that, otherwise
        /// use the start of the day.
        /// </summary>
        private DateTime ActualMinimum
        {
            get
            {
                DateTime dateTime;
                if (!this.Minimum.HasValue)
                {
                    DateTime? nullable = this.Value;
                    if (!nullable.HasValue)
                    {
                        dateTime = DateTime.Now.Date;
                    }
                    else
                    {
                        nullable = this.Value;
                        dateTime = nullable.Value.Date;
                    }
                }
                else
                {
                    DateTime? minimum = this.Value;
                    if (!minimum.HasValue)
                    {
                        minimum = this.Minimum;
                        dateTime = minimum.Value;
                    }
                    else
                    {
                        minimum = this.Value;
                        DateTime date = minimum.Value;
                        date = date.Date;
                        ref DateTime local = ref date;
                        minimum = this.Minimum;
                        TimeSpan timeOfDay = minimum.Value.TimeOfDay;
                        dateTime = local.Add(timeOfDay);
                    }
                }
                return dateTime;
            }
        }

        /// <summary>
        /// Gets the actual maximum. If a Maximum is set, use that, otherwise
        /// use the end of the day.
        /// </summary>
        /// <value>The actual maximum.</value>
        private DateTime ActualMaximum
        {
            get
            {
                DateTime dateTime;
                if (!this.Maximum.HasValue)
                {
                    DateTime? nullable = this.Value;
                    if (!nullable.HasValue)
                    {
                        dateTime = DateTime.Now.Date.AddDays(1.0).Subtract(TimeSpan.FromSeconds(1.0));
                    }
                    else
                    {
                        nullable = this.Value;
                        DateTime date = nullable.Value;
                        date = date.Date;
                        dateTime = date.Add(TimeSpan.FromDays(1.0).Subtract(TimeSpan.FromSeconds(1.0)));
                    }
                }
                else
                {
                    DateTime? maximum = this.Value;
                    if (!maximum.HasValue)
                    {
                        maximum = this.Maximum;
                        dateTime = maximum.Value;
                    }
                    else
                    {
                        maximum = this.Value;
                        DateTime date = maximum.Value;
                        date = date.Date;
                        ref DateTime local = ref date;
                        maximum = this.Maximum;
                        TimeSpan timeOfDay = maximum.Value.TimeOfDay;
                        dateTime = local.Add(timeOfDay);
                    }
                }
                return dateTime;
            }
        }

        /// <summary>Gets or sets the currently selected time.</summary>
        [TypeConverter(typeof(TimeTypeConverter))]
        public override DateTime? Value
        {
            get
            {
                return base.Value;
            }
            set
            {
                base.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the minimum time considered valid by the control.
        /// </summary>
        /// <remarks>Setting the minimum property is applicable for the following
        /// features: Parsing a new value from the textbox, spinning a new value
        /// and programmatically specifying a value.</remarks>
        [TypeConverter(typeof(TimeTypeConverter))]
        public DateTime? Minimum
        {
            get
            {
                return (DateTime?)this.GetValue(TimeUpDown.MinimumProperty);
            }
            set
            {
                this.SetValue(TimeUpDown.MinimumProperty, (object)value);
            }
        }

        /// <summary>MinimumProperty property changed handler.</summary>
        /// <param name="d">TimeUpDown that changed its Minimum.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnMinimumPropertyChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            TimeUpDown timeUpDown = (TimeUpDown)d;
            DateTime? oldValue = (DateTime?)e.OldValue;
            DateTime? newValue = (DateTime?)e.NewValue;
            timeUpDown._ignoreValueChange = true;
            timeUpDown._timeCoercionHelper.ProcessMinimumChange(newValue);
            timeUpDown._ignoreValueChange = false;
            timeUpDown.OnMinimumChanged(oldValue, newValue);
            timeUpDown.SetValidSpinDirection();
        }

        /// <summary>Called when the Minimum property value has changed.</summary>
        /// <param name="oldValue">Old value of the Minimum property.</param>
        /// <param name="newValue">New value of the Minimum property.</param>
        protected virtual void OnMinimumChanged(DateTime? oldValue, DateTime? newValue)
        {
        }

        /// <summary>
        /// Gets or sets the maximum time considered valid by the control.
        /// </summary>
        /// <remarks>Setting the Maximum property is applicable for the following
        /// features: Parsing a new value from the textbox, spinning a new value
        /// and programmatically specifying a value. </remarks>
        [TypeConverter(typeof(TimeTypeConverter))]
        public DateTime? Maximum
        {
            get
            {
                return (DateTime?)this.GetValue(TimeUpDown.MaximumProperty);
            }
            set
            {
                this.SetValue(TimeUpDown.MaximumProperty, (object)value);
            }
        }

        /// <summary>MaximumProperty property changed handler.</summary>
        /// <param name="d">TimeUpDown that changed its Maximum.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnMaximumPropertyChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            TimeUpDown timeUpDown = (TimeUpDown)d;
            DateTime? oldValue = (DateTime?)e.OldValue;
            DateTime? newValue = (DateTime?)e.NewValue;
            timeUpDown._ignoreValueChange = true;
            timeUpDown._timeCoercionHelper.ProcessMaximumChange(newValue);
            timeUpDown._ignoreValueChange = false;
            timeUpDown.OnMaximumChanged(oldValue, newValue);
            timeUpDown.SetValidSpinDirection();
        }

        /// <summary>Called when the Maximum property value has changed.</summary>
        /// <param name="oldValue">Old value of the Maximum property.</param>
        /// <param name="newValue">New value of the Maximum property.</param>
        protected virtual void OnMaximumChanged(DateTime? oldValue, DateTime? newValue)
        {
        }

        /// <summary>
        /// Gets or sets a collection of TimeParsers that are used when parsing
        /// text to time.
        /// </summary>
        public TimeParserCollection TimeParsers
        {
            get
            {
                return this.GetValue(TimeUpDown.TimeParsersProperty) as TimeParserCollection;
            }
            set
            {
                this.SetValue(TimeUpDown.TimeParsersProperty, (object)value);
            }
        }

        /// <summary>TimeParsersProperty property changed handler.</summary>
        /// <param name="d">TimeUpDown that changed its TimeParsers.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnTimeParsersPropertyChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Gets the actual TimeParsers that will be used for parsing by the control.
        /// </summary>
        /// <remarks>Includes the TimeParsers introduced in the TimeGlobalizationInfo.</remarks>
        public TimeParserCollection ActualTimeParsers
        {
            get
            {
                return new TimeParserCollection(this.ActualTimeGlobalizationInfo.GetActualTimeParsers(this.TimeParsers == null ? (IEnumerable<TimeParser>)null : (IEnumerable<TimeParser>)this.TimeParsers.ToList<TimeParser>()));
            }
        }

        /// <summary>
        /// Gets or sets the Format used by the control.
        /// From XAML Use either "Short", "Long" or a custom format.
        /// Custom formats can only contain "H", "h", "m", "s" or "t".
        /// For example: use 'hh:mm:ss' is used to format time as "13:45:30".
        /// </summary>
        [TypeConverter(typeof(TimeFormatConverter))]
        public ITimeFormat Format
        {
            get
            {
                return this.GetValue(TimeUpDown.FormatProperty) as ITimeFormat;
            }
            set
            {
                this.SetValue(TimeUpDown.FormatProperty, (object)value);
            }
        }

        /// <summary>FormatProperty property changed handler.</summary>
        /// <param name="d">TimePicker that changed its Format.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnFormatPropertyChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            TimeUpDown timeUpDown = (TimeUpDown)d;
            if (e.NewValue != null)
                timeUpDown._actualFormat = (ITimeFormat)null;
            timeUpDown.SetTextBoxText();
        }

        /// <summary>
        /// Gets the actual format that will be used to display Time  in the
        /// TimeUpDown. If no format is specified, ShortTimeFormat is used.
        /// </summary>
        /// <value>The actual display format.</value>
        public ITimeFormat ActualFormat
        {
            get
            {
                if (this.Format != null)
                    return this.Format;
                if (this._actualFormat == null)
                    this._actualFormat = (ITimeFormat)new ShortTimeFormat();
                return this._actualFormat;
            }
        }

        /// <summary>
        /// Gets or sets the culture that will be used by the control for
        /// parsing and formatting.
        /// </summary>
        public CultureInfo Culture
        {
            get
            {
                return (CultureInfo)this.GetValue(TimeUpDown.CultureProperty);
            }
            set
            {
                this.SetValue(TimeUpDown.CultureProperty, (object)value);
            }
        }

        /// <summary>CultureProperty property changed handler.</summary>
        /// <param name="d">TimeUpDown that changed its Culture.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnCulturePropertyChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            TimeUpDown timeUpDown = (TimeUpDown)d;
            timeUpDown.ActualTimeGlobalizationInfo.Culture = e.NewValue as CultureInfo;
            timeUpDown.SetTextBoxText();
        }

        /// <summary>
        /// Gets the actual culture used by the control for formatting and parsing.
        /// </summary>
        /// <value>The actual culture.</value>
        public CultureInfo ActualCulture
        {
            get
            {
                return this.ActualTimeGlobalizationInfo.ActualCulture;
            }
        }

        /// <summary>
        /// Gets or sets the strategy object that determines how the control
        /// interacts with DateTime and CultureInfo.
        /// </summary>
        public TimeGlobalizationInfo TimeGlobalizationInfo
        {
            get
            {
                return (TimeGlobalizationInfo)this.GetValue(TimeUpDown.TimeGlobalizationInfoProperty);
            }
            set
            {
                this.SetValue(TimeUpDown.TimeGlobalizationInfoProperty, (object)value);
            }
        }

        /// <summary>
        /// TimeGlobalizationInfoProperty property changed handler.
        /// </summary>
        /// <param name="d">TimeUpDown that changed its TimeGlobalizationInfo.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnTimeGlobalizationInfoPropertyChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            TimeUpDown timeUpDown = (TimeUpDown)d;
            if (!(e.NewValue is TimeGlobalizationInfo newValue))
                return;
            newValue.Culture = timeUpDown.Culture;
            timeUpDown._actualTimeGlobalizationInfo = (TimeGlobalizationInfo)null;
        }

        /// <summary>
        /// Gets the actual TimeGlobalization info used by the control.
        /// </summary>
        /// <remarks>If TimeGlobalizationInfo is not set, will return
        /// default TimeGlobalizationInfo instance.</remarks>
        public TimeGlobalizationInfo ActualTimeGlobalizationInfo
        {
            get
            {
                TimeGlobalizationInfo globalizationInfo = this.TimeGlobalizationInfo;
                if (globalizationInfo != null)
                    return globalizationInfo;
                if (this._actualTimeGlobalizationInfo == null)
                    this._actualTimeGlobalizationInfo = new TimeGlobalizationInfo();
                return this._actualTimeGlobalizationInfo;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the TimeUpDown control will
        /// cycle through values when trying to spin the first and last item.
        /// </summary>
        public bool IsCyclic
        {
            get
            {
                return (bool)this.GetValue(TimeUpDown.IsCyclicProperty);
            }
            set
            {
                this.SetValue(TimeUpDown.IsCyclicProperty, (object)value);
            }
        }

        /// <summary>IsCyclicProperty property changed handler.</summary>
        /// <param name="d">DomainUpDown instance that changed its IsCyclic value.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnIsCyclicPropertyChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            ((TimeUpDown)d).SetValidSpinDirection();
        }

        /// <summary>
        /// Gets the text used to guide the user when entering time.
        /// </summary>
        public object TimeHintContent
        {
            get
            {
                return this.GetValue(TimeUpDown.TimeHintContentProperty);
            }
            private set
            {
                this._allowHintContentChange = true;
                this.SetValue(TimeUpDown.TimeHintContentProperty, value);
                this._allowHintContentChange = false;
            }
        }

        /// <summary>TimeHintContentProperty property changed handler.</summary>
        /// <param name="d">TimeUpDown that changed its TimeHintContent.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnTimeHintContentPropertyChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            TimeUpDown timeUpDown = (TimeUpDown)d;
            if (!timeUpDown._allowHintContentChange)
            {
                timeUpDown.TimeHintContent = e.OldValue;
                throw new InvalidOperationException("TimeUpDown_OnTimeHintContentChanged");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Windows.Controls.TimeUpDown" /> class.
        /// </summary>
        public TimeUpDown()
        {
            this.DefaultStyleKey = (object)typeof(TimeUpDown);
            this._timeCoercionHelper = new TimeCoercionHelper((ITimeInput)this);
        }

        /// <summary>
        /// Builds the visual tree for the TimeUpDown control when a new
        /// template is applied.
        /// </summary>
#if MIGRATION
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            if (this.Text != null)
            {
                this.Text.SelectionChanged -= new RoutedEventHandler(this.SelectionChanged);
                this.Text.TextChanged -= new TextChangedEventHandler(this.InputChanged);
                this.Text.LostFocus -= new RoutedEventHandler(this.OnTextLostFocus);
            }
            base.OnApplyTemplate();
            if (this.Text != null)
            {
                this.Text.SelectionChanged += new RoutedEventHandler(this.SelectionChanged);
                this.Text.TextChanged += new TextChangedEventHandler(this.InputChanged);
                this.Text.LostFocus += new RoutedEventHandler(this.OnTextLostFocus);
            }
            this.TimeHintPopupPart = this.GetTemplateChild("TimeHintPopup") as Popup;
            this.SetValidSpinDirection();
        }

        /// <summary>Provides handling for the ValueChanging event.</summary>
        /// <param name="e">Event args.</param>
        protected override void OnValueChanging(RoutedPropertyChangingEventArgs<DateTime?> e)
        {
            if (this._ignoreValueChange)
            {
                e.InCoercion = true;
                this.OnValueChanged(new RoutedPropertyChangedEventArgs<DateTime?>(e.OldValue, e.NewValue));
            }
            else if (this._timeCoercionHelper.CoerceValue(e.OldValue, e.NewValue))
            {
                e.InCoercion = false;
                e.NewValue = this.Value;
                base.OnValueChanging(e);
            }
            else
                e.InCoercion = true;
        }

        /// <summary>
        /// Raises the ValueChanged event when Value property has changed.
        /// </summary>
        /// <param name="e">Event args.</param>
        protected override void OnValueChanged(RoutedPropertyChangedEventArgs<DateTime?> e)
        {
            base.OnValueChanged(e);
            if (FocusManager.GetFocusedElement() != this.Text)
            {
                this._isIgnoreSelectionOfAllText = false;
                this.SelectAllText();
                this._isIgnoreSelectionOfAllText = true;
            }
            this.SetValidSpinDirection();
        }

        /// <summary>Called by ApplyValue to parse user input.</summary>
        /// <param name="text">User input.</param>
        /// <returns>Value parsed from user input.</returns>
        protected override DateTime? ParseValue(string text)
        {
            DateTime? time = this.ActualTimeGlobalizationInfo.ParseTime(text, this.ActualFormat, (IEnumerable<TimeParser>)this.TimeParsers);
            DateTime? nullable;
            int num;
            if (time.HasValue)
            {
                nullable = this.Value;
                num = !nullable.HasValue ? 1 : 0;
            }
            else
                num = 1;
            if (num != 0)
                return time;
            nullable = this.Value;
            return new DateTime?(nullable.Value.Date.Add(time.Value.TimeOfDay));
        }

        /// <summary>Renders the value property into the textbox text.</summary>
        /// <returns>Formatted Value.</returns>
        protected internal override string FormatValue()
        {
            this._lastParsedText = this.ActualTimeGlobalizationInfo.FormatTime(this.Value, this.ActualFormat);
            return this._lastParsedText;
        }

        /// <summary>
        /// Called by OnSpin when the spin direction is SpinDirection.Increase.
        /// </summary>
        protected override void OnIncrement()
        {
            if (this.Text == null)
                return;
            int selectionStart = this.Text.SelectionStart;
            TimeSpan unitAtTextPosition = this.ActualTimeGlobalizationInfo.GetTimeUnitAtTextPosition(this.Text.Text, selectionStart, this.ActualFormat);
            DateTime dateTime = this.ActualTimeGlobalizationInfo.OnIncrement(this.Value.Value, unitAtTextPosition);
            if (dateTime > this.ActualMaximum)
            {
                if (this.IsCyclic)
                {
                    if (this.Maximum.HasValue)
                        dateTime = this.ActualMinimum;
                }
                else
                    dateTime = this.ActualMaximum;
            }
            this.Value = new DateTime?(dateTime);
            if (unitAtTextPosition == this.ActualTimeGlobalizationInfo.GetTimeUnitAtTextPosition(this.Text.Text, selectionStart, this.ActualFormat))
            {
                this.Text.SelectionStart = selectionStart;
            }
            else
            {
                int positionForTimeUnit = this.ActualTimeGlobalizationInfo.GetTextPositionForTimeUnit(this.Text.Text, unitAtTextPosition, this.ActualFormat);
                this.Text.SelectionStart = positionForTimeUnit > -1 ? positionForTimeUnit : selectionStart;
            }
        }

        /// <summary>
        /// Called by OnSpin when the spin direction is SpinDirection.Increase.
        /// </summary>
        protected override void OnDecrement()
        {
            if (this.Text == null)
                return;
            int selectionStart = this.Text.SelectionStart;
            TimeSpan unitAtTextPosition = this.ActualTimeGlobalizationInfo.GetTimeUnitAtTextPosition(this.Text.Text, selectionStart, this.ActualFormat);
            DateTime dateTime = this.ActualTimeGlobalizationInfo.OnDecrement(this.Value.Value, unitAtTextPosition);
            if (dateTime < this.ActualMinimum)
            {
                if (this.IsCyclic)
                {
                    if (this.Minimum.HasValue)
                        dateTime = this.ActualMaximum;
                }
                else
                    dateTime = this.ActualMinimum;
            }
            this.Value = new DateTime?(dateTime);
            if (unitAtTextPosition == this.ActualTimeGlobalizationInfo.GetTimeUnitAtTextPosition(this.Text.Text, selectionStart, this.ActualFormat))
            {
                this.Text.SelectionStart = selectionStart;
            }
            else
            {
                int positionForTimeUnit = this.ActualTimeGlobalizationInfo.GetTextPositionForTimeUnit(this.Text.Text, unitAtTextPosition, this.ActualFormat);
                this.Text.SelectionStart = positionForTimeUnit > -1 ? positionForTimeUnit : selectionStart;
            }
        }

        /// <summary>
        /// Sets the valid spin direction based on the position of the caret,
        /// the value and the minimum and maximum.
        /// </summary>
        private void SetValidSpinDirection()
        {
            ValidSpinDirections validSpinDirections = ValidSpinDirections.None;
            if (this.Text != null && FocusManager.GetFocusedElement() == this.Text && !string.IsNullOrEmpty(this.Text.Text))
            {
                DateTime? nullable = new DateTime?();
                try
                {
                    nullable = this.ParseValue(this.Text.Text);
                }
                catch (ArgumentException ex)
                {
                }
                if (!nullable.HasValue)
                    validSpinDirections = ValidSpinDirections.None;
                else if (this.IsCyclic)
                {
                    validSpinDirections = ValidSpinDirections.Increase | ValidSpinDirections.Decrease;
                }
                else
                {
                    TimeSpan unitAtTextPosition = this.ActualTimeGlobalizationInfo.GetTimeUnitAtTextPosition(this.Text.Text, this.Text.SelectionStart, this.ActualFormat);
                    if (this.ActualTimeGlobalizationInfo.OnIncrement(nullable.Value, unitAtTextPosition) <= this.ActualMaximum)
                        validSpinDirections |= ValidSpinDirections.Increase;
                    if (this.ActualTimeGlobalizationInfo.OnDecrement(nullable.Value, unitAtTextPosition) >= this.ActualMinimum)
                        validSpinDirections |= ValidSpinDirections.Decrease;
                }
            }
            if (this.Spinner == null)
                return;
            this.Spinner.ValidSpinDirection = validSpinDirections;
        }

        /// <summary>Handles the SelectionChanged event from TextBox.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs" />
        /// instance containing the event data.</param>
        private void SelectionChanged(object sender, RoutedEventArgs e)
        {
            this.SetValidSpinDirection();
        }

        /// <summary>Handles the TextChanged event.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="T:System.Windows.Controls.TextChangedEventArgs" />
        /// instance containing the event data.</param>
        private void InputChanged(object sender, TextChangedEventArgs e)
        {
            this.DetermineHint();
        }

        /// <summary>Determines the value of the hint property.</summary>
        private void DetermineHint()
        {
            if (DesignerProperties.GetIsInDesignMode((DependencyObject)this))
                return;
            if (FocusManager.GetFocusedElement() != this.Text)
            {
                this.IsShowTimeHint = false;
            }
            else
            {
                Func<DateTime, DateTime> func = (Func<DateTime, DateTime>)(time =>
                {
                    if (time.TimeOfDay > this.ActualMaximum.TimeOfDay)
                        return this.ActualMaximum;
                    return time.TimeOfDay < this.ActualMinimum.TimeOfDay ? this.ActualMinimum : time;
                });
                DateTime dateTime1 = func(DateTime.Now);
                UpDownParsingEventArgs<DateTime?> e = new UpDownParsingEventArgs<DateTime?>(this.Text.Text);
                DateTime? result;
                if (!this.ActualTimeGlobalizationInfo.TryParseTime(this.Text.Text, this.ActualFormat, (IEnumerable<TimeParser>)this.TimeParsers, out result))
                {
                    try
                    {
                        this.OnParsing(e);
                        if (e.Handled && e.Value.HasValue)
                            dateTime1 = func(e.Value.Value);
                    }
                    catch (Exception ex)
                    {
                    }
                    this.IsShowTimeHint = true;
                    VisualStateManager.GoToState((Control)this, "InvalidTime", true);
                    this._timeHintDate = this.ActualTimeGlobalizationInfo.FormatTime(new DateTime?(dateTime1), this.ActualFormat);
                    this.TimeHintContent = (object)string.Format((IFormatProvider)CultureInfo.InvariantCulture, "<{0}>", (object)this._timeHintDate);
                }
                else
                {
                    e.Value = result;
                    try
                    {
                        this.OnParsing(e);
                        if (e.Handled)
                            result = e.Value;
                    }
                    catch (Exception ex)
                    {
                    }
                    if (result.HasValue)
                    {
                        TimeSpan timeOfDay1 = result.Value.TimeOfDay;
                        DateTime dateTime2 = this.ActualMaximum;
                        TimeSpan timeOfDay2 = dateTime2.TimeOfDay;
                        int num;
                        if (!(timeOfDay1 > timeOfDay2))
                        {
                            dateTime2 = result.Value;
                            TimeSpan timeOfDay3 = dateTime2.TimeOfDay;
                            dateTime2 = this.ActualMinimum;
                            TimeSpan timeOfDay4 = dateTime2.TimeOfDay;
                            num = !(timeOfDay3 < timeOfDay4) ? 1 : 0;
                        }
                        else
                            num = 0;
                        if (num == 0)
                        {
                            this.IsShowTimeHint = true;
                            VisualStateManager.GoToState((Control)this, "InvalidTime", true);
                            this._timeHintDate = this.ActualTimeGlobalizationInfo.FormatTime(new DateTime?(func(result.Value)), this.ActualFormat);
                            this.TimeHintContent = (object)string.Format((IFormatProvider)CultureInfo.InvariantCulture, "<{0}>", (object)this._timeHintDate);
                        }
                        else
                        {
                            this._timeHintDate = this.ActualTimeGlobalizationInfo.FormatTime(new DateTime?(result.Value), this.ActualFormat);
                            this.IsShowTimeHint = !this.Text.Text.Equals(this._timeHintDate, StringComparison.OrdinalIgnoreCase);
                            VisualStateManager.GoToState((Control)this, "ValidTime", true);
                            this.TimeHintContent = (object)this._timeHintDate;
                        }
                    }
                    else
                    {
                        this.IsShowTimeHint = true;
                        this._timeHintDate = this.ActualTimeGlobalizationInfo.FormatTime(new DateTime?(dateTime1), this.ActualFormat);
                        VisualStateManager.GoToState((Control)this, "EmptyTime", true);
                        this.TimeHintContent = (object)string.Format((IFormatProvider)CultureInfo.InvariantCulture, "<{0}>", (object)this._timeHintDate);
                    }
                }
            }
        }

        /// <summary>
        /// Handles the Left Mouse Button Down event of the TimeHint.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs" />
        /// instance containing the event data.</param>
        private void OnTimeHintMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.IsShowTimeHint = false;
            this.ApplyValue(this._timeHintDate);
        }

        /// <summary>Provides handling for the KeyDown event.</summary>
        /// <param name="e">Key event args.</param>
        /// <remarks>Only support up and down arrow keys.</remarks>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (!this.IsEditable)
            {
                e.Handled = true;
            }
            else
            {
                if (this.Text.Text != this._lastParsedText && (e.Key == Key.Up || e.Key == Key.Down))
                {
                    int selectionStart = this.Text.SelectionStart;
                    this.ApplyValue(this.Text.Text);
                    e.Handled = true;
                    if (selectionStart >= 0 && selectionStart < this.Text.Text.Length)
                    {
                        this.Text.SelectionStart = selectionStart;
                        if (e.Key == Key.Up)
                            this.OnIncrement();
                        else
                            this.OnDecrement();
                    }
                }
                base.OnKeyDown(e);
            }
        }

        /// <summary>Provides handling for the GotFocus event.</summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            if (!this.Interaction.AllowGotFocus(e))
                return;
            this.DetermineHint();
            this.SetValidSpinDirection();
            this.Interaction.OnGotFocusBase();
            base.OnGotFocus(e);
        }

        /// <summary>Provides handling for the LostFocus event.</summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            if (!this.Interaction.AllowLostFocus(e))
                return;
            this.IsShowTimeHint = false;
            this.SetValidSpinDirection();
            this.Interaction.OnLostFocusBase();
            base.OnLostFocus(e);
        }

        /// <summary>Provides handling for the MouseEnter event.</summary>
        /// <param name="e">The data for the event.</param>
#if MIGRATION
        protected override void OnMouseEnter(MouseEventArgs e)
#else
        protected virtual void OnPointerEntered(PointerRoutedEventArgs e)
#endif
        {
            if (!this.Interaction.AllowMouseEnter(e))
                return;
            this.Interaction.OnMouseEnterBase();
            base.OnMouseEnter(e);
        }

        /// <summary>Provides handling for the MouseLeave event.</summary>
        /// <param name="e">The data for the event.</param>
#if MIGRATION
        protected internal override void OnMouseLeave(MouseEventArgs e)
#else 
        protected internal virtual void OnPointerExited(PointerRoutedEventArgs e)
#endif
        {
            if (!this.Interaction.AllowMouseLeave(e))
                return;
            this.Interaction.OnMouseLeaveBase();
            base.OnMouseLeave(e);
        }

        /// <summary>Provides handling for the MouseLeftButtonDown event.</summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (!this.Interaction.AllowMouseLeftButtonDown(e))
                return;
            this.Interaction.OnMouseLeftButtonDownBase();
            base.OnMouseLeave((MouseEventArgs)e);
        }

        /// <summary>Called before the MouseLeftButtonUp event occurs.</summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (!this.Interaction.AllowMouseLeftButtonUp(e))
                return;
            this.Interaction.OnMouseLeftButtonUpBase();
            base.OnMouseLeftButtonUp(e);
        }

        /// <summary>Selects all text.</summary>
        protected override void SelectAllText()
        {
            if (this._isIgnoreSelectionOfAllText)
                return;
            base.SelectAllText();
        }

        /// <summary>
        /// Event handler for Text template part's LostFocus event.
        /// We use this event to compare current TextBox.Text with cached previous
        /// value to decide whether user has typed in a new value.
        /// </summary>
        /// <param name="sender">The Text template part.</param>
        /// <param name="e">Event args.</param>
        private void OnTextLostFocus(object sender, RoutedEventArgs e)
        {
            this._isIgnoreSelectionOfAllText = false;
            this.SelectAllText();
            this._isIgnoreSelectionOfAllText = true;
        }

        /// <summary>Update current visual state.</summary>
        /// <param name="useTransitions">True to use transitions when updating the visual state, false to
        /// snap directly to the new visual state.</param>
        internal override void UpdateVisualState(bool useTransitions)
        {
            if (!DesignerProperties.GetIsInDesignMode((DependencyObject)this))
            {
                if (!this._timeHintExpandDirection.HasValue || !this.IsShowTimeHint)
                {
                    VisualStateManager.GoToState((Control)this, "TimeHintClosed", useTransitions);
                }
                else
                {
                    ExpandDirection? hintExpandDirection = this._timeHintExpandDirection;
                    VisualStateManager.GoToState((Control)this, (hintExpandDirection.GetValueOrDefault() != ExpandDirection.Up ? 0 : (hintExpandDirection.HasValue ? 1 : 0)) != 0 ? "TimeHintOpenedUp" : "TimeHintOpenedDown", useTransitions);
                }
            }
            base.UpdateVisualState(useTransitions);
        }

        void IUpdateVisualState.UpdateVisualState(bool useTransitions)
        {
            this.UpdateVisualState(useTransitions);
        }

        DateTime? ITimeInput.Value
        {
            get
            {
                return this.Value;
            }
            set
            {
                this.Value = value;
            }
        }

        DateTime? ITimeInput.Minimum
        {
            get
            {
                return this.Minimum;
            }
            set
            {
                this.Minimum = value;
            }
        }

        DateTime? ITimeInput.Maximum
        {
            get
            {
                return this.Maximum;
            }
            set
            {
                this.Maximum = value;
            }
        }
    }
}
