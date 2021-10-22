

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
using System.Globalization;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents a control that uses a spinner and textbox to allow a user to
    /// input time.
    /// </summary>
    [OpenSilver.NotImplemented]
    public class TimeUpDown : UpDownBase, IUpdateVisualState
    {
        /// <summary>
        /// Update the visual state of the control.
        /// </summary>
        /// <param name="useTransitions">
        /// A value indicating whether to automatically generate transitions to
        /// the new state, or instantly transition to the new state.
        /// </param>
        void IUpdateVisualState.UpdateVisualState(bool useTransitions)
        {
            throw new NotImplementedException();
        }


        #region public DateTime? Minimum
        /// <summary>
        /// Gets or sets the minimum time considered valid by the control.
        /// </summary>
        /// <remarks>Setting the minimum property is applicable for the following
        /// features: Parsing a new value from the textbox, spinning a new value
        /// and programmatically specifying a value.</remarks>
        [OpenSilver.NotImplemented]
        public DateTime? Minimum
        {
            get => (DateTime?)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        /// <summary>
        /// Identifies the Minimum dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(
                "Minimum",
                typeof(DateTime?),
                typeof(TimeUpDown),
                new PropertyMetadata(null, OnMinimumPropertyChanged));

        /// <summary>
        /// MinimumProperty property changed handler.
        /// </summary>
        /// <param name="d">TimeUpDown that changed its Minimum.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnMinimumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Called when the Minimum property value has changed.
        /// </summary>
        /// <param name="oldValue">Old value of the Minimum property.</param>
        /// <param name="newValue">New value of the Minimum property.</param>
        [OpenSilver.NotImplemented]
        protected virtual void OnMinimumChanged(DateTime? oldValue, DateTime? newValue)
        {
        }
        #endregion public DateTime? Minimum

        #region public DateTime? Maximum
        /// <summary>
        /// Gets or sets the maximum time considered valid by the control.
        /// </summary>
        /// <remarks>Setting the Maximum property is applicable for the following
        /// features: Parsing a new value from the textbox, spinning a new value
        /// and programmatically specifying a value. </remarks>
        [OpenSilver.NotImplemented]
        public DateTime? Maximum
        {
            get => (DateTime?)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        /// <summary>
        /// Identifies the Maximum dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(
                "Maximum",
                typeof(DateTime?),
                typeof(TimeUpDown),
                new PropertyMetadata(null, OnMaximumPropertyChanged));

        /// <summary>
        /// MaximumProperty property changed handler.
        /// </summary>
        /// <param name="d">TimeUpDown that changed its Maximum.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnMaximumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Called when the Maximum property value has changed.
        /// </summary>
        /// <param name="oldValue">Old value of the Maximum property.</param>
        /// <param name="newValue">New value of the Maximum property.</param>
        [OpenSilver.NotImplemented]
        protected virtual void OnMaximumChanged(DateTime? oldValue, DateTime? newValue)
        {
        }
        #endregion public DateTime? Maximum

        #region public TimeParserCollection TimeParsers
        /// <summary>
        /// Gets or sets a collection of TimeParsers that are used when parsing
        /// text to time.
        /// </summary>
        [OpenSilver.NotImplemented]
        public TimeParserCollection TimeParsers
        {
            get => GetValue(TimeParsersProperty) as TimeParserCollection;
            set => SetValue(TimeParsersProperty, value);
        }

        /// <summary>
        /// Identifies the TimeParsers dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty TimeParsersProperty =
            DependencyProperty.Register(
                "TimeParsers",
                typeof(TimeParserCollection),
                typeof(TimeUpDown),
                new PropertyMetadata(OnTimeParsersPropertyChanged));

        /// <summary>
        /// TimeParsersProperty property changed handler.
        /// </summary>
        /// <param name="d">TimeUpDown that changed its TimeParsers.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnTimeParsersPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
        #endregion public TimeParserCollection TimeParsers

        #region public ITimeFormat Format
        /// <summary>
        /// Gets or sets the Format used by the control.
        /// From XAML Use either "Short", "Long" or a custom format.
        /// Custom formats can only contain "H", "h", "m", "s" or "t".
        /// For example: use 'hh:mm:ss' is used to format time as "13:45:30".
        /// </summary>
        [OpenSilver.NotImplemented]
        public ITimeFormat Format
        {
            get => GetValue(FormatProperty) as ITimeFormat;
            set => SetValue(FormatProperty, value);
        }

        /// <summary>
        /// Identifies the Format dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty FormatProperty =
            DependencyProperty.Register(
                "Format",
                typeof(ITimeFormat),
                typeof(TimeUpDown),
                new PropertyMetadata(OnFormatPropertyChanged));

        /// <summary>
        /// FormatProperty property changed handler.
        /// </summary>
        /// <param name="d">TimePicker that changed its Format.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnFormatPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion public ITimeFormat Format

        #region public CultureInfo Culture
        /// <summary>
        /// Gets or sets the culture that will be used by the control for
        /// parsing and formatting.
        /// </summary>
        [OpenSilver.NotImplemented]
        public CultureInfo Culture
        {
            get => (CultureInfo)GetValue(CultureProperty);
            set => SetValue(CultureProperty, value);
        }

        /// <summary>
        /// Identifies the Culture dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty CultureProperty =
            DependencyProperty.Register(
                "Culture",
                typeof(CultureInfo),
                typeof(TimeUpDown),
                new PropertyMetadata(null, OnCulturePropertyChanged));

        /// <summary>
        /// CultureProperty property changed handler.
        /// </summary>
        /// <param name="d">TimeUpDown that changed its Culture.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnCulturePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
        #endregion public CultureInfo Culture


        #region public TimeGlobalizationInfo TimeGlobalizationInfo
        /// <summary>
        /// Gets or sets the strategy object that determines how the control
        /// interacts with DateTime and CultureInfo.
        /// </summary>
        [OpenSilver.NotImplemented]
        public TimeGlobalizationInfo TimeGlobalizationInfo
        {
            get => (TimeGlobalizationInfo)GetValue(TimeGlobalizationInfoProperty);
            set => SetValue(TimeGlobalizationInfoProperty, value);
        }

        /// <summary>
        /// Identifies the TimeGlobalizationInfo dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty TimeGlobalizationInfoProperty =
            DependencyProperty.Register(
                "TimeGlobalizationInfo",
                typeof(TimeGlobalizationInfo),
                typeof(TimeUpDown),
                new PropertyMetadata(OnTimeGlobalizationInfoPropertyChanged));

        /// <summary>
        /// TimeGlobalizationInfoProperty property changed handler.
        /// </summary>
        /// <param name="d">TimeUpDown that changed its TimeGlobalizationInfo.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnTimeGlobalizationInfoPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimeUpDown source = (TimeUpDown)d;
            TimeGlobalizationInfo newValue = e.NewValue as TimeGlobalizationInfo;
            if (newValue != null)
            {
                newValue.Culture = source.Culture;
                source._actualTimeGlobalizationInfo = null; // no need for default any more.
            }
        }

        /// <summary>
        /// Gets the actual TimeGlobalization info used by the control.
        /// </summary>
        /// <remarks>If TimeGlobalizationInfo is not set, will return
        /// default TimeGlobalizationInfo instance.</remarks>
        [OpenSilver.NotImplemented]
        public TimeGlobalizationInfo ActualTimeGlobalizationInfo
        {
            get
            {
                TimeGlobalizationInfo info = TimeGlobalizationInfo;

                if (info == null)
                {
                    if (_actualTimeGlobalizationInfo == null)
                    {
                        // set the default strategy object
                        _actualTimeGlobalizationInfo = new TimeGlobalizationInfo();
                    }
                    return _actualTimeGlobalizationInfo;
                }
                else
                {
                    return info;
                }
            }
        }

        /// <summary>
        /// BackingField for ActualTimeGlobalizationInfo.
        /// </summary>
        private TimeGlobalizationInfo _actualTimeGlobalizationInfo;
        #endregion public TimeGlobalizationInfo TimeGlobalizationInfo

    }
}
