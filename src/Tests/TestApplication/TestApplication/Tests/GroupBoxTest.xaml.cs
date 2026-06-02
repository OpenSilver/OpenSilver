using System.Windows;
using System.Windows.Controls;

namespace TestApplication.Tests
{
    public partial class GroupBoxTest : Page
    {
        private int _headerChangeCount;
        private int _contentChangeCount;

        public GroupBoxTest()
        {
            InitializeComponent();

            TemplatedGroupBox.DataContext = new Person
            {
                Name = "Jane Doe",
                Role = "Admin",
                Email = "jane.doe@example.com",
                Phone = "+1 555-0100",
            };
        }

        private void ChangeHeaderButton_Click(object sender, RoutedEventArgs e)
        {
            _headerChangeCount++;
            MutableGroupBox.Header = $"Updated Header #{_headerChangeCount}";
        }

        private void ClearHeaderButton_Click(object sender, RoutedEventArgs e)
        {
            MutableGroupBox.Header = null;
        }

        private void ChangeContentButton_Click(object sender, RoutedEventArgs e)
        {
            _contentChangeCount++;
            MutableGroupBox.Content = new TextBlock
            {
                Text = $"New content #{_contentChangeCount} (replaced at runtime)."
            };
        }

        public class Person
        {
            public string Name { get; set; }
            public string Role { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
        }
    }
}