using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
//using System.Collections.Specialized;
//using System.ComponentModel;
using System.IO;

#if SLMIGRATION
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using System.Windows.Data;
using System.Windows.Shapes;
using System.Windows.Threading;
#else
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Data;
using Windows.Foundation;
using Windows.UI.Xaml.Shapes;
#endif
#if WORKINPROGRESS
#if SLMIGRATION
using System.Windows.Media.Imaging;
#else
using Windows.UI.Xaml.Media.Imaging;
#endif //SLMIGRATION
#endif //WORKINPROGRESS
//using System.Xml.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.IO.IsolatedStorage;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;
using System.Windows.Interactivity;

namespace TestApplication
{
    public sealed partial class MainPage : Page
    {
        Setter _setterForMyStyle1Background;
        Setter _setterForMyStyle2Foreground;
        MyColor _myColor;

        public MainPage()
        {
            // Measure time:
            int initialTickCount = Environment.TickCount;
            this.Loaded += (s, e) =>
            {
                MessageBox.Show("Time before Loaded event: " + (Environment.TickCount - initialTickCount).ToString() + "ms");
            };

            try
            {

                this.InitializeComponent();
            }
            catch (Exception ex)
            {

                throw;
            }


            //----------------------
            // Test programmatically setting the LinearGradientBrush angle:
            //----------------------
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

            //----------------------
            // Other
            //----------------------


            //ObservableCollection<string> ooo = new ObservableCollection<string>();
            //ooo.Move(3, 4);
            //TestGroupBy();

            //for testing the DataGrid:
            _cats.Add(new Cat("Tom", 10));
            _cats.Add(new Cat("Blacky", 20));
            _cats.Add(new Cat("Pasha", 4));

            // Initial list box and combo box items:
            ItemsControl1.Items.Add("Initial item 1");
            ItemsControl1.Items.Add("Initial item 2");
            ListBox1.Items.Add("Initial item 1");
            ListBox1.Items.Add("Initial item 2");
            ListBox1.SelectedIndex = 1;
            ComboBox1.Items.Add("Initial item 1");
            ComboBox1.Items.Add("Initial item 2");
            ComboBox1.SelectedIndex = 1;
            ComboBoxNonNative.Items.Add("Initial item 1");
            ComboBoxNonNative.Items.Add("Initial item 2");
            ComboBoxNonNative.SelectedIndex = 1;
            AutoCompleteBox1.Items.Add("Initial item 1");
            AutoCompleteBox1.Items.Add("Initial item 2");
            AutoCompleteBox1.SelectedIndex = 1;
            //DataGrid1.Items.Add(new Cat("Tom", 10));
            //DataGrid1.Items.Add(new Cat("Blacky", 20));

            // Window size:
            WindowWidthTextBlock.Text = Window.Current.Bounds.Width.ToString();
            WindowHeightTextBlock.Text = Window.Current.Bounds.Height.ToString();
            Window.Current.SizeChanged += Window_SizeChanged;

            PrepareICommandTest();
            PrepareStyleTest();

            //Validation:
            Person person = new Person();
            ValidationBorder.DataContext = person;

            //BindingExpression bindingExpression = NameTextBoxForValidation.GetBindingExpression(TextBox.TextProperty);
            //Validation.MarkInvalid(bindingExpression, new ValidationError(bindingExpression) { ErrorContent = "Field cannot be empty.", Exception = new Exception("Field cannot be empty.") });

            //BindingExpression bindingExpression2 = AgeTextBoxForValidation.GetBindingExpression(TextBox.TextProperty);
            //Validation.MarkInvalid(bindingExpression, new ValidationError(bindingExpression) { ErrorContent = "Age cannot be lower than 0.", Exception = new Exception("Age cannot be lower than 0.") });

            //Adding the Behavior to a TextBox through c#:
            Interaction.GetBehaviors(TestBehaviorTextBox).Add(new HintBehavior("Pls type something.", new SolidColorBrush(Colors.Gold)));

        }

        void TestBehaviorButton_Click(object sender, RoutedEventArgs e)
        {
            string firstTextBoxText = "";
            foreach(Behavior behavior in Interaction.GetBehaviors(TestBehaviorTextBox)) //there is only one but eh.
            {
                if(behavior is HintBehavior)
                {
                    if(!((HintBehavior)behavior).IsHintDisplayed)
                    {
                        firstTextBoxText = TestBehaviorTextBox.Text;
                    }
                }
            }

            string secondTextBoxText = "";
            foreach (Behavior behavior in Interaction.GetBehaviors(TestBehaviorTextBox2)) //there is only one but eh.
            {
                if (behavior is HintBehavior)
                {
                    if (!((HintBehavior)behavior).IsHintDisplayed)
                    {
                        secondTextBoxText = TestBehaviorTextBox2.Text;
                    }
                }
            }

            string resultString = string.Format(@"First TextBox text: {0}
Second TextBox text: {1}", firstTextBoxText, secondTextBoxText);
            MessageBox.Show(resultString);
        }

        private void TestGroupBy()
        {
            List<double> list = new List<double>();
            list.Add(2);
            list.Add(3);
            list.Add(4);
            list.Add(5);
            list.Add(6);
            //var v = System.Linq.Enumerable.GroupBy(list, o => o > 3); //works too
            var v = list.GroupBy(o => o > 3);
            foreach (var element in v)
            {
                var w = element.Key;

                StackPanel stackPanel = new StackPanel();
                stackPanel.Orientation = Orientation.Vertical;
                stackPanel.Background = new SolidColorBrush(Colors.LightBlue);
                stackPanel.Margin = new Thickness(2);
                TextBlock t = new TextBlock();
                t.Text = w.ToString();
                t.Margin = new Thickness(2);
                stackPanel.Children.Add(t);
                GroupByStackPanel.Children.Add(stackPanel);

                foreach (var element2 in element)
                {
                    var x = element2;
                    TextBlock t2 = new TextBlock();
                    t2.Text = x.ToString();
                    t2.Margin = new Thickness(2);
                    stackPanel.Children.Add(t2);
                    x -= 1;
                }
            }
        }


        void Window_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            WindowWidthTextBlock.Text = (double.IsNaN(e.Size.Width) ? "NaN" : e.Size.Width.ToString());
            WindowHeightTextBlock.Text = (double.IsNaN(e.Size.Height) ? "NaN" : e.Size.Height.ToString());
        }

        void ButtonSerialize_Click(object sender, RoutedEventArgs e)
        {
            //TestClass testObj = new TestClass();

            //testObj.SomeString = "foo";
            //testObj.Settings.Add("A");
            //testObj.Settings.Add("B");
            //testObj.Settings.Add("C");

            //var xmlSerializer = new XmlSerializer(testObj.GetType());
            //StringWriter textWriter = new StringWriter();
            //xmlSerializer.Serialize(textWriter, testObj);
            //SerializationResult.Text = textWriter.ToString();
        }

        void TestButtonUnderOverlay_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("It works");
        }

        void TestLeftButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Left button clicked.");
        }

        void TestRightButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Right button clicked.");
        }

#region Testing Template on TextBox

        void ButtonTestNormalTextBoxSelectAll_Click(object sender, RoutedEventArgs e)
        {
            SelectAllTestNormalTextBox.SelectAll();
        }

        void ButtonTestTemplatedTextBoxSelectAll_Click(object sender, RoutedEventArgs e)
        {
            SelectAllTestTemplatedTextBox.SelectAll();
        }

#endregion

#region Testing events

#if SLMIGRATION
        void TestButtonParent_Click(object sender, MouseEventArgs e)
#else
        void TestButtonParent_Click(object sender, PointerRoutedEventArgs e)
#endif
        {
            TestButtonParentCount.Text = (int.Parse(TestButtonParentCount.Text) + 1).ToString();
        }

        void TestButtonParent1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Parent 1");
        }

        void TestButtonParent2_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Parent 2");
        }

#if SLMIGRATION
        void TestButton1_Click(object sender, MouseEventArgs e)
#else
        void TestButton1_Click(object sender, PointerRoutedEventArgs e)
#endif
        {
            TestButton1Count.Text = (int.Parse(TestButton1Count.Text) + 1).ToString();
            //e.Handled = true;
        }

#if SLMIGRATION
        void TestButton2_Click(object sender, MouseEventArgs e)
#else
        void TestButton2_Click(object sender, PointerRoutedEventArgs e)
#endif
        {
            TestButton2Count.Text = (int.Parse(TestButton2Count.Text) + 1).ToString();
            e.Handled = true;
        }

        void TestAttachClickButton_Click(object sender, RoutedEventArgs e)
        {
            ClickMeButton.Click += TestClickMeButton_Click;
        }

        void TestDetachClickButton_Click(object sender, RoutedEventArgs e)
        {
            ClickMeButton.Click -= TestClickMeButton_Click;
        }

        void TestClickMeButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Clicked");
        }


#endregion

#region Testing Printing

        void ButtonPrint_SpecificElement_Click(object sender, RoutedEventArgs e)
        {
            CSHTML5.Native.Html.Printing.PrintManager.Print(ElementToPrint);
        }

        void ButtonPrint_ShowDialog_Click(object sender, RoutedEventArgs e)
        {
            CSHTML5.Native.Html.Printing.PrintManager.Print();
        }

        void ButtonPrint_SetPrintArea_Click(object sender, RoutedEventArgs e)
        {
            CSHTML5.Native.Html.Printing.PrintManager.SetPrintArea(ElementToPrint);
        }

        void ButtonPrint_ResetPrintArea_Click(object sender, RoutedEventArgs e)
        {
            CSHTML5.Native.Html.Printing.PrintManager.ResetPrintArea();
        }

        void ButtonPrint_InMemoryElement_Click(object sender, RoutedEventArgs e)
        {
            // Create the element to print (some black text on a yellow page):
            var pageBackground = new StackPanel() { Background = new SolidColorBrush(Colors.Yellow) };
            pageBackground.Children.Add(new TextBlock() { Text = "This is some text to print.", TextWrapping = TextWrapping.Wrap, Foreground = new SolidColorBrush(Colors.Black) }); ;

            // Print it:
            CSHTML5.Native.Html.Printing.PrintManager.Print(pageBackground);
        }

#endregion

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

