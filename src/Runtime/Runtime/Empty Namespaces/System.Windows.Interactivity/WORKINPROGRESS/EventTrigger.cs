#if WORKINPROGRESS

using System;

#if MIGRATION
namespace System.Windows.Interactivity
#else
namespace Windows.UI.Xaml.Input.Interactivity
#endif
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