using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Navigation;

namespace TestApplication.Tests.Paths
{
    public partial class PathChangeTest : Page
    {
        public PathChangeTest()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        Random rd = new Random();
        private void TestChangePathWidth_Click(object sender, RoutedEventArgs e)
        {
            path1.Width = rd.Next(300);
        }
        private void TestChangePathHeight_Click(object sender, RoutedEventArgs e)
        {
            path1.Height = rd.Next(300);
        }

        private void TestChangePathFill_Click(object sender, RoutedEventArgs e)
        {
            path1.Fill = new SolidColorBrush(new Color() { A = (byte)rd.Next(255), B = (byte)rd.Next(255), G = (byte)rd.Next(255), R = (byte)rd.Next(255) });
        }
        private void TestChangePathStretch_Click(object sender, RoutedEventArgs e)
        {
            int i = rd.Next(4);
            switch (i)
            {
                case 0:
                    path1.Stretch = Stretch.None;
                    break;
                case 1:
                    path1.Stretch = Stretch.Fill;
                    break;
                case 2:
                    path1.Stretch = Stretch.Uniform;
                    break;
                case 3:
                    path1.Stretch = Stretch.UniformToFill;
                    break;
                default:
                    break;
            }
        }
        private void TestChangePathStroke_Click(object sender, RoutedEventArgs e)
        {
            path1.Stroke = new SolidColorBrush(new Color() { A = (byte)rd.Next(255), B = (byte)rd.Next(255), G = (byte)rd.Next(255), R = (byte)rd.Next(255) });
        }
        private void TestChangePathStrokeThickness_Click(object sender, RoutedEventArgs e)
        {
            path1.StrokeThickness = rd.Next(7);
        }

        // SLDISABLED
        //private void RedrawPath_Click(object sender, RoutedEventArgs e)
        //{
        //    path1.Refresh();
        //}

        #region EllipseGeometry
        private void TestEllipseGeometry_Click(object sender, RoutedEventArgs e)
        {
            path1.Data = new EllipseGeometry() { Center = new Point(100, 100), RadiusX = 100, RadiusY = 50 };
            EllipseGeometryButtons.Visibility = Visibility.Visible;
            LineGeometryButtons.Visibility = Visibility.Collapsed;
            PathGeometryButtons.Visibility = Visibility.Collapsed;
            PathGeometrySegmentTypeButtons.Visibility = Visibility.Collapsed;

            PathArcButtons.Visibility = Visibility.Collapsed;
            PathBezierButtons.Visibility = Visibility.Collapsed;
            PathLineButtons.Visibility = Visibility.Collapsed;
            PathPolyBezierButtons.Visibility = Visibility.Collapsed;
            PathPolyLineButtons.Visibility = Visibility.Collapsed;
            PathPolyQuadraticButtons.Visibility = Visibility.Collapsed;
            PathQuadraticButtons.Visibility = Visibility.Collapsed;

        }

        private void TestEllipseCenter_Click(object sender, RoutedEventArgs e)
        {
            EllipseGeometry ellipse = (EllipseGeometry)path1.Data;
            ellipse.Center = new Point(rd.Next(250), rd.Next(150));
        }
        private void TestEllipseRadiusX_Click(object sender, RoutedEventArgs e)
        {
            EllipseGeometry ellipse = (EllipseGeometry)path1.Data;
            ellipse.RadiusX = rd.Next(200);
        }
        private void TestEllipseRadiusY_Click(object sender, RoutedEventArgs e)
        {
            EllipseGeometry ellipse = (EllipseGeometry)path1.Data;
            ellipse.RadiusY = rd.Next(150);
        }
        #endregion

