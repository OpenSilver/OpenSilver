using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace TestApplication.Tests.Events
{
    public class XTextBox : TextBox
    {
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            var block = (Parent as StackPanel).FindName("blockMsg") as TextBlock;

            if (e.Key == Key.Right || e.Key == Key.Left)
                block.Text = "Right or Left arrow key pressed with OnKeyDown";
        }

        protected override void OnTextInput(TextCompositionEventArgs e)
        {
            if (Name == "box7")
                return;

            e.Handled = true;  
            base.OnTextInput(e);
        }
    }

    public class XButton : Button
    {
        protected override void OnTextInput(TextCompositionEventArgs e)
        {
            e.Handled = true;
            base.OnTextInput(e);
        }
    }
}
