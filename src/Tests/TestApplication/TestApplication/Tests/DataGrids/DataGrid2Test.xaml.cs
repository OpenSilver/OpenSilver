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

namespace TestApplication.Tests.DataGrids
{
    public partial class DataGrid2Test : Page
    {
        public DataGrid2Test()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // SLDISABLED
            //_cats.Add(new Cat("Tom", 10));
            //_cats.Add(new Cat("Blacky", 20));
            //_cats.Add(new Cat("Pasha", 4));
        }

        // SLDISABLED
        /*
        ObservableCollection<Cat> _cats = new ObservableCollection<Cat>();
        ObservableCollection<Cat> _catsForFirstDataGrid = new ObservableCollection<Cat>();
        ObservableCollection<Cat> _catsForSecondDataGrid = new ObservableCollection<Cat>();
        DataGrid _fourthDataGrid;
        DataGrid _thirdDataGrid;

        private void ButtonClearDataGrid_Click(object sender, RoutedEventArgs e)
        {
            if (_catsForFirstDataGrid != null)
                _catsForFirstDataGrid.Clear();
            if (_catsForSecondDataGrid != null)
                _catsForSecondDataGrid.Clear();
            if (_thirdDataGrid != null && _thirdDataGrid.Items != null)
                _thirdDataGrid.Items.Clear();
            if (_fourthDataGrid != null && _fourthDataGrid.Items != null)
                _fourthDataGrid.Items.Clear();
        }

        private void ButtonTestDataGrid_Click(object sender, RoutedEventArgs e)
        {
            //we clear the lists of cats in case we click multiple times on the button:
            _catsForFirstDataGrid.Clear();
            _catsForSecondDataGrid.Clear();

            //initial dataGrid
            InitialDataGrid.ItemsSource = _cats;

            //dataGrid for Column's Visibility
            if (DataGridForColumnVisibility.ItemsSource == null)
            {
                DataGridForColumnVisibility.ItemsSource = _cats;
            }

            //First DataGrid: ItemsSource set before adding the DataGrid to the visual tree then we add a cat.
            foreach (Cat cat in _cats)
            {
                _catsForFirstDataGrid.Add(cat);
            }
            DataGrid dataGrid = new DataGrid();
            Style style = new Style(typeof(DataGridColumnHeader));
            Setter setter = new Setter(DataGrid.ForegroundProperty, new SolidColorBrush(Colors.Orange));
            style.Setters.Add(setter);
            dataGrid.ColumnHeaderStyle = style;
            dataGrid.ItemsSource = _catsForFirstDataGrid;
            dataGrid.SelectionMode = DataGridSelectionMode.Single;
            dataGrid.Loaded += FirstDataGrid_Loaded;
            DataGridItemsSourceBeforeAddingThenAddingCatContainer.Child = dataGrid;

            //Second DataGrid: ItemsSource set after the DataGrid.Loaded event then we remove a cat.
            dataGrid = new DataGrid();
            style = new Style(typeof(DataGridColumnHeader));
            setter = new Setter(DataGrid.ForegroundProperty, new SolidColorBrush(Colors.Orange));
            style.Setters.Add(setter);
            dataGrid.ColumnHeaderStyle = style;
            dataGrid.SelectionMode = DataGridSelectionMode.Single;
            dataGrid.Loaded += SecondDataGrid_Loaded;
            DataGridItemsSourceAfterLoadedThenRemovingCatContainer.Child = dataGrid;

            //Third DataGrid: Items set before adding the DataGrid to the visual tree then we add a cat.
            _thirdDataGrid = new DataGrid();
            style = new Style(typeof(DataGridColumnHeader));
            setter = new Setter(DataGrid.ForegroundProperty, new SolidColorBrush(Colors.Orange));
            style.Setters.Add(setter);
            _thirdDataGrid.ColumnHeaderStyle = style;
            foreach (Cat cat in _cats)
            {
                _thirdDataGrid.Items.Add(cat);
            }

            _thirdDataGrid.SelectionMode = DataGridSelectionMode.Single;
            _thirdDataGrid.Loaded += ThirdDataGrid_Loaded;
            DataGridItemsBeforeAddingThenAddingCatContainer.Child = _thirdDataGrid;

            //Fourth DataGrid: Items set after the DataGrid.Loaded event to the visual tree then we remove a cat
            _fourthDataGrid = new DataGrid();
            style = new Style(typeof(DataGridColumnHeader));
            setter = new Setter(DataGrid.ForegroundProperty, new SolidColorBrush(Colors.Orange));
            style.Setters.Add(setter);
            _fourthDataGrid.ColumnHeaderStyle = style;
            _fourthDataGrid.SelectionMode = DataGridSelectionMode.Single;
            _fourthDataGrid.Loaded += FourthDataGrid_Loaded;
            DataGridItemsAfterLoadedThenRemovingCatContainer.Child = _fourthDataGrid;
        }

        void FirstDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            _catsForFirstDataGrid.Add(new Cat("Bob", 3));
        }

        void SecondDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (Cat cat in _cats)
            {
                _catsForSecondDataGrid.Add(cat);
            }
            DataGrid dataGrid = (DataGrid)sender;
            dataGrid.ItemsSource = _catsForSecondDataGrid;
            string catsAsString = "";
            foreach (Cat cat in _catsForSecondDataGrid)
            {
                catsAsString += ", " + cat.Name;
            }
            DispatcherTimer t = new DispatcherTimer();
            t.Interval = new TimeSpan(0, 0, 0, 0, 500);
            t.Tick += timerForTestOnDataGrid_Tick;
            t.Start();
        }

        void timerForTestOnDataGrid_Tick(object sender, object e)
        {
            DispatcherTimer t = (DispatcherTimer)sender;
            t.Stop();
            string catsAsString = "";
            foreach (Cat cat in _catsForSecondDataGrid)
            {
                catsAsString += ", " + cat.Name;
            }
            _catsForSecondDataGrid.RemoveAt(1);
            catsAsString = "";
            foreach (Cat cat in _catsForSecondDataGrid)
            {
                catsAsString += ", " + cat.Name;
            }
        }

        void ThirdDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)sender;
            dataGrid.Items.Add(new Cat("Fluffy", 1));
        }
        void FourthDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)sender;
            foreach (Cat cat in _cats)
            {
                dataGrid.Items.Add(cat);
            }
            DispatcherTimer t = new DispatcherTimer();
            t.Interval = new TimeSpan(0, 0, 0, 0, 500);
            t.Tick += timerForTestOnDataGrid2_Tick;
            t.Start();
        }

        void timerForTestOnDataGrid2_Tick(object sender, object e)
        {
            DispatcherTimer t = (DispatcherTimer)sender;
            t.Stop();
            _fourthDataGrid.Items.RemoveAt(0);
        }

        public class Cat
        {
            public Cat(string name, int age)
            {
                Name = name;
                Age = age;
            }

            public string Name { get; set; }
            public int Age { get; set; }

        }
        */
    }
}
