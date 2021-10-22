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
    public partial class ICommandTest : Page
    {
        public ICommandTest()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            List<string> items = new List<string>();
            items.Add("MessageBox Yay!");
            items.Add("TextBox Boo!");
            items.Add("MessageBox Wow!");
            MyComboBoxForICommand.ItemsSource = items;
            MyComboBoxForICommand.SelectedIndex = 0;
            items = new List<string>();
            items.Add("Display in TextBlock");
            items.Add("Display in MessageBox");
            MyComboBoxForCommandTest.ItemsSource = items;
            MyComboBoxForCommandTest.SelectedIndex = 0;
            MyButtonForTestCommand.Command = new TestCommandInTextBlock(MessageTextBlock);
        }

        ICommand _myICommand;

        private void ButtonTestICommand_Click(object sender, RoutedEventArgs e)
        {
            if (_myICommand != null && _myICommand.CanExecute(MessageTextTextBox))
            {
                _myICommand.Execute(MessageTextTextBox);
            }
        }

        private void ComboBoxForCommandTest_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            switch (comboBox.SelectedIndex)
            {
                case 0:
                    MyButtonForTestCommand.Command = new TestCommandInTextBlock(MessageTextBlock);
                    break;
                case 1:
                default:
                    MyButtonForTestCommand.Command = new TestCommandInMessageBox();
                    break;
            }
        }


        private void MyComboBoxForICommand_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            switch (comboBox.SelectedIndex)
            {
                case 0:
                    _myICommand = new TestICommandClass();
                    break;
                case 1:
                    _myICommand = new TestICommandClass2();
                    break;
                case 2:
                    _myICommand = new TestICommandClass3();
                    break;
                default:
                    _myICommand = new TestICommandClass();
                    break;
            }
        }
        public class TestICommandClass : ICommand
        {
            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                MessageBox.Show("Yay!");
            }
        }

        public class TestICommandClass2 : ICommand
        {
            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                return parameter is TextBox;
            }

            public void Execute(object parameter)
            {
                if (parameter is TextBox)
                {
                    ((TextBox)parameter).Text = "Boo!";
                }
            }
        }

        public class TestICommandClass3 : ICommand
        {
            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                MessageBox.Show("Wow!");
            }
        }

        public class TestCommandInTextBlock : ICommand
        {
            TextBlock _messageTextTextBlock;

            public TestCommandInTextBlock(TextBlock messageTextTextBlock)
            {
                _messageTextTextBlock = messageTextTextBlock;
            }

            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                return (parameter != null && parameter is string);
            }

            public void Execute(object parameter)
            {
                _messageTextTextBlock.Text = (string)parameter;
            }
        }
        public class TestCommandInMessageBox : ICommand
        {
            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                return (parameter != null && parameter is string);
            }

            public void Execute(object parameter)
            {
                MessageBox.Show((string)parameter);
            }
        }
    }
}
