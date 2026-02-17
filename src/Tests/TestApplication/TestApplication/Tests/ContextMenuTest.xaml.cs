using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Navigation;

namespace TestApplication.Tests
{
    public partial class ContextMenuTest : Page
    {
        public ContextMenuTest()
        {
            InitializeComponent();
            
            // Set DataContext for command binding example
            DataContext = new ContextMenuViewModel(this);
            
            // Create programmatic context menu
            SetupProgrammaticContextMenu();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        /// <summary>
        /// Generic click handler for menu items
        /// </summary>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem)
            {
                string header = menuItem.Header?.ToString() ?? "Unknown";
                UpdateOutput($"Clicked: {header}");
            }
        }

        /// <summary>
        /// Updates the output text block with the specified message
        /// </summary>
        public void UpdateOutput(string message)
        {
            OutputTextBlock.Text = message;
            OutputTextBlock.Foreground = new SolidColorBrush(Colors.Black);
        }

        /// <summary>
        /// Sets up a context menu programmatically
        /// </summary>
        private void SetupProgrammaticContextMenu()
        {
            var contextMenu = new ContextMenu();

            var item1 = new MenuItem { Header = "Dynamic Item 1" };
            item1.Click += (s, e) => UpdateOutput("Clicked: Dynamic Item 1 (Programmatic)");
            contextMenu.Items.Add(item1);

            var item2 = new MenuItem { Header = "Dynamic Item 2" };
            item2.Click += (s, e) => UpdateOutput("Clicked: Dynamic Item 2 (Programmatic)");
            contextMenu.Items.Add(item2);

            contextMenu.Items.Add(new Separator());

            var item3 = new MenuItem 
            { 
                Header = "With Icon",
                Icon = new Image 
                { 
                    Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Images/Logo1.png", UriKind.Relative)),
                    Width = 16,
                    Height = 16,
                    Stretch = Stretch.Uniform
                }
            };
            item3.Click += (s, e) => UpdateOutput("Clicked: With Icon (Programmatic)");
            contextMenu.Items.Add(item3);

            ContextMenuService.SetContextMenu(ProgrammaticMenuBorder, contextMenu);
        }
    }

    /// <summary>
    /// ViewModel for demonstrating ICommand binding with ContextMenu
    /// </summary>
    public class ContextMenuViewModel
    {
        private readonly ContextMenuTest _page;

        public ContextMenuViewModel(ContextMenuTest page)
        {
            _page = page;
            ActionCommand = new RelayCommand(ExecuteAction);
        }

        public ICommand ActionCommand { get; }

        private void ExecuteAction(object parameter)
        {
            string message = parameter?.ToString() ?? "Command executed";
            _page.UpdateOutput($"Command: {message}");
        }
    }

    /// <summary>
    /// Simple ICommand implementation for demonstration
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => _canExecute?.Invoke(parameter) ?? true;

        public void Execute(object parameter) => _execute(parameter);

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
