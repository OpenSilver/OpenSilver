using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if WORKINPROGRESS

#if MIGRATION
namespace System.Windows.Media.Animation
#else
namespace Windows.UI.Xaml.Media.Animation
#endif
{
    //
    // Summary:
    //     Specifies how a System.Windows.Media.Animation.Timeline behaves when it is outside
    //     its active period but its parent is inside its active or hold period.
    public enum FillBehavior
    {
        //
        // Summary:
        //     After it reaches the end of its active period, the timeline holds its progress
        //     until the end of its parent's active and hold periods.
        HoldEnd = 0,
        //
        // Summary:
        //     The timeline stops if it is outside its active period while its parent is inside
        //     its active period.
        Stop = 1
    }
}
#endif
