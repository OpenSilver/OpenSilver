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
using System.Windows.Interactivity;

namespace TestApplication.Tests
{
    public partial class BehaviorTest : Page
    {
        public BehaviorTest()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Interaction.GetBehaviors(TestBehaviorTextBox).Add(new HintBehavior("Pls type something.", new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xD7, 0x00))));
        }

        void TestBehaviorButton_Click(object sender, RoutedEventArgs e)
        {
            string firstTextBoxText = "";
            foreach (Behavior behavior in Interaction.GetBehaviors(TestBehaviorTextBox)) //there is only one but eh.
            {
                if (behavior is HintBehavior)
                {
                    if (!((HintBehavior)behavior).IsHintDisplayed)
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
    }
}
