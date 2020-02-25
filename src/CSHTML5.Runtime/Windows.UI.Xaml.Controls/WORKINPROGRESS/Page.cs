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
        public NavigationService NavigationService { get; }

        private string _title;

        /// <summary>
        /// Gets or sets the name for the page.
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

    }
}

#endif