#region Testing Binding

        class BindingTestClass : INotifyPropertyChanged
        {
            private string _text = "before";
            public string Text
            {
                get { return _text; }
                set { _text = value; OnPropertyChanged("Text"); }
            }


            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged(string propertyName)
            {
                if ((PropertyChanged != null))
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        private void ButtonSetDataContext_Click(object sender, RoutedEventArgs e)
        {
            StackPanelForBinding.DataContext = new BindingTestClass();
        }

        bool isTextBlockInVisualTree = true;
        TextBlock testBindingTextBlock;

        private void ButtonTestBinding_Click(object sender, RoutedEventArgs e)
        {
            if (isTextBlockInVisualTree)
            {
                testBindingTextBlock = TestBindingTextblock;
                StackPanelForBinding.Children.Remove(TestBindingTextblock);
                isTextBlockInVisualTree = false;
            }
            else
            {
                StackPanelForBinding.Children.Add(testBindingTextBlock);
                isTextBlockInVisualTree = true;
            }
        }

        private void ButtonTestBinding2_Click(object sender, RoutedEventArgs e)
        {
            ((BindingTestClass)StackPanelForBinding.DataContext).Text = new Random().Next().ToString();
        }

#endregion

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

#region Testing TextBox control


        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            MessageBox.Show("Text Changed!");
        }

        private void FocusTextBox1_LostFocus(object sender, RoutedEventArgs e)
        {
            FocusTextBlockLog.Text = FocusTextBlockLog.Text + Environment.NewLine + "Lost focus on text box 1";
        }

        private void FocusTextBox2_GotFocus(object sender, RoutedEventArgs e)
        {
            FocusTextBlockLog.Text = FocusTextBlockLog.Text + Environment.NewLine + "Got focus on text box 2";
        }

        private void CheckBoxForWrapping_Checked(object sender, RoutedEventArgs e)
        {
            TextBoxForWrapping.TextWrapping = TextWrapping.Wrap;
        }

        private void CheckBoxForWrapping_Unchecked(object sender, RoutedEventArgs e)
        {
            TextBoxForWrapping.TextWrapping = TextWrapping.NoWrap;
        }

#endregion

#region controls showcase part
        string _lastButtonClicked = string.Empty;

        void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            //((Button)sender).Content = TextBox1.Text;
            //TextBox1.Text = "test";
            //Uri uri = new Uri("C:\\Users\\Sylvain\\Documents\\Adventure Maker v4.7\\Projects\\ASA_game\\Icons\\settings.ico");
            //BitmapImage bmpImage = new BitmapImage(uri);
            //Image1.Source = bmpImage;
            //Image1.Stretch = Stretch.Fill;
        }

        private void StandardControlsButton_Click(object sender, RoutedEventArgs e)
        {
            if (_lastButtonClicked != "StandardControlsButton")
            {
                //we remove the controls that were there before clicking on the button
                ControlsArea.Children.Clear();

                Button button = new Button();
                button.Content = "Button";
                ControlsArea.Children.Add(button);

                _lastButtonClicked = "StandardControlsButton";
            }
        }

        private void PanelsButton_Click(object sender, RoutedEventArgs e)
        {
            if (_lastButtonClicked != "PanelsButton")
            {
                //we remove the controls that were there before clicking on the button
                ControlsArea.Children.Clear();

                TextBlock textBlock = new TextBlock();
                textBlock.Text = "StackPanel:";
                ControlsArea.Children.Add(textBlock);

                StackPanel stackPanel = new StackPanel();
                stackPanel.Orientation = Orientation.Horizontal;

                Border border = new Border();
                Color color = Color.FromArgb(255, 0, 0, 0);
                border.Background = new SolidColorBrush(color);
                stackPanel.Children.Add(border);

                border = new Border();
                color = Color.FromArgb(255, 30, 30, 30);
                border.Background = new SolidColorBrush(color);
                stackPanel.Children.Add(border);

                border = new Border();
                color = Color.FromArgb(255, 100, 100, 100);
                border.Background = new SolidColorBrush(color);
                stackPanel.Children.Add(border);

                border = new Border();
                color = Color.FromArgb(255, 200, 200, 200);
                border.Background = new SolidColorBrush(color);
                stackPanel.Children.Add(border);

                _lastButtonClicked = "PanelsButton";
            }
        }

#endregion

#region testing Frame

        private void ButtonGoBack_Click(object sender, RoutedEventArgs e)
        {
            if (MyFrame.CanGoBack)
            {
                MyFrame.GoBack();
            }
            else
            {
                MessageBox.Show("There is nowhere to go back");
            }
        }

        private void ButtonGoToInnerPage_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/FrameSubPage1", UriKind.Relative);
            MyFrame.Source = uri;
        }

        private void ButtonGoToSubPage_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/FrameSubPage2", UriKind.Relative);
            MyFrame.Source = uri;
        }

        private void ButtonGoForward_Click(object sender, RoutedEventArgs e)
        {
            if (MyFrame.CanGoForward)
            {
                MyFrame.GoForward();
            }
            else
            {
                MessageBox.Show("There is nowhere to go to");
            }
        }


#region frame1

        private void Frame1_GoBack_Clicked(object sender, RoutedEventArgs e)
        {
            if (MyFrame.CanGoBack)
            {
                MyFrame.GoBack();
            }
            else
            {
                MessageBox.Show("There is nowhere to go back");
            }
        }

        private void Frame1_p1_Clicked(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/FrameSubPage1", UriKind.Relative);
            MyFrame.Source = uri;
        }

        private void Frame1_p2_Clicked(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/FrameSubPage2", UriKind.Relative);
            MyFrame.Source = uri;
        }

        private void Frame1_GoForward_Clicked(object sender, RoutedEventArgs e)
        {
            if (MyFrame.CanGoForward)
            {
                MyFrame.GoForward();
            }
            else
            {
                MessageBox.Show("There is nowhere to go to");
            }
        }

#endregion

#region frame2

        private void Frame2_GoBack_Clicked(object sender, RoutedEventArgs e)
        {
            if (MyFrame2.CanGoBack)
            {
                MyFrame2.GoBack();
            }
            else
            {
                MessageBox.Show("There is nowhere to go back");
            }
        }

        private void Frame2_p1_Clicked(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/FrameSubPage1", UriKind.Relative);
            MyFrame2.Source = uri;
        }

        private void Frame2_p2_Clicked(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/FrameSubPage2", UriKind.Relative);
            MyFrame2.Source = uri;
        }

        private void Frame2_GoForward_Clicked(object sender, RoutedEventArgs e)
        {
            if (MyFrame2.CanGoForward)
            {
                MyFrame2.GoForward();
            }
            else
            {
                MessageBox.Show("There is nowhere to go to");
            }
        }

#endregion

#region frame3

        private void Frame3_GoBack_Clicked(object sender, RoutedEventArgs e)
        {
            if (MyFrame3.CanGoBack)
            {
                MyFrame3.GoBack();
            }
            else
            {
                MessageBox.Show("There is nowhere to go back");
            }
        }

        private void Frame3_p1_Clicked(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/FrameSubPage1", UriKind.Relative);
            MyFrame3.Source = uri;
        }

        private void Frame3_p2_Clicked(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/FrameSubPage2", UriKind.Relative);
            MyFrame3.Source = uri;
        }

        private void Frame3_GoForward_Clicked(object sender, RoutedEventArgs e)
        {
            if (MyFrame3.CanGoForward)
            {
                MyFrame3.GoForward();
            }
            else
            {
                MessageBox.Show("There is nowhere to go to");
            }
        }

#endregion

#region inner frame

        private void InnerFrame_GoBack_Clicked(object sender, RoutedEventArgs e)
        {
            Frame frame = (Frame)((Button)sender).DataContext;
            if (frame.CanGoBack)
            {
                frame.GoBack();
            }
            else
            {
                MessageBox.Show("There is nowhere to go back");
            }
        }

        private void InnerFrame_p1_Clicked(object sender, RoutedEventArgs e)
        {
            Frame frame = (Frame)((Button)sender).DataContext;

            Uri uri = new Uri("/FrameSubPage1", UriKind.Relative);
            frame.Source = uri;
        }

        private void InnerFrame_p2_Clicked(object sender, RoutedEventArgs e)
        {
            Frame frame = (Frame)((Button)sender).DataContext;
            Uri uri = new Uri("/FrameSubPage2", UriKind.Relative);
            frame.Source = uri;
        }

        private void InnerFrame_GoForward_Clicked(object sender, RoutedEventArgs e)
        {
            Frame frame = (Frame)((Button)sender).DataContext;
            if (frame.CanGoForward)
            {
                frame.GoForward();
            }
            else
            {
                MessageBox.Show("There is nowhere to go to");
            }
        }

#endregion

