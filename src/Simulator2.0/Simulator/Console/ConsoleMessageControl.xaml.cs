using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OpenSilver.Simulator.Console
{
    /// <summary>
    /// Interaction logic for ConsoleMessageControl.xaml
    /// </summary>
    public partial class ConsoleMessageControl : UserControl
    {
        #region MessageBackground
        public Brush MessageBackground
        {
            get { return (Brush)GetValue(MessageBackgroundProperty); }
            set { SetValue(MessageBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MessageBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageBackgroundProperty =
            DependencyProperty.Register("MessageBackground", typeof(Brush), typeof(ConsoleMessageControl), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));
        #endregion

        #region MessageForeground
        public Brush MessageForeground
        {
            get { return (Brush)GetValue(MessageForegroundProperty); }
            set { SetValue(MessageForegroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MessageForeground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageForegroundProperty =
            DependencyProperty.Register("MessageForeground", typeof(Brush), typeof(ConsoleMessageControl), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));
        #endregion

        public ConsoleMessageControl()
        {
            InitializeComponent();

            DataContextChanged += ConsoleMessageChanged;
        }

        private void ConsoleMessageChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ConsoleMessage message = (ConsoleMessage)e.NewValue;

            switch (message.Level)
            {
                case ConsoleMessage.MessageLevel.Log:
                    MessageBackground = new SolidColorBrush(Colors.Transparent);
                    MessageForeground = new SolidColorBrush(Color.FromRgb(0xD5, 0xD5, 0xD5));
                    break;
                case ConsoleMessage.MessageLevel.Warning:
                    MessageBackground = new SolidColorBrush(Color.FromRgb(0x33, 0x2B, 0x00));
                    MessageForeground = new SolidColorBrush(Color.FromRgb(0xFF, 0xDD, 0x9E));
                    break;
                case ConsoleMessage.MessageLevel.Error:
                    MessageBackground = new SolidColorBrush(Color.FromRgb(0x29, 0x00, 0x00));
                    MessageForeground = new SolidColorBrush(Color.FromRgb(0xFF, 0x80, 0x80));
                    break;
            }

            if (message.Source is FileSource fileSource)
            {
                Expander.Visibility = System.Windows.Visibility.Visible;

                ExpanderHeader.Text = message.Message;
                ExpanderContent.Text = $"at: {fileSource.Path}>>func:{fileSource.FunctionName}>>Line:{fileSource.Line}";
            }
            else if (message.Source is InteropSource interopSource)
            {
                Expander.Visibility = System.Windows.Visibility.Visible;

                ExpanderHeader.Text = message.Message;
                ExpanderContent.Text = $"Error in the following code:\n{interopSource.Code}";
            }
            else
            {
                SimpleContent.Visibility = System.Windows.Visibility.Visible;

                SimpleContent.Text = message.Message;
            }
        }
    }
}
