
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
    public sealed class DoubleCollection : List<double> //: IList<double>, IEnumerable<double>
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