#endregion

        void ButtonDeserialize_Click(object sender, RoutedEventArgs e)
        {
            ////var serializedObject = "<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<TestClass xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <SomeString>foo</SomeString>\r\n  <Settings>\r\n    <string>A</string>\r\n    <string>B</string>\r\n    <string>C</string>\r\n  </Settings>\r\n</TestClass>";
            ////var serializedObject = "<?xml version=\"1.0\" ?>\r\n<TestClass xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <SomeString>foo</SomeString>\r\n  <Settings>\r\n    <string>A</string>\r\n    <string>B</string>\r\n    <string>C</string>\r\n  </Settings>\r\n</TestClass>";
            //var serializedObject = "<TestClass xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <SomeString>foo</SomeString>\r\n  <Settings>\r\n    <string>A</string>\r\n    <string>B</string>\r\n    <string>C</string>\r\n  </Settings>\r\n</TestClass>";

            //var xmlSerializer = new XmlSerializer(typeof(TestClass));
            ////StringReader textReader = new StringReader(serializedObject);
            //var stream = GenerateStreamFromString(serializedObject);
            //TestClass testObj = (TestClass)xmlSerializer.Deserialize(stream);
            //SerializationResult.Text = "Deserialization succeeded\r\n" + testObj.SomeString + "\r\n" + testObj.Settings.Count.ToString() + "\r\n" + testObj.Settings[2];
        }

        static Stream GenerateStreamFromString(string s)
        {
            //return new MemoryStream(Encoding.ASCII.GetBytes(s));

            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        private void TextBlock_GotFocus(object sender, RoutedEventArgs e)
        {
            Control s = (Control)sender;
            s.Background = new SolidColorBrush(Colors.Azure);
        }

        private void TextBlock_LostFocus(object sender, RoutedEventArgs e)
        {
            Control s = (Control)sender;
            s.Background = new SolidColorBrush(Colors.Black);
        }

        private void ButtonVisibility_Click(object sender, RoutedEventArgs e)
        {
            if (VisibilityBorder.Visibility == Visibility.Collapsed)
                VisibilityBorder.Visibility = Visibility.Visible;
            else
                VisibilityBorder.Visibility = Visibility.Collapsed;
        }

        private void TestButtonLinq_Click(object sender, RoutedEventArgs e)
        {
            // Testing Linq:
            LinqSamples samples = new LinqSamples();

            samples.Linq1(); // This sample  uses the where clause  to find all elements  of an array with a value 
            // less than 5

            samples.Linq2(); // This sample uses the where clause to find all products that are out of stock

            samples.Linq3(); // This sample uses the where clause to find all products that are in  stock and cost 
            // more than 3.00 per unit

            //samples.Linq4(); // This sample uses the where  clause to find all customers in Washington and then it 
            // uses a foreach loop to iterate over the orders collection that belongs to each 
            // customer

            samples.Linq5(); // This sample demonstrates an indexed where clause that returns digits whose name is 
            // shorter than their value

            samples.Linq6();
            samples.Linq7();
            samples.Linq8();
            samples.Linq9();
            samples.Linq10();
            samples.Linq11();
            samples.Linq12();
            samples.Linq13();
            samples.Linq14();

            samples.Linq54();
            samples.Linq55();
            samples.Linq56();
            samples.Linq57();

            samples.Linq29();
            samples.Linq30();

            samples.Linq31();

            samples.Linq78();
            samples.Linq79();

            samples.LinqCustom1();
        }

        private void ButtonTestDecoder_Click(object sender, RoutedEventArgs e)
        {
            TestEncoder();
        }


        void TestEncoder()
        {
            throw new NotImplementedException();
            /*
            //For personnal knowledge:
            //First byte    < 192 -->   1 byte =    1 character
            //First byte    < 224 -->   2 bytes =   1 character; Second byte must be >= 128
            //First byte    < 240 -->   3 bytes =   1 character; Following bytes must be >= 128
            //First byte    >= 240 -->   4 bytes =   1 character; Following bytes must be >= 128

            string charsNumbers = TestDecoderTextBox.Text;
            string[] splittedCharsNumbers = charsNumbers.Split(',');
            List<Byte> numbers = new List<Byte>();
            foreach (string str in splittedCharsNumbers)
            {
                int i;
                if (int.TryParse(str, out i))
                {
                    if (i < 255)
                    {
                        numbers.Add((Byte)i);
                    }
                }
            }

            Char[] chars;
            Byte[] bytes = numbers.ToArray();
            Decoder utf8Decoder = Encoding.UTF8.GetDecoder();

            int charCount = utf8Decoder.GetCharCount(bytes, 0, bytes.Length);
            chars = new Char[charCount];
            int charsDecodedCount = utf8Decoder.GetChars(bytes, 0, bytes.Length, chars, 0);
            string s = string.Format("{0} characters used to decode bytes.", charsDecodedCount);
            s += Environment.NewLine;
            s += "Decoded chars: ";
            s += Environment.NewLine;

            foreach (Char c in chars)
            {
                s += string.Format("[{0}]", c);
            }
            int charsCountFromGetCharCount = utf8Decoder.GetCharCount(bytes, 0, bytes.Length);
            s += Environment.NewLine;
            s += string.Format("GetCharCount result: {0}", charsCountFromGetCharCount);
            TestDecoderTextBlock.Text = s;
            */
        }

#region testing Grid

        private void TestBugCanvasInGrid_AddCanvas_Click(object sender, RoutedEventArgs e)
        {
            Random rd = new Random();
            TestBugCanvasInGrid.Children.Add(new Rectangle() { Width = 100, Height = 30, Fill = new SolidColorBrush(new Color() { A = (byte)255, B = (byte)rd.Next(256), G = (byte)rd.Next(256), R = (byte)rd.Next(256) }) });
        }
        int i = 0;
        private void TestBugCanvasInGrid_AddText_Click(object sender, RoutedEventArgs e)
        {
            TestBugCanvasInGrid.Children.Add(new TextBlock() { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, Text = "Test " + i });
            ++i;
        }
        private void TestBugCanvasInGrid_Reset_Click(object sender, RoutedEventArgs e)
        {
            TestBugCanvasInGrid.Children.Clear();
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
                        border.Background = new SolidColorBrush(Colors.LightGreen);
                        border.Opacity = 0.5;
                        Grid.SetColumn(border, 1);
                        Grid.SetRow(border, 0);
                        Grid.SetRowSpan(border, 2);
                        break;
                    case 4:
                        border.Background = new SolidColorBrush(Colors.Lime);
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
                        border.Background = new SolidColorBrush(Colors.LightBlue);
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
                        border.Background = new SolidColorBrush(Colors.DarkBlue);
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
                border01.Background = new SolidColorBrush(Colors.LightGreen);
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
                border11.Background = new SolidColorBrush(Colors.Lime);
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
                border02.Background = new SolidColorBrush(Colors.LightBlue);
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
                border22.Background = new SolidColorBrush(Colors.DarkBlue);
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
#endregion

#region test Path changes

        Random rd = new Random();
        private void TestChangePathWidth_Click(object sender, RoutedEventArgs e)
        {
            path1.Width = rd.Next(300);
        }
        private void TestChangePathHeight_Click(object sender, RoutedEventArgs e)
        {
            path1.Height = rd.Next(300);
        }

        private void TestChangePathFill_Click(object sender, RoutedEventArgs e)
        {
            path1.Fill = new SolidColorBrush(new Color() { A = (byte)rd.Next(255), B = (byte)rd.Next(255), G = (byte)rd.Next(255), R = (byte)rd.Next(255) });
        }
        private void TestChangePathStretch_Click(object sender, RoutedEventArgs e)
        {
            int i = rd.Next(4);
            switch (i)
            {
                case 0:
                    path1.Stretch = Stretch.None;
                    break;
                case 1:
                    path1.Stretch = Stretch.Fill;
                    break;
                case 2:
                    path1.Stretch = Stretch.Uniform;
                    break;
                case 3:
                    path1.Stretch = Stretch.UniformToFill;
                    break;
                default:
                    break;
            }
        }
        private void TestChangePathStroke_Click(object sender, RoutedEventArgs e)
        {
            path1.Stroke = new SolidColorBrush(new Color() { A = (byte)rd.Next(255), B = (byte)rd.Next(255), G = (byte)rd.Next(255), R = (byte)rd.Next(255) });
        }
        private void TestChangePathStrokeThickness_Click(object sender, RoutedEventArgs e)
        {
            path1.StrokeThickness = rd.Next(7);
        }

        private void RedrawPath_Click(object sender, RoutedEventArgs e)
        {
            path1.Refresh();
        }

#region EllipseGeometry
        private void TestEllipseGeometry_Click(object sender, RoutedEventArgs e)
        {
            path1.Data = new EllipseGeometry() { Center = new Point(100, 100), RadiusX = 100, RadiusY = 50 };
            EllipseGeometryButtons.Visibility = Visibility.Visible;
            LineGeometryButtons.Visibility = Visibility.Collapsed;
            PathGeometryButtons.Visibility = Visibility.Collapsed;
            PathGeometrySegmentTypeButtons.Visibility = Visibility.Collapsed;

            PathArcButtons.Visibility = Visibility.Collapsed;
        PathBezierButtons.Visibility = Visibility.Collapsed;
        PathLineButtons.Visibility = Visibility.Collapsed;
        PathPolyBezierButtons.Visibility = Visibility.Collapsed;
        PathPolyLineButtons.Visibility = Visibility.Collapsed;
        PathPolyQuadraticButtons.Visibility = Visibility.Collapsed;
        PathQuadraticButtons.Visibility = Visibility.Collapsed;

        }

        private void TestEllipseCenter_Click(object sender, RoutedEventArgs e)
        {
            EllipseGeometry ellipse = (EllipseGeometry)path1.Data;
            ellipse.Center = new Point(rd.Next(250), rd.Next(150));
        }
        private void TestEllipseRadiusX_Click(object sender, RoutedEventArgs e)
        {
            EllipseGeometry ellipse = (EllipseGeometry)path1.Data;
            ellipse.RadiusX = rd.Next(200);
        }
        private void TestEllipseRadiusY_Click(object sender, RoutedEventArgs e)
        {
            EllipseGeometry ellipse = (EllipseGeometry)path1.Data;
            ellipse.RadiusY = rd.Next(150);
        }
#endregion

#region LineGeometry
        private void TestLineGeometry_Click(object sender, RoutedEventArgs e)
        {
            path1.Data = new LineGeometry() { StartPoint = new Point(10, 10), EndPoint = new Point(200, 150) };
            EllipseGeometryButtons.Visibility = Visibility.Collapsed;
            LineGeometryButtons.Visibility = Visibility.Visible;
            PathGeometryButtons.Visibility = Visibility.Collapsed;
            PathGeometrySegmentTypeButtons.Visibility = Visibility.Collapsed;

            PathArcButtons.Visibility = Visibility.Collapsed;
            PathBezierButtons.Visibility = Visibility.Collapsed;
            PathLineButtons.Visibility = Visibility.Collapsed;
            PathPolyBezierButtons.Visibility = Visibility.Collapsed;
            PathPolyLineButtons.Visibility = Visibility.Collapsed;
            PathPolyQuadraticButtons.Visibility = Visibility.Collapsed;
            PathQuadraticButtons.Visibility = Visibility.Collapsed;

        }
        private void TestLineStartPoint_Click(object sender, RoutedEventArgs e)
        {
            LineGeometry line = (LineGeometry)path1.Data;
            line.StartPoint = new Point(rd.Next(290), rd.Next(190));
        }
        private void TestLineEndPoint_Click(object sender, RoutedEventArgs e)
        {
            LineGeometry line = (LineGeometry)path1.Data;
            line.EndPoint = new Point(rd.Next(290), rd.Next(190));
        }
#endregion

#region PathGeometry
        ArcSegment arc;
        BezierSegment bezier;
        LineSegment line;
        PolyBezierSegment polyBezier;
        PolyLineSegment polyLine;
        PolyQuadraticBezierSegment polyQuadratic;
        QuadraticBezierSegment quadratic;


#region generic pathGeometry and PathFigure stuff
        private void TestPathGeometry_Click(object sender, RoutedEventArgs e)
        {
            //PathGeometry pathGeometry = new PathGeometry();
            //PathFigureCollection figures = new PathFigureCollection();
            //PathFigure figure = new PathFigure() { IsClosed = false, IsFilled = false, StartPoint = new Point(10, 10) };
            //figure.Segments = new PathSegmentCollection();

            //arc = new ArcSegment() { IsLargeArc = false, Point = new Point(25, 50), RotationAngle = 35, Size = new Size(100, 25), SweepDirection = SweepDirection.Clockwise };
            //figure.Segments.Add(arc);

            //bezier = new BezierSegment() { Point1 = new Point(100, 0), Point2 = new Point(150, 25), Point3 = new Point(10, 150) };
            //figure.Segments.Add(bezier);

            //line = new LineSegment() { Point = new Point(250,150) };
            //figure.Segments.Add(line);

            //polyBezier = new PolyBezierSegment();
            //PointCollection points = new PointCollection();
            //points.Add(new Point(250, 0));
            //points.Add(new Point(250, 0));
            //points.Add(new Point(150, 200));
            //points.Add(new Point(100, 250));
            //points.Add(new Point(300, 0));
            //points.Add(new Point(50, 100));
            //polyBezier.Points = points;
            //figure.Segments.Add(polyBezier);

            //polyLine = new PolyLineSegment();
            //polyLine.Points = new PointCollection();
            //polyLine.Points.Add(new Point(50, 200));
            //polyLine.Points.Add(new Point(100, 200));
            //figure.Segments.Add(polyLine);

            //polyQuadratic = new PolyQuadraticBezierSegment();
            //polyQuadratic.Points = new PointCollection();
            //polyQuadratic.Points.Add(new Point(50, 100));
            //polyQuadratic.Points.Add(new Point(150, 200));
            //polyQuadratic.Points.Add(new Point(250, 200));
            //polyQuadratic.Points.Add(new Point(150, 100));
            //figure.Segments.Add(polyQuadratic);


            //quadratic = new QuadraticBezierSegment() { Point1 = new Point(0, 0), Point2 = new Point(200, 200) };
            //figure.Segments.Add(quadratic);

            //figures.Add(figure);
            //path1.Data = pathGeometry;
            EllipseGeometryButtons.Visibility = Visibility.Collapsed;
            LineGeometryButtons.Visibility = Visibility.Collapsed;
            PathGeometryButtons.Visibility = Visibility.Visible;
            PathGeometrySegmentTypeButtons.Visibility = Visibility.Visible;
        }
        private void TestPathFillRule_Click(object sender, RoutedEventArgs e)
        {
            PathGeometry pathGeometry = (PathGeometry)path1.Data;
            if (pathGeometry.FillRule == FillRule.EvenOdd)
            {
                pathGeometry.FillRule = FillRule.Nonzero;
            }
            else
            {
                pathGeometry.FillRule = FillRule.EvenOdd;
            }
        }
        private void TestPathIsClosed_Click(object sender, RoutedEventArgs e)
        {
            PathGeometry pathGeometry = (PathGeometry)path1.Data;
            if (pathGeometry.Figures != null && pathGeometry.Figures.Count >= 1)
            {
                PathFigure pathFigure = pathGeometry.Figures[0];
                pathFigure.IsClosed = !pathFigure.IsClosed;
            }
        }
        private void TestPathIsFilled_Click(object sender, RoutedEventArgs e)
        {
            PathGeometry pathGeometry = (PathGeometry)path1.Data;
            if (pathGeometry.Figures != null && pathGeometry.Figures.Count >= 1)
            {
                PathFigure pathFigure = pathGeometry.Figures[0];
                pathFigure.IsFilled = !pathFigure.IsFilled;
            }
        }
        private void TestPathStartPoint_Click(object sender, RoutedEventArgs e)
        {
            PathGeometry pathGeometry = (PathGeometry)path1.Data;
            if (pathGeometry.Figures != null && pathGeometry.Figures.Count >= 1)
            {
                PathFigure pathFigure = pathGeometry.Figures[0];
                pathFigure.StartPoint = new Point(rd.Next(290), rd.Next(190));
            }
        }
#endregion

#region ArcSegment
        private void TestPathArc_Click(object sender, RoutedEventArgs e)
        {
            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.Figures = new PathFigureCollection();
            PathFigure figure = new PathFigure() { IsClosed = false, IsFilled = false, StartPoint = new Point(10, 10) };
            figure.Segments = new PathSegmentCollection();

            arc = new ArcSegment() { IsLargeArc = false, Point = new Point(25, 50), RotationAngle = 35, Size = new Size(100, 25), SweepDirection = SweepDirection.Clockwise };
            figure.Segments.Add(arc);

            pathGeometry.Figures.Add(figure);
            path1.Data = pathGeometry;

            PathArcButtons.Visibility = Visibility.Visible;
            PathBezierButtons.Visibility = Visibility.Collapsed;
            PathLineButtons.Visibility = Visibility.Collapsed;
            PathPolyBezierButtons.Visibility = Visibility.Collapsed;
            PathPolyLineButtons.Visibility = Visibility.Collapsed;
            PathPolyQuadraticButtons.Visibility = Visibility.Collapsed;
            PathQuadraticButtons.Visibility = Visibility.Collapsed;
        }
        private void TestArcSegmentIsLargeArc_Click(object sender, RoutedEventArgs e)
        {
            arc.IsLargeArc = !arc.IsLargeArc;
        }
        private void TestArcSegmentPoint_Click(object sender, RoutedEventArgs e)
        {
            arc.Point = new Point(rd.Next(290), rd.Next(190));
        }
        private void TestArcSegmentRotationAngle_Click(object sender, RoutedEventArgs e)
        {
            arc.RotationAngle = (rd.NextDouble() - 0.5) * 720;
        }
        private void TestArcSegmentSize_Click(object sender, RoutedEventArgs e)
        {
            arc.Size = new Size(rd.Next(290), rd.Next(190));
        }
        private void TestArcSegmentSweepDirection_Click(object sender, RoutedEventArgs e)
        {
            if (arc.SweepDirection == SweepDirection.Clockwise)
            {
                arc.SweepDirection = SweepDirection.Counterclockwise;
            }
            else
            {
                arc.SweepDirection = SweepDirection.Clockwise;
            }
        }
#endregion

#region BezierSegment
        private void TestPathBezier_Click(object sender, RoutedEventArgs e)
        {
            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.Figures = new PathFigureCollection();
            PathFigure figure = new PathFigure() { IsClosed = false, IsFilled = false, StartPoint = new Point(10, 10) };
            figure.Segments = new PathSegmentCollection();

            bezier = new BezierSegment() { Point1 = new Point(100, 0), Point2 = new Point(150, 25), Point3 = new Point(10, 150) };
            figure.Segments.Add(bezier);

            pathGeometry.Figures.Add(figure);
            path1.Data = pathGeometry;

            PathArcButtons.Visibility = Visibility.Collapsed;
            PathBezierButtons.Visibility = Visibility.Visible;
            PathLineButtons.Visibility = Visibility.Collapsed;
            PathPolyBezierButtons.Visibility = Visibility.Collapsed;
            PathPolyLineButtons.Visibility = Visibility.Collapsed;
            PathPolyQuadraticButtons.Visibility = Visibility.Collapsed;
            PathQuadraticButtons.Visibility = Visibility.Collapsed;
        }
        private void TestBezierPoint1_Click(object sender, RoutedEventArgs e)
        {
            bezier.Point1 = new Point(rd.Next(290), rd.Next(190));
        }
        private void TestBezierPoint2_Click(object sender, RoutedEventArgs e)
        {
            bezier.Point2 = new Point(rd.Next(290), rd.Next(190));
        }
        private void TestBezierPoint3_Click(object sender, RoutedEventArgs e)
        {
            bezier.Point3 = new Point(rd.Next(290), rd.Next(190));
        }
#endregion

#region LineSegment
        private void TestPathLine_Click(object sender, RoutedEventArgs e)
        {
            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.Figures = new PathFigureCollection();
            PathFigure figure = new PathFigure() { IsClosed = false, IsFilled = false, StartPoint = new Point(10, 10) };
            figure.Segments = new PathSegmentCollection();

            line = new LineSegment() { Point = new Point(250, 150) };
            figure.Segments.Add(line);

            pathGeometry.Figures.Add(figure);
            path1.Data = pathGeometry;

            PathArcButtons.Visibility = Visibility.Collapsed;
            PathBezierButtons.Visibility = Visibility.Collapsed;
            PathLineButtons.Visibility = Visibility.Visible;
            PathPolyBezierButtons.Visibility = Visibility.Collapsed;
            PathPolyLineButtons.Visibility = Visibility.Collapsed;
            PathPolyQuadraticButtons.Visibility = Visibility.Collapsed;
            PathQuadraticButtons.Visibility = Visibility.Collapsed;
        }
        private void TestLinePoint_Click(object sender, RoutedEventArgs e)
        {
            line.Point = new Point(rd.Next(290), rd.Next(190));
        }
#endregion

#region PolyBezierSegment
        private void TestPathPolyBezier_Click(object sender, RoutedEventArgs e)
        {
            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.Figures = new PathFigureCollection();
            PathFigure figure = new PathFigure() { IsClosed = false, IsFilled = false, StartPoint = new Point(10, 10) };
            figure.Segments = new PathSegmentCollection();

            polyBezier = new PolyBezierSegment();
            PointCollection points = new PointCollection();
            points.Add(new Point(250, 0));
            points.Add(new Point(250, 0));
            points.Add(new Point(150, 200));
            points.Add(new Point(100, 250));
            points.Add(new Point(300, 0));
            points.Add(new Point(50, 100));
            polyBezier.Points = points;
            figure.Segments.Add(polyBezier);

            pathGeometry.Figures.Add(figure);
            path1.Data = pathGeometry;

            PathArcButtons.Visibility = Visibility.Collapsed;
            PathBezierButtons.Visibility = Visibility.Collapsed;
            PathLineButtons.Visibility = Visibility.Collapsed;
            PathPolyBezierButtons.Visibility = Visibility.Visible;
            PathPolyLineButtons.Visibility = Visibility.Collapsed;
            PathPolyQuadraticButtons.Visibility = Visibility.Collapsed;
            PathQuadraticButtons.Visibility = Visibility.Collapsed;
        }
        private void TestPolyBezierSegmentPoints_Click(object sender, RoutedEventArgs e)
        {
            PointCollection collection = new PointCollection();
            int amountOfPoints = rd.Next(6);
            while (amountOfPoints > 0)
            {
                collection.Add(new Point(rd.Next(290), rd.Next(190)));
                collection.Add(new Point(rd.Next(290), rd.Next(190)));
                collection.Add(new Point(rd.Next(290), rd.Next(190)));
                --amountOfPoints;
            }
            polyBezier.Points = collection;
        }
#endregion

#region PolyLineSegment
        private void TestPathPolyLine_Click(object sender, RoutedEventArgs e)
        {
            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.Figures = new PathFigureCollection();
            PathFigure figure = new PathFigure() { IsClosed = false, IsFilled = false, StartPoint = new Point(10, 10) };
            figure.Segments = new PathSegmentCollection();

            polyLine = new PolyLineSegment();
            polyLine.Points = new PointCollection();
            polyLine.Points.Add(new Point(50, 200));
            polyLine.Points.Add(new Point(100, 200));
            figure.Segments.Add(polyLine);

            pathGeometry.Figures.Add(figure);
            path1.Data = pathGeometry;

            PathArcButtons.Visibility = Visibility.Collapsed;
            PathBezierButtons.Visibility = Visibility.Collapsed;
            PathLineButtons.Visibility = Visibility.Collapsed;
            PathPolyBezierButtons.Visibility = Visibility.Collapsed;
            PathPolyLineButtons.Visibility = Visibility.Visible;
            PathPolyQuadraticButtons.Visibility = Visibility.Collapsed;
            PathQuadraticButtons.Visibility = Visibility.Collapsed;
        }
        private void TestPolyLineSegmentPoints_Click(object sender, RoutedEventArgs e)
        {
            PointCollection collection = new PointCollection();
            int amountOfPoints = rd.Next(6);
            while (amountOfPoints > 0)
            {
                collection.Add(new Point(rd.Next(290), rd.Next(190)));
                --amountOfPoints;
            }
            polyLine.Points = collection;
        }
#endregion

#region PolyQuadraticBezierSegment
        private void TestPathPolyQuadratic_Click(object sender, RoutedEventArgs e)
        {
            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.Figures = new PathFigureCollection();
            PathFigure figure = new PathFigure() { IsClosed = false, IsFilled = false, StartPoint = new Point(10, 10) };
            figure.Segments = new PathSegmentCollection();

            polyQuadratic = new PolyQuadraticBezierSegment();
            polyQuadratic.Points = new PointCollection();
            polyQuadratic.Points.Add(new Point(50, 100));
            polyQuadratic.Points.Add(new Point(150, 200));
            polyQuadratic.Points.Add(new Point(250, 200));
            polyQuadratic.Points.Add(new Point(150, 100));
            figure.Segments.Add(polyQuadratic);

            pathGeometry.Figures.Add(figure);
            path1.Data = pathGeometry;

            PathArcButtons.Visibility = Visibility.Collapsed;
            PathBezierButtons.Visibility = Visibility.Collapsed;
            PathLineButtons.Visibility = Visibility.Collapsed;
            PathPolyBezierButtons.Visibility = Visibility.Collapsed;
            PathPolyLineButtons.Visibility = Visibility.Collapsed;
            PathPolyQuadraticButtons.Visibility = Visibility.Visible;
            PathQuadraticButtons.Visibility = Visibility.Collapsed;
        }

        private void TestPolyQuadraticBezierSegmentPoints_Click(object sender, RoutedEventArgs e)
        {
            PointCollection collection = new PointCollection();
            int amountOfPoints = rd.Next(6);
            while (amountOfPoints > 0)
            {
                collection.Add(new Point(rd.Next(290), rd.Next(190)));
                collection.Add(new Point(rd.Next(290), rd.Next(190)));
                --amountOfPoints;
            }
            polyQuadratic.Points = collection;
        }
#endregion

#region QuadraticBezierSegment
        private void TestPathQuadratic_Click(object sender, RoutedEventArgs e)
        {
            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.Figures = new PathFigureCollection();
            PathFigure figure = new PathFigure() { IsClosed = false, IsFilled = false, StartPoint = new Point(10, 10) };
            figure.Segments = new PathSegmentCollection();

            quadratic = new QuadraticBezierSegment() { Point1 = new Point(0, 0), Point2 = new Point(200, 200) };
            figure.Segments.Add(quadratic);

            pathGeometry.Figures.Add(figure);
            path1.Data = pathGeometry;

            PathArcButtons.Visibility = Visibility.Collapsed;
            PathBezierButtons.Visibility = Visibility.Collapsed;
            PathLineButtons.Visibility = Visibility.Collapsed;
            PathPolyBezierButtons.Visibility = Visibility.Collapsed;
            PathPolyLineButtons.Visibility = Visibility.Collapsed;
            PathPolyQuadraticButtons.Visibility = Visibility.Collapsed;
            PathQuadraticButtons.Visibility = Visibility.Visible;
        }

        private void TestQuadraticBezierPoint1_Click(object sender, RoutedEventArgs e)
        {
            quadratic.Point1 = new Point(rd.Next(290), rd.Next(190));
        }
        private void TestQuadraticBezierPoint2_Click(object sender, RoutedEventArgs e)
        {
            quadratic.Point2 = new Point(rd.Next(290), rd.Next(190));
        }

#endregion
#endregion

#endregion

#region isolated storage related tests

        private void ButtonSaveToIsolatedStorage_Click(object sender, RoutedEventArgs e)
        {
            string path = TextBoxWithIsolatedStorageFilePath.Text;
            FileSystemHelpers.WriteTextToFile(path, TextBoxWithNewTextForIsolatedStorage.Text);
        }

        private void ButtonLoadFromIsolatedStorage_Click(object sender, RoutedEventArgs e)
        {
            string path = TextBoxWithIsolatedStorageFilePath.Text;
            TextBlockWithLoadedText.Text = FileSystemHelpers.ReadTextFromFile(path);
        }

        private void ButtonDeleteFromIsolatedStorage_Click(object sender, RoutedEventArgs e)
        {
            string path = TextBoxWithIsolatedStorageFilePath.Text;
            FileSystemHelpers.DeleteFile(path);
        }

        private void ButtonSaveToIsolatedStorageSettings_Click(object sender, RoutedEventArgs e)
        {
            string key = TextBoxWithIsolatedStorageSettingsKey.Text;
            FileSystemHelpers.WriteTextToSettings(key, TextBoxWithIsolatedStorageSettingsValue.Text);
        }
        private void ButtonSaveToIsolatedStorageSettingsWithAdd_Click(object sender, RoutedEventArgs e)
        {
            string key = TextBoxWithIsolatedStorageSettingsKey.Text;
            IsolatedStorageSettings.ApplicationSettings.Add(key, TextBoxWithIsolatedStorageSettingsValue.Text);
        }


        private void ButtonRemoveFromIsolatedStorageSettings_Click(object sender, RoutedEventArgs e)
        {
            string key = TextBoxWithIsolatedStorageSettingsKey.Text;
            IsolatedStorageSettings.ApplicationSettings.Remove(key);
        }

        private void ButtonLoadFromIsolatedStorageSettings_Click(object sender, RoutedEventArgs e)
        {
            string key = TextBoxWithIsolatedStorageSettingsKey.Text;
            TextBlockWithIsolatedStorageSettingsLoadedText.Text = FileSystemHelpers.ReadTextFromSettings(key);
            TextBlockWithIsolatedStorageSettingsElementsCount.Text = IsolatedStorageSettings.ApplicationSettings.Count.ToString();
            string temp = "";
            foreach (var pair in IsolatedStorageSettings.ApplicationSettings)
            {
                temp += "{" + pair.Key + "," + pair.Value + "},";
            }
            TextBlockWithIsolatedStorageSettingsElements.Text = temp;
        }

        private void ButtonLoadFromIsolatedStorageUsingTryGetValue_Click(object sender, RoutedEventArgs e)
        {
            string key = TextBoxWithIsolatedStorageSettingsKey.Text;
            string value = string.Empty;
            IsolatedStorageSettings.ApplicationSettings.TryGetValue(key, out value);
            TextBlockWithIsolatedStorageSettingsLoadedText.Text = value;
            TextBlockWithIsolatedStorageSettingsElementsCount.Text = IsolatedStorageSettings.ApplicationSettings.Count.ToString();
            string temp = "";
            foreach (var pair in IsolatedStorageSettings.ApplicationSettings)
            {
                temp += "{" + pair.Key + "," + pair.Value + "},";
            }
            TextBlockWithIsolatedStorageSettingsElements.Text = temp;
        }

        public static class FileSystemHelpers
        {
            public static void WriteTextToFile(string fileName, string fileContent)
            {
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForAssembly())
                {
                    IsolatedStorageFileStream fs = null;
                    using (fs = storage.CreateFile(fileName))
                    {
                        if (fs != null)
                        {
                            //using (StreamWriter sw = new StreamWriter(fs))
                            //{
                            //    sw.Write(fileContent);
                            //}
                            Encoding encoding = new UTF8Encoding();
                            byte[] bytes = encoding.GetBytes(fileContent);
                            fs.Write(bytes, 0, bytes.Length);
                            fs.Close();
                        }
                    }
                }
            }

            public static void DeleteFile(string fileName)
            {
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForAssembly())
                {
                    storage.DeleteFile(fileName);
                }
            }

            public static string ReadTextFromFile(string fileName)
            {
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForAssembly())
                {
                    if (storage.FileExists(fileName))
                    {
                        using (IsolatedStorageFileStream fs = storage.OpenFile(fileName, System.IO.FileMode.Open))
                        {
                            if (fs != null)
                            {
                                using (StreamReader sr = new StreamReader(fs))
                                {
                                    return sr.ReadToEnd();
                                }

                                //byte[] saveBytes = new byte[4];
                                //int count = fs.Read(saveBytes, 0, 4);
                                //if (count > 0)
                                //{
                                //    number = System.BitConverter.ToInt32(saveBytes, 0);
                                //}
                            }
                        }
                    }
                }
                return null;
            }

            public static void WriteTextToSettings(string key, string value)
            {
                IsolatedStorageSettings.ApplicationSettings[key] = value;
            }

            public static string ReadTextFromSettings(string key)
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(key))
                {
                    object value = IsolatedStorageSettings.ApplicationSettings[key];
                    if (value is string)
                    {
                        return (string)value;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }


        }



