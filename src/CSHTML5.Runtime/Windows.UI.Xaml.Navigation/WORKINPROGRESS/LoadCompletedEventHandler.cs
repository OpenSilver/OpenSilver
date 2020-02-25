using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if WORKINPROGRESS

#if MIGRATION
namespace System.Windows.Navigation
#else
namespace Windows.UI.Xaml.Navigation
#endif
{
    //
    // Summary:
    //     Represents the method that will handle the System.Windows.Controls.WebBrowser.LoadCompleted
    //     event.
    //
    // Parameters:
    //   sender:
    //     The source of the event.
    //
    //   e:
    //     The event data.
    public delegate void LoadCompletedEventHandler(object sender, NavigationEventArgs e);
}

#endif
