#if WORKINPROGRESS
using System;
using System.Collections.Generic;
using System.Text;

namespace System.ComponentModel
{
    public partial struct SortDescription
    {
        public ListSortDirection Direction { get; set; }
        public string PropertyName { get; set; }
    }
}

#endif