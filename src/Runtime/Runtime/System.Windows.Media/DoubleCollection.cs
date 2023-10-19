
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

using System.Globalization;
using OpenSilver.Internal;

namespace System.Windows.Media
{
    /// <summary>
    /// Represents an ordered collection of Double values.
    /// </summary>
    public sealed class DoubleCollection : PresentationFrameworkCollection<double>
    {
        public DoubleCollection() : base(false) 
        { 
        }

        public static DoubleCollection Parse(string source)
        {
            var db = new DoubleCollection();

            if (source != null)
            {
                IFormatProvider formatProvider = CultureInfo.InvariantCulture;
                char[] separator = new char[2] { TokenizerHelper.GetNumericListSeparator(formatProvider), ' ' };
                string[] split = source.Split(separator, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < split.Length; i++)
                {
                    db.Add(Convert.ToDouble(split[i], formatProvider));
                }
            }

            return db;
        }

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

        internal override double GetItemOverride(int index)
        {
            return this.GetItemInternal(index);
        }

        internal override void SetItemOverride(int index, double value)
        {
            this.SetItemInternal(index, value);
        }
    }
}