using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace TestApplication
{
    public sealed partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();

            // Enter construction logic here...

            this.RootVisual = new MainPage();
        }
    }
}
