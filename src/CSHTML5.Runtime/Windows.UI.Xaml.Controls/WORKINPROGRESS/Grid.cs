#if WORKINPROGRESS

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    public partial class Grid
    {
        public static readonly DependencyProperty ShowGridLinesProperty = DependencyProperty.Register("ShowGridLines", typeof(bool), typeof(Grid), new PropertyMetadata(false)
        { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        public bool ShowGridLines
        {
            get { return (bool)this.GetValue(ShowGridLinesProperty); }
            set { this.SetValue(ShowGridLinesProperty, value); }
        }
    }
}

#endif