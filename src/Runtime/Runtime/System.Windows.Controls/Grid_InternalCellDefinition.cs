

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
        public object DomElement;
        public object RowDomElement;
        public object ColumnDomElement;
        internal string INTERNAL_previousValueOfDisplayCssProperty = "table-cell";
        internal int numberOfChildren = 0;
    }

}
