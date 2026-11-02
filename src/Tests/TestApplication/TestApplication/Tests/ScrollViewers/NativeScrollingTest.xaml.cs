using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using OpenSilver;

namespace TestApplication.Tests.ScrollViewers
{
    public partial class NativeScrollingTest : Page
    {
        private const int FindRowIndex = 150;

        private int _basicScrollChangedCount;
        private int _clickCount;

        public NativeScrollingTest()
        {
            InitializeComponent();

            FeatureText.Text = $"Features.Scrolling.UseNativeScrolling = {Features.Scrolling.UseNativeScrolling}";

            for (int i = 0; i < 300; i++)
            {
                BasicPanel.Children.Add(new TextBlock
                {
                    Text = i == FindRowIndex ? $"Row {i}: FIND-ME-NATIVE-SCROLL" : $"Row {i}",
                    Padding = new Thickness(4, 2, 4, 2),
                    Background = new SolidColorBrush(i % 2 == 0 ? Colors.White : Color.FromArgb(255, 235, 240, 250)),
                });
            }

            for (int i = 0; i < 30; i++)
            {
                HorizontalStrip.Children.Add(new Border
                {
                    Width = 60,
                    Margin = new Thickness(2),
                    Background = new SolidColorBrush(Color.FromArgb(255, (byte)(i * 8), 120, (byte)(255 - i * 8))),
                    Child = new TextBlock { Text = i.ToString(), Foreground = new SolidColorBrush(Colors.White), Margin = new Thickness(4) },
                });
            }

            for (int i = 0; i < 30; i++)
            {
                InnerVerticalPanel.Children.Add(new TextBlock { Text = $"Inner row {i}", Margin = new Thickness(4, 1, 4, 1) });
                OptOutPanel.Children.Add(new TextBlock { Text = $"Managed row {i}", Margin = new Thickness(4, 1, 4, 1) });
            }

            NativeListBox.ItemsSource = Enumerable.Range(0, 1000).Select(i => $"Item {i}").ToList();
            VirtualizedListBox.ItemsSource = Enumerable.Range(0, 1000).Select(i => $"Virtualized item {i}").ToList();

            foreach (ComboBox comboBox in new[] { HorizontalVisibilityComboBox, VerticalVisibilityComboBox })
            {
                foreach (ScrollBarVisibility visibility in new[] { ScrollBarVisibility.Disabled, ScrollBarVisibility.Auto, ScrollBarVisibility.Hidden, ScrollBarVisibility.Visible })
                {
                    comboBox.Items.Add(visibility);
                }
            }
            HorizontalVisibilityComboBox.SelectedItem = ScrollBarVisibility.Auto;
            VerticalVisibilityComboBox.SelectedItem = ScrollBarVisibility.Auto;

            UpdateBasicReadout();
        }

        private static string Describe(ScrollViewer sv) =>
            $"H={sv.HorizontalOffset:0.#} V={sv.VerticalOffset:0.#} " +
            $"Extent={sv.ExtentWidth:0.#}x{sv.ExtentHeight:0.#} Viewport={sv.ViewportWidth:0.#}x{sv.ViewportHeight:0.#} " +
            $"Scrollable={sv.ScrollableWidth:0.#}x{sv.ScrollableHeight:0.#}";

        private void UpdateBasicReadout()
        {
            BasicReadout.Text = $"{Describe(BasicScrollViewer)} ScrollChanged={_basicScrollChangedCount}";
        }

        private void BasicScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            _basicScrollChangedCount++;
            UpdateBasicReadout();
        }

        private void Top_Click(object sender, RoutedEventArgs e) => BasicScrollViewer.ScrollToVerticalOffset(0);

        private void Offset1000_Click(object sender, RoutedEventArgs e) => BasicScrollViewer.ScrollToVerticalOffset(1000);

        private void End_Click(object sender, RoutedEventArgs e) => BasicScrollViewer.ScrollToVerticalOffset(double.MaxValue);

