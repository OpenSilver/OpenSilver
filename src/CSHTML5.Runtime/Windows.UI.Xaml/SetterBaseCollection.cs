

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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// Represents a collection of objects that inherit from System.Windows.SetterBase.
    /// </summary>
    public sealed partial class SetterBaseCollection : PresentationFrameworkCollection<SetterBase>
    {
        #region Data

        private bool _sealed;

        #endregion Data

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the System.Windows.SetterBaseCollection class.
        /// </summary>
        public SetterBaseCollection()
        {

        }

        #endregion

        #region Public Properties

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

        #endregion

        #region Overriden Methods

        // Note: Even if SetterBase derives from DependencyObject, we don't use
        // the methods that are supposed to handle collections of DependencyObject
        // as we don't want the inheritance context to be propagated to Setters.

        internal override void AddOverride(SetterBase value)
        {
            this.CheckSealed();
            this.SetterBaseValidation(value);
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
            this.SetterBaseValidation(value);
            this.InsertInternal(index, value);
        }

        internal override void RemoveAtOverride(int index)
        {
            this.CheckSealed();
            this.RemoveAtInternal(index);
        }

        internal override bool RemoveOverride(SetterBase value)
        {
            this.CheckSealed();
            this.SetterBaseValidation(value);
            return this.RemoveInternal(value);
        }

        internal override void SetItemOverride(int index, SetterBase value)
        {
            this.CheckSealed();
            this.SetterBaseValidation(value);
            this.SetItemInternal(index, value);
        }

        #endregion

        #region Internal Methods

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
                throw new InvalidOperationException(string.Format("Cannot modify a '{0}' after it is sealed.", "SetterBaseCollection"));
            }
        }

        private void SetterBaseValidation(SetterBase setterBase)
        {
            if (setterBase == null)
            {
                throw new ArgumentNullException("setterBase");
            }
        }

        #endregion
    }
}
