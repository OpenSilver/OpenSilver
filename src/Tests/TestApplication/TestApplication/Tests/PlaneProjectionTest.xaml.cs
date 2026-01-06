using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace TestApplication.Tests
{
    public partial class PlaneProjectionTest : Page
    {
        private int _clickCount = 0;

        public PlaneProjectionTest()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void RotationSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Update the PlaneProjection based on slider values
            if (RectangleProjection != null && ButtonProjection != null)
            {
                RectangleProjection.RotationX = RotationXSlider.Value;
                RectangleProjection.RotationY = RotationYSlider.Value;
                RectangleProjection.RotationZ = RotationZSlider.Value;
                RectangleProjection.GlobalOffsetX = GlobalOffsetXSlider.Value;
                RectangleProjection.GlobalOffsetY = GlobalOffsetYSlider.Value;

                ButtonProjection.RotationX = RotationXSlider.Value;
                ButtonProjection.RotationY = RotationYSlider.Value;
                ButtonProjection.RotationZ = RotationZSlider.Value;
                ButtonProjection.GlobalOffsetX = GlobalOffsetXSlider.Value;
                ButtonProjection.GlobalOffsetY = GlobalOffsetYSlider.Value;

                // Update value displays
                RotationXValue.Text = Math.Round(RotationXSlider.Value).ToString();
                RotationYValue.Text = Math.Round(RotationYSlider.Value).ToString();
                RotationZValue.Text = Math.Round(RotationZSlider.Value).ToString();
                GlobalOffsetXValue.Text = Math.Round(GlobalOffsetXSlider.Value).ToString();
                GlobalOffsetYValue.Text = Math.Round(GlobalOffsetYSlider.Value).ToString();
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            RotationXSlider.Value = 0;
            RotationYSlider.Value = 0;
            RotationZSlider.Value = 0;
            GlobalOffsetXSlider.Value = 0;
            GlobalOffsetYSlider.Value = 0;
        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            _clickCount++;
            ClickCount.Text = $"Clicks: {_clickCount}";
        }
    }
}

