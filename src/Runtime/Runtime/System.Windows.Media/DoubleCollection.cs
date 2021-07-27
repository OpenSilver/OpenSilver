

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


#if BRIDGE
using System;
#endif
using System.ComponentModel;
using System.Globalization;

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    [TypeConverter(typeof(DoubleCollectionConverter))]
    /// <summary>
    /// Represents an ordered collection of Double values.
    /// </summary>
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

        public override bool Equals(object obj)
        {
            return obj is DoubleCollection;
        }

        internal object ToString(object p, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}