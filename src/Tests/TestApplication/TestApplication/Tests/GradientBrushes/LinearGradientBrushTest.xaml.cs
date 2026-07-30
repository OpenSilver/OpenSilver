using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace TestApplication.Tests.GradientBrushes
{
    public partial class LinearGradientBrushTest : Page
    {
        private LinearGradientBrush _borderBrushEndPointBrush;
        private LinearGradientBrush _gridBorderBrush;
        private LinearGradientBrush _stackPanelBorderBrush;

        public LinearGradientBrushTest()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            BorderForLinearGradientBrush.Background = new LinearGradientBrush(
                new GradientStopCollection() {
                    new GradientStop()
                    {
                        Color = Colors.Blue,
                        Offset = 0.0
                    },
                    new GradientStop()
                    {
                        Color = Colors.Orange,
                        Offset = 0.25
                    },
                    new GradientStop()
                    {
                        Color = Colors.Yellow,
                        Offset = 0.50
                    },
                    new GradientStop()
                    {
                        Color = Colors.Green,
                        Offset = 0.75
                    },
                    new GradientStop()
                    {
                        Color = Colors.Red,
                        Offset = 1.0
                    }
                }, 60);

            Point start = new Point(0, 0.5);
            Point end = new Point(1, 0.5);

            _borderBrushEndPointBrush = CreateBlueToRedBrush(start, end);
            BorderBrushEndPointTest.BorderBrush = _borderBrushEndPointBrush;

            _gridBorderBrush = CreateBlueToRedBrush(start, end);
            GridBorderBrushTest.BorderBrush = _gridBorderBrush;

            _stackPanelBorderBrush = CreateBlueToRedBrush(start, end);
            StackPanelBorderBrushTest.BorderBrush = _stackPanelBorderBrush;
        }

        private void BorderBrushDirectionRight_Click(object sender, RoutedEventArgs e)
        {
            SetBorderBrushDirection(new Point(0, 0.5), new Point(1, 0.5));
        }

        private void BorderBrushDirectionDown_Click(object sender, RoutedEventArgs e)
        {
            SetBorderBrushDirection(new Point(0.5, 0), new Point(0.5, 1));
        }

        private void BorderBrushDirectionLeft_Click(object sender, RoutedEventArgs e)
        {
            SetBorderBrushDirection(new Point(1, 0.5), new Point(0, 0.5));
        }

        private void BorderBrushDirectionUp_Click(object sender, RoutedEventArgs e)
        {
            SetBorderBrushDirection(new Point(0.5, 1), new Point(0.5, 0));
        }

        private void SetBorderBrushDirection(Point startPoint, Point endPoint)
        {
            ApplyDirection(_borderBrushEndPointBrush, startPoint, endPoint);
            ApplyDirection(_gridBorderBrush, startPoint, endPoint);
            ApplyDirection(_stackPanelBorderBrush, startPoint, endPoint);
        }

        private static void ApplyDirection(LinearGradientBrush brush, Point startPoint, Point endPoint)
        {
            brush.StartPoint = startPoint;
            brush.EndPoint = endPoint;
        }

        private static LinearGradientBrush CreateBlueToRedBrush(Point startPoint, Point endPoint)
        {
            return new LinearGradientBrush(
                new GradientStopCollection
                {
                    new GradientStop(Colors.Blue, 0),
                    new GradientStop(Colors.Red, 1),
                },
                startPoint,
                endPoint);
        }
    }
}
