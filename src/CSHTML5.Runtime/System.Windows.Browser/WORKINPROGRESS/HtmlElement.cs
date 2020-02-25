
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



using System;
using System.Security;

namespace System.Windows.Browser
{
    #if WORKINPROGRESS
    public sealed partial class HtmlElement : HtmlObject
    {
        #region Fields
        private ScriptObjectCollection _children;
        #endregion

        #region Properties
        public ScriptObjectCollection Children
        {
            get { return _children; }
        }
        #endregion

        #region Methods
        // Summary:
        //     Sets the browser focus to the current HTML element.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     All errors.
        [SecuritySafeCritical]
        public void Focus()
        {

        }

        internal HtmlElement()
        {
            _children = null;
        }
        public void AppendChild(HtmlElement @element)
        {
        }
        public void AppendChild(HtmlElement @element, HtmlElement @referenceElement)
        {
        }
        public void RemoveChild(HtmlElement @element)
        {
        }
        public void SetAttribute(string @name, string @value)
        {
        }
        public string GetStyleAttribute(string @name)
        {
            return default(string);
        }
        public void SetStyleAttribute(string @name, string @value)
        {
        }
        #endregion
    }
#endif
}
