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

namespace TestApplication
{
    public partial class TestPage : UserControl
    {
        public TestPage()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }

        private DateTime? _currentDate = DateTime.Now;
        public DateTime? CurrentDate 
        {
            get => _currentDate;
            set
            {
                _currentDate = value;
            }
        }
    }
}
