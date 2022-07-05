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
    /// <summary>A label used to display data in an axis.</summary>
    public class AxisLabel : Control
    {
        /// <summary>Identifies the StringFormat dependency property.</summary>
        public static readonly DependencyProperty StringFormatProperty = DependencyProperty.Register(nameof(StringFormat), typeof(string), typeof(AxisLabel), new PropertyMetadata((object)null, new PropertyChangedCallback(AxisLabel.OnStringFormatPropertyChanged)));
        /// <summary>Identifies the FormattedContent dependency property.</summary>
        public static readonly DependencyProperty FormattedContentProperty = DependencyProperty.Register(nameof(FormattedContent), typeof(string), typeof(AxisLabel), new PropertyMetadata((PropertyChangedCallback)null));

        /// <summary>Gets or sets the text string format.</summary>
        public string StringFormat
        {
            get
            {
                return this.GetValue(AxisLabel.StringFormatProperty) as string;
            }
            set
            {
                this.SetValue(AxisLabel.StringFormatProperty, (object)value);
            }
        }

        /// <summary>StringFormatProperty property changed handler.</summary>
        /// <param name="d">AxisLabel that changed its StringFormat.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnStringFormatPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AxisLabel)d).OnStringFormatPropertyChanged((string)e.NewValue);
        }

        /// <summary>StringFormatProperty property changed handler.</summary>
        /// <param name="newValue">New value.</param>
        protected virtual void OnStringFormatPropertyChanged(string newValue)
        {
            this.UpdateFormattedContent();
        }

        /// <summary>Gets the formatted content property.</summary>
        public string FormattedContent
        {
            get
            {
                return this.GetValue(AxisLabel.FormattedContentProperty) as string;
            }
            protected set
            {
                this.SetValue(AxisLabel.FormattedContentProperty, (object)value);
            }
        }

        /// <summary>Instantiates a new instance of the AxisLabel class.</summary>
        public AxisLabel()
        {
            this.DefaultStyleKey = (object)typeof(AxisLabel);
            this.SetBinding(AxisLabel.FormattedContentProperty, new Binding()
            {
                Converter = (IValueConverter)new StringFormatConverter(),
                ConverterParameter = (object)(this.StringFormat ?? "{0}")
            });
        }

        /// <summary>Updates the formatted text.</summary>
        protected virtual void UpdateFormattedContent()
        {
            this.SetBinding(AxisLabel.FormattedContentProperty, new Binding()
            {
                Converter = (IValueConverter)new StringFormatConverter(),
                ConverterParameter = (object)(this.StringFormat ?? "{0}")
            });
        }
    }
}
