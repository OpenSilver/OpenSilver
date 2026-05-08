using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace TestApplication.Tests
{
    public partial class CombinedGeometryViewer : UserControl
    {
        public CombinedGeometryViewer()
        {
            InitializeComponent();
            DataContext = this;
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                nameof(Title),
                typeof(string),
                typeof(CombinedGeometryViewer),
                new PropertyMetadata(string.Empty));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty ShowResultProperty =
            DependencyProperty.Register(
                nameof(ShowResult),
                typeof(bool),
                typeof(CombinedGeometryViewer),
                new PropertyMetadata(true));

        public bool ShowResult
        {
            get { return (bool)GetValue(ShowResultProperty); }
            set { SetValue(ShowResultProperty, value); }
        }

        public static readonly DependencyProperty Geometry1Property =
            DependencyProperty.Register(
                nameof(Geometry1),
                typeof(Geometry),
                typeof(CombinedGeometryViewer),
                new PropertyMetadata((object)null));

        public Geometry Geometry1
        {
            get { return (Geometry)GetValue(Geometry1Property); }
            set { SetValue(Geometry1Property, value); }
        }

        public static readonly DependencyProperty Geometry2Property =
            DependencyProperty.Register(
                nameof(Geometry2),
                typeof(Geometry),
                typeof(CombinedGeometryViewer),
                new PropertyMetadata((object)null));

        public Geometry Geometry2
        {
            get { return (Geometry)GetValue(Geometry2Property); }
            set { SetValue(Geometry2Property, value); }
        }

        public static readonly DependencyProperty GeometryCombineModeProperty =
            DependencyProperty.Register(
                nameof(GeometryCombineMode),
                typeof(GeometryCombineMode),
                typeof(CombinedGeometryViewer),
                new PropertyMetadata(GeometryCombineMode.Union));

        public GeometryCombineMode GeometryCombineMode
        {
            get { return (GeometryCombineMode)GetValue(GeometryCombineModeProperty); }
            set { SetValue(GeometryCombineModeProperty, value); }
        }

        public static readonly DependencyProperty TransformProperty =
            DependencyProperty.Register(
                nameof(Transform),
                typeof(Transform),
                typeof(CombinedGeometryViewer),
                new PropertyMetadata(Transform.Identity));

        public Transform Transform
        {
            get { return (Transform)GetValue(TransformProperty); }
            set { SetValue(TransformProperty, value); }
        }

        private void Toggle_Click(object sender, MouseButtonEventArgs e)
        {
            ShowResult = !ShowResult;
        }
    }

    public sealed class BooleanToAnyConverter : IValueConverter
    {
        public object TrueValue { get; set; }

        public object FalseValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool b = value is bool v && v;
            return b ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