#endregion

#region FileInfo
        private void Button_FileInfo_SetFileContent_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
            /*
            string inputString = InputTextBox.Text;
            string inputFile = FileNameTextBox.Text;

            if (!string.IsNullOrWhiteSpace(inputString) && !string.IsNullOrWhiteSpace(inputFile))
            {
                FileInfo fileInfo = new FileInfo(inputFile);
                using (FileStream fs = fileInfo.OpenWrite())
                {
                    Encoding encoding = new UTF8Encoding();
                    byte[] bytes = encoding.GetBytes(inputString);
                    fs.Write(bytes, 0, bytes.Length);
                    fs.Close();
                }
                ResultTextBlock.Text = "Done.";
            }
            else
            {
                ResultTextBlock.Text = "Error: Both the file name and the input string must be set to set the file's content.";
            }
            */
        }

        private void Button_FileInfo_GetFileContent_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
            /*
            string inputFile = FileNameTextBox.Text;
            if (!string.IsNullOrWhiteSpace(inputFile))
            {
                FileInfo fileInfo = new FileInfo(inputFile);
                if (fileInfo.Exists)
                {
                    using (FileStream fs = fileInfo.OpenRead())
                    {
                        if (fs != null)
                        {
                            using (StreamReader sr = new StreamReader(fs))
                            {
                                ResultTextBlock.Text = sr.ReadToEnd();
                            }
                        }
                    }
                }
            }
            else
            {
                ResultTextBlock.Text = "Error: The file name must be set to read the file's content.";
            }
            */
        }

        private void Button_FileInfo_RemoveFile_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
            /*
            string inputFile = FileNameTextBox.Text;
            if (!string.IsNullOrWhiteSpace(inputFile))
            {
                FileInfo fileInfo = new FileInfo(inputFile);
                fileInfo.Delete();
                ResultTextBlock.Text = "File deleted.";
            }
            else
            {
                ResultTextBlock.Text = "Error: The file name must be set to delete it.";
            }
            */
        }