        #region LineGeometry
        private void TestLineGeometry_Click(object sender, RoutedEventArgs e)
        {
            path1.Data = new LineGeometry() { StartPoint = new Point(10, 10), EndPoint = new Point(200, 150) };
            EllipseGeometryButtons.Visibility = Visibility.Collapsed;
            LineGeometryButtons.Visibility = Visibility.Visible;
            PathGeometryButtons.Visibility = Visibility.Collapsed;
            PathGeometrySegmentTypeButtons.Visibility = Visibility.Collapsed;

            PathArcButtons.Visibility = Visibility.Collapsed;
            PathBezierButtons.Visibility = Visibility.Collapsed;
            PathLineButtons.Visibility = Visibility.Collapsed;
            PathPolyBezierButtons.Visibility = Visibility.Collapsed;
            PathPolyLineButtons.Visibility = Visibility.Collapsed;
            PathPolyQuadraticButtons.Visibility = Visibility.Collapsed;
            PathQuadraticButtons.Visibility = Visibility.Collapsed;

        }
        private void TestLineStartPoint_Click(object sender, RoutedEventArgs e)
        {
            LineGeometry line = (LineGeometry)path1.Data;
            line.StartPoint = new Point(rd.Next(290), rd.Next(190));
        }
        private void TestLineEndPoint_Click(object sender, RoutedEventArgs e)
        {
            LineGeometry line = (LineGeometry)path1.Data;
            line.EndPoint = new Point(rd.Next(290), rd.Next(190));
        }
        #endregion

        #region PathGeometry
        ArcSegment arc;
        BezierSegment bezier;
        LineSegment line;
        PolyBezierSegment polyBezier;
        PolyLineSegment polyLine;
        PolyQuadraticBezierSegment polyQuadratic;
        QuadraticBezierSegment quadratic;


        #region generic pathGeometry and PathFigure stuff
        private void TestPathGeometry_Click(object sender, RoutedEventArgs e)
        {
            //PathGeometry pathGeometry = new PathGeometry();
            //PathFigureCollection figures = new PathFigureCollection();
            //PathFigure figure = new PathFigure() { IsClosed = false, IsFilled = false, StartPoint = new Point(10, 10) };
            //figure.Segments = new PathSegmentCollection();

            //arc = new ArcSegment() { IsLargeArc = false, Point = new Point(25, 50), RotationAngle = 35, Size = new Size(100, 25), SweepDirection = SweepDirection.Clockwise };
            //figure.Segments.Add(arc);

            //bezier = new BezierSegment() { Point1 = new Point(100, 0), Point2 = new Point(150, 25), Point3 = new Point(10, 150) };
            //figure.Segments.Add(bezier);

            //line = new LineSegment() { Point = new Point(250,150) };
            //figure.Segments.Add(line);

            //polyBezier = new PolyBezierSegment();
            //PointCollection points = new PointCollection();
            //points.Add(new Point(250, 0));
            //points.Add(new Point(250, 0));
            //points.Add(new Point(150, 200));
            //points.Add(new Point(100, 250));
            //points.Add(new Point(300, 0));
            //points.Add(new Point(50, 100));
            //polyBezier.Points = points;
            //figure.Segments.Add(polyBezier);

            //polyLine = new PolyLineSegment();
            //polyLine.Points = new PointCollection();
            //polyLine.Points.Add(new Point(50, 200));
            //polyLine.Points.Add(new Point(100, 200));
            //figure.Segments.Add(polyLine);

            //polyQuadratic = new PolyQuadraticBezierSegment();
            //polyQuadratic.Points = new PointCollection();
            //polyQuadratic.Points.Add(new Point(50, 100));
            //polyQuadratic.Points.Add(new Point(150, 200));
            //polyQuadratic.Points.Add(new Point(250, 200));
            //polyQuadratic.Points.Add(new Point(150, 100));
            //figure.Segments.Add(polyQuadratic);


            //quadratic = new QuadraticBezierSegment() { Point1 = new Point(0, 0), Point2 = new Point(200, 200) };
            //figure.Segments.Add(quadratic);

            //figures.Add(figure);
            //path1.Data = pathGeometry;
            EllipseGeometryButtons.Visibility = Visibility.Collapsed;
            LineGeometryButtons.Visibility = Visibility.Collapsed;
            PathGeometryButtons.Visibility = Visibility.Visible;
            PathGeometrySegmentTypeButtons.Visibility = Visibility.Visible;
        }
        private void TestPathFillRule_Click(object sender, RoutedEventArgs e)
        {
            PathGeometry pathGeometry = (PathGeometry)path1.Data;
            if (pathGeometry.FillRule == FillRule.EvenOdd)
            {
                pathGeometry.FillRule = FillRule.Nonzero;
            }
            else
            {
                pathGeometry.FillRule = FillRule.EvenOdd;
            }
        }
        private void TestPathIsClosed_Click(object sender, RoutedEventArgs e)
        {
            PathGeometry pathGeometry = (PathGeometry)path1.Data;
            if (pathGeometry.Figures != null && pathGeometry.Figures.Count >= 1)
            {
                PathFigure pathFigure = pathGeometry.Figures[0];
                pathFigure.IsClosed = !pathFigure.IsClosed;
            }
        }
        private void TestPathIsFilled_Click(object sender, RoutedEventArgs e)
        {
            PathGeometry pathGeometry = (PathGeometry)path1.Data;
            if (pathGeometry.Figures != null && pathGeometry.Figures.Count >= 1)
            {
                PathFigure pathFigure = pathGeometry.Figures[0];
                pathFigure.IsFilled = !pathFigure.IsFilled;
            }
        }
        private void TestPathStartPoint_Click(object sender, RoutedEventArgs e)
        {
            PathGeometry pathGeometry = (PathGeometry)path1.Data;
            if (pathGeometry.Figures != null && pathGeometry.Figures.Count >= 1)
            {
                PathFigure pathFigure = pathGeometry.Figures[0];
                pathFigure.StartPoint = new Point(rd.Next(290), rd.Next(190));
            }
        }
        #endregion

