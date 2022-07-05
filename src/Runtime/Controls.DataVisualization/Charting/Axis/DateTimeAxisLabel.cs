// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows.Data;

#if MIGRATION
namespace System.Windows.Controls.DataVisualization.Charting
#else
namespace Windows.UI.Xaml.Controls.DataVisualization.Charting
#endif
{
    /// <summary>An axis label for displaying DateTime values.</summary>
    public class DateTimeAxisLabel : AxisLabel
    {
        /// <summary>Identifies the IntervalType dependency property.</summary>
        public static readonly DependencyProperty IntervalTypeProperty = DependencyProperty.Register(nameof(IntervalType), typeof(DateTimeIntervalType), typeof(DateTimeAxisLabel), new PropertyMetadata((object)DateTimeIntervalType.Auto, new PropertyChangedCallback(DateTimeAxisLabel.OnIntervalTypePropertyChanged)));
        /// <summary>
        /// Identifies the YearsIntervalStringFormat dependency property.
        /// </summary>
        public static readonly DependencyProperty YearsIntervalStringFormatProperty = DependencyProperty.Register(nameof(YearsIntervalStringFormat), typeof(string), typeof(DateTimeAxisLabel), new PropertyMetadata((object)null, new PropertyChangedCallback(DateTimeAxisLabel.OnYearsIntervalStringFormatPropertyChanged)));
        /// <summary>
        /// Identifies the MonthsIntervalStringFormat dependency property.
        /// </summary>
        public static readonly DependencyProperty MonthsIntervalStringFormatProperty = DependencyProperty.Register(nameof(MonthsIntervalStringFormat), typeof(string), typeof(DateTimeAxisLabel), new PropertyMetadata((object)null, new PropertyChangedCallback(DateTimeAxisLabel.OnMonthsIntervalStringFormatPropertyChanged)));
        /// <summary>
        /// Identifies the WeeksIntervalStringFormat dependency property.
        /// </summary>
        public static readonly DependencyProperty WeeksIntervalStringFormatProperty = DependencyProperty.Register(nameof(WeeksIntervalStringFormat), typeof(string), typeof(DateTimeAxisLabel), new PropertyMetadata((object)null, new PropertyChangedCallback(DateTimeAxisLabel.OnWeeksIntervalStringFormatPropertyChanged)));
        /// <summary>
        /// Identifies the DaysIntervalStringFormat dependency property.
        /// </summary>
        public static readonly DependencyProperty DaysIntervalStringFormatProperty = DependencyProperty.Register(nameof(DaysIntervalStringFormat), typeof(string), typeof(DateTimeAxisLabel), new PropertyMetadata((object)null, new PropertyChangedCallback(DateTimeAxisLabel.OnDaysIntervalStringFormatPropertyChanged)));
        /// <summary>
        /// Identifies the HoursIntervalStringFormat dependency property.
        /// </summary>
        public static readonly DependencyProperty HoursIntervalStringFormatProperty = DependencyProperty.Register(nameof(HoursIntervalStringFormat), typeof(string), typeof(DateTimeAxisLabel), new PropertyMetadata((object)null, new PropertyChangedCallback(DateTimeAxisLabel.OnHoursIntervalStringFormatPropertyChanged)));
        /// <summary>
        /// Identifies the MinutesIntervalStringFormat dependency property.
        /// </summary>
        public static readonly DependencyProperty MinutesIntervalStringFormatProperty = DependencyProperty.Register(nameof(MinutesIntervalStringFormat), typeof(string), typeof(DateTimeAxisLabel), new PropertyMetadata((object)null, new PropertyChangedCallback(DateTimeAxisLabel.OnMinutesIntervalStringFormatPropertyChanged)));
        /// <summary>
        /// Identifies the SecondsIntervalStringFormat dependency property.
        /// </summary>
        public static readonly DependencyProperty SecondsIntervalStringFormatProperty = DependencyProperty.Register(nameof(SecondsIntervalStringFormat), typeof(string), typeof(DateTimeAxisLabel), new PropertyMetadata((object)null, new PropertyChangedCallback(DateTimeAxisLabel.OnSecondsIntervalStringFormatPropertyChanged)));
        /// <summary>
        /// Identifies the MillisecondsIntervalStringFormat dependency property.
        /// </summary>
        public static readonly DependencyProperty MillisecondsIntervalStringFormatProperty = DependencyProperty.Register(nameof(MillisecondsIntervalStringFormat), typeof(string), typeof(DateTimeAxisLabel), new PropertyMetadata((object)null, new PropertyChangedCallback(DateTimeAxisLabel.OnMillisecondsIntervalStringFormatPropertyChanged)));

        /// <summary>Gets or sets the interval type of the DateTimeAxis2.</summary>
        public DateTimeIntervalType IntervalType
        {
            get
            {
                return (DateTimeIntervalType)this.GetValue(DateTimeAxisLabel.IntervalTypeProperty);
            }
            set
            {
                this.SetValue(DateTimeAxisLabel.IntervalTypeProperty, (object)value);
            }
        }

        /// <summary>IntervalTypeProperty property changed handler.</summary>
        /// <param name="d">DateTimeAxisLabel that changed its IntervalType.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnIntervalTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DateTimeAxisLabel)d).OnIntervalTypePropertyChanged((DateTimeIntervalType)e.OldValue, (DateTimeIntervalType)e.NewValue);
        }

        /// <summary>IntervalTypeProperty property changed handler.</summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        protected virtual void OnIntervalTypePropertyChanged(DateTimeIntervalType oldValue, DateTimeIntervalType newValue)
        {
            this.UpdateFormattedContent();
        }

        /// <summary>
        /// Gets or sets the format string to use when the interval is hours.
        /// </summary>
        public string YearsIntervalStringFormat
        {
            get
            {
                return this.GetValue(DateTimeAxisLabel.YearsIntervalStringFormatProperty) as string;
            }
            set
            {
                this.SetValue(DateTimeAxisLabel.YearsIntervalStringFormatProperty, (object)value);
            }
        }

        /// <summary>
        /// YearsIntervalStringFormatProperty property changed handler.
        /// </summary>
        /// <param name="d">DateTimeAxisLabel that changed its YearsIntervalStringFormat.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnYearsIntervalStringFormatPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DateTimeAxisLabel)d).OnYearsIntervalStringFormatPropertyChanged();
        }

        /// <summary>
        /// YearsIntervalStringFormatProperty property changed handler.
        /// </summary>
        protected virtual void OnYearsIntervalStringFormatPropertyChanged()
        {
            this.UpdateFormattedContent();
        }

        /// <summary>
        /// Gets or sets the format string to use when the interval is hours.
        /// </summary>
        public string MonthsIntervalStringFormat
        {
            get
            {
                return this.GetValue(DateTimeAxisLabel.MonthsIntervalStringFormatProperty) as string;
            }
            set
            {
                this.SetValue(DateTimeAxisLabel.MonthsIntervalStringFormatProperty, (object)value);
            }
        }

        /// <summary>
        /// MonthsIntervalStringFormatProperty property changed handler.
        /// </summary>
        /// <param name="d">DateTimeAxisLabel that changed its MonthsIntervalStringFormat.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnMonthsIntervalStringFormatPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DateTimeAxisLabel)d).OnMonthsIntervalStringFormatPropertyChanged();
        }

        /// <summary>
        /// MonthsIntervalStringFormatProperty property changed handler.
        /// </summary>
        protected virtual void OnMonthsIntervalStringFormatPropertyChanged()
        {
            this.UpdateFormattedContent();
        }

        /// <summary>
        /// Gets or sets the format string to use when the interval is hours.
        /// </summary>
        public string WeeksIntervalStringFormat
        {
            get
            {
                return this.GetValue(DateTimeAxisLabel.WeeksIntervalStringFormatProperty) as string;
            }
            set
            {
                this.SetValue(DateTimeAxisLabel.WeeksIntervalStringFormatProperty, (object)value);
            }
        }

        /// <summary>
        /// WeeksIntervalStringFormatProperty property changed handler.
        /// </summary>
        /// <param name="d">DateTimeAxisLabel that changed its WeeksIntervalStringFormat.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnWeeksIntervalStringFormatPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DateTimeAxisLabel)d).OnWeeksIntervalStringFormatPropertyChanged();
        }

        /// <summary>
        /// WeeksIntervalStringFormatProperty property changed handler.
        /// </summary>
        protected virtual void OnWeeksIntervalStringFormatPropertyChanged()
        {
            this.UpdateFormattedContent();
        }

        /// <summary>
        /// Gets or sets the format string to use when the interval is hours.
        /// </summary>
        public string DaysIntervalStringFormat
        {
            get
            {
                return this.GetValue(DateTimeAxisLabel.DaysIntervalStringFormatProperty) as string;
            }
            set
            {
                this.SetValue(DateTimeAxisLabel.DaysIntervalStringFormatProperty, (object)value);
            }
        }

        /// <summary>
        /// DaysIntervalStringFormatProperty property changed handler.
        /// </summary>
        /// <param name="d">DateTimeAxisLabel that changed its DaysIntervalStringFormat.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnDaysIntervalStringFormatPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DateTimeAxisLabel)d).OnDaysIntervalStringFormatPropertyChanged();
        }

        /// <summary>
        /// DaysIntervalStringFormatProperty property changed handler.
        /// </summary>
        protected virtual void OnDaysIntervalStringFormatPropertyChanged()
        {
            this.UpdateFormattedContent();
        }

        /// <summary>
        /// Gets or sets the format string to use when the interval is hours.
        /// </summary>
        public string HoursIntervalStringFormat
        {
            get
            {
                return this.GetValue(DateTimeAxisLabel.HoursIntervalStringFormatProperty) as string;
            }
            set
            {
                this.SetValue(DateTimeAxisLabel.HoursIntervalStringFormatProperty, (object)value);
            }
        }

        /// <summary>
        /// HoursIntervalStringFormatProperty property changed handler.
        /// </summary>
        /// <param name="d">DateTimeAxisLabel that changed its HoursIntervalStringFormat.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnHoursIntervalStringFormatPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DateTimeAxisLabel)d).OnHoursIntervalStringFormatPropertyChanged();
        }

        /// <summary>
        /// HoursIntervalStringFormatProperty property changed handler.
        /// </summary>
        protected virtual void OnHoursIntervalStringFormatPropertyChanged()
        {
            this.UpdateFormattedContent();
        }

        /// <summary>
        /// Gets or sets the format string to use when the interval is hours.
        /// </summary>
        public string MinutesIntervalStringFormat
        {
            get
            {
                return this.GetValue(DateTimeAxisLabel.MinutesIntervalStringFormatProperty) as string;
            }
            set
            {
                this.SetValue(DateTimeAxisLabel.MinutesIntervalStringFormatProperty, (object)value);
            }
        }

        /// <summary>
        /// MinutesIntervalStringFormatProperty property changed handler.
        /// </summary>
        /// <param name="d">DateTimeAxisLabel that changed its MinutesIntervalStringFormat.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnMinutesIntervalStringFormatPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DateTimeAxisLabel)d).OnMinutesIntervalStringFormatPropertyChanged();
        }

        /// <summary>
        /// MinutesIntervalStringFormatProperty property changed handler.
        /// </summary>
        protected virtual void OnMinutesIntervalStringFormatPropertyChanged()
        {
            this.UpdateFormattedContent();
        }

        /// <summary>
        /// Gets or sets the format string to use when the interval is hours.
        /// </summary>
        public string SecondsIntervalStringFormat
        {
            get
            {
                return this.GetValue(DateTimeAxisLabel.SecondsIntervalStringFormatProperty) as string;
            }
            set
            {
                this.SetValue(DateTimeAxisLabel.SecondsIntervalStringFormatProperty, (object)value);
            }
        }

        /// <summary>
        /// SecondsIntervalStringFormatProperty property changed handler.
        /// </summary>
        /// <param name="d">DateTimeAxisLabel that changed its SecondsIntervalStringFormat.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnSecondsIntervalStringFormatPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DateTimeAxisLabel)d).OnSecondsIntervalStringFormatPropertyChanged();
        }

        /// <summary>
        /// SecondsIntervalStringFormatProperty property changed handler.
        /// </summary>
        protected virtual void OnSecondsIntervalStringFormatPropertyChanged()
        {
            this.UpdateFormattedContent();
        }

        /// <summary>
        /// Gets or sets the format string to use when the interval is hours.
        /// </summary>
        public string MillisecondsIntervalStringFormat
        {
            get
            {
                return this.GetValue(DateTimeAxisLabel.MillisecondsIntervalStringFormatProperty) as string;
            }
            set
            {
                this.SetValue(DateTimeAxisLabel.MillisecondsIntervalStringFormatProperty, (object)value);
            }
        }

        /// <summary>
        /// MillisecondsIntervalStringFormatProperty property changed handler.
        /// </summary>
        /// <param name="d">DateTimeAxisLabel that changed its MillisecondsIntervalStringFormat.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnMillisecondsIntervalStringFormatPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DateTimeAxisLabel)d).OnMillisecondsIntervalStringFormatPropertyChanged();
        }

        /// <summary>
        /// MillisecondsIntervalStringFormatProperty property changed handler.
        /// </summary>
        protected virtual void OnMillisecondsIntervalStringFormatPropertyChanged()
        {
            this.UpdateFormattedContent();
        }

        /// <summary>
        /// Instantiates a new instance of the DateTimeAxisLabel class.
        /// </summary>
        public DateTimeAxisLabel()
        {
            this.DefaultStyleKey = (object)typeof(DateTimeAxisLabel);
        }

        /// <summary>Updates the formatted text.</summary>
        protected override void UpdateFormattedContent()
        {
            if (this.StringFormat == null)
            {
                switch (this.IntervalType)
                {
                    case DateTimeIntervalType.Milliseconds:
                        this.SetBinding(AxisLabel.FormattedContentProperty, new Binding()
                        {
                            Converter = (IValueConverter)new StringFormatConverter(),
                            ConverterParameter = (object)(this.MillisecondsIntervalStringFormat ?? this.StringFormat ?? "{0}")
                        });
                        break;
                    case DateTimeIntervalType.Seconds:
                        this.SetBinding(AxisLabel.FormattedContentProperty, new Binding()
                        {
                            Converter = (IValueConverter)new StringFormatConverter(),
                            ConverterParameter = (object)(this.SecondsIntervalStringFormat ?? this.StringFormat ?? "{0}")
                        });
                        break;
                    case DateTimeIntervalType.Minutes:
                        this.SetBinding(AxisLabel.FormattedContentProperty, new Binding()
                        {
                            Converter = (IValueConverter)new StringFormatConverter(),
                            ConverterParameter = (object)(this.MinutesIntervalStringFormat ?? this.StringFormat ?? "{0}")
                        });
                        break;
                    case DateTimeIntervalType.Hours:
                        this.SetBinding(AxisLabel.FormattedContentProperty, new Binding()
                        {
                            Converter = (IValueConverter)new StringFormatConverter(),
                            ConverterParameter = (object)(this.HoursIntervalStringFormat ?? this.StringFormat ?? "{0}")
                        });
                        break;
                    case DateTimeIntervalType.Days:
                        this.SetBinding(AxisLabel.FormattedContentProperty, new Binding()
                        {
                            Converter = (IValueConverter)new StringFormatConverter(),
                            ConverterParameter = (object)(this.DaysIntervalStringFormat ?? this.StringFormat ?? "{0}")
                        });
                        break;
                    case DateTimeIntervalType.Weeks:
                        this.SetBinding(AxisLabel.FormattedContentProperty, new Binding()
                        {
                            Converter = (IValueConverter)new StringFormatConverter(),
                            ConverterParameter = (object)(this.WeeksIntervalStringFormat ?? this.StringFormat ?? "{0}")
                        });
                        break;
                    case DateTimeIntervalType.Months:
                        this.SetBinding(AxisLabel.FormattedContentProperty, new Binding()
                        {
                            Converter = (IValueConverter)new StringFormatConverter(),
                            ConverterParameter = (object)(this.MonthsIntervalStringFormat ?? this.StringFormat ?? "{0}")
                        });
                        break;
                    case DateTimeIntervalType.Years:
                        this.SetBinding(AxisLabel.FormattedContentProperty, new Binding()
                        {
                            Converter = (IValueConverter)new StringFormatConverter(),
                            ConverterParameter = (object)(this.YearsIntervalStringFormat ?? this.StringFormat ?? "{0}")
                        });
                        break;
                    default:
                        base.UpdateFormattedContent();
                        break;
                }
            }
            else
                base.UpdateFormattedContent();
        }
    }
}