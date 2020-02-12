using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if WORKINPROGRESS
#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    public partial class WindowSettings : DependencyObject
    {
        public static readonly DependencyProperty WindowStyleProperty = DependencyProperty.Register("WindowStyle", typeof(WindowStyle), typeof(WindowSettings), null);

        public WindowStyle WindowStyle
        {
            get { return (WindowStyle)this.GetValue(WindowStyleProperty); }
            private set { this.SetValue(WindowStyleProperty, value); }
        }
    }
}
#endif