#endregion

#region DataGrid

        ObservableCollection<Cat> _cats = new ObservableCollection<Cat>();
        ObservableCollection<Cat> _catsForFirstDataGrid = new ObservableCollection<Cat>();
        ObservableCollection<Cat> _catsForSecondDataGrid = new ObservableCollection<Cat>();
        DataGrid _fourthDataGrid;
        DataGrid _thirdDataGrid;

        private void ButtonClearDataGrid_Click(object sender, RoutedEventArgs e)
        {
            if (_catsForFirstDataGrid != null)
                _catsForFirstDataGrid.Clear();
            if (_catsForSecondDataGrid != null)
                _catsForSecondDataGrid.Clear();
            if (_thirdDataGrid != null && _thirdDataGrid.Items != null)
                _thirdDataGrid.Items.Clear();
            if (_fourthDataGrid != null && _fourthDataGrid.Items != null)
                _fourthDataGrid.Items.Clear();
        }

        private void ButtonTestDataGrid_Click(object sender, RoutedEventArgs e)
        {
            //we clear the lists of cats in case we click multiple times on the button:
            _catsForFirstDataGrid.Clear();
            _catsForSecondDataGrid.Clear();

            //initial dataGrid
            InitialDataGrid.ItemsSource = _cats;

            //dataGrid for Column's Visibility
            if (DataGridForColumnVisibility.ItemsSource == null)
            {
                DataGridForColumnVisibility.ItemsSource = _cats;
            }

            //First DataGrid: ItemsSource set before adding the DataGrid to the visual tree then we add a cat.
            foreach (Cat cat in _cats)
            {
                _catsForFirstDataGrid.Add(cat);
            }
            DataGrid dataGrid = new DataGrid();
            Style style = new Style(typeof(DataGridColumnHeader));
            Setter setter = new Setter(DataGrid.ForegroundProperty, new SolidColorBrush(Colors.Orange));
            style.Setters.Add(setter);
            dataGrid.ColumnHeaderStyle = style;
            dataGrid.ItemsSource = _catsForFirstDataGrid;
            dataGrid.SelectionMode = DataGridSelectionMode.Single;
            dataGrid.Loaded += FirstDataGrid_Loaded;
            DataGridItemsSourceBeforeAddingThenAddingCatContainer.Child = dataGrid;

            //Second DataGrid: ItemsSource set after the DataGrid.Loaded event then we remove a cat.
            dataGrid = new DataGrid();
            style = new Style(typeof(DataGridColumnHeader));
            setter = new Setter(DataGrid.ForegroundProperty, new SolidColorBrush(Colors.Orange));
            style.Setters.Add(setter);
            dataGrid.ColumnHeaderStyle = style;
            dataGrid.SelectionMode = DataGridSelectionMode.Single;
            dataGrid.Loaded += SecondDataGrid_Loaded;
            DataGridItemsSourceAfterLoadedThenRemovingCatContainer.Child = dataGrid;

            //Third DataGrid: Items set before adding the DataGrid to the visual tree then we add a cat.
            _thirdDataGrid = new DataGrid();
            style = new Style(typeof(DataGridColumnHeader));
            setter = new Setter(DataGrid.ForegroundProperty, new SolidColorBrush(Colors.Orange));
            style.Setters.Add(setter);
            _thirdDataGrid.ColumnHeaderStyle = style;
            foreach (Cat cat in _cats)
            {
                _thirdDataGrid.Items.Add(cat);
            }

            _thirdDataGrid.SelectionMode = DataGridSelectionMode.Single;
            _thirdDataGrid.Loaded += ThirdDataGrid_Loaded;
            DataGridItemsBeforeAddingThenAddingCatContainer.Child = _thirdDataGrid;

            //Fourth DataGrid: Items set after the DataGrid.Loaded event to the visual tree then we remove a cat
            _fourthDataGrid = new DataGrid();
            style = new Style(typeof(DataGridColumnHeader));
            setter = new Setter(DataGrid.ForegroundProperty, new SolidColorBrush(Colors.Orange));
            style.Setters.Add(setter);
            _fourthDataGrid.ColumnHeaderStyle = style;
            _fourthDataGrid.SelectionMode = DataGridSelectionMode.Single;
            _fourthDataGrid.Loaded += FourthDataGrid_Loaded;
            DataGridItemsAfterLoadedThenRemovingCatContainer.Child = _fourthDataGrid;
        }

        void FirstDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            _catsForFirstDataGrid.Add(new Cat("Bob", 3));
        }

        void SecondDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (Cat cat in _cats)
            {
                _catsForSecondDataGrid.Add(cat);
            }
            DataGrid dataGrid = (DataGrid)sender;
            dataGrid.ItemsSource = _catsForSecondDataGrid;
            string catsAsString = "";
            foreach (Cat cat in _catsForSecondDataGrid)
            {
                catsAsString += ", " + cat.Name;
            }
            DispatcherTimer t = new DispatcherTimer();
            t.Interval = new TimeSpan(0, 0, 0, 0, 500);
            t.Tick += timerForTestOnDataGrid_Tick;
            t.Start();
        }

        void timerForTestOnDataGrid_Tick(object sender, object e)
        {
            DispatcherTimer t = (DispatcherTimer)sender;
            t.Stop();
            string catsAsString = "";
            foreach (Cat cat in _catsForSecondDataGrid)
            {
                catsAsString += ", " + cat.Name;
            }
            _catsForSecondDataGrid.RemoveAt(1);
            catsAsString = "";
            foreach (Cat cat in _catsForSecondDataGrid)
            {
                catsAsString += ", " + cat.Name;
            }
        }

        void ThirdDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)sender;
            dataGrid.Items.Add(new Cat("Fluffy", 1));
        }
        void FourthDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)sender;
            foreach (Cat cat in _cats)
            {
                dataGrid.Items.Add(cat);
            }
            DispatcherTimer t = new DispatcherTimer();
            t.Interval = new TimeSpan(0, 0, 0, 0, 500);
            t.Tick += timerForTestOnDataGrid2_Tick;
            t.Start();
        }

        void timerForTestOnDataGrid2_Tick(object sender, object e)
        {
            DispatcherTimer t = (DispatcherTimer)sender;
            t.Stop();
            _fourthDataGrid.Items.RemoveAt(0);
        }

        int visibilityState = 0;
        private void ButtonTestDataGridColumnVisibility_Click(object sender, RoutedEventArgs e)
        {
            if (visibilityState == 0)
            {
                NameDataGridColumn.Visibility = Visibility.Collapsed;
                visibilityState = 1;
            }
            else if (visibilityState == 1)
            {
                AgeDataGridColumn.Visibility = Visibility.Collapsed;
                visibilityState = 2;
            }
            else if (visibilityState == 2)
            {
                NameDataGridColumn.Visibility = Visibility.Visible;
                visibilityState = 3;
            }
            else if (visibilityState == 3)
            {
                AgeDataGridColumn.Visibility = Visibility.Visible;
                visibilityState = 0;
            }
        }

        public class Cat
        {
            public Cat(string name, int age)
            {
                Name = name;
                Age = age;
            }

            public string Name { get; set; }
            public int Age { get; set; }

        }

