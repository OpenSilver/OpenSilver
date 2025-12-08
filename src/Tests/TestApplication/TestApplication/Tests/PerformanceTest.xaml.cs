using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using CSHTML5.Internal;

namespace TestApplication.Tests
{
    public partial class PerformanceTest : Page
    {
        private const string HistoryFileName = "perf_history.json";
        private const string BaselineFileName = "perf_baseline.json";
        
        private ObservableCollection<PerformanceResult> _history;
        private PerformanceResult _baseline;
        private PerformanceResult _lastResult;
        private double _startTime;
        private Dictionary<string, int> _controlCounts;

        public PerformanceTest()
        {
            InitializeComponent();
            _history = new ObservableCollection<PerformanceResult>();
            _controlCounts = new Dictionary<string, int>();
            HistoryList.ItemsSource = _history;
            LoadHistory();
            LoadBaseline();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void StartBenchmarkButton_Click(object sender, RoutedEventArgs e)
        {
            // Parse configuration
            if (!int.TryParse(ElementCountTextBox.Text, out int elementCount))
                elementCount = 5000;
            elementCount = Math.Max(100, Math.Min(50000, elementCount));

            // Disable buttons during benchmark
            StartBenchmarkButton.IsEnabled = false;
            ClearButton.IsEnabled = false;
            StatusText.Text = "Running benchmark...";
            StatusText.Foreground = new SolidColorBrush(Colors.Orange);

            // Clear previous results
            RenderTarget.Children.Clear();
            _controlCounts.Clear();

            // Record start time RIGHT before we start creating elements
            _startTime = Performance.now();

            // Create the heavy workload with mixed controls
            CreateMixedControlWorkload(elementCount);

            // Use Dispatcher to measure when UI is fully rendered
            // The dispatcher will execute this after all pending layout/render operations
            Dispatcher.BeginInvoke(() =>
            {
                // This runs after the UI has been fully rendered
                double endTime = Performance.now();
                double renderTimeMs = endTime - _startTime;
                
                OnBenchmarkComplete(renderTimeMs, elementCount);
            });
        }

        private void OnBenchmarkComplete(double renderTimeMs, int requestedCount)
        {
            // Calculate total elements created
            int totalElements = 0;
            foreach (var count in _controlCounts.Values)
                totalElements += count;

            // Update UI with results
            _lastResult = new PerformanceResult
            {
                Timestamp = DateTime.Now,
                ElementCount = totalElements,
                RequestedCount = requestedCount,
                RenderTimeMs = renderTimeMs,
                ControlBreakdown = new Dictionary<string, int>(_controlCounts)
            };

            RenderTimeText.Text = $"{renderTimeMs:N0} ms ({renderTimeMs / 1000:N2} seconds)";
            ElementCountText.Text = totalElements.ToString("N0");
            
            // Show control breakdown
            var breakdown = new StringBuilder();
            foreach (var kvp in _controlCounts)
            {
                breakdown.Append($"{kvp.Key}: {kvp.Value}, ");
            }
            BindingCountText.Text = breakdown.ToString().TrimEnd(',', ' ');
            
            StatusText.Text = "Completed!";
            StatusText.Foreground = new SolidColorBrush(Colors.Green);

            // Update comparison
            UpdateComparison();

            // Save to history
            _history.Insert(0, _lastResult);
            if (_history.Count > 10)
                _history.RemoveAt(_history.Count - 1);
            SaveHistory();

            // Re-enable buttons
            StartBenchmarkButton.IsEnabled = true;
            ClearButton.IsEnabled = true;
        }

        private void CreateMixedControlWorkload(int targetElementCount)
        {
            // Create a scrollable container
            var mainPanel = new StackPanel();
            RenderTarget.Children.Add(mainPanel);

            // We'll create "rows" of mixed controls
            // Each row contains different control types
            int elementsPerRow = 15; // Approximate elements per row
            int rowCount = targetElementCount / elementsPerRow;

            for (int row = 0; row < rowCount; row++)
            {
                var rowPanel = CreateMixedControlRow(row);
                mainPanel.Children.Add(rowPanel);
            }
        }

        private Border CreateMixedControlRow(int rowIndex)
        {
            var border = new Border
            {
                BorderBrush = new SolidColorBrush(Colors.LightGray),
                BorderThickness = new Thickness(0, 0, 0, 1),
                Padding = new Thickness(5),
                Margin = new Thickness(0, 0, 0, 2)
            };
            IncrementCount("Border");

            var wrapPanel = new WrapPanel();
            IncrementCount("WrapPanel");

            // Rotate through different control combinations based on row index
            int controlType = rowIndex % 10;

            switch (controlType)
            {
                case 0:
                    AddTextControls(wrapPanel, rowIndex);
                    break;
                case 1:
                    AddButtonControls(wrapPanel, rowIndex);
                    break;
                case 2:
                    AddInputControls(wrapPanel, rowIndex);
                    break;
                case 3:
                    AddSelectionControls(wrapPanel, rowIndex);
                    break;
                case 4:
                    AddListControls(wrapPanel, rowIndex);
                    break;
                case 5:
                    AddLayoutControls(wrapPanel, rowIndex);
                    break;
                case 6:
                    AddShapeControls(wrapPanel, rowIndex);
                    break;
                case 7:
                    AddMixedFormControls(wrapPanel, rowIndex);
                    break;
                case 8:
                    AddDataDisplayControls(wrapPanel, rowIndex);
                    break;
                case 9:
                    AddComplexNestedControls(wrapPanel, rowIndex);
                    break;
            }

            border.Child = wrapPanel;
            return border;
        }

        private void AddTextControls(Panel parent, int rowIndex)
        {
            // TextBlocks with various properties
            for (int i = 0; i < 5; i++)
            {
                var tb = new TextBlock
                {
                    Text = $"TextBlock {rowIndex}.{i} - Sample text content",
                    FontSize = 10 + (i % 4),
                    Margin = new Thickness(2),
                    Foreground = new SolidColorBrush(GetColorForIndex(i))
                };
                parent.Children.Add(tb);
                IncrementCount("TextBlock");
            }

            // TextBoxes
            for (int i = 0; i < 3; i++)
            {
                var textBox = new TextBox
                {
                    Text = $"TextBox {rowIndex}.{i}",
                    Width = 100,
                    Margin = new Thickness(2)
                };
                parent.Children.Add(textBox);
                IncrementCount("TextBox");
            }

            // PasswordBox
            var pwdBox = new PasswordBox
            {
                Width = 100,
                Margin = new Thickness(2)
            };
            parent.Children.Add(pwdBox);
            IncrementCount("PasswordBox");
        }

        private void AddButtonControls(Panel parent, int rowIndex)
        {
            // Regular Buttons
            for (int i = 0; i < 4; i++)
            {
                var btn = new Button
                {
                    Content = $"Button {rowIndex}.{i}",
                    Padding = new Thickness(8, 4, 8, 4),
                    Margin = new Thickness(2)
                };
                parent.Children.Add(btn);
                IncrementCount("Button");
            }

            // ToggleButtons / CheckBoxes as toggle
            for (int i = 0; i < 3; i++)
            {
                var toggle = new CheckBox
                {
                    Content = $"Toggle {i}",
                    IsChecked = i % 2 == 0,
                    Margin = new Thickness(2)
                };
                parent.Children.Add(toggle);
                IncrementCount("CheckBox");
            }

            // RadioButtons
            var radioPanel = new StackPanel { Orientation = Orientation.Horizontal };
            IncrementCount("StackPanel");
            for (int i = 0; i < 3; i++)
            {
                var radio = new RadioButton
                {
                    Content = $"Option {i}",
                    GroupName = $"Group{rowIndex}",
                    IsChecked = i == 0,
                    Margin = new Thickness(2)
                };
                radioPanel.Children.Add(radio);
                IncrementCount("RadioButton");
            }
            parent.Children.Add(radioPanel);

            // HyperlinkButton
            var link = new HyperlinkButton
            {
                Content = "Hyperlink",
                NavigateUri = new Uri("http://example.com"),
                Margin = new Thickness(2)
            };
            parent.Children.Add(link);
            IncrementCount("HyperlinkButton");
        }

        private void AddInputControls(Panel parent, int rowIndex)
        {
            // Slider
            var slider = new Slider
            {
                Minimum = 0,
                Maximum = 100,
                Value = rowIndex % 100,
                Width = 150,
                Margin = new Thickness(2)
            };
            parent.Children.Add(slider);
            IncrementCount("Slider");

            // ProgressBar
            var progress = new ProgressBar
            {
                Minimum = 0,
                Maximum = 100,
                Value = (rowIndex * 7) % 100,
                Width = 150,
                Height = 20,
                Margin = new Thickness(2)
            };
            parent.Children.Add(progress);
            IncrementCount("ProgressBar");

            // NumericUpDown / TextBox with numbers
            for (int i = 0; i < 2; i++)
            {
                var numBox = new TextBox
                {
                    Text = (rowIndex * 10 + i).ToString(),
                    Width = 60,
                    Margin = new Thickness(2)
                };
                parent.Children.Add(numBox);
                IncrementCount("TextBox");
            }

            // DatePicker
            try
            {
                var datePicker = new DatePicker
                {
                    SelectedDate = DateTime.Now.AddDays(rowIndex),
                    Width = 150,
                    Margin = new Thickness(2)
                };
                parent.Children.Add(datePicker);
                IncrementCount("DatePicker");
            }
            catch { }

            // Calendar (smaller)
            if (rowIndex % 5 == 0) // Only add occasionally as it's heavy
            {
                try
                {
                    var calendar = new Calendar
                    {
                        DisplayDate = DateTime.Now,
                        Margin = new Thickness(2)
                    };
                    parent.Children.Add(calendar);
                    IncrementCount("Calendar");
                }
                catch { }
            }
        }

        private void AddSelectionControls(Panel parent, int rowIndex)
        {
            // ComboBox
            var comboBox = new ComboBox
            {
                Width = 120,
                Margin = new Thickness(2)
            };
            for (int i = 0; i < 5; i++)
            {
                comboBox.Items.Add($"ComboItem {i}");
                IncrementCount("ComboBoxItem");
            }
            comboBox.SelectedIndex = 0;
            parent.Children.Add(comboBox);
            IncrementCount("ComboBox");

            // ListBox (small)
            var listBox = new ListBox
            {
                Width = 100,
                Height = 60,
                Margin = new Thickness(2)
            };
            for (int i = 0; i < 4; i++)
            {
                listBox.Items.Add($"ListItem {rowIndex}.{i}");
                IncrementCount("ListBoxItem");
            }
            parent.Children.Add(listBox);
            IncrementCount("ListBox");

            // AutoCompleteBox
            try
            {
                var autoComplete = new AutoCompleteBox
                {
                    Width = 120,
                    Margin = new Thickness(2)
                };
                autoComplete.ItemsSource = new[] { "Apple", "Banana", "Cherry", "Date", "Elderberry" };
                parent.Children.Add(autoComplete);
                IncrementCount("AutoCompleteBox");
            }
            catch { }
        }

        private void AddListControls(Panel parent, int rowIndex)
        {
            // ItemsControl
            var itemsControl = new ItemsControl
            {
                Width = 150,
                Margin = new Thickness(2)
            };
            for (int i = 0; i < 3; i++)
            {
                itemsControl.Items.Add($"Item {rowIndex}.{i}");
            }
            parent.Children.Add(itemsControl);
            IncrementCount("ItemsControl");

            // TreeView (small)
            if (rowIndex % 3 == 0) // Occasionally
            {
                var treeView = new TreeView
                {
                    Width = 150,
                    Height = 80,
                    Margin = new Thickness(2)
                };
                var rootItem = new TreeViewItem { Header = $"Root {rowIndex}" };
                IncrementCount("TreeViewItem");
                for (int i = 0; i < 2; i++)
                {
                    var child = new TreeViewItem { Header = $"Child {i}" };
                    rootItem.Items.Add(child);
                    IncrementCount("TreeViewItem");
                }
                treeView.Items.Add(rootItem);
                parent.Children.Add(treeView);
                IncrementCount("TreeView");
            }

            // TabControl
            var tabControl = new TabControl
            {
                Width = 200,
                Height = 80,
                Margin = new Thickness(2)
            };
            for (int i = 0; i < 3; i++)
            {
                var tabItem = new TabItem
                {
                    Header = $"Tab {i}",
                    Content = new TextBlock { Text = $"Content {rowIndex}.{i}" }
                };
                tabControl.Items.Add(tabItem);
                IncrementCount("TabItem");
                IncrementCount("TextBlock");
            }
            parent.Children.Add(tabControl);
            IncrementCount("TabControl");
        }

        private void AddLayoutControls(Panel parent, int rowIndex)
        {
            // Grid with cells
            var grid = new Grid
            {
                Width = 150,
                Height = 60,
                Margin = new Thickness(2)
            };
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            
            for (int r = 0; r < 2; r++)
            {
                for (int c = 0; c < 2; c++)
                {
                    var cell = new Border
                    {
                        Background = new SolidColorBrush(GetColorForIndex(r * 2 + c)),
                        Child = new TextBlock 
                        { 
                            Text = $"{r},{c}", 
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center
                        }
                    };
                    Grid.SetRow(cell, r);
                    Grid.SetColumn(cell, c);
                    grid.Children.Add(cell);
                    IncrementCount("Border");
                    IncrementCount("TextBlock");
                }
            }
            parent.Children.Add(grid);
            IncrementCount("Grid");

            // StackPanel
            var stack = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Width = 80,
                Margin = new Thickness(2)
            };
            for (int i = 0; i < 3; i++)
            {
                stack.Children.Add(new TextBlock { Text = $"Stack {i}" });
                IncrementCount("TextBlock");
            }
            parent.Children.Add(stack);
            IncrementCount("StackPanel");

            // Border with content
            var border = new Border
            {
                BorderBrush = new SolidColorBrush(Colors.Blue),
                BorderThickness = new Thickness(2),
                CornerRadius = new CornerRadius(5),
                Padding = new Thickness(5),
                Margin = new Thickness(2),
                Child = new TextBlock { Text = "Bordered" }
            };
            parent.Children.Add(border);
            IncrementCount("Border");
            IncrementCount("TextBlock");

            // ScrollViewer
            var scrollViewer = new ScrollViewer
            {
                Width = 100,
                Height = 50,
                Margin = new Thickness(2),
                Content = new TextBlock 
                { 
                    Text = "Scrollable content that is long enough to scroll",
                    TextWrapping = TextWrapping.Wrap
                }
            };
            parent.Children.Add(scrollViewer);
            IncrementCount("ScrollViewer");
            IncrementCount("TextBlock");
        }

