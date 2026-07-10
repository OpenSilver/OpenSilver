using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Navigation;

namespace TestApplication.Tests
{
    public partial class ListViewTest : Page
    {
        private string _lastSortHeader;
        private ListSortDirection _lastSortDirection = ListSortDirection.Descending;

        public ListViewTest()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SimpleListView.Items.Add("Initial item 1");
            SimpleListView.Items.Add("Initial item 2");
            SimpleListView.Items.Add("Initial item 3");
            SimpleListView.SelectedIndex = 1;

            EmployeesListView.ItemsSource = CreateEmployees();

            SwitchableListView.ItemsSource = new ObservableCollection<string>
            {
                "Apple", "Banana", "Cherry", "Date", "Elderberry",
            };
        }

        private static ObservableCollection<Employee> CreateEmployees()
        {
            return new ObservableCollection<Employee>
            {
                new Employee { FirstName = "Alice", LastName = "Miller", Age = 34, Department = "Engineering", IsActive = true },
                new Employee { FirstName = "Bob", LastName = "Taylor", Age = 28, Department = "Sales", IsActive = true },
                new Employee { FirstName = "Carol", LastName = "Anderson", Age = 45, Department = "Engineering", IsActive = false },
                new Employee { FirstName = "David", LastName = "Wilson", Age = 39, Department = "Marketing", IsActive = true },
                new Employee { FirstName = "Emma", LastName = "Brown", Age = 22, Department = "Sales", IsActive = true },
                new Employee { FirstName = "Frank", LastName = "Jones", Age = 51, Department = "Support", IsActive = false },
            };
        }

        private string RandomId() => new Random().Next(1000).ToString();

        #region Section 1: Default ListView (no View)

        private void SimpleListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SimpleSelectedItemsCountText.Text = SimpleListView.SelectedItems.Count.ToString();
        }

        private void ButtonSimple_ItemsAdd_Click(object sender, RoutedEventArgs e)
        {
            SimpleListView.Items.Add("Item #" + RandomId());
        }

        private void ButtonSimple_ItemsClear_Click(object sender, RoutedEventArgs e)
        {
            SimpleListView.Items.Clear();
        }

        private void ButtonSimple_ItemsRemoveFirst_Click(object sender, RoutedEventArgs e)
        {
            if (SimpleListView.Items.Count > 0)
            {
                SimpleListView.Items.RemoveAt(0);
            }
        }

        private void ButtonSimple_SetNewItemsSource_Click(object sender, RoutedEventArgs e)
        {
            SimpleListView.ItemsSource = new ObservableCollection<string> { "One", "Two", "Three" };
        }

        private void ButtonSimple_SetItemsSourceToNull_Click(object sender, RoutedEventArgs e)
        {
            SimpleListView.ItemsSource = null;
        }

        private void ButtonSimple_SelectSecondItem_Click(object sender, RoutedEventArgs e)
        {
            if (SimpleListView.Items.Count > 1)
            {
                SimpleListView.SelectedItem = SimpleListView.Items[1];
            }
        }

        private void ButtonSimple_SelectSecondIndex_Click(object sender, RoutedEventArgs e)
        {
            SimpleListView.SelectedIndex = 1;
        }

        private void ButtonSimple_SelectItemNull_Click(object sender, RoutedEventArgs e)
        {
            SimpleListView.SelectedItem = null;
        }

        private void ButtonSimple_SelectedIndexMinusOne_Click(object sender, RoutedEventArgs e)
        {
            SimpleListView.SelectedIndex = -1;
        }

        #endregion

        #region Section 2: ListView with a GridView view

        private void ButtonEmployees_ItemsAdd_Click(object sender, RoutedEventArgs e)
        {
            var newEmployee = new Employee
            {
                FirstName = "New",
                LastName = "Employee #" + RandomId(),
                Age = 25,
                Department = "Unassigned",
                IsActive = true,
            };

            if (EmployeesListView.ItemsSource is ObservableCollection<Employee> source)
            {
                source.Add(newEmployee);
            }
            else
            {
                EmployeesListView.Items.Add(newEmployee);
            }
        }

        private void ButtonEmployees_ItemsRemoveFirst_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeesListView.ItemsSource is ObservableCollection<Employee> source && source.Count > 0)
            {
                source.RemoveAt(0);
            }
            else if (EmployeesListView.Items.Count > 0)
            {
                EmployeesListView.Items.RemoveAt(0);
            }
        }

        private void ButtonEmployees_SetNewItemsSource_Click(object sender, RoutedEventArgs e)
        {
            EmployeesListView.ItemsSource = CreateEmployees();
            _lastSortHeader = null;
            SortedByText.Text = "(none, click a column header)";
        }

        private void ButtonEmployees_ItemsSourceAdd_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeesListView.ItemsSource is ObservableCollection<Employee> source)
            {
                source.Add(new Employee
                {
                    FirstName = "Extra",
                    LastName = "Person #" + RandomId(),
                    Age = 30,
                    Department = "Support",
                    IsActive = false,
                });
            }
        }

        private void ButtonEmployees_ItemsSourceRemove_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeesListView.ItemsSource is ObservableCollection<Employee> source && source.Count > 0)
            {
                source.RemoveAt(0);
            }
        }

        // Classic WPF "click column header to sort" pattern.
        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not GridViewColumnHeader header || header.Column?.Header is not string sortBy)
            {
                return;
            }

            if (EmployeesListView.ItemsSource is not ObservableCollection<Employee> source)
            {
                return;
            }

            ListSortDirection direction = sortBy == _lastSortHeader && _lastSortDirection == ListSortDirection.Ascending
                ? ListSortDirection.Descending
                : ListSortDirection.Ascending;

            IEnumerable<Employee> Sorted(Func<Employee, object> keySelector) =>
                direction == ListSortDirection.Ascending
                    ? source.OrderBy(keySelector)
                    : source.OrderByDescending(keySelector);

            IEnumerable<Employee> sorted = sortBy switch
            {
                "First Name" => Sorted(emp => emp.FirstName),
                "Last Name" => Sorted(emp => emp.LastName),
                "Age" => Sorted(emp => emp.Age),
                "Department" => Sorted(emp => emp.Department),
                "Active" => Sorted(emp => emp.IsActive),
                _ => null,
            };

            if (sorted is null)
            {
                return;
            }

            Employee[] ordered = sorted.ToArray();
            source.Clear();
            foreach (Employee employee in ordered)
            {
                source.Add(employee);
            }

            _lastSortHeader = sortBy;
            _lastSortDirection = direction;
            SortedByText.Text = $"{sortBy} ({direction})";
        }

        #endregion

        #region Section 3: Switching View at run time

        private void ButtonSwitchable_UseGridView_Click(object sender, RoutedEventArgs e)
        {
            // A GridView instance can only be assigned to a single ListView at a time
            // (ListView.View throws if the GridView is already in use), so a new one is created here.
            var gridView = new GridView();
            gridView.Columns.Add(new GridViewColumn
            {
                Header = "Fruit",
                Width = 200,
                DisplayMemberBinding = new Binding(),
            });

            SwitchableListView.View = gridView;
        }

        private void ButtonSwitchable_UsePlainList_Click(object sender, RoutedEventArgs e)
        {
            SwitchableListView.View = null;
        }

        #endregion
    }

    public class Employee
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int Age { get; set; }

        public string Department { get; set; }

        public bool IsActive { get; set; }

        public override string ToString() => $"{FirstName} {LastName}";
    }
}
