

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
#if FOR_DESIGN_TIME
    [TypeConverter(typeof(DoubleCollectionConverter))]
#endif
    public sealed partial class DoubleCollection : List<double> //: IList<double>, IEnumerable<double>
    {
        static DoubleCollection()
        {
            TypeFromStringConverters.RegisterConverter(typeof(DoubleCollection), INTERNAL_ConvertFromString);
        }

        internal static object INTERNAL_ConvertFromString(string doubleCollectionAsString)
        {
            char separator = ' ';
            if(doubleCollectionAsString.Trim().Contains(","))
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

        //// Summary:
        ////     Initializes a new instance of the DoubleCollection class.
        //public DoubleCollection();

        //int Count { get; }
        //bool IsReadOnly { get; }

        //double this[int index] { get; set; }

        //void Add(double item);
        ////
        //// Summary:
        ////     Removes all items from the collection.
        //void Clear();
        //bool Contains(double item);
        //void CopyTo(double[] array, int arrayIndex);
        //int IndexOf(double item);
        //void Insert(int index, double item);
        //bool Remove(double item);
        //void RemoveAt(int index);
    }
}
