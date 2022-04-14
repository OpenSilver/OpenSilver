using System.Windows.Controls;
using System.Windows.Input;

namespace TestApplication.Tests.Events
{
    public partial class TextInputTest : Page
    {
        public TextInputTest()
        {
            this.InitializeComponent();

            box1.TextInput += Box1_TextInput;
            box2.AddHandler(TextInputEvent, new TextCompositionEventHandler(Box2_TextInput), true);
            box3.AddHandler(KeyDownEvent, new KeyEventHandler(Box3_KeyDown), false);
            box3.AddHandler(TextInputEvent, new TextCompositionEventHandler(Box3_TextInput), true);
            box4.TextInput += Box4_TextInput;
            box5.AddHandler(TextInputEvent, new TextCompositionEventHandler(Box5_TextInput), true);
            box6.AddHandler(TextInputEvent, new TextCompositionEventHandler(Box6_TextInput), true);
            box7.AddHandler(TextInputEvent, new TextCompositionEventHandler(Box7_TextInput), true);

            button2.AddHandler(TextInputEvent, new TextCompositionEventHandler(button2_TextInput), true);
            button3.AddHandler(TextInputEvent, new TextCompositionEventHandler(button3_TextInput), true);
            button1.Click += Button1_Click;
        }

        private void button3_TextInput(object sender, TextCompositionEventArgs e)
        {
            blockMsg.Text = "should still fire with OnTextInput and even with e.Handled=true before calling base";
        }

        private void button2_TextInput(object sender, TextCompositionEventArgs e)
        {
            blockMsg.Text = "fired for non editable elements";
        }

        private void Box7_TextInput(object sender, TextCompositionEventArgs e)
        {
            blockMsg.Text = "event should fire";

        }

        private void Box6_TextInput(object sender, TextCompositionEventArgs e)
        {
            blockMsg.Text = "event should fire";
        }

        private void Box5_TextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = true;
        }

        private void Box4_TextInput(object sender, TextCompositionEventArgs e)
        {
            blockMsg.Text = "event fires for Esc and Enter without handler handledeventstoo=true";
        }

        private void Box3_TextInput(object sender, TextCompositionEventArgs e)
        {
            blockMsg.Text = "even shouldn't fire when the keydown event e.Handled=true even if this handler set with handledeventstoo=true";
        }

        private void Box2_TextInput(object sender, TextCompositionEventArgs e)
        {
            blockMsg.Text = "event fires with handler handledeventstoo=true";
        }

        private void Box1_TextInput(object sender, TextCompositionEventArgs e)
        {
            blockMsg.Text = "even shouldn't fire without handler handledeventstoo=true";
        }

        private void Button1_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            blockMsg.Text = "";
        }

        private void Box3_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        private void box8_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left || e.Key == Key.Right)
                blockMsg.Text = "KeyDown event should fire only if the cursor is in position 0 or at the end.";
        }

        private void box9_KeyDown(object sender, KeyEventArgs e)
        {
            var textBox = sender as TextBox;
            if (e.Key == Key.Up || e.Key == Key.Down)
                blockMsg.Text = string.Format("SelectionStart: {0} Cursor remains in position where it was and SelectionStart is the current position.", textBox.SelectionStart);
        }

        private void box10_KeyDown(object sender, KeyEventArgs e)
        {
            var textBox = sender as TextBox;
            if (e.Key == Key.Up || e.Key == Key.Down)
                blockMsg.Text = string.Format("SelectionStart: {0}, SelectionLength: {1} Selected removed, cursor moves to the beginning/end of the text. SelectionStart is the position, SelectionLength is 0.", textBox.SelectionStart, textBox.SelectionLength);
        }
    }
}