        #region ArcSegment
        private void TestPathArc_Click(object sender, RoutedEventArgs e)
        {
            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.Figures = new PathFigureCollection();
            PathFigure figure = new PathFigure() { IsClosed = false, IsFilled = false, StartPoint = new Point(10, 10) };
            figure.Segments = new PathSegmentCollection();

            arc = new ArcSegment() { IsLargeArc = false, Point = new Point(25, 50), RotationAngle = 35, Size = new Size(100, 25), SweepDirection = SweepDirection.Clockwise };
            figure.Segments.Add(arc);

            pathGeometry.Figures.Add(figure);
            path1.Data = pathGeometry;

            PathArcButtons.Visibility = Visibility.Visible;
            PathBezierButtons.Visibility = Visibility.Collapsed;
            PathLineButtons.Visibility = Visibility.Collapsed;
            PathPolyBezierButtons.Visibility = Visibility.Collapsed;
            PathPolyLineButtons.Visibility = Visibility.Collapsed;
            PathPolyQuadraticButtons.Visibility = Visibility.Collapsed;
            PathQuadraticButtons.Visibility = Visibility.Collapsed;
        }
        private void TestArcSegmentIsLargeArc_Click(object sender, RoutedEventArgs e)
        {
            arc.IsLargeArc = !arc.IsLargeArc;
        }
        private void TestArcSegmentPoint_Click(object sender, RoutedEventArgs e)
        {
            arc.Point = new Point(rd.Next(290), rd.Next(190));
        }
        private void TestArcSegmentRotationAngle_Click(object sender, RoutedEventArgs e)
        {
            arc.RotationAngle = (rd.NextDouble() - 0.5) * 720;
        }
        private void TestArcSegmentSize_Click(object sender, RoutedEventArgs e)
        {
            arc.Size = new Size(rd.Next(290), rd.Next(190));
        }
        private void TestArcSegmentSweepDirection_Click(object sender, RoutedEventArgs e)
        {
            if (arc.SweepDirection == SweepDirection.Clockwise)
            {
                arc.SweepDirection = SweepDirection.Counterclockwise;
            }
            else
            {
                arc.SweepDirection = SweepDirection.Clockwise;
            }
        }
        #endregion

        #region BezierSegment
        private void TestPathBezier_Click(object sender, RoutedEventArgs e)
        {
            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.Figures = new PathFigureCollection();
            PathFigure figure = new PathFigure() { IsClosed = false, IsFilled = false, StartPoint = new Point(10, 10) };
            figure.Segments = new PathSegmentCollection();

            bezier = new BezierSegment() { Point1 = new Point(100, 0), Point2 = new Point(150, 25), Point3 = new Point(10, 150) };
            figure.Segments.Add(bezier);

            pathGeometry.Figures.Add(figure);
            path1.Data = pathGeometry;

            PathArcButtons.Visibility = Visibility.Collapsed;
            PathBezierButtons.Visibility = Visibility.Visible;
            PathLineButtons.Visibility = Visibility.Collapsed;
            PathPolyBezierButtons.Visibility = Visibility.Collapsed;
            PathPolyLineButtons.Visibility = Visibility.Collapsed;
            PathPolyQuadraticButtons.Visibility = Visibility.Collapsed;
            PathQuadraticButtons.Visibility = Visibility.Collapsed;
        }
        private void TestBezierPoint1_Click(object sender, RoutedEventArgs e)
        {
            bezier.Point1 = new Point(rd.Next(290), rd.Next(190));
        }
        private void TestBezierPoint2_Click(object sender, RoutedEventArgs e)
        {
            bezier.Point2 = new Point(rd.Next(290), rd.Next(190));
        }
        private void TestBezierPoint3_Click(object sender, RoutedEventArgs e)
        {
            bezier.Point3 = new Point(rd.Next(290), rd.Next(190));
        }
        #endregion

