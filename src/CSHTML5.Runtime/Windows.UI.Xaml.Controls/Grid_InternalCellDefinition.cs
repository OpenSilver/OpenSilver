
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
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    class INTERNAL_CellDefinition
    {
        public int RowSpan = 1;
        public int ColumnSpan = 1;
        public int Row = 0;
        public int Column = 0;
        public bool IsOverlapped = false; // Means that another cell is overlapping this cell due to the "ColumnSpan" or "RowSpan".
        public bool IsOccupied = false;
        public INTERNAL_CellDefinition ParentCell;
        public dynamic DomElement;
        public dynamic RowDomElement;
        public dynamic ColumnDomElement;
        internal string INTERNAL_previousValueOfDisplayCssProperty = "table-cell";
        internal int numberOfChildren = 0;
    }

}