        private void LineDown_Click(object sender, RoutedEventArgs e) => BasicScrollViewer.LineDown();

        private void PageDown_Click(object sender, RoutedEventArgs e) => BasicScrollViewer.PageDown();

        private void BringIntoView_Click(object sender, RoutedEventArgs e)
        {
            // Uses TransformToVisual, which must account for the native scroll offset.
            var row = (UIElement)BasicPanel.Children[200];
            double y = row.TransformToVisual(BasicScrollViewer).Transform(new Point(0, 0)).Y;
            BasicScrollViewer.ScrollToVerticalOffset(BasicScrollViewer.VerticalOffset + y);
        }

        private void Native_Click(object sender, RoutedEventArgs e)
        {
            NativeScrolling.SetIsEnabled(BasicScrollViewer, NativeCheckBox.IsChecked == true);
        }

        private void Visible_Click(object sender, RoutedEventArgs e)
        {
            BasicScrollViewer.Visibility = VisibleCheckBox.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
        }

        private void VisibilityComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (HorizontalVisibilityComboBox.SelectedItem is ScrollBarVisibility h)
            {
                MatrixScrollViewer.HorizontalScrollBarVisibility = h;
            }
            if (VerticalVisibilityComboBox.SelectedItem is ScrollBarVisibility v)
            {
                MatrixScrollViewer.VerticalScrollBarVisibility = v;
            }
        }

        private void ContentSizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (MatrixContent != null)
            {
                MatrixContent.Width = e.NewValue;
                MatrixContent.Height = e.NewValue;
            }
        }

        private void MatrixScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            MatrixReadout.Text = Describe(MatrixScrollViewer);
        }

        private void CounterButton_Click(object sender, RoutedEventArgs e)
        {
            _clickCount++;
            CounterButton.Content = $"Click me ({_clickCount})";
        }

        private void InnerSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (InteractionReadout != null)
            {
                InteractionReadout.Text = $"Slider={e.NewValue:0}";
            }
        }

        private void DragThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Canvas.SetLeft(DragThumb, Math.Max(0, Math.Min(220, Canvas.GetLeft(DragThumb) + e.HorizontalChange)));
            Canvas.SetTop(DragThumb, Math.Max(0, Math.Min(50, Canvas.GetTop(DragThumb) + e.VerticalChange)));
            InteractionReadout.Text = $"Thumb=({Canvas.GetLeft(DragThumb):0},{Canvas.GetTop(DragThumb):0})";
        }

        private void ClickArea_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(ClickArea);
            InteractionReadout.Text = $"Click at ({p.X:0},{p.Y:0}) relative to the blue area (expected within 0..250 x 0..80)";
        }

        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            HeavyPanel.Children.Clear();
            for (int i = 0; i < 2000; i++)
            {
                var grid = new Grid { Height = 40, Background = new SolidColorBrush(i % 2 == 0 ? Colors.White : Color.FromArgb(255, 245, 245, 245)) };
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(40) });
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                var ellipse = new Ellipse { Width = 28, Height = 28, Fill = new SolidColorBrush(Color.FromArgb(255, (byte)(i % 256), 100, 200)) };
                var text = new TextBlock { Text = $"Heavy row {i}", VerticalAlignment = VerticalAlignment.Center };
                var button = new Button { Content = "Action", Margin = new Thickness(4) };
                Grid.SetColumn(text, 1);
                Grid.SetColumn(button, 2);
                grid.Children.Add(ellipse);
                grid.Children.Add(text);
                grid.Children.Add(button);
                HeavyPanel.Children.Add(grid);
            }
        }

        private void HeavyNative_Click(object sender, RoutedEventArgs e)
        {
            NativeScrolling.SetIsEnabled(HeavyScrollViewer, HeavyNativeCheckBox.IsChecked == true);
        }

        private void HeavyScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            HeavyReadout.Text = $"V={HeavyScrollViewer.VerticalOffset:0}";
        }
    }
}
