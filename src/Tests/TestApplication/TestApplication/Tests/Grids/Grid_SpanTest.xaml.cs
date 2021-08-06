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

namespace TestApplication.Tests.Grids
{
    public partial class Grid_SpanTest : Page
    {
        public Grid_SpanTest()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void ElementRowIndexTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int row = 0;
            int column = 0;
            double columnWidth = double.NaN;
            double rowHeight = double.NaN;
            if (int.TryParse(ElementColumnIndexTextBox.Text, out column))
            {
                if (column >= 0 && column < FirstGrid.ColumnDefinitions.Count)
                {
                    columnWidth = FirstGrid.ColumnDefinitions[column].ActualWidth;
                }
                else
                {
                    ColumnWidthTextBlock.Text = "Index out of range.";
                }

            }
            if (int.TryParse(ElementRowIndexTextBox.Text, out row))
            {
                if (row >= 0 && row < FirstGrid.RowDefinitions.Count)
                {
                    rowHeight = FirstGrid.RowDefinitions[row].ActualHeight;
                }
                else
                {
                    ColumnWidthTextBlock.Text = "Index out of range.";
                }
            }

            if (double.IsNaN(columnWidth))
            {
                ColumnWidthTextBlock.Text = "Could not parse column TextBox.";
            }
            else
            {
                ColumnWidthTextBlock.Text = columnWidth.ToString();
            }
            if (double.IsNaN(rowHeight))
            {
                RowHeightTextBlock.Text = "Could not parse column TextBox.";
            }
            else
            {
                RowHeightTextBlock.Text = rowHeight.ToString();
            }
        }

