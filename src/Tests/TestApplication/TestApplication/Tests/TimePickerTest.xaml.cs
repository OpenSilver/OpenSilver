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

namespace TestApplication.OpenSilver.Tests
{
    public partial class TimePickerTest : Page
    {
        public TimePickerTest()
        {
            this.InitializeComponent();
            Loaded += TimePickerTest_Loaded;
            this.DataContext = new TimePickerViewModel();
        }

        private void TimePickerTest_Loaded(object sender, RoutedEventArgs e)
        {
            TimePicker1.Format = new CustomTimeFormat("HHmm");
            TimePicker2.Format = new ShortTimeFormat();
        }
    }

    public class TimePickerViewModel : NotifyPropertyChanged
    {
        private DateTime _selectedDate = DateTime.Now;

        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            set { Set(ref _selectedDate, value); }
        }

    }
}