#endregion

#region ICommand

        ICommand _myICommand;

        private void PrepareICommandTest()
        {
            List<string> items = new List<string>();
            items.Add("MessageBox Yay!");
            items.Add("TextBox Boo!");
            items.Add("MessageBox Wow!");
            MyComboBoxForICommand.ItemsSource = items;
            MyComboBoxForICommand.SelectedIndex = 0;
            items = new List<string>();
            items.Add("Display in TextBlock");
            items.Add("Display in MessageBox");
            MyComboBoxForCommandTest.ItemsSource = items;
            MyComboBoxForCommandTest.SelectedIndex = 0;
            MyButtonForTestCommand.Command = new TestCommandInTextBlock(MessageTextBlock);
        }

        private void ButtonTestICommand_Click(object sender, RoutedEventArgs e)
        {
            if (_myICommand != null && _myICommand.CanExecute(MessageTextTextBox))
            {
                _myICommand.Execute(MessageTextTextBox);
            }
        }

        private void ComboBoxForCommandTest_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            switch (comboBox.SelectedIndex)
            {
                case 0:
                    MyButtonForTestCommand.Command = new TestCommandInTextBlock(MessageTextBlock);
                    break;
                case 1:
                default:
                    MyButtonForTestCommand.Command = new TestCommandInMessageBox();
                    break;
            }
        }


        private void MyComboBoxForICommand_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            switch (comboBox.SelectedIndex)
            {
                case 0:
                    _myICommand = new TestICommandClass();
                    break;
                case 1:
                    _myICommand = new TestICommandClass2();
                    break;
                case 2:
                    _myICommand = new TestICommandClass3();
                    break;
                default:
                    _myICommand = new TestICommandClass();
                    break;
            }
        }
        public class TestICommandClass : ICommand
        {
            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                MessageBox.Show("Yay!");
            }
        }

        public class TestICommandClass2 : ICommand
        {
            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                return parameter is TextBox;
            }

            public void Execute(object parameter)
            {
                if (parameter is TextBox)
                {
                    ((TextBox)parameter).Text = "Boo!";
                }
            }
        }

        public class TestICommandClass3 : ICommand
        {
            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                MessageBox.Show("Wow!");
            }
        }

        public class TestCommandInTextBlock : ICommand
        {
            TextBlock _messageTextTextBlock;

            public TestCommandInTextBlock(TextBlock messageTextTextBlock)
            {
                _messageTextTextBlock = messageTextTextBlock;
            }

            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                return (parameter != null && parameter is string);
            }

            public void Execute(object parameter)
            {
                _messageTextTextBlock.Text = (string)parameter;
            }
        }
        public class TestCommandInMessageBox : ICommand
        {
            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                return (parameter != null && parameter is string);
            }

            public void Execute(object parameter)
            {
                MessageBox.Show((string)parameter);
            }
        }

#endregion

#region Style tests

        private void PrepareStyleTest()
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
            _setterForMyStyle1Background = new Setter(TextBlock.BackgroundProperty, new SolidColorBrush(Colors.Black));
            MyStyle1.Setters.Add(_setterForMyStyle1Background);

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
            Random rand = new Random();
            _setterForMyStyle1Background.Value = new SolidColorBrush(Color.FromArgb((byte)255, (byte)rand.Next(256), (byte)rand.Next(256), (byte)rand.Next(256)));
            _setterForMyStyle2Foreground.Value = new SolidColorBrush(Color.FromArgb((byte)255, (byte)rand.Next(256), (byte)rand.Next(256), (byte)rand.Next(256)));
            _myColor.BackgroundColor = new SolidColorBrush(Color.FromArgb((byte)255, (byte)rand.Next(256), (byte)rand.Next(256), (byte)rand.Next(256)));
        }

#endregion

#region Async/Await tests

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TextBlock2.Text = "Before call to Async.";
            test();
            TextBlock2.Text = "After call to Async.";

        }

        private async void Button2_Click(object sender, RoutedEventArgs e)
        {
            TextBlock2.Text = "Before call to Async.";
            await test();
            TextBlock2.Text = "After call to Async.";

        }

        public async Task test()
        {
            TextBlock2.Text = await TestAsync();
        }

        public static Task<string> TestAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<string>();
            var cameraCaptureTask = new Bidon();
            cameraCaptureTask.Completed += (sender, result) => taskCompletionSource.SetResult("Result obtained.");
            cameraCaptureTask.Start();
            return taskCompletionSource.Task;
        }

        class Bidon
        {
            public event EventHandler Completed;
            DispatcherTimer t = new DispatcherTimer();
            public void Start()
            {
                t.Interval = new TimeSpan(0, 0, 5);
                t.Tick += t_Tick;
                t.Start();
            }

            void t_Tick(object sender, object e)
            {
                t.Stop();
                Completed(sender, new EventArgs());
            }
#endregion

        }

#region Pointer events + pointer capture tests


        double mouseVerticalPosition;
        double mouseHorizontalPosition;
        bool isMouseCaptured;
        
#if SLMIGRATION
        private void InnerBorderForPointerEvents_PointerPressed_1(object sender, MouseEventArgs e)
#else
        private void InnerBorderForPointerEvents_PointerPressed_1(object sender, PointerRoutedEventArgs e)
#endif
        {
            Border border = (Border)sender;
#if SLMIGRATION
            mouseVerticalPosition = e.GetPosition(null).Y;
            mouseHorizontalPosition = e.GetPosition(null).X;
            border.CaptureMouse();
#else
            mouseVerticalPosition = e.GetCurrentPoint(null).Position.Y;
            mouseHorizontalPosition = e.GetCurrentPoint(null).Position.X;
            border.CapturePointer(e.Pointer);
#endif
            PointerCaptureTextBlock.Text = "Pointer captured";
            isMouseCaptured = true;
        }

#if SLMIGRATION
        private void InnerBorderForPointerEvents_PointerReleased_1(object sender, MouseEventArgs e)
#else
        private void InnerBorderForPointerEvents_PointerReleased_1(object sender, PointerRoutedEventArgs e)
