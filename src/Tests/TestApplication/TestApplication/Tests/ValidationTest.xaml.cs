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
using System.ComponentModel;

namespace TestApplication.Tests
{
    public partial class ValidationTest : Page
    {
        public ValidationTest()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Person person = new Person();
            ValidationBorder.DataContext = person;
        }

        public class Person : INotifyPropertyChanged
        {
            private string _name;

            public string Name
            {
                get { return _name; }
                set
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        throw new Exception("Name cannot be empty.");
                    }
                    _name = value;
                    RaisePropertyChanged("Name");
                }
            }

            private int _age;
            public int Age
            {
                get { return _age; }
                set
                {
                    if (value <= 0)
                    {
                        throw new Exception("Age cannot be lower than 0.");
                    }
                    _age = value;
                    RaisePropertyChanged("Age");
                }
            }

            private void RaisePropertyChanged(string propertyName)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
            public event PropertyChangedEventHandler PropertyChanged;
        }

        private void ValidationBorder_BindingValidationError(object sender, ValidationErrorEventArgs e)
        {
            if (Validation.GetHasError(NameTextBoxForValidation) || Validation.GetHasError(AgeTextBoxForValidation))
            {
                (MyButtonForValidation).IsEnabled = false;
            }
            else
            {
                (MyButtonForValidation).IsEnabled = true;
            }
        }

        private void MyButtonForValidation_Click(object sender, RoutedEventArgs e)
        {
            Person person = (Person)((Button)sender).DataContext;
            string str = "Name: \"" + person.Name + "\"" + Environment.NewLine + "Age: " + person.Age + ".";
            MessageBox.Show(str);
        }
    }
}
