
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

namespace System.Windows
{
    /// <summary>
    /// Represents a collection of objects that inherit from <see cref="SetterBase"/>.
    /// </summary>
    public sealed partial class SetterBaseCollection : PresentationFrameworkCollection<SetterBase>
    {
        private bool _sealed;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetterBaseCollection"/> class.
        /// </summary>
        public SetterBaseCollection() : base(false)
        {
        }

        /// <summary>
        ///     Returns the sealed state of this object.  If true, any attempt
        ///     at modifying the state of this object will trigger an exception.
        /// </summary>
        public bool IsSealed
        {
            get
            {
                return _sealed;
            }
        }

        // Note: Even if SetterBase derives from DependencyObject, we don't use
        // the methods that are supposed to handle collections of DependencyObject
        // as we don't want the inheritance context to be propagated to Setters.

        internal override void AddOverride(SetterBase value)
        {
            this.CheckSealed();
            this.AddInternal(value);
        }

        internal override void ClearOverride()
        {
            this.CheckSealed();
            this.ClearInternal();
        }

        internal override void InsertOverride(int index, SetterBase value)
        {
            this.CheckSealed();
            this.InsertInternal(index, value);
        }

        internal override void RemoveAtOverride(int index)
        {
            this.CheckSealed();
            this.RemoveAtInternal(index);
        }

        internal override SetterBase GetItemOverride(int index)
        {
            return this.GetItemInternal(index);
        }

        internal override void SetItemOverride(int index, SetterBase value)
        {
            this.CheckSealed();
            this.SetItemInternal(index, value);
        }

        internal void Seal()
        {
            _sealed = true;

            // Seal all the setters
            for (int i = 0; i < Count; i++)
            {
                this[i].Seal();
            }
        }

        private void CheckSealed()
        {
            if (_sealed)
            {
                throw new InvalidOperationException(string.Format("Cannot modify a '{0}' after it is sealed.", typeof(SetterBaseCollection).Name));
            }
        }
    }
}
