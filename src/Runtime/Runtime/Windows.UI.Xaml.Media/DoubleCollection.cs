

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
using DotNetForHtml5.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    /// <summary>
    /// Represents an ordered collection of Double values.
    /// </summary>
    public sealed class DoubleCollection : PresentationFrameworkCollection<double>
    {
        #region Constructor

        static DoubleCollection()
        {
            TypeFromStringConverters.RegisterConverter(typeof(DoubleCollection), INTERNAL_ConvertFromString);
        }

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

        internal static object INTERNAL_ConvertFromString(string doubleCollectionAsString)
        {
            char separator = ' ';
            if (doubleCollectionAsString.Trim().Contains(","))
            {
                separator = ',';
            }
            string[] split = doubleCollectionAsString.Split(separator);
            DoubleCollection doubleCollection = new DoubleCollection();
            foreach (string element in split)
            {
                if (!string.IsNullOrWhiteSpace(element))
                {
                    doubleCollection.Add(double.Parse(element));
                }
            }
            return doubleCollection;
        }

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