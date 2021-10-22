

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
        
        internal void INTERNAL_OnNavigatedTo(NavigationEventArgs e) => OnNavigatedTo(e);

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