        private void AddShapeControls(Panel parent, int rowIndex)
        {
            // Rectangle
            var rect = new Rectangle
            {
                Width = 40,
                Height = 30,
                Fill = new SolidColorBrush(GetColorForIndex(rowIndex)),
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 1,
                Margin = new Thickness(2)
            };
            parent.Children.Add(rect);
            IncrementCount("Rectangle");

            // Ellipse
            var ellipse = new Ellipse
            {
                Width = 40,
                Height = 30,
                Fill = new SolidColorBrush(GetColorForIndex(rowIndex + 1)),
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 1,
                Margin = new Thickness(2)
            };
            parent.Children.Add(ellipse);
            IncrementCount("Ellipse");

            // Line
            var line = new Line
            {
                X1 = 0,
                Y1 = 0,
                X2 = 40,
                Y2 = 30,
                Stroke = new SolidColorBrush(Colors.Red),
                StrokeThickness = 2,
                Margin = new Thickness(2)
            };
            parent.Children.Add(line);
            IncrementCount("Line");

            // Path
            var path = new System.Windows.Shapes.Path
            {
                Data = new EllipseGeometry 
                { 
                    Center = new Point(20, 15), 
                    RadiusX = 15, 
                    RadiusY = 10 
                },
                Fill = new SolidColorBrush(Colors.LightGreen),
                Stroke = new SolidColorBrush(Colors.DarkGreen),
                StrokeThickness = 1,
                Margin = new Thickness(2)
            };
            parent.Children.Add(path);
            IncrementCount("Path");

            // Polygon
            var polygon = new Polygon
            {
                Points = new PointCollection 
                { 
                    new Point(20, 0), 
                    new Point(40, 30), 
                    new Point(0, 30) 
                },
                Fill = new SolidColorBrush(Colors.Orange),
                Stroke = new SolidColorBrush(Colors.DarkOrange),
                StrokeThickness = 1,
                Margin = new Thickness(2)
            };
            parent.Children.Add(polygon);
            IncrementCount("Polygon");

            // Image placeholder (using a colored rectangle)
            var imagePlaceholder = new Border
            {
                Width = 50,
                Height = 50,
                Background = new SolidColorBrush(Colors.LightGray),
                BorderBrush = new SolidColorBrush(Colors.Gray),
                BorderThickness = new Thickness(1),
                Margin = new Thickness(2),
                Child = new TextBlock 
                { 
                    Text = "IMG", 
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                }
            };
            parent.Children.Add(imagePlaceholder);
            IncrementCount("Border");
            IncrementCount("TextBlock");
        }

