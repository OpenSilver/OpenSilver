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

namespace TestApplication.Tests
{
    public partial class MouseMove_MouseCaptureTest : Page
    {
        public MouseMove_MouseCaptureTest()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        double mouseVerticalPosition;
        double mouseHorizontalPosition;
        bool isMouseCaptured;

        private void InnerBorderForPointerEvents_PointerPressed_1(object sender, MouseEventArgs e)
        {
            Border border = (Border)sender;
            mouseVerticalPosition = e.GetPosition(null).Y;
            mouseHorizontalPosition = e.GetPosition(null).X;
            border.CaptureMouse();
            PointerCaptureTextBlock.Text = "Pointer captured";
            isMouseCaptured = true;
        }

        private void InnerBorderForPointerEvents_PointerReleased_1(object sender, MouseEventArgs e)
        {
            Border border = (Border)sender;
            isMouseCaptured = false;
            border.ReleaseMouseCapture();
            PointerCaptureTextBlock.Text = "";
        }

        private void ContainerBorderForPointerEvents_PointerMoved(object sender, MouseEventArgs e)
        {
            Border border = (Border)sender;
            Random r = new Random();
            border.Background = new SolidColorBrush(Color.FromArgb((Byte)255, (Byte)r.Next(255), (Byte)r.Next(255), (Byte)r.Next(255)));
        }

        private void InnerBorderForPointerEvents_PointerMoved(object sender, MouseEventArgs e)
        {
            Border item = (Border)sender;
            if (isMouseCaptured)
            {
                // Calculate the current position of the object.
                double deltaV = e.GetPosition(null).Y - mouseVerticalPosition;
                double deltaH = e.GetPosition(null).X - mouseHorizontalPosition;
                Thickness margin = item.Margin;
                double newTop = deltaV + margin.Top;
                double newLeft = deltaH + margin.Left;

                // Set new position of object.
                margin.Left = newLeft;
                margin.Top = newTop;
                item.Margin = margin;

                // Update position global variables.
                mouseVerticalPosition = e.GetPosition(null).Y;
                mouseHorizontalPosition = e.GetPosition(null).X;
            }
        }
    }
}
