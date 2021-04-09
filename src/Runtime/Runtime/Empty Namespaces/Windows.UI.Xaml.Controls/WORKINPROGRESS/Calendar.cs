#if WORKINPROGRESS

using System;

#if MIGRATION
using System.Windows.Media;
#else
using Windows.UI.Xaml.Media;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    public partial class Calendar
    {
        public string SourceName { get; set; }
        public bool IsTabStop { get; set; }
        public Brush Background { get; set; }
        public Thickness Padding { get; set; }
        public Thickness BorderThickness { get; set; }

        public static DependencyProperty IsTabStopProperty
        {
            get; set;
        }

        public static DependencyProperty BackgroundProperty
        {
            get; set;
        }

        public static DependencyProperty PaddingProperty
        {
            get; set;
        }

        public static DependencyProperty BorderBrushProperty
        {
            get; set;
        }

        public static DependencyProperty BorderThicknessProperty
        {
            get; set;
        }

        public static DependencyProperty TemplateProperty
        {
            get; set;
        }
    }
}
#endif