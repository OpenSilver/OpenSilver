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
using System.ComponentModel;

namespace TestApplication.Tests
{
    public partial class StyleTest : Page
    {
        public StyleTest()
        {
            InitializeComponent();
        }

        public class MyColor : INotifyPropertyChanged
        {
            private SolidColorBrush _backgroundColor = new SolidColorBrush(Colors.White);
            public SolidColorBrush BackgroundColor
            {
                get { return _backgroundColor; }
                set
                {
                    _backgroundColor = value;
                    OnPropertyChanged("BackgroundColor");
                }
            }


            public event PropertyChangedEventHandler PropertyChanged;
            public void OnPropertyChanged(string propertyName)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }

        }

        // SLDISABLED
        //Setter _setterForMyStyle1Background;
        Setter _setterForMyStyle2Foreground;
        MyColor _myColor;

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _myColor = (MyColor)this.Resources["MyColor"];

            //<Style x:Key="MyStyle1" TargetType="TextBlock">
            //    <Setter Property="Foreground" Value="White" />
            //    <Setter Property="Background" Value="Black" />
            //</Style>
            //<Style x:Key="MyStyle2" TargetType="TextBlock" BasedOn="{StaticResource MyStyle1}">
            //    <Setter Property="Foreground" Value="Red" />
            //</Style>
            Style MyStyle1 = new Style(typeof(TextBlock));
            Setter setter = new Setter(TextBlock.ForegroundProperty, new SolidColorBrush(Colors.White));
            MyStyle1.Setters.Add(setter);
            setter = new Setter(TextBlock.FontSizeProperty, 21d);
            MyStyle1.Setters.Add(setter);
            // SLDISABLED
            //_setterForMyStyle1Background = new Setter(TextBlock.BackgroundProperty, new SolidColorBrush(Colors.Black));
            //MyStyle1.Setters.Add(_setterForMyStyle1Background);

            Style MyStyle2 = new Style(typeof(TextBlock));
            MyStyle2.BasedOn = MyStyle1;
            _setterForMyStyle2Foreground = new Setter(TextBlock.ForegroundProperty, new SolidColorBrush(Colors.Red));
            MyStyle2.Setters.Add(_setterForMyStyle2Foreground);

            MyTextBlockForStyle1.Style = MyStyle1;
            MyTextBlockForStyle2.Style = MyStyle2;
            MyTextBlockForStyle3.Style = MyStyle1;
        }

        private void ButtonTestChangeInStyles_Click(object sender, RoutedEventArgs e)
        {
            // SLDISABLED      -      This need to be rethought because in SL, styles cannot be modified once they are assigned to an UI element. (see Style.IsSealed and Setter.IsSealed)
            //Random rand = new Random();
            //_setterForMyStyle1Background.Value = new SolidColorBrush(Color.FromArgb((byte)255, (byte)rand.Next(256), (byte)rand.Next(256), (byte)rand.Next(256)));
            //_setterForMyStyle2Foreground.Property = TextBlock.ForegroundProperty;
            //_setterForMyStyle2Foreground.Value = new SolidColorBrush(Color.FromArgb((byte)255, (byte)rand.Next(256), (byte)rand.Next(256), (byte)rand.Next(256)));
            //_myColor.BackgroundColor = new SolidColorBrush(Color.FromArgb((byte)255, (byte)rand.Next(256), (byte)rand.Next(256), (byte)rand.Next(256)));
        }
    }
}
