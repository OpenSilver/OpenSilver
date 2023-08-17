using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DotNetForHtml5.EmulatorWithoutJavascript.Console
{
    /// <summary>
    /// Interaction logic for Console.xaml
    /// </summary>
    public partial class ConsoleControl : UserControl
    {
        #region ConsoleVisible
        private bool ConsoleVisible
        {
            get { return (bool)GetValue(ConsoleVisibleProperty); }
            set { SetValue(ConsoleVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Visible.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty ConsoleVisibleProperty =
            DependencyProperty.Register("ConsoleVisible", typeof(bool), typeof(ConsoleControl), new PropertyMetadata(false, ConsoleVisibleChanged));
        #endregion

        #region ErrorCount
        private int ErrorCount
        {
            get { return (int)GetValue(ErrorCountProperty); }
            set { SetValue(ErrorCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ErrorCount.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty ErrorCountProperty =
            DependencyProperty.Register("ErrorCount", typeof(int), typeof(ConsoleControl), new PropertyMetadata(0));
        #endregion

        #region UnseenErrorCount
        private int UnseenErrorCount
        {
            get { return (int)GetValue(UnseenErrorCountProperty); }
            set { SetValue(UnseenErrorCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UnseenErrorCount.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty UnseenErrorCountProperty =
            DependencyProperty.Register("UnseenErrorCount", typeof(int), typeof(ConsoleControl), new PropertyMetadata(0, UnseenErrorCountChanged));
        #endregion

        private bool _consoleEnabled = false;
        public ConsoleControl()
        {
            InitializeComponent();
        }

        public void AddMessage(ConsoleMessage message)
        {
            Dispatcher.Invoke(() =>
            {
                if (!_consoleEnabled)
                {
                    Root.Visibility = Visibility.Visible;
                    _consoleEnabled = true;
                }

                Console.Items.Add(message);

                if (message.Level == ConsoleMessage.ErrorLevel)
                {
                    ErrorCount++;
                    if (!ConsoleVisible)
                    {
                        UnseenErrorCount++;
                    }
                }

                ConsoleScrollViewer.ScrollToEnd();
            });
        }

        private static void UnseenErrorCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((int)e.OldValue == 0 && (int)e.NewValue > 0)
            {
                ((ConsoleControl)d).ShowConsoleBanner.Background = new SolidColorBrush(Colors.Red);
            }
            else if ((int)e.OldValue > 0 && (int)e.NewValue == 0)
            {
                ((ConsoleControl)d).ShowConsoleBanner.Background = new SolidColorBrush(Colors.Gray);
            }
        }

        private static void ConsoleVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ConsoleControl console = (ConsoleControl)d;
            if ((bool)e.NewValue)
            {
                console.UnseenErrorCount = 0;
                console.ShowConsoleBanner.Visibility = Visibility.Collapsed;
                console.ConsoleRoot.Visibility = Visibility.Visible;
            }
            else
            {
                console.ShowConsoleBanner.Visibility = Visibility.Visible;
                console.ConsoleRoot.Visibility = Visibility.Collapsed;
            }
            
        }

        private void ShowConsole(object sender, MouseButtonEventArgs e)
        {
            ConsoleVisible = true;
        }

        private void HideConsole(object sender, RoutedEventArgs e)
        {
            ConsoleVisible = false;
        }

        private void ClearConsole(object sender, RoutedEventArgs e)
        {
            Console.Items.Clear();
            ErrorCount = 0;
            UnseenErrorCount = 0;
        }

        private void CopyErrorsToClipboard(object sender, RoutedEventArgs e)
        {
            if (ErrorCount == 0) return;

            StringBuilder sb = new StringBuilder();

            bool firstIteration = true;

            foreach (ConsoleMessage message in Console.Items)
            {
                if (message.Level != ConsoleMessage.ErrorLevel) continue;

                if (!firstIteration)
                {
                    sb.Append("\n\n-----------------\n\n");
                }

                sb.Append(message);

                firstIteration = false;
            }

            Clipboard.SetText(sb.ToString());
        }
    }
}
