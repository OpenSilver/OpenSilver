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
    public sealed partial class OutOfBrowserSettings : DependencyObject
    {

        public static readonly DependencyProperty WindowSettingsProperty = DependencyProperty.Register("WindowSettings", typeof(WindowSettings), typeof(OutOfBrowserSettings), null);
        public WindowSettings WindowSettings
        {
            get { return (WindowSettings)this.GetValue(WindowSettingsProperty); }
            private set { this.SetValue(WindowSettingsProperty, value); }
        }
    }
}
#endif
