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

namespace TestApplication.Tests.GradientBrushes
{
    public partial class LinearGradientBrushTest : Page
    {
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
        }

    }
}
