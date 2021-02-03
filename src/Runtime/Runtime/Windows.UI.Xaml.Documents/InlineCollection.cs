

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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

#if MIGRATION
namespace System.Windows.Documents
#else
namespace Windows.UI.Xaml.Documents
#endif
{
    public partial class InlineCollection : TextElementCollection<Inline>, IList
    {
        #region Constructor
        internal InlineCollection(DependencyObject owner) : base(owner)
        {

        }
        #endregion

        #region Public Methods
        public void Add(string text)
        {
            this.AddInternal(text);
        }
#if WORKINPROGRESS
        public void RemoveAt(int index)
        {

        }
#endif
#endregion

        #region Internal Properties
        internal Inline FirstElement
        {
            get
            {
                if (this.Count == 0)
                {
                    return null; //note: maybe throw an exception ?
                }
                return (Inline)this[0];
            }
        }
        #endregion

        #region Internal Methods
        private void AddInternal(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            Run run = new Run()
            {
                Text = text
            };
            this.Add(run);
        }
        #endregion
    }
}