        #region LineSegment
        private void TestPathLine_Click(object sender, RoutedEventArgs e)
        {
            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.Figures = new PathFigureCollection();
            PathFigure figure = new PathFigure() { IsClosed = false, IsFilled = false, StartPoint = new Point(10, 10) };
            figure.Segments = new PathSegmentCollection();

            line = new LineSegment() { Point = new Point(250, 150) };
            figure.Segments.Add(line);

            pathGeometry.Figures.Add(figure);
            path1.Data = pathGeometry;

            PathArcButtons.Visibility = Visibility.Collapsed;
            PathBezierButtons.Visibility = Visibility.Collapsed;
            PathLineButtons.Visibility = Visibility.Visible;
            PathPolyBezierButtons.Visibility = Visibility.Collapsed;
            PathPolyLineButtons.Visibility = Visibility.Collapsed;
            PathPolyQuadraticButtons.Visibility = Visibility.Collapsed;
            PathQuadraticButtons.Visibility = Visibility.Collapsed;
        }
        private void TestLinePoint_Click(object sender, RoutedEventArgs e)
        {
            line.Point = new Point(rd.Next(290), rd.Next(190));
        }
        #endregion

        #region PolyBezierSegment
        private void TestPathPolyBezier_Click(object sender, RoutedEventArgs e)
        {
            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.Figures = new PathFigureCollection();
            PathFigure figure = new PathFigure() { IsClosed = false, IsFilled = false, StartPoint = new Point(10, 10) };
            figure.Segments = new PathSegmentCollection();

            polyBezier = new PolyBezierSegment();
            PointCollection points = new PointCollection();
            points.Add(new Point(250, 0));
            points.Add(new Point(250, 0));
            points.Add(new Point(150, 200));
            points.Add(new Point(100, 250));
            points.Add(new Point(300, 0));
            points.Add(new Point(50, 100));
            polyBezier.Points = points;
            figure.Segments.Add(polyBezier);

            pathGeometry.Figures.Add(figure);
            path1.Data = pathGeometry;

            PathArcButtons.Visibility = Visibility.Collapsed;
            PathBezierButtons.Visibility = Visibility.Collapsed;
            PathLineButtons.Visibility = Visibility.Collapsed;
            PathPolyBezierButtons.Visibility = Visibility.Visible;
            PathPolyLineButtons.Visibility = Visibility.Collapsed;
            PathPolyQuadraticButtons.Visibility = Visibility.Collapsed;
            PathQuadraticButtons.Visibility = Visibility.Collapsed;
        }
        private void TestPolyBezierSegmentPoints_Click(object sender, RoutedEventArgs e)
        {
            PointCollection collection = new PointCollection();
            int amountOfPoints = rd.Next(6);
            while (amountOfPoints > 0)
            {
                collection.Add(new Point(rd.Next(290), rd.Next(190)));
                collection.Add(new Point(rd.Next(290), rd.Next(190)));
                collection.Add(new Point(rd.Next(290), rd.Next(190)));
                --amountOfPoints;
            }
            polyBezier.Points = collection;
        }
        #endregion

