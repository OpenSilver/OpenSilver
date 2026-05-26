using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Navigation;

namespace TestApplication.Tests
{
    public partial class VisualBrushTest : Page
    {
        public VisualBrushTest()
        {
            this.InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private int _count;

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var vb = Resources["vb"] as VisualBrush;
            vb.PixelRatio++;

            _count++;

            string[] colors;

            if (_count % 2 == 0)
            {
                colors = ["Pink", "Black", "Gray", "Purple"];
            }
            else
            {
                colors = ["Brown", "AliceBlue", "Orange", "LimeGreen"];
            }

            string obj =
                $"""
                <Grid xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                      Width="150"
                      Height="150">
                    <Grid.RowDefinitions>
                        <RowDefinition Height="*" />
                        <RowDefinition Height="*" />
                    </Grid.RowDefinitions>
                    <Grid.ColumnDefinitions>
                        <ColumnDefinition Width="*" />
                        <ColumnDefinition Width="*" />
                    </Grid.ColumnDefinitions>
                    <Border Background="{colors[0]}"
                            CornerRadius="5"
                            Grid.Row="0"
                            Grid.Column="0">
                        <TextBlock VerticalAlignment="Center"
                                   HorizontalAlignment="Center"
                                   FontSize="16"
                                   Text="Child 1"
                                   Foreground="White" />
                    </Border>
                    <Border Background="{colors[1]}"
                            CornerRadius="5"
                            Grid.Row="0"
                            Grid.Column="1">
                        <TextBlock VerticalAlignment="Center"
                                   HorizontalAlignment="Center"
                                   FontSize="16"
                                   Text="Child 2"
                                   Foreground="White" />
                    </Border>
                    <Border Background="{colors[2]}"
                            CornerRadius="5"
                            Grid.Row="1"
                            Grid.Column="0">
                        <TextBlock VerticalAlignment="Center"
                                   HorizontalAlignment="Center"
                                   FontSize="16"
                                   Text="Child 3"
                                   Foreground="White" />
                    </Border>
                    <Border Background="{colors[3]}"
                            CornerRadius="5"
                            Grid.Row="1"
                            Grid.Column="1">
                        <TextBlock VerticalAlignment="Center"
                                   HorizontalAlignment="Center"
                                   FontSize="16"
                                   Text="Child 4"
                                   Foreground="White" />
                    </Border>
                </Grid>
                """;

            //vb.Visual = new Grid { Width = 50, Height = 50, Background = Brushes.Red };

            //vb.Visual = XamlReader.Load(obj) as UIElement;
        }
    }
}
