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

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateAllProjections();
        }

        private void UpdateAllProjections()
        {
            if (RectangleProjection == null) return;

            // Update all projections with the same values
            PlaneProjection[] projections = { RectangleProjection, ButtonProjection, CardProjection, TextProjection };

            foreach (var projection in projections)
            {
                if (projection == null) continue;

                // Rotation
                projection.RotationX = RotationXSlider.Value;
                projection.RotationY = RotationYSlider.Value;
                projection.RotationZ = RotationZSlider.Value;

                // Center of Rotation
                projection.CenterOfRotationX = CenterXSlider.Value;
                projection.CenterOfRotationY = CenterYSlider.Value;
                projection.CenterOfRotationZ = CenterZSlider.Value;

                // Local Offset
                projection.LocalOffsetX = LocalOffsetXSlider.Value;
                projection.LocalOffsetY = LocalOffsetYSlider.Value;
                projection.LocalOffsetZ = LocalOffsetZSlider.Value;

                // Global Offset
                projection.GlobalOffsetX = GlobalOffsetXSlider.Value;
                projection.GlobalOffsetY = GlobalOffsetYSlider.Value;
                projection.GlobalOffsetZ = GlobalOffsetZSlider.Value;
            }

            // Update value displays
            UpdateValueDisplays();
        }

        private void UpdateValueDisplays()
        {
            RotationXValue.Text = Math.Round(RotationXSlider.Value).ToString();
            RotationYValue.Text = Math.Round(RotationYSlider.Value).ToString();
            RotationZValue.Text = Math.Round(RotationZSlider.Value).ToString();

            CenterXValue.Text = Math.Round(CenterXSlider.Value, 2).ToString();
            CenterYValue.Text = Math.Round(CenterYSlider.Value, 2).ToString();
            CenterZValue.Text = Math.Round(CenterZSlider.Value).ToString();

            LocalOffsetXValue.Text = Math.Round(LocalOffsetXSlider.Value).ToString();
            LocalOffsetYValue.Text = Math.Round(LocalOffsetYSlider.Value).ToString();
            LocalOffsetZValue.Text = Math.Round(LocalOffsetZSlider.Value).ToString();

            GlobalOffsetXValue.Text = Math.Round(GlobalOffsetXSlider.Value).ToString();
            GlobalOffsetYValue.Text = Math.Round(GlobalOffsetYSlider.Value).ToString();
            GlobalOffsetZValue.Text = Math.Round(GlobalOffsetZSlider.Value).ToString();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            // Rotation
            RotationXSlider.Value = 0;
            RotationYSlider.Value = 0;
            RotationZSlider.Value = 0;

            // Center of Rotation
            CenterXSlider.Value = 0.5;
            CenterYSlider.Value = 0.5;
            CenterZSlider.Value = 0;

            // Local Offset
            LocalOffsetXSlider.Value = 0;
            LocalOffsetYSlider.Value = 0;
            LocalOffsetZSlider.Value = 0;

            // Global Offset
            GlobalOffsetXSlider.Value = 0;
            GlobalOffsetYSlider.Value = 0;
            GlobalOffsetZSlider.Value = 0;
        }

        private void FlipCardButton_Click(object sender, RoutedEventArgs e)
        {
            // Toggle between 0 and 180 degrees on Y axis
            if (RotationYSlider.Value < 90)
            {
                RotationYSlider.Value = 180;
            }
            else
            {
                RotationYSlider.Value = 0;
            }
        }

        private void SpinButton_Click(object sender, RoutedEventArgs e)
        {
            // Rotate 45 degrees on Y axis
            RotationYSlider.Value = (RotationYSlider.Value + 45) % 360;
            if (RotationYSlider.Value > 180)
            {
                RotationYSlider.Value -= 360;
            }
        }

        private void PresetTiltLeft_Click(object sender, RoutedEventArgs e)
        {
            ResetButton_Click(sender, e);
            RotationYSlider.Value = -30;
            RotationXSlider.Value = 10;
        }

        private void PresetTiltRight_Click(object sender, RoutedEventArgs e)
        {
            ResetButton_Click(sender, e);
            RotationYSlider.Value = 30;
            RotationXSlider.Value = 10;
        }

        private void PresetLeanBack_Click(object sender, RoutedEventArgs e)
        {
            ResetButton_Click(sender, e);
            RotationXSlider.Value = -45;
        }

        private void PresetCarousel_Click(object sender, RoutedEventArgs e)
        {
            ResetButton_Click(sender, e);
            RotationYSlider.Value = 45;
            LocalOffsetZSlider.Value = -100;
        }

        private void PresetZoomIn_Click(object sender, RoutedEventArgs e)
        {
            ResetButton_Click(sender, e);
            LocalOffsetZSlider.Value = 200;
        }

        private void Preset3DRotate_Click(object sender, RoutedEventArgs e)
        {
            ResetButton_Click(sender, e);
            RotationXSlider.Value = 20;
            RotationYSlider.Value = 30;
            RotationZSlider.Value = 10;
        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            _clickCount++;
            ClickCount.Text = $"Clicks: {_clickCount}";
        }
    }
}
