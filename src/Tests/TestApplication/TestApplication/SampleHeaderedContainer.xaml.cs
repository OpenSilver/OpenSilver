#if SLMIGRATION
using System.Windows;
using System.Windows.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif


namespace TestApplication
{
    public sealed partial class SampleHeaderedContainer : UserControl
    {
        public SampleHeaderedContainer()
        {
            this.InitializeComponent();
        }

        // Important: do not name the property below "Content", otherwise it will conflict with the existing "Content" property of the UserControl. This limitation will be lifted when CSHTML5 provides the "OverrideMetadata" method.
        public UIElement Body
        {
            get { return (UIElement)GetValue(BodyProperty); }
            set { SetValue(BodyProperty, value); }
        }

        public static readonly DependencyProperty BodyProperty =
            DependencyProperty.Register("Body", typeof(UIElement), typeof(SampleHeaderedContainer), new PropertyMetadata(null, Body_Changed));

        static void Body_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (SampleHeaderedContainer)d;
            if (CSharpXamlForHtml5.DomManagement.IsControlInVisualTree(control))
            {
                control.BodyContainer1.Child = e.NewValue as UIElement;
            }
        }

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(SampleHeaderedContainer), new PropertyMetadata("", Header_Changed));

        static void Header_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (SampleHeaderedContainer)d;
            if (CSharpXamlForHtml5.DomManagement.IsControlInVisualTree(control))
            {
                control.HeaderContainer1.Text = (e.NewValue ?? "").ToString();
            }
        }
    }
}
