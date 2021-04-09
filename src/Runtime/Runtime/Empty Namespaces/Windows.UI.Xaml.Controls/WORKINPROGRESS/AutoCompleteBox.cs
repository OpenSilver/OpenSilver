#if WORKINPROGRESS

using System;

#if MIGRATION
using System.Windows.Controls.Primitives;
#else
using Windows.UI.Xaml.Controls.Primitives;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    public partial class AutoCompleteBox : Selector
    {
        virtual protected void OnPopulated(PopulatedEventArgs e)
        {

        }
        public static DependencyProperty ItemFilterProperty
        {
            get; set;
        }
    }
}
#endif