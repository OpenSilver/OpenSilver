
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
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
