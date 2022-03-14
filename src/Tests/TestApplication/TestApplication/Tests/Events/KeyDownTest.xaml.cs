using System.Windows.Controls;
using System.Windows.Input;

namespace TestApplication.Tests.Events
{
    public partial class KeyDownTest : Page
    {
        public KeyDownTest()
        {
            this.InitializeComponent();

            box1.KeyDown += Box1_KeyDown;
            box2.KeyDown += Box2_KeyDown;
            box3.AddHandler(KeyDownEvent, new KeyEventHandler(Box3_KeyDown), true);

            button1.Click += Button1_Click;
        }

        private void Button1_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            blockMsg.Text = "";
        }

        private void Box1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left && box1.SelectionStart > 0)
                blockMsg.Text = "Left arrow shouldn't trigger keydown unless caret in first position";
        }

        private void Box2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Right && box2.SelectionStart < box2.Text.Length)
                blockMsg.Text = "Right arrow shouldn't trigger keydown unless caret in last position";
        }

        private void Box3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Right || e.Key == Key.Left)
                blockMsg.Text = "Right or Left arrow key pressed with add handler with handledeventstoo=true";
        }
    }

}
