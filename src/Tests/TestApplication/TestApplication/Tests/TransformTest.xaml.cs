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
    public partial class TransformTest : Page
    {
        public TransformTest()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        void ButtonTranslate_Click(object sender, RoutedEventArgs e)
        {
            if (TestTransformBorder.RenderTransform == null || !(TestTransformBorder.RenderTransform is TranslateTransform))
            {
                TranslateTransform translateTransform = new TranslateTransform();
                TestTransformBorder.RenderTransform = translateTransform;
            }
            ((TranslateTransform)TestTransformBorder.RenderTransform).X += 10;
            ((TranslateTransform)TestTransformBorder.RenderTransform).Y += 10;
        }

        void ButtonRotate_Click(object sender, RoutedEventArgs e)
        {
            if (TestTransformBorder.RenderTransform == null || !(TestTransformBorder.RenderTransform is RotateTransform))
            {
                RotateTransform rotateTransform = new RotateTransform();
                TestTransformBorder.RenderTransform = rotateTransform;
            }
            ((RotateTransform)TestTransformBorder.RenderTransform).Angle += 10;
        }

        void TransformButton_Click(object sender, RoutedEventArgs e)
        {
            Random r = new Random();

            SolidColorBrush brush = new SolidColorBrush();

            brush.Color = Color.FromArgb((byte)r.Next(255), (byte)r.Next(255), (byte)r.Next(255), (byte)r.Next(255));
            TestTransformBorder.Background = brush;
        }
    }
}
