#if WORKINPROGRESS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Windows.Interactivity
{
    public partial class EventTrigger
    {
        public object SourceObject { get; set; }
        public string SourceName { get; set; }

        public static DependencyProperty SourceObjectProperty
        {
            get; set;
        }
    }
}
#endif