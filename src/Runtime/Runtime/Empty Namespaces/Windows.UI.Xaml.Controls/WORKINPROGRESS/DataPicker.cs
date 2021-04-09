#if WORKINPROGRESS

using System;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    public partial class DatePicker
    {
        public static DependencyProperty CalendarStyleProperty
        {
            get; set;
        }
    }
}
#endif