        #region PolyLineSegment
        private void TestPathPolyLine_Click(object sender, RoutedEventArgs e)
        {
            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.Figures = new PathFigureCollection();
            PathFigure figure = new PathFigure() { IsClosed = false, IsFilled = false, StartPoint = new Point(10, 10) };
            figure.Segments = new PathSegmentCollection();

            polyLine = new PolyLineSegment();
            polyLine.Points = new PointCollection();
            polyLine.Points.Add(new Point(50, 200));
            polyLine.Points.Add(new Point(100, 200));
            figure.Segments.Add(polyLine);

            pathGeometry.Figures.Add(figure);
            path1.Data = pathGeometry;

            PathArcButtons.Visibility = Visibility.Collapsed;
            PathBezierButtons.Visibility = Visibility.Collapsed;
            PathLineButtons.Visibility = Visibility.Collapsed;
            PathPolyBezierButtons.Visibility = Visibility.Collapsed;
            PathPolyLineButtons.Visibility = Visibility.Visible;
            PathPolyQuadraticButtons.Visibility = Visibility.Collapsed;
            PathQuadraticButtons.Visibility = Visibility.Collapsed;
        }
        private void TestPolyLineSegmentPoints_Click(object sender, RoutedEventArgs e)
        {
            PointCollection collection = new PointCollection();
            int amountOfPoints = rd.Next(6);
            while (amountOfPoints > 0)
            {
                collection.Add(new Point(rd.Next(290), rd.Next(190)));
                --amountOfPoints;
            }
            polyLine.Points = collection;
        }
        #endregion

        #region PolyQuadraticBezierSegment
        private void TestPathPolyQuadratic_Click(object sender, RoutedEventArgs e)
        {
            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.Figures = new PathFigureCollection();
            PathFigure figure = new PathFigure() { IsClosed = false, IsFilled = false, StartPoint = new Point(10, 10) };
            figure.Segments = new PathSegmentCollection();

            polyQuadratic = new PolyQuadraticBezierSegment();
            polyQuadratic.Points = new PointCollection();
            polyQuadratic.Points.Add(new Point(50, 100));
            polyQuadratic.Points.Add(new Point(150, 200));
            polyQuadratic.Points.Add(new Point(250, 200));
            polyQuadratic.Points.Add(new Point(150, 100));
            figure.Segments.Add(polyQuadratic);

            pathGeometry.Figures.Add(figure);
            path1.Data = pathGeometry;

            PathArcButtons.Visibility = Visibility.Collapsed;
            PathBezierButtons.Visibility = Visibility.Collapsed;
            PathLineButtons.Visibility = Visibility.Collapsed;
            PathPolyBezierButtons.Visibility = Visibility.Collapsed;
            PathPolyLineButtons.Visibility = Visibility.Collapsed;
            PathPolyQuadraticButtons.Visibility = Visibility.Visible;
            PathQuadraticButtons.Visibility = Visibility.Collapsed;
        }

        private void TestPolyQuadraticBezierSegmentPoints_Click(object sender, RoutedEventArgs e)
        {
            PointCollection collection = new PointCollection();
            int amountOfPoints = rd.Next(6);
            while (amountOfPoints > 0)
            {
                collection.Add(new Point(rd.Next(290), rd.Next(190)));
                collection.Add(new Point(rd.Next(290), rd.Next(190)));
                --amountOfPoints;
            }
            polyQuadratic.Points = collection;
        }
        #endregion

        #region QuadraticBezierSegment
        private void TestPathQuadratic_Click(object sender, RoutedEventArgs e)
        {
            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.Figures = new PathFigureCollection();
            PathFigure figure = new PathFigure() { IsClosed = false, IsFilled = false, StartPoint = new Point(10, 10) };
            figure.Segments = new PathSegmentCollection();

            quadratic = new QuadraticBezierSegment() { Point1 = new Point(0, 0), Point2 = new Point(200, 200) };
            figure.Segments.Add(quadratic);

            pathGeometry.Figures.Add(figure);
            path1.Data = pathGeometry;

            PathArcButtons.Visibility = Visibility.Collapsed;
            PathBezierButtons.Visibility = Visibility.Collapsed;
            PathLineButtons.Visibility = Visibility.Collapsed;
            PathPolyBezierButtons.Visibility = Visibility.Collapsed;
            PathPolyLineButtons.Visibility = Visibility.Collapsed;
            PathPolyQuadraticButtons.Visibility = Visibility.Collapsed;
            PathQuadraticButtons.Visibility = Visibility.Visible;
        }

        private void TestQuadraticBezierPoint1_Click(object sender, RoutedEventArgs e)
        {
            quadratic.Point1 = new Point(rd.Next(290), rd.Next(190));
        }
        private void TestQuadraticBezierPoint2_Click(object sender, RoutedEventArgs e)
        {
            quadratic.Point2 = new Point(rd.Next(290), rd.Next(190));
        }

        #endregion
        #endregion
    }
}