        int amountOfElementsAdded = 0;
        private void ButtonAddElementsToGrid_Click(object sender, RoutedEventArgs e)
        {
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Yellow" Grid.ColumnSpan="2"  Margin="5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Orange" Grid.Column="0" Grid.Row="1" Grid.RowSpan="2" Margin="5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Red" Grid.Column="0" Grid.Row="2" Opacity="0.5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="LightGreen" Grid.Column="1" Grid.Row="0"  Grid.RowSpan="2" Opacity="0.5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Lime" Grid.Column="1" Grid.Row="1" Grid.ColumnSpan="2" Margin="5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Green" Grid.Column="1" Grid.Row="2" />
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="LightBlue" Grid.Column="2" Grid.Row="0" Grid.RowSpan="2" Margin="10" Opacity="0.5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Blue" Grid.Column="2" Grid.Row="1" Opacity="0.5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="DarkBlue" Grid.Column="2" Grid.Row="2"/>
            if (amountOfElementsAdded < 9)
            {
                Border border = new Border() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };

                switch (amountOfElementsAdded)
                {

                    case 0:
                        border.Background = new SolidColorBrush(Colors.Yellow);
                        border.Margin = new Thickness(5);
                        Grid.SetColumnSpan(border, 2);
                        break;
                    case 1:
                        border.Background = new SolidColorBrush(Colors.Orange);
                        border.Margin = new Thickness(5);
                        Grid.SetColumn(border, 0);
                        Grid.SetRow(border, 1);
                        Grid.SetRowSpan(border, 2);
                        break;
                    case 2:
                        border.Background = new SolidColorBrush(Colors.Red);
                        border.Opacity = 0.5;
                        Grid.SetColumn(border, 0);
                        Grid.SetRow(border, 2);
                        break;
                    case 3:
                        border.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x90, 0xEE, 0x90));
                        border.Opacity = 0.5;
                        Grid.SetColumn(border, 1);
                        Grid.SetRow(border, 0);
                        Grid.SetRowSpan(border, 2);
                        break;
                    case 4:
                        border.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0xFF, 0x00));
                        border.Margin = new Thickness(5);
                        Grid.SetColumn(border, 1);
                        Grid.SetRow(border, 1);
                        Grid.SetColumnSpan(border, 2);
                        break;
                    case 5:
                        border.Background = new SolidColorBrush(Colors.Green);
                        Grid.SetColumn(border, 1);
                        Grid.SetRow(border, 2);
                        break;
                    case 6:
                        border.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xAD, 0xD8, 0xE6));
                        border.Margin = new Thickness(10);
                        border.Opacity = 0.5;
                        Grid.SetColumn(border, 2);
                        Grid.SetRow(border, 0);
                        Grid.SetRowSpan(border, 2);
                        break;
                    case 7:
                        border.Background = new SolidColorBrush(Colors.Blue);
                        border.Opacity = 0.5;
                        Grid.SetColumn(border, 2);
                        Grid.SetRow(border, 1);
                        break;
                    case 8:
                        border.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0x00, 0x8B));
                        Grid.SetColumn(border, 2);
                        Grid.SetRow(border, 2);
                        break;
                    default:
                        break;
                }

                ++amountOfElementsAdded;
                AddChildrenGrid.Children.Add(border);
            }
        }

        Border border00 = null;
        private void ButtonAddRemove_0_0_Click(object sender, RoutedEventArgs e)
        {
            ////<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Yellow" Grid.ColumnSpan="2"  Margin="5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Orange" Grid.Column="0" Grid.Row="1" Grid.RowSpan="2" Margin="5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Red" Grid.Column="0" Grid.Row="2" Opacity="0.5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="LightGreen" Grid.Column="1" Grid.Row="0"  Grid.RowSpan="2" Opacity="0.5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Lime" Grid.Column="1" Grid.Row="1" Grid.ColumnSpan="2" Margin="5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Green" Grid.Column="1" Grid.Row="2" />
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="LightBlue" Grid.Column="2" Grid.Row="0" Grid.RowSpan="2" Margin="10" Opacity="0.5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Blue" Grid.Column="2" Grid.Row="1" Opacity="0.5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="DarkBlue" Grid.Column="2" Grid.Row="2"/>
            if (border00 == null)
            {
                border00 = new Border() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
                border00.Background = new SolidColorBrush(Colors.Yellow);
                border00.Margin = new Thickness(5);
                Grid.SetColumnSpan(border00, 2);
                AddRemoveChildrenGrid.Children.Add(border00);
            }
            else
            {
                AddRemoveChildrenGrid.Children.Remove(border00);
                border00 = null;
            }
        }

        Border border10 = null;
        private void ButtonAddRemove_1_0_Click(object sender, RoutedEventArgs e)
        {
            ////<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Yellow" Grid.ColumnSpan="2"  Margin="5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Orange" Grid.Column="0" Grid.Row="1" Grid.RowSpan="2" Margin="5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Red" Grid.Column="0" Grid.Row="2" Opacity="0.5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="LightGreen" Grid.Column="1" Grid.Row="0"  Grid.RowSpan="2" Opacity="0.5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Lime" Grid.Column="1" Grid.Row="1" Grid.ColumnSpan="2" Margin="5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Green" Grid.Column="1" Grid.Row="2" />
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="LightBlue" Grid.Column="2" Grid.Row="0" Grid.RowSpan="2" Margin="10" Opacity="0.5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Blue" Grid.Column="2" Grid.Row="1" Opacity="0.5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="DarkBlue" Grid.Column="2" Grid.Row="2"/>
            if (border10 == null)
            {
                border10 = new Border() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
                border10.Background = new SolidColorBrush(Colors.Orange);
                border10.Margin = new Thickness(5);
                Grid.SetColumn(border10, 0);
                Grid.SetRow(border10, 1);
                Grid.SetRowSpan(border10, 2);
                AddRemoveChildrenGrid.Children.Add(border10);
            }
            else
            {
                AddRemoveChildrenGrid.Children.Remove(border10);
                border10 = null;
            }
        }

        Border border20 = null;
        private void ButtonAddRemove_2_0_Click(object sender, RoutedEventArgs e)
        {
            ////<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Yellow" Grid.ColumnSpan="2"  Margin="5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Orange" Grid.Column="0" Grid.Row="1" Grid.RowSpan="2" Margin="5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Red" Grid.Column="0" Grid.Row="2" Opacity="0.5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="LightGreen" Grid.Column="1" Grid.Row="0"  Grid.RowSpan="2" Opacity="0.5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Lime" Grid.Column="1" Grid.Row="1" Grid.ColumnSpan="2" Margin="5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Green" Grid.Column="1" Grid.Row="2" />
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="LightBlue" Grid.Column="2" Grid.Row="0" Grid.RowSpan="2" Margin="10" Opacity="0.5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Blue" Grid.Column="2" Grid.Row="1" Opacity="0.5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="DarkBlue" Grid.Column="2" Grid.Row="2"/>
            if (border20 == null)
            {
                border20 = new Border() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
                border20.Background = new SolidColorBrush(Colors.Red);
                border20.Opacity = 0.5;
                Grid.SetColumn(border20, 0);
                Grid.SetRow(border20, 2);
                AddRemoveChildrenGrid.Children.Add(border20);
            }
            else
            {
                AddRemoveChildrenGrid.Children.Remove(border20);
                border20 = null;
            }
        }

        Border border01 = null;
        private void ButtonAddRemove_0_1_Click(object sender, RoutedEventArgs e)
        {
            ////<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Yellow" Grid.ColumnSpan="2"  Margin="5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Orange" Grid.Column="0" Grid.Row="1" Grid.RowSpan="2" Margin="5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Red" Grid.Column="0" Grid.Row="2" Opacity="0.5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="LightGreen" Grid.Column="1" Grid.Row="0"  Grid.RowSpan="2" Opacity="0.5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Lime" Grid.Column="1" Grid.Row="1" Grid.ColumnSpan="2" Margin="5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Green" Grid.Column="1" Grid.Row="2" />
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="LightBlue" Grid.Column="2" Grid.Row="0" Grid.RowSpan="2" Margin="10" Opacity="0.5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Blue" Grid.Column="2" Grid.Row="1" Opacity="0.5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="DarkBlue" Grid.Column="2" Grid.Row="2"/>
            if (border01 == null)
            {
                border01 = new Border() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
                border01.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x90, 0xEE, 0x90));
                border01.Opacity = 0.5;
                Grid.SetColumn(border01, 1);
                Grid.SetRow(border01, 0);
                Grid.SetRowSpan(border01, 2);
                AddRemoveChildrenGrid.Children.Add(border01);
            }
            else
            {
                AddRemoveChildrenGrid.Children.Remove(border01);
                border01 = null;
            }
        }

        Border border11 = null;
        private void ButtonAddRemove_1_1_Click(object sender, RoutedEventArgs e)
        {
            ////<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Yellow" Grid.ColumnSpan="2"  Margin="5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Orange" Grid.Column="0" Grid.Row="1" Grid.RowSpan="2" Margin="5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Red" Grid.Column="0" Grid.Row="2" Opacity="0.5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="LightGreen" Grid.Column="1" Grid.Row="0"  Grid.RowSpan="2" Opacity="0.5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Lime" Grid.Column="1" Grid.Row="1" Grid.ColumnSpan="2" Margin="5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Green" Grid.Column="1" Grid.Row="2" />
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="LightBlue" Grid.Column="2" Grid.Row="0" Grid.RowSpan="2" Margin="10" Opacity="0.5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Blue" Grid.Column="2" Grid.Row="1" Opacity="0.5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="DarkBlue" Grid.Column="2" Grid.Row="2"/>
            if (border11 == null)
            {
                border11 = new Border() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
                border11.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0xFF, 0x00));
                border11.Margin = new Thickness(5);
                Grid.SetColumn(border11, 1);
                Grid.SetRow(border11, 1);
                Grid.SetColumnSpan(border11, 2);
                AddRemoveChildrenGrid.Children.Add(border11);
            }
            else
            {
                AddRemoveChildrenGrid.Children.Remove(border11);
                border11 = null;
            }
        }

        Border border21 = null;
        private void ButtonAddRemove_2_1_Click(object sender, RoutedEventArgs e)
        {
            ////<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Yellow" Grid.ColumnSpan="2"  Margin="5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Orange" Grid.Column="0" Grid.Row="1" Grid.RowSpan="2" Margin="5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Red" Grid.Column="0" Grid.Row="2" Opacity="0.5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="LightGreen" Grid.Column="1" Grid.Row="0"  Grid.RowSpan="2" Opacity="0.5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Lime" Grid.Column="1" Grid.Row="1" Grid.ColumnSpan="2" Margin="5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Green" Grid.Column="1" Grid.Row="2" />
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="LightBlue" Grid.Column="2" Grid.Row="0" Grid.RowSpan="2" Margin="10" Opacity="0.5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Blue" Grid.Column="2" Grid.Row="1" Opacity="0.5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="DarkBlue" Grid.Column="2" Grid.Row="2"/>
            if (border21 == null)
            {
                border21 = new Border() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
                border21.Background = new SolidColorBrush(Colors.Green);
                Grid.SetColumn(border21, 1);
                Grid.SetRow(border21, 2);
                AddRemoveChildrenGrid.Children.Add(border21);
            }
            else
            {
                AddRemoveChildrenGrid.Children.Remove(border21);
                border21 = null;
            }
        }

        Border border02 = null;
        private void ButtonAddRemove_0_2_Click(object sender, RoutedEventArgs e)
        {
            ////<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Yellow" Grid.ColumnSpan="2"  Margin="5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Orange" Grid.Column="0" Grid.Row="1" Grid.RowSpan="2" Margin="5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Red" Grid.Column="0" Grid.Row="2" Opacity="0.5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="LightGreen" Grid.Column="1" Grid.Row="0"  Grid.RowSpan="2" Opacity="0.5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Lime" Grid.Column="1" Grid.Row="1" Grid.ColumnSpan="2" Margin="5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Green" Grid.Column="1" Grid.Row="2" />
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="LightBlue" Grid.Column="2" Grid.Row="0" Grid.RowSpan="2" Margin="10" Opacity="0.5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Blue" Grid.Column="2" Grid.Row="1" Opacity="0.5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="DarkBlue" Grid.Column="2" Grid.Row="2"/>
            if (border02 == null)
            {
                border02 = new Border() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
                border02.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xAD, 0xD8, 0xE6));
                border02.Margin = new Thickness(10);
                border02.Opacity = 0.5;
                Grid.SetColumn(border02, 2);
                Grid.SetRow(border02, 0);
                Grid.SetRowSpan(border02, 2);
                AddRemoveChildrenGrid.Children.Add(border02);
            }
            else
            {
                AddRemoveChildrenGrid.Children.Remove(border02);
                border02 = null;
            }
        }

        Border border12 = null;
        private void ButtonAddRemove_1_2_Click(object sender, RoutedEventArgs e)
        {
            ////<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Yellow" Grid.ColumnSpan="2"  Margin="5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Orange" Grid.Column="0" Grid.Row="1" Grid.RowSpan="2" Margin="5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Red" Grid.Column="0" Grid.Row="2" Opacity="0.5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="LightGreen" Grid.Column="1" Grid.Row="0"  Grid.RowSpan="2" Opacity="0.5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Lime" Grid.Column="1" Grid.Row="1" Grid.ColumnSpan="2" Margin="5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Green" Grid.Column="1" Grid.Row="2" />
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="LightBlue" Grid.Column="2" Grid.Row="0" Grid.RowSpan="2" Margin="10" Opacity="0.5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Blue" Grid.Column="2" Grid.Row="1" Opacity="0.5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="DarkBlue" Grid.Column="2" Grid.Row="2"/>
            if (border12 == null)
            {
                border12 = new Border() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
                border12.Background = new SolidColorBrush(Colors.Blue);
                border12.Opacity = 0.5;
                Grid.SetColumn(border12, 2);
                Grid.SetRow(border12, 1);
                AddRemoveChildrenGrid.Children.Add(border12);
            }
            else
            {
                AddRemoveChildrenGrid.Children.Remove(border12);
                border12 = null;
            }
        }

        Border border22 = null;
        private void ButtonAddRemove_2_2_Click(object sender, RoutedEventArgs e)
        {
            ////<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Yellow" Grid.ColumnSpan="2"  Margin="5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Orange" Grid.Column="0" Grid.Row="1" Grid.RowSpan="2" Margin="5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Red" Grid.Column="0" Grid.Row="2" Opacity="0.5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="LightGreen" Grid.Column="1" Grid.Row="0"  Grid.RowSpan="2" Opacity="0.5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Lime" Grid.Column="1" Grid.Row="1" Grid.ColumnSpan="2" Margin="5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Green" Grid.Column="1" Grid.Row="2" />
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="LightBlue" Grid.Column="2" Grid.Row="0" Grid.RowSpan="2" Margin="10" Opacity="0.5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="Blue" Grid.Column="2" Grid.Row="1" Opacity="0.5"/>
            //<Border HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="DarkBlue" Grid.Column="2" Grid.Row="2"/>
            if (border22 == null)
            {
                border22 = new Border() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
                border22.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0x00, 0x8B));
                Grid.SetColumn(border22, 2);
                Grid.SetRow(border22, 2);
                AddRemoveChildrenGrid.Children.Add(border22);
            }
            else
            {
                AddRemoveChildrenGrid.Children.Remove(border22);
                border22 = null;
            }
        }

        private void ButtonAddColumn(object sender, RoutedEventArgs e)
        {
            ColumnDefinition col = new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) };
            AddRemoveRowsColumnsGrid.ColumnDefinitions.Add(col);
        }

        private void ButtonRemoveColumn(object sender, RoutedEventArgs e)
        {
            if (AddRemoveRowsColumnsGrid.ColumnDefinitions.Count > 0)
            {
                AddRemoveRowsColumnsGrid.ColumnDefinitions.RemoveAt(AddRemoveRowsColumnsGrid.ColumnDefinitions.Count - 1);
            }
        }

        private void ButtonAddRow(object sender, RoutedEventArgs e)
        {
            RowDefinition row = new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) };
            AddRemoveRowsColumnsGrid.RowDefinitions.Add(row);

        }

        private void ButtonRemoveRow(object sender, RoutedEventArgs e)
        {
            if (AddRemoveRowsColumnsGrid.RowDefinitions.Count > 0)
            {
                AddRemoveRowsColumnsGrid.RowDefinitions.RemoveAt(AddRemoveRowsColumnsGrid.RowDefinitions.Count - 1);
            }
        }
    }
}
