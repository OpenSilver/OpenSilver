using System.Windows;
using System.Windows.Controls;

namespace TestApplication.OpenSilver.Tests
{
    public class Person
    {
        private string _firstName;

        public Person(string firstName)
        {
            this.FirstName = firstName;
        }

        public string FirstName
        {
            get { return _firstName; }

            set { _firstName = value; }
        }
    }

    public partial class ValidationSummaryTest : UserControl
    {
        private Person _person = new Person("");

        public ValidationSummaryTest()
        {
            this.InitializeComponent();
            this.DataContext = _person;
        }

        private void btnValidate_Click(object sender, RoutedEventArgs e)
        {
            ValidateInputs();
        }

        private void ValidateInputs()
        {
            vsError.Errors.Clear();

            if (string.IsNullOrEmpty(_person.FirstName))
            {
                vsError.Errors.Add(new ValidationSummaryItem("First Name can not be empty"));
            }
        }
    }
}
