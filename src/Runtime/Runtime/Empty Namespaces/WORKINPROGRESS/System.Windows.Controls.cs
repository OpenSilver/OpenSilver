#if WORKINPROGRESS
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;

namespace System.Windows.Controls
{
    public partial class WebBrowser
    {
        public string SaveToString()
        {
            return "";
        }

        public static DependencyProperty SourceProperty
        {
            get; set;
        }

        public System.Uri Source { get; set; }
    }

    public partial class Calendar
    {
        public string SourceName { get; set; }
        public bool IsTabStop { get; set; }
        public System.Windows.Media.Brush Background { get; set; }
        public System.Windows.Thickness Padding { get; set; }
        public System.Windows.Thickness BorderThickness { get; set; }

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

    public partial class DatePicker
    {
        public static System.Windows.DependencyProperty CalendarStyleProperty
        {
            get; set;
        }
    }

    public abstract partial class Control : FrameworkElement
    {
        public int CharacterSpacing { get; set; }
    }

    public class SelectorSelectionAdapter : ISelectionAdapter
    {
        public SelectorSelectionAdapter(Selector s)
        {

        }
        public IEnumerable ItemsSource { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public object SelectedItem { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event SelectionChangedEventHandler SelectionChanged;
        public event RoutedEventHandler Commit;
        public event RoutedEventHandler Cancel;

        public void HandleKeyDown(KeyEventArgs e)
        {
            throw new NotImplementedException();
        }
    }

    public partial class TextBlock : Control
    {
        public FontSource FontSource { get; set; }

        public static DependencyProperty LineHeightProperty
        {
            get; set;
        }
    }

    public partial class AutoCompleteBox : Selector
    {
        virtual protected void OnPopulated(PopulatedEventArgs e)
        {

        }
        public static DependencyProperty ItemFilterProperty
        {
            get; set;
        }
    }

    public partial class PopulatedEventArgs
    {

    }

    public interface ISelectionAdapter
    {
        event SelectionChangedEventHandler SelectionChanged;
        event RoutedEventHandler Commit;
        event RoutedEventHandler Cancel;
        IEnumerable ItemsSource { get; set; }
        Object SelectedItem { get; set; }
        void HandleKeyDown(KeyEventArgs e);
    }

    public delegate void RoutedPropertyChangingEventHandler<T>(Object sender, RoutedPropertyChangingEventArgs<T> e);

    public partial class HeaderedItemsControl : ItemsControl
    {
        public object Header { get; set; }
    }

        public partial class TabControl : System.Windows.Controls.ItemsControl
    {
        public System.Windows.Controls.Dock TabStripPlacement { get; set; }
    }

    public abstract partial class DataGridBoundColumn : System.Windows.Controls.DataGridColumn
    {
        public System.Windows.Style ElementStyle { get; set; }
    }
}
#endif