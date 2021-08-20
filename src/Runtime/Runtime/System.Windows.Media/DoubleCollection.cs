

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
using System.ComponentModel;
using System.Globalization;

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    /// <summary>
    /// Represents an ordered collection of Double values.
    /// </summary>
    [TypeConverter(typeof(DoubleCollectionConverter))]
    public sealed class DoubleCollection : PresentationFrameworkCollection<double>
    {
        #region Constructor

        public DoubleCollection()
        {

        }

        #endregion

        #region Overriden Methods

        internal override void AddOverride(double value)
        {
            this.AddInternal(value);
        }

        internal override void ClearOverride()
        {
            this.ClearInternal();
        }

        internal override void InsertOverride(int index, double value)
        {
            this.InsertInternal(index, value);
        }

        internal override void RemoveAtOverride(int index)
        {
            this.RemoveAtInternal(index);
        }

        internal override bool RemoveOverride(double value)
        {
            return this.RemoveInternal(value);
        }

        internal override double GetItemOverride(int index)
        {
            return this.GetItemInternal(index);
        }

        internal override void SetItemOverride(int index, double value)
        {
            this.SetItemInternal(index, value);
        }

        #endregion
    }
}