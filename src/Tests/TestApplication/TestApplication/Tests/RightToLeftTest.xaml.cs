using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;

namespace TestApplication.Tests
{
    public partial class RightToLeftTest : Page
    {
        public RightToLeftTest()
        {
            this.InitializeComponent();
            btnTest.Click += BtnTest_Click;
        }

        private void BtnTest_Click(object sender, RoutedEventArgs e)
        {
            spLayout.FlowDirection = spLayout.FlowDirection == FlowDirection.LeftToRight
                ? FlowDirection.RightToLeft
                : FlowDirection.LeftToRight;

            txtNormal.Text = txtNormal.FlowDirection.ToString();
        }

        private void NewTest_Click(object sender, RoutedEventArgs e)
        {
            txtNormal.FlowDirection = txtNormal.FlowDirection == FlowDirection.LeftToRight
                ? FlowDirection.RightToLeft
                : FlowDirection.LeftToRight;

            txtNormal.Text = txtNormal.FlowDirection.ToString();
        }
    }
}
