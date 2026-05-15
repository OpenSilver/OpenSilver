using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TestApplication.Tests
{
    public partial class GeometryBoundsViewer : UserControl
    {
        public GeometryBoundsViewer()
        {
            InitializeComponent();
            DataContext = this;

            Loaded += (o, e) => Bounds = Geometry.Bounds;
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                nameof(Title),
                typeof(string),
                typeof(GeometryBoundsViewer),
                new PropertyMetadata(string.Empty));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty GeometryProperty =
            DependencyProperty.Register(
                nameof(Geometry),
                typeof(Geometry),
                typeof(GeometryBoundsViewer),
                new PropertyMetadata((object)null));

        public Geometry Geometry
        {
            get { return (Geometry)GetValue(GeometryProperty); }
            set { SetValue(GeometryProperty, value); }
        }

        public static readonly DependencyProperty BoundsProperty =
            DependencyProperty.Register(
                nameof(Bounds),
                typeof(Rect),
                typeof(GeometryBoundsViewer),
                new PropertyMetadata(Rect.Empty));

        public Rect Bounds
        {
            get { return (Rect)GetValue(BoundsProperty); }
            set { SetValue(BoundsProperty, value); }
        }
    }
}
