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
    public partial class AnimationTest : Page
    {
        public AnimationTest()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        void ButtonTestAnimations_Click(object sender, RoutedEventArgs e)
        {
            Storyboard storyboard = (Storyboard)Resources["TestStoryboard"];
            ColorAnimation colorAnimation = (ColorAnimation)storyboard.Children[0];
            Random rd = new Random();
            colorAnimation.To = new Color() { A = (byte)rd.Next(255), B = (byte)rd.Next(255), G = (byte)rd.Next(255), R = (byte)rd.Next(255) };
            storyboard.Begin();

            Storyboard storyboard2 = (Storyboard)Resources["TestStoryboard2"];
            ColorAnimation colorAnimation2 = (ColorAnimation)storyboard2.Children[0];
            colorAnimation2.From = new Color() { A = (byte)rd.Next(255), B = (byte)rd.Next(255), G = (byte)rd.Next(255), R = (byte)rd.Next(255) };
            colorAnimation2.To = new Color() { A = (byte)rd.Next(255), B = (byte)rd.Next(255), G = (byte)rd.Next(255), R = (byte)rd.Next(255) };
            storyboard2.Begin();

            Storyboard storyboard3 = (Storyboard)Resources["TestStoryboard3"];
            ColorAnimation colorAnimation3 = (ColorAnimation)storyboard3.Children[0];
            colorAnimation3.To = new Color() { A = (byte)rd.Next(255), B = (byte)rd.Next(255), G = (byte)rd.Next(255), R = (byte)rd.Next(255) };
            ColorAnimation colorAnimation3_2 = (ColorAnimation)storyboard3.Children[1];
            colorAnimation3_2.To = new Color() { A = (byte)rd.Next(255), B = (byte)rd.Next(255), G = (byte)rd.Next(255), R = (byte)rd.Next(255) };
            storyboard3.Begin();

            Storyboard storyboard4 = (Storyboard)Resources["TestStoryboard4"];
            ColorAnimation colorAnimation4 = (ColorAnimation)storyboard4.Children[0];
            colorAnimation4.To = new Color() { A = (byte)rd.Next(255), B = (byte)rd.Next(255), G = (byte)rd.Next(255), R = (byte)rd.Next(255) };
            ColorAnimation colorAnimation4_2 = (ColorAnimation)storyboard4.Children[1];
            colorAnimation4_2.To = new Color() { A = (byte)rd.Next(255), B = (byte)rd.Next(255), G = (byte)rd.Next(255), R = (byte)rd.Next(255) };
            storyboard4.Begin();
        }
    }
}
