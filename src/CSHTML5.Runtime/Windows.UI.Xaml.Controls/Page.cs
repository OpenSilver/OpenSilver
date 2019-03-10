
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
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
    public class Page : UserControl
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

#if WORKINPROGRESS
        #region Not supported yet

        private string _title;

        /// <summary>
        /// Gets or sets the name for the page.
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        #endregion
#endif
    }
}
