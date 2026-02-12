using System.Windows;
using System.Windows.Controls;

namespace TestApplication.Tests
{
    /// <summary>
    /// A simple custom control used to test whether triggers work in default (theme) styles.
    /// The default style is defined in Themes/generic.xaml and includes property triggers.
    /// </summary>
    public class TriggerTestControl : Control
    {
        static TriggerTestControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(TriggerTestControl),
                new PropertyMetadata(typeof(TriggerTestControl)));
        }
    }
}