        private void AddMixedFormControls(Panel parent, int rowIndex)
        {
            // Label + TextBox pairs
            for (int i = 0; i < 2; i++)
            {
                var formGroup = new StackPanel 
                { 
                    Orientation = Orientation.Horizontal, 
                    Margin = new Thickness(2) 
                };
                IncrementCount("StackPanel");

                formGroup.Children.Add(new TextBlock 
                { 
                    Text = $"Field {i}:", 
                    Width = 50,
                    VerticalAlignment = VerticalAlignment.Center
                });
                IncrementCount("TextBlock");

                formGroup.Children.Add(new TextBox 
                { 
                    Width = 80,
                    Text = $"Value {rowIndex}.{i}"
                });
                IncrementCount("TextBox");

                parent.Children.Add(formGroup);
            }

            // CheckBox group
            var checkGroup = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(2) };
            IncrementCount("StackPanel");
            for (int i = 0; i < 3; i++)
            {
                checkGroup.Children.Add(new CheckBox 
                { 
                    Content = $"Opt{i}", 
                    IsChecked = i == 1,
                    Margin = new Thickness(2, 0, 2, 0)
                });
                IncrementCount("CheckBox");
            }
            parent.Children.Add(checkGroup);

            // Button row
            var btnRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(2) };
            IncrementCount("StackPanel");
            btnRow.Children.Add(new Button { Content = "Save", Padding = new Thickness(10, 5, 10, 5), Margin = new Thickness(2) });
            IncrementCount("Button");
            btnRow.Children.Add(new Button { Content = "Cancel", Padding = new Thickness(10, 5, 10, 5), Margin = new Thickness(2) });
            IncrementCount("Button");
            parent.Children.Add(btnRow);
        }

        private void AddDataDisplayControls(Panel parent, int rowIndex)
        {
            /*
            // DataGrid (if available)
            if (rowIndex % 4 == 0) // Only occasionally - DataGrid is heavy
            {
                try
                {
                    var dataGrid = new System.Windows.DataGrid
                    {
                        Width = 300,
                        Height = 100,
                        AutoGenerateColumns = true,
                        Margin = new Thickness(2)
                    };
                    
                    var items = new List<SampleDataItem>();
                    for (int i = 0; i < 5; i++)
                    {
                        items.Add(new SampleDataItem 
                        { 
                            Id = rowIndex * 10 + i, 
                            Name = $"Item {i}", 
                            Value = (rowIndex + i) * 1.5 
                        });
                    }
                    dataGrid.ItemsSource = items;
                    parent.Children.Add(dataGrid);
                    IncrementCount("DataGrid");
                }
                catch { }
            }
            */

            // Simple data display using TextBlocks
            var dataPanel = new StackPanel { Margin = new Thickness(2) };
            IncrementCount("StackPanel");
            
            for (int i = 0; i < 3; i++)
            {
                var row = new StackPanel { Orientation = Orientation.Horizontal };
                IncrementCount("StackPanel");
                
                row.Children.Add(new TextBlock 
                { 
                    Text = $"Row {i}:", 
                    FontWeight = FontWeights.Bold,
                    Width = 50
                });
                IncrementCount("TextBlock");
                
                row.Children.Add(new TextBlock { Text = $"Data value {rowIndex * 100 + i}" });
                IncrementCount("TextBlock");
                
                dataPanel.Children.Add(row);
            }
            parent.Children.Add(dataPanel);
        }

        private void AddComplexNestedControls(Panel parent, int rowIndex)
        {
            // Deeply nested structure
            var outerBorder = new Border
            {
                BorderBrush = new SolidColorBrush(Colors.DarkGray),
                BorderThickness = new Thickness(1),
                Padding = new Thickness(3),
                Margin = new Thickness(2)
            };
            IncrementCount("Border");

            var outerStack = new StackPanel();
            IncrementCount("StackPanel");

            // Header
            outerStack.Children.Add(new TextBlock 
            { 
                Text = $"Section {rowIndex}", 
                FontWeight = FontWeights.Bold,
                FontSize = 12
            });
            IncrementCount("TextBlock");

            // Nested Grid
            var innerGrid = new Grid { Margin = new Thickness(2) };
            IncrementCount("Grid");
            innerGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            innerGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            innerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            innerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            // Row 0
            var lbl1 = new TextBlock { Text = "Name:", Margin = new Thickness(2) };
            Grid.SetRow(lbl1, 0);
            Grid.SetColumn(lbl1, 0);
            innerGrid.Children.Add(lbl1);
            IncrementCount("TextBlock");

            var txt1 = new TextBox { Text = $"Value {rowIndex}", Width = 100, Margin = new Thickness(2) };
            Grid.SetRow(txt1, 0);
            Grid.SetColumn(txt1, 1);
            innerGrid.Children.Add(txt1);
            IncrementCount("TextBox");

            // Row 1
            var lbl2 = new TextBlock { Text = "Type:", Margin = new Thickness(2) };
            Grid.SetRow(lbl2, 1);
            Grid.SetColumn(lbl2, 0);
            innerGrid.Children.Add(lbl2);
            IncrementCount("TextBlock");

            var combo = new ComboBox { Width = 100, Margin = new Thickness(2) };
            combo.Items.Add("Type A");
            combo.Items.Add("Type B");
            combo.Items.Add("Type C");
            combo.SelectedIndex = rowIndex % 3;
            Grid.SetRow(combo, 1);
            Grid.SetColumn(combo, 1);
            innerGrid.Children.Add(combo);
            IncrementCount("ComboBox");
            IncrementCount("ComboBoxItem");
            IncrementCount("ComboBoxItem");
            IncrementCount("ComboBoxItem");

            outerStack.Children.Add(innerGrid);

            // Footer buttons
            var footer = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(2) };
            IncrementCount("StackPanel");
            footer.Children.Add(new Button { Content = "Edit", Margin = new Thickness(2), Padding = new Thickness(5, 2, 5, 2) });
            IncrementCount("Button");
            footer.Children.Add(new Button { Content = "Delete", Margin = new Thickness(2), Padding = new Thickness(5, 2, 5, 2) });
            IncrementCount("Button");
            outerStack.Children.Add(footer);

            outerBorder.Child = outerStack;
            parent.Children.Add(outerBorder);
        }

        private void IncrementCount(string controlType)
        {
            if (!_controlCounts.ContainsKey(controlType))
                _controlCounts[controlType] = 0;
            _controlCounts[controlType]++;
        }

        private Color GetColorForIndex(int index)
        {
            var colors = new[] 
            { 
                Colors.LightBlue, Colors.LightGreen, Colors.LightPink, 
                Colors.LightYellow, Colors.LightCoral, Colors.LightCyan,
                Colors.LightGoldenrodYellow, Colors.LightSalmon, Colors.LightSeaGreen
            };
            return colors[Math.Abs(index) % colors.Length];
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            RenderTarget.Children.Clear();
            RenderTimeText.Text = "--";
            ElementCountText.Text = "--";
            BindingCountText.Text = "--";
            ComparisonText.Text = "--";
            StatusText.Text = "Ready";
            StatusText.Foreground = new SolidColorBrush(Colors.Green);
            _lastResult = null;
        }

        private void SaveBaselineButton_Click(object sender, RoutedEventArgs e)
        {
            if (_lastResult == null)
            {
                StatusText.Text = "Run a benchmark first!";
                StatusText.Foreground = new SolidColorBrush(Colors.Red);
                return;
            }

            _baseline = _lastResult;
            SaveBaseline();
            UpdateComparison();
            StatusText.Text = "Baseline saved!";
            StatusText.Foreground = new SolidColorBrush(Colors.Blue);
        }

        private void CompareButton_Click(object sender, RoutedEventArgs e)
        {
            if (_baseline == null)
            {
                StatusText.Text = "No baseline saved!";
                StatusText.Foreground = new SolidColorBrush(Colors.Red);
                return;
            }

            UpdateComparison();
        }

        private void UpdateComparison()
        {
            if (_baseline == null || _lastResult == null)
            {
                ComparisonText.Text = "No baseline to compare";
                return;
            }

            double diff = _lastResult.RenderTimeMs - _baseline.RenderTimeMs;
            double percentChange = (_baseline.RenderTimeMs > 0) 
                ? (diff / _baseline.RenderTimeMs) * 100 
                : 0;

            string sign = diff > 0 ? "+" : "";
            string status = diff < 0 ? "FASTER ✓" : (diff > 0 ? "SLOWER ✗" : "SAME");
            
            ComparisonText.Text = $"{sign}{diff:N0} ms ({sign}{percentChange:N1}%) - {status}";
            ComparisonText.Foreground = new SolidColorBrush(diff <= 0 ? Colors.Green : Colors.Red);

            // Calculate speedup factor
            if (_baseline.RenderTimeMs > 0 && _lastResult.RenderTimeMs > 0)
            {
                double speedup = _baseline.RenderTimeMs / _lastResult.RenderTimeMs;
                if (speedup > 1.01)
                {
                    ComparisonText.Text += $" ({speedup:N2}x faster)";
                }
                else if (speedup < 0.99)
                {
                    ComparisonText.Text += $" ({1/speedup:N2}x slower)";
                }
            }
        }

        #region Persistence

        private void SaveHistory()
        {
            try
            {
                var list = new List<PerformanceResult>(_history);
                string json = JsonSerializer.Serialize(list);
                WriteToStorage(HistoryFileName, json);
            }
            catch { }
        }

        private void LoadHistory()
        {
            try
            {
                string json = ReadFromStorage(HistoryFileName);
                if (!string.IsNullOrEmpty(json))
                {
                    var list = JsonSerializer.Deserialize<List<PerformanceResult>>(json);
                    if (list != null)
                    {
                        foreach (var item in list)
                        {
                            _history.Add(item);
                        }
                    }
                }
            }
            catch { }
        }

        private void SaveBaseline()
        {
            try
            {
                string json = JsonSerializer.Serialize(_baseline);
                WriteToStorage(BaselineFileName, json);
            }
            catch { }
        }

        private void LoadBaseline()
        {
            try
            {
                string json = ReadFromStorage(BaselineFileName);
                if (!string.IsNullOrEmpty(json))
                {
                    _baseline = JsonSerializer.Deserialize<PerformanceResult>(json);
                }
            }
            catch { }
        }

        private void WriteToStorage(string fileName, string content)
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream fs = storage.CreateFile(fileName))
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(content);
                    fs.Write(bytes, 0, bytes.Length);
                }
            }
        }

        private string ReadFromStorage(string fileName)
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (storage.FileExists(fileName))
                {
                    using (IsolatedStorageFileStream fs = storage.OpenFile(fileName, FileMode.Open))
                    {
                        using (StreamReader sr = new StreamReader(fs))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
            return null;
        }

        #endregion
    }

    /// <summary>
    /// Stores a single performance measurement result.
    /// </summary>
    public class PerformanceResult
    {
        public DateTime Timestamp { get; set; }
        public int ElementCount { get; set; }
        public int RequestedCount { get; set; }
        public double RenderTimeMs { get; set; }
        public Dictionary<string, int> ControlBreakdown { get; set; }
    }

    /// <summary>
    /// Sample data item for DataGrid testing.
    /// </summary>
    public class SampleDataItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Value { get; set; }
    }
}
