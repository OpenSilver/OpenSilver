using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interactivity;
#if SLMIGRATION
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
#else
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endif

namespace TestApplication
{
    public class HintBehavior : Behavior<TextBox>
    {
        public HintBehavior() { }
        public HintBehavior(string text)
        {
            Text = text;
        }
        public HintBehavior(string text, Brush hintBrush)
        {
            Text = text;
            HintForeground = hintBrush;
        }

        private bool _isHintDisplayed = false;

        public bool IsHintDisplayed
        {
            get { return _isHintDisplayed; }
            set { _isHintDisplayed = value; }
        }


        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(HintBehavior), new PropertyMetadata("Type here..."));



        public Brush HintForeground
        {
            get { return (Brush)GetValue(HintForegroundProperty); }
            set { SetValue(HintForegroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HintForeground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HintForegroundProperty =
            DependencyProperty.Register("HintForeground", typeof(Brush), typeof(HintBehavior), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));

        private Brush _elementOriginalBrush = null;


        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.GotFocus += AssociatedObject_GotFocus;
            AssociatedObject.LostFocus += AssociatedObject_LostFocus;
            AssociatedObject_LostFocus(null, null);
        }

        protected override void OnDetaching()
        {
            AssociatedObject.GotFocus -= AssociatedObject_GotFocus;
            AssociatedObject.LostFocus -= AssociatedObject_LostFocus;
            base.OnDetaching();
        }

        void AssociatedObject_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(AssociatedObject.Text))
            {
                _elementOriginalBrush = AssociatedObject.Foreground;
                AssociatedObject.Foreground = HintForeground;
                AssociatedObject.Text = Text;
                _isHintDisplayed = true;
            }
        }

        void AssociatedObject_GotFocus(object sender, RoutedEventArgs e)
        {
            if (_isHintDisplayed)
            {
                AssociatedObject.Foreground = _elementOriginalBrush;
                AssociatedObject.Text = "";
                _isHintDisplayed = false;
            }
        }
    }
}