#endif
        {
            Border border = (Border)sender;
            isMouseCaptured = false;
#if SLMIGRATION
            border.ReleaseMouseCapture();
#else
            border.ReleasePointerCapture(e.Pointer);
#endif
            PointerCaptureTextBlock.Text = "";
        }

#if SLMIGRATION
        private void ContainerBorderForPointerEvents_PointerMoved(object sender, MouseEventArgs e)
#else
        private void ContainerBorderForPointerEvents_PointerMoved(object sender, PointerRoutedEventArgs e)
#endif
        {
            Border border = (Border)sender;
            Random r = new Random();
            border.Background = new SolidColorBrush(Color.FromArgb((Byte)255, (Byte)r.Next(255), (Byte)r.Next(255), (Byte)r.Next(255)));
        }

#if SLMIGRATION
        private void InnerBorderForPointerEvents_PointerMoved(object sender, MouseEventArgs e)
#else
        private void InnerBorderForPointerEvents_PointerMoved(object sender, PointerRoutedEventArgs e)
#endif
        {
            Border item = (Border)sender;
            if (isMouseCaptured)
            {
                // Calculate the current position of the object.
#if SLMIGRATION
                double deltaV = e.GetPosition(null).Y - mouseVerticalPosition;
                double deltaH = e.GetPosition(null).X - mouseHorizontalPosition;
#else
                double deltaV = e.GetCurrentPoint(null).Position.Y - mouseVerticalPosition;
                double deltaH = e.GetCurrentPoint(null).Position.X - mouseHorizontalPosition;
#endif
                Thickness margin = item.Margin;
                double newTop = deltaV + margin.Top;
                double newLeft = deltaH + margin.Left;

                // Set new position of object.
                margin.Left = newLeft;
                margin.Top = newTop;
                item.Margin = margin;

                // Update position global variables.
#if SLMIGRATION
                mouseVerticalPosition = e.GetPosition(null).Y;
                mouseHorizontalPosition = e.GetPosition(null).X;
#else
                mouseVerticalPosition = e.GetCurrentPoint(null).Position.Y;
                mouseHorizontalPosition = e.GetCurrentPoint(null).Position.X;
#endif
            }
        }

#endregion


#region ChildWindow test

        private void ButtonTestChildWindow_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWnd = new LoginWindow();
            loginWnd.Closed += new EventHandler(loginWnd_Closed);
            loginWnd.Show();
        }

        void loginWnd_Closed(object sender, EventArgs e)
        {
            LoginWindow lw = (LoginWindow)sender;
            if (lw.DialogResult == true && lw.NameBox.Text != string.Empty)
            {
                this.TextBlockForTestingChildWindow.Text = "Hello " + lw.NameBox.Text;
            }
            else if (lw.DialogResult == false)
            {
                this.TextBlockForTestingChildWindow.Text = "Login canceled.";
            }
        }

#endregion




#region ListBox tests

        string RandomId()
        {
            return (new Random()).Next(1000).ToString();
        }

        private void ButtonTestListBox_ItemsAdd_Click(object sender, RoutedEventArgs e)
        {
            ListBox1.Items.Add("Item #" + RandomId());
        }

        private void ButtonTestListBox_ItemsClear_Click(object sender, RoutedEventArgs e)
        {
            ListBox1.Items.Clear();
        }

        private void ButtonTestListBox_ItemsRemoveFirst_Click(object sender, RoutedEventArgs e)
        {
            ListBox1.Items.Remove(ListBox1.Items[0]);
        }

        private void ButtonTestListBox_SetNewItemsSource_Click(object sender, RoutedEventArgs e)
        {
            ListBox1.ItemsSource = new ObservableCollection<string>()
            {
                "One", "Two", "Three"
            };
        }

        private void ButtonTestListBox_ItemsSourceAdd_Click(object sender, RoutedEventArgs e)
        {
            ((ObservableCollection<string>)ListBox1.ItemsSource).Add("Item #" + RandomId());
        }

        private void ButtonTestListBox_ItemsSourceClear_Click(object sender, RoutedEventArgs e)
        {
            ((ObservableCollection<string>)ListBox1.ItemsSource).Clear();
        }

        private void ButtonTestListBox_ItemsSourceRemove_Click(object sender, RoutedEventArgs e)
        {
            ((ObservableCollection<string>)ListBox1.ItemsSource).Remove(((ObservableCollection<string>)ListBox1.ItemsSource).FirstOrDefault());
        }

        private void ButtonTestListBox_SetItemsSourceToNull_Click(object sender, RoutedEventArgs e)
        {
            ListBox1.ItemsSource = null;
        }

        private void ButtonTestListBox_SelectSecondItem_Click(object sender, RoutedEventArgs e)
        {
            ListBox1.SelectedItem = ListBox1.Items[1];
        }

        private void ButtonTestListBox_SelectSecondIndex_Click(object sender, RoutedEventArgs e)
        {
            ListBox1.SelectedIndex = 1;
        }

        private void ButtonTestListBox_SelectItemNull_Click(object sender, RoutedEventArgs e)
        {
            ListBox1.SelectedItem = null;
        }

        private void ButtonTestListBox_SelectedIndexMinusOne_Click(object sender, RoutedEventArgs e)
        {
            ListBox1.SelectedIndex = -1;
        }

#endregion

#region ComboBox NATIVE tests

        private void ButtonTestComboBox_ItemsAdd_Click(object sender, RoutedEventArgs e)
        {
            ComboBox1.Items.Add("Item #" + RandomId());
        }

        private void ButtonTestComboBox_ItemsAddString_Click(object sender, RoutedEventArgs e)
        {
            ComboBox1.Items.Add("Test String");
        }

        private void ButtonTestComboBox_ItemsClear_Click(object sender, RoutedEventArgs e)
        {
            ComboBox1.Items.Clear();
        }

        private void ButtonTestComboBox_ItemsRemoveFirst_Click(object sender, RoutedEventArgs e)
        {
            ComboBox1.Items.Remove(ComboBox1.Items[0]);
        }

        private void ButtonTestComboBox_SetNewItemsSource_Click(object sender, RoutedEventArgs e)
        {
            ComboBox1.ItemsSource = new ObservableCollection<string>()
            {
                "One", "Two", "Three"
            };
        }

        private void ButtonTestComboBox_ItemsSourceAdd_Click(object sender, RoutedEventArgs e)
        {
            ((ObservableCollection<string>)ComboBox1.ItemsSource).Add("Item #" + RandomId());
        }

        private void ButtonTestComboBox_ItemsSourceClear_Click(object sender, RoutedEventArgs e)
        {
            ((ObservableCollection<string>)ComboBox1.ItemsSource).Clear();
        }

        private void ButtonTestComboBox_ItemsSourceRemove_Click(object sender, RoutedEventArgs e)
        {
            ((ObservableCollection<string>)ComboBox1.ItemsSource).Remove(((ObservableCollection<string>)ComboBox1.ItemsSource).FirstOrDefault());
        }

        private void ButtonTestComboBox_SetItemsSourceToNull_Click(object sender, RoutedEventArgs e)
        {
            ComboBox1.ItemsSource = null;
        }

        private void ButtonTestComboBox_SelectSecondItem_Click(object sender, RoutedEventArgs e)
        {
            ComboBox1.SelectedItem = ComboBox1.Items[1];
        }

        private void ButtonTestComboBox_SelectSecondIndex_Click(object sender, RoutedEventArgs e)
        {
            ComboBox1.SelectedIndex = 1;
        }

        private void ButtonTestComboBox_SelectItemNull_Click(object sender, RoutedEventArgs e)
        {
            ComboBox1.SelectedItem = null;
        }

        private void ButtonTestComboBox_SelectedIndexMinusOne_Click(object sender, RoutedEventArgs e)
        {
            ComboBox1.SelectedIndex = -1;
        }

#endregion

#region ComboBox NON-NATIVE tests

        private void ButtonTestComboBoxNonNative_ItemsAddUIElement_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxNonNative.Items.Add(new TextBlock() { Text = "This is a UI element", Foreground = new SolidColorBrush(Colors.Red) });
        }

        private void ButtonTestComboBoxNonNative_ItemsAddString_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxNonNative.Items.Add("Test String");
        }

        private void ButtonTestComboBoxNonNative_ItemsAdd_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxNonNative.Items.Add("Item #" + RandomId());
        }

        private void ButtonTestComboBoxNonNative_ItemsClear_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxNonNative.Items.Clear();
        }

        private void ButtonTestComboBoxNonNative_ItemsRemoveFirst_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxNonNative.Items.Remove(ComboBoxNonNative.Items[0]);
        }

        private void ButtonTestComboBoxNonNative_SetNewItemsSource_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxNonNative.ItemsSource = new ObservableCollection<string>()
            {
                "One", "Two", "Three"
            };
        }

        private void ButtonTestComboBoxNonNative_ItemsSourceAdd_Click(object sender, RoutedEventArgs e)
        {
            ((ObservableCollection<string>)ComboBoxNonNative.ItemsSource).Add("Item #" + RandomId());
        }

        private void ButtonTestComboBoxNonNative_ItemsSourceClear_Click(object sender, RoutedEventArgs e)
        {
            ((ObservableCollection<string>)ComboBoxNonNative.ItemsSource).Clear();
        }

        private void ButtonTestComboBoxNonNative_ItemsSourceRemove_Click(object sender, RoutedEventArgs e)
        {
            ((ObservableCollection<string>)ComboBoxNonNative.ItemsSource).Remove(((ObservableCollection<string>)ComboBoxNonNative.ItemsSource).FirstOrDefault());
        }

        private void ButtonTestComboBoxNonNative_SetItemsSourceToNull_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxNonNative.ItemsSource = null;
        }

        private void ButtonTestComboBoxNonNative_SelectSecondItem_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxNonNative.SelectedItem = ComboBoxNonNative.Items[1];
        }

        private void ButtonTestComboBoxNonNative_SelectSecondIndex_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxNonNative.SelectedIndex = 1;
        }

        private void ButtonTestComboBoxNonNative_SelectItemNull_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxNonNative.SelectedItem = null;
        }

        private void ButtonTestComboBoxNonNative_SelectedIndexMinusOne_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxNonNative.SelectedIndex = -1;
        }

#endregion

