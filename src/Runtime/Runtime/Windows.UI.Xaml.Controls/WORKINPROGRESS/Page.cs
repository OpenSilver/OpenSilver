

/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/


#if WORKINPROGRESS

#if MIGRATION
using System.Windows.Navigation;
#else
using Windows.UI.Xaml.Navigation;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    public partial class Page : UserControl
    {
        [OpenSilver.NotImplemented]
        public NavigationService NavigationService { get; }

        private string _title;

        /// <summary>
        /// Gets or sets the name for the page.
        /// </summary>
        [OpenSilver.NotImplemented]
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        [OpenSilver.NotImplemented]
        public NavigationContext NavigationContext { get; }

        /// <summary>
        /// Called just before a page is no longer the active page in a frame.
        /// </summary>
        /// <param name="e">
        /// An object that contains the event data.
        /// </param>
        [OpenSilver.NotImplemented]
        protected virtual void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
        }

        /// <summary>
        /// Called when a page is no longer the active page in a frame. 
        /// </summary>
        /// <param name="e">
        /// An object that contains the event data.
        /// </param>
        [OpenSilver.NotImplemented]
        protected virtual void OnNavigatedFrom(NavigationEventArgs e)
        {
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            return base.ArrangeOverride(finalSize);
        }
    }
}

#endif
