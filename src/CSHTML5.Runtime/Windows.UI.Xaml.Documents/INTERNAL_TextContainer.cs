

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


using System;
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
    internal abstract partial class INTERNAL_TextContainer : INTERNAL_ITextContainer
    {
        #region Data
        private readonly DependencyObject _parent;
        #endregion

        #region Constructors
        protected INTERNAL_TextContainer(DependencyObject parent)
        {
            if (parent == null)
            {
                throw new ArgumentNullException("parent");
            }
            this._parent = parent;
        }
        #endregion

        #region Public Properties
        public abstract string Text { get; }
        #endregion

        #region Public Methods
        public virtual void BeginChange()
        {
            //do nothing here
        }

        public virtual void EndChange()
        {
            //do nothing here
        }

        public void OnTextAdded(TextElement textElement)
        {
            this.OnTextAddedOverride(textElement);
        }

        public void OnTextRemoved(TextElement textElement)
        {
            this.OnTextRemovedOverride(textElement);
        }

        protected abstract void OnTextAddedOverride(TextElement textElement);

        protected abstract void OnTextRemovedOverride(TextElement textElement);
        #endregion

        #region Internal Properties
        internal DependencyObject Parent
        {
            get
            {
                return this._parent;
            }
        }
        #endregion
    }

    internal partial interface INTERNAL_ITextContainer
    {
        string Text { get; }
        void BeginChange();
        void EndChange();
        void OnTextAdded(TextElement textElement);
        void OnTextRemoved(TextElement textElement);
    }
}