#region AutoCompleteBox tests

        private void ButtonTestAutoCompleteBox1_ItemsAddUIElement_Click(object sender, RoutedEventArgs e)
        {
            AutoCompleteBox1.Items.Add(new TextBlock() { Text = "This is a UI element", Foreground = new SolidColorBrush(Colors.Red) });
        }

        private void ButtonTestAutoCompleteBox1_ItemsAddString_Click(object sender, RoutedEventArgs e)
        {
            AutoCompleteBox1.Items.Add("Test String");
        }

        private void ButtonTestAutoCompleteBox1_ItemsAdd_Click(object sender, RoutedEventArgs e)
        {
            AutoCompleteBox1.Items.Add("Item #" + RandomId());
        }

        private void ButtonTestAutoCompleteBox1_ItemsClear_Click(object sender, RoutedEventArgs e)
        {
            AutoCompleteBox1.Items.Clear();
        }

        private void ButtonTestAutoCompleteBox1_ItemsRemoveFirst_Click(object sender, RoutedEventArgs e)
        {
            AutoCompleteBox1.Items.Remove(AutoCompleteBox1.Items[0]);
        }

        private void ButtonTestAutoCompleteBox1_SetNewItemsSource_Click(object sender, RoutedEventArgs e)
        {
            AutoCompleteBox1.ItemsSource = new ObservableCollection<string>()
            {
                "One", "Two", "Three"
            };
        }

        private void ButtonTestAutoCompleteBox1_ItemsSourceAdd_Click(object sender, RoutedEventArgs e)
        {
            ((ObservableCollection<string>)AutoCompleteBox1.ItemsSource).Add("Item #" + RandomId());
        }

        private void ButtonTestAutoCompleteBox1_ItemsSourceClear_Click(object sender, RoutedEventArgs e)
        {
            ((ObservableCollection<string>)AutoCompleteBox1.ItemsSource).Clear();
        }

        private void ButtonTestAutoCompleteBox1_ItemsSourceRemove_Click(object sender, RoutedEventArgs e)
        {
            ((ObservableCollection<string>)AutoCompleteBox1.ItemsSource).Remove(((ObservableCollection<string>)AutoCompleteBox1.ItemsSource).FirstOrDefault());
        }

        private void ButtonTestAutoCompleteBox1_SetItemsSourceToNull_Click(object sender, RoutedEventArgs e)
        {
            AutoCompleteBox1.ItemsSource = null;
        }

        private void ButtonTestAutoCompleteBox1_SelectSecondItem_Click(object sender, RoutedEventArgs e)
        {
            AutoCompleteBox1.SelectedItem = AutoCompleteBox1.Items[1];
        }

        private void ButtonTestAutoCompleteBox1_SelectSecondIndex_Click(object sender, RoutedEventArgs e)
        {
            AutoCompleteBox1.SelectedIndex = 1;
        }

        private void ButtonTestAutoCompleteBox1_SelectItemNull_Click(object sender, RoutedEventArgs e)
        {
            AutoCompleteBox1.SelectedItem = null;
        }

        private void ButtonTestAutoCompleteBox1_SelectedIndexMinusOne_Click(object sender, RoutedEventArgs e)
        {
            AutoCompleteBox1.SelectedIndex = -1;
        }

        #endregion

        //#region DataGrid tests

        //private void ButtonTestDataGrid_ItemsAdd_Click(object sender, RoutedEventArgs e)
        //{
        //    DataGrid1.Items.Add("Item #" + RandomId());
        //}

        //private void ButtonTestDataGrid_ItemsAddString_Click(object sender, RoutedEventArgs e)
        //{
        //    DataGrid1.Items.Add("Test String");
        //}

        //private void ButtonTestDataGrid_ItemsClear_Click(object sender, RoutedEventArgs e)
        //{
        //    DataGrid1.Items.Clear();
        //}

        //private void ButtonTestDataGrid_ItemsRemoveFirst_Click(object sender, RoutedEventArgs e)
        //{
        //    DataGrid1.Items.Remove(DataGrid1.Items[0]);
        //}

        //private void ButtonTestDataGrid_SetNewItemsSource_Click(object sender, RoutedEventArgs e)
        //{
        //    DataGrid1.ItemsSource = new ObservableCollection<string>()
        //    {
        //        "One", "Two", "Three"
        //    };
        //}

        //private void ButtonTestDataGrid_ItemsSourceAdd_Click(object sender, RoutedEventArgs e)
        //{
        //    ((ObservableCollection<string>)DataGrid1.ItemsSource).Add("Item #" + RandomId());
        //}

        //private void ButtonTestDataGrid_ItemsSourceClear_Click(object sender, RoutedEventArgs e)
        //{
        //    ((ObservableCollection<string>)DataGrid1.ItemsSource).Clear();
        //}

        //private void ButtonTestDataGrid_ItemsSourceRemove_Click(object sender, RoutedEventArgs e)
        //{
        //    ((ObservableCollection<string>)DataGrid1.ItemsSource).Remove(((ObservableCollection<string>)DataGrid1.ItemsSource).FirstOrDefault());
        //}

        //private void ButtonTestDataGrid_SetItemsSourceToNull_Click(object sender, RoutedEventArgs e)
        //{
        //    DataGrid1.ItemsSource = null;
        //}

        //private void ButtonTestDataGrid_SelectSecondItem_Click(object sender, RoutedEventArgs e)
        //{
        //    DataGrid1.SelectedItem = DataGrid1.Items[1];
        //}

        //private void ButtonTestDataGrid_SelectSecondIndex_Click(object sender, RoutedEventArgs e)
        //{
        //    DataGrid1.SelectedIndex = 1;
        //}

        //private void ButtonTestDataGrid_SelectItemNull_Click(object sender, RoutedEventArgs e)
        //{
        //    DataGrid1.SelectedItem = null;
        //}

        //private void ButtonTestDataGrid_SelectedIndexMinusOne_Click(object sender, RoutedEventArgs e)
        //{
        //    DataGrid1.SelectedIndex = -1;
        //}

        //#endregion


        #region Testing Right-click

#if SLMIGRATION
        void BorderToTestRightClick_RightTapped(object sender, MouseButtonEventArgs e)
#else
        void BorderToTestRightClick_RightTapped(object sender, RightTappedRoutedEventArgs e)
#endif
        {
            MessageBox.Show("Right tapped!");
        }

#endregion

#region Testing ContextMenu MenuItem

        private void MenuItem1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Menu Item 1");
        }

        private void MenuItem2_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Menu Item 2");
        }

        private void MenuItem3_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Menu Item 3");
        }

#endregion

#region Testing Validation

        private void ValidationBorder_BindingValidationError(object sender, ValidationErrorEventArgs e)
        {
            if (Validation.GetHasError(NameTextBoxForValidation) || Validation.GetHasError(AgeTextBoxForValidation))
            {
                (MyButtonForValidation).IsEnabled = false;
            }
            else
            {
                (MyButtonForValidation).IsEnabled = true;
            }
        }

        private void MyButtonForValidation_Click(object sender, RoutedEventArgs e)
        {
            Person person = (Person)((Button)sender).DataContext;
            string str = "Name: \"" + person.Name + "\"" + Environment.NewLine + "Age: " + person.Age + ".";
            MessageBox.Show(str);
        }

#endregion

#region Testing DateTime

        private void ButtonTestDateTime1_Click(object sender, RoutedEventArgs e)
        {
            TestDateTime(new DateTime(2014, 02, 01, 15, 16, 17));
        }

        private void ButtonTestDateTime2_Click(object sender, RoutedEventArgs e)
        {
            TestDateTime(new DateTime(2014, 02, 01, 15, 16, 17, DateTimeKind.Local));
        }

        private void ButtonTestDateTime3_Click(object sender, RoutedEventArgs e)
        {
            TestDateTime(new DateTime(2014, 02, 01, 15, 16, 17, DateTimeKind.Utc));
        }

        private void ButtonTestDateTime4_Click(object sender, RoutedEventArgs e)
        {
            TestDateTime(DateTime.Now);
        }

        private void ButtonTestDateTime5_Click(object sender, RoutedEventArgs e)
        {
            TestDateTime(DateTime.Now);
        }

        void TestDateTime(DateTime dateTime)
        {
            DisplayDateTimeFullInfo(dateTime, "Input DateTime:");

            //----------------------
            // Test DateTime serialization/deserialization:
            //----------------------
            var _classToSerialize = new ClassToTestDateTimeSerialization()
            {
                DateField = dateTime
            };

            // Serialize:
            var serializer = new XmlSerializer(typeof(ClassToTestDateTimeSerialization));
            var stream = new MemoryStream(); serializer.Serialize(stream, _classToSerialize);
            stream.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(stream);
            var serializedXml = reader.ReadToEnd();

            MessageBox.Show("Serialized: " + serializedXml);

            // Deserialize:
            var deserializer = new XmlSerializer(typeof(ClassToTestDateTimeSerialization));
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(serializedXml));
            var xmlReader = XmlReader.Create(memoryStream);
            ClassToTestDateTimeSerialization deserializedObject = (ClassToTestDateTimeSerialization)deserializer.Deserialize(xmlReader);

            DisplayDateTimeFullInfo(deserializedObject.DateField, "Deserialized DateTime:");
        }

        void DisplayDateTimeFullInfo(DateTime dateTime, string title)
        {
            MessageBox.Show(title + Environment.NewLine
                + "   ToString()=" + dateTime.ToString() + Environment.NewLine
                + "   ToShortDateString()=" + dateTime.ToShortDateString() + Environment.NewLine
                + "   ToShortTimeString()=" + dateTime.ToShortTimeString() + Environment.NewLine
                //+ "   ToLongTimeString()=" + dateTime.ToLongTimeString() + Environment.NewLine
                + "   ToUniversalTime().ToString()=" + dateTime.ToUniversalTime().ToString() + Environment.NewLine
                + "   ToLocalTime().ToString()=" + dateTime.ToLocalTime().ToString() + Environment.NewLine
                + "   ToUniversalTime().ToLocalTime().ToString()=" + dateTime.ToUniversalTime().ToLocalTime().ToString() + Environment.NewLine
                + "   ToLocalTime().ToUniversalTime().ToString()=" + dateTime.ToLocalTime().ToUniversalTime() + Environment.NewLine
                + "   Hours=" + dateTime.Hour.ToString() + Environment.NewLine
                + "   Minutes=" + dateTime.Minute.ToString() + Environment.NewLine
                + "   Seconds=" + dateTime.Second.ToString() + Environment.NewLine
                );
        }

#endregion

#region Testing Double Click

#if SLMIGRATION
        private void TestDoubleClick_PointerPressed(object sender, MouseEventArgs e)
#else
        private void TestDoubleClick_PointerPressed(object sender, PointerRoutedEventArgs e)
#endif
        {
            if (e.ClickCount == 2)
                MessageBox.Show("You double-clicked!");
        }

#endregion

#region Testing "TextChanged" event

        void TestTextChanged_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TestTextChanged_Count.Text = (int.Parse(TestTextChanged_Count.Text) + 1).ToString();
        }

#endregion

    }

    [DataContract]
    public class ClassToTestDateTimeSerialization
    {
        public DateTime DateField { get; set; }
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

    //Validation:
    public class Person : INotifyPropertyChanged
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new Exception("Name cannot be empty.");
                }
                _name = value;
                RaisePropertyChanged("Name");
            }
        }

        private int _age;
        public int Age
        {
            get { return _age; }
            set
            {
                if (value <= 0)
                {
                    throw new Exception("Age cannot be lower than 0.");
                }
                _age = value;
                RaisePropertyChanged("Age");
            }
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }

#region Serialization Tests

    public class TestClass
    {
        private string someString;
        public string SomeString
        {
            get { return someString; }
            set { someString = value; }
        }

        private List<string> settings = new List<string>();
        public List<string> Settings
        {
            get { return settings; }
            set { settings = value; }
        }

        private int willBeIgnored1 = 1;
        private int willBeIgnored2 = 1;

    }

#endregion



}
