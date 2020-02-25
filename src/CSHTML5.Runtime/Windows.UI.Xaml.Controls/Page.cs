
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



using CSHTML5.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
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
    /// <summary>
    /// Encapsulates a page of content that can be navigated to.
    /// </summary>
    [ContentProperty("Content")]
    public partial class Page : UserControl
    {
      
        /// <summary>
        /// Invoked when the Page is loaded and becomes the current source of a parent
        /// Frame.
        /// </summary>
        /// <param name="e">
        /// Event data that can be examined by overriding code. The event data is representative
        /// of the pending navigation that will load the current Page. Usually the most
        /// relevant property to examine is Parameter.
        /// </param>
        protected virtual void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            var div = INTERNAL_HtmlDomManager.CreateDomElementAndAppendIt("div", parentRef, this);
            var divStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(div);
            domElementWhereToPlaceChildren = div;

            divStyle.width = "100%"; //todo: see if there are cases where we do not want this to be 100%
            divStyle.height = "100%"; //todo: see if there are cases where we do not want this to be 100%

            return div;
        }


    }
}
