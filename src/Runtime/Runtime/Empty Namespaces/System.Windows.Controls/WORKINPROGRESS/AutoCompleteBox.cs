#if WORKINPROGRESS

using System;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;

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