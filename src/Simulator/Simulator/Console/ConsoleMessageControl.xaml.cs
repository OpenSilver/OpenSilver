using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DotNetForHtml5.EmulatorWithoutJavascript.Console
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
                case ConsoleMessage.LogLevel:
                case ConsoleMessage.InfoLevel:
                case ConsoleMessage.DebugLevel:
                    MessageBackground = new SolidColorBrush(Colors.Transparent);
                    MessageForeground = new SolidColorBrush(Color.FromRgb(0xD5, 0xD5, 0xD5));
                    break;
                case ConsoleMessage.WarningLevel:
                    MessageBackground = new SolidColorBrush(Color.FromRgb(0x33, 0x2B, 0x00));
                    MessageForeground = new SolidColorBrush(Color.FromRgb(0xFF, 0xDD, 0x9E));
                    break;
                case ConsoleMessage.ErrorLevel:
                    MessageBackground = new SolidColorBrush(Color.FromRgb(0x29, 0x00, 0x00));
                    MessageForeground = new SolidColorBrush(Color.FromRgb(0xFF, 0x80, 0x80));
                    break;
            }

            ExpanderContent.Text = message.ToString();
            ExpanderHeader.Text = $"{message.Level} message from {message.Source}";
        }
    }
}
