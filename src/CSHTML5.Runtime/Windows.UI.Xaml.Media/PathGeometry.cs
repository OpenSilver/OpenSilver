
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



using CSHTML5.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
#if MIGRATION
using System.Windows.Shapes;
#else
using Windows.UI.Xaml.Shapes;
using Windows.Foundation;
#endif

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    /// <summary>
    /// Represents a complex shape that may be composed of arcs, curves, ellipses,
    /// lines, and rectangles.
    /// </summary>
    [ContentProperty("Figures")]
    public sealed class PathGeometry : Geometry
    {
        bool _messageAboutArcsAlreadyShown = false;
        // Note: the following lines were commented because they are not supported by the Simulator when running the Bridge-based version. We took the opportunity to use a HashSet instead which provides faster lookup.
        //char[] _commandCharacters = { 'm', 'M', 'l', 'L', 'h', 'H', 'v', 'V', 'c', 'C', 's', 'S', 'q', 'Q', 't', 'T', 'a', 'A', 'z', 'Z' };
        //char[] _numberCharacters = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '.', '+', '-', 'E', 'e' };
        static HashSet2<char> _commandCharacters;
        static HashSet2<char> _numberCharacters;
        double _strokeThickness;

        static PathGeometry()
        {
            _commandCharacters = new HashSet2<char>();
            _commandCharacters.Add('m');
            _commandCharacters.Add('M');
            _commandCharacters.Add('l');
            _commandCharacters.Add('L');
            _commandCharacters.Add('h');
            _commandCharacters.Add('H');
            _commandCharacters.Add('v');
            _commandCharacters.Add('V');
            _commandCharacters.Add('c');
            _commandCharacters.Add('C');
            _commandCharacters.Add('s');
            _commandCharacters.Add('S');
            _commandCharacters.Add('q');
            _commandCharacters.Add('Q');
            _commandCharacters.Add('t');
            _commandCharacters.Add('T');
            _commandCharacters.Add('a');
            _commandCharacters.Add('A');
            _commandCharacters.Add('z');
            _commandCharacters.Add('Z');

            _numberCharacters = new HashSet2<char>();
            _numberCharacters.Add('0');
            _numberCharacters.Add('1');
            _numberCharacters.Add('2');
            _numberCharacters.Add('3');
            _numberCharacters.Add('4');
            _numberCharacters.Add('5');
            _numberCharacters.Add('6');
            _numberCharacters.Add('7');
            _numberCharacters.Add('8');
            _numberCharacters.Add('9');
            _numberCharacters.Add('.');
            _numberCharacters.Add('+');
            _numberCharacters.Add('-');
            _numberCharacters.Add('E');
            _numberCharacters.Add('e');
        }

        internal override void SetParentPath(Path path)
        {
            INTERNAL_parentPath = path;
            Figures.SetParentPath(path);
        }


        ///// <summary>
        ///// Initializes a new instance of the PathGeometry class.
        ///// </summary>
        //public PathGeometry();

        // Returns:
        //     A collection of PathFigure objects that describe the contents of a path.
        //     Each individual PathFigure describes a shape.
        /// <summary>
        /// Gets or sets the collection of PathFigure objects that describe the contents
        /// of a path.
        /// </summary>
        public PathFigureCollection Figures
        {
            get
            {
                PathFigureCollection collection = (PathFigureCollection)GetValue(FiguresProperty);
                if (collection == null)
                {
                    collection = new PathFigureCollection();
                    SetValue(FiguresProperty, collection);
                }
                return collection;
            }
            set { SetValue(FiguresProperty, value); }
        }
        /// <summary>
        /// Identifies the Figures dependency property.
        /// </summary>
        public static readonly DependencyProperty FiguresProperty =
            DependencyProperty.Register("Figures", typeof(PathFigureCollection), typeof(PathGeometry), new PropertyMetadata(null, Figures_Changed) { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.Never });

        private static void Figures_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //todo: find a way to know when the points changed in the collection
            PathGeometry geometry = (PathGeometry)d;
            PathFigureCollection oldCollection = (PathFigureCollection)e.OldValue;
            PathFigureCollection newCollection = (PathFigureCollection)e.NewValue;
            if (oldCollection != newCollection)
            {
                if (oldCollection != null)
                {
                    oldCollection.SetParentPath(null);
                }
                if (geometry.INTERNAL_parentPath != null && geometry.INTERNAL_parentPath._isLoaded)
                {
                    if (newCollection != null)
                    {
                        newCollection.SetParentPath(geometry.INTERNAL_parentPath);
                    }

                    geometry.INTERNAL_parentPath.ScheduleRedraw();
                }
            }
        }


        /// <summary>
        /// Gets or sets a value that determines how the intersecting areas contained
        /// in the PathGeometry are combined.
        /// </summary>
        public FillRule FillRule
        {
            get { return (FillRule)GetValue(FillRuleProperty); }
            set { SetValue(FillRuleProperty, value); }
        }
        /// <summary>
        /// Identifies the FillRule dependency property.
        /// </summary>
        public static readonly DependencyProperty FillRuleProperty =
            DependencyProperty.Register("FillRule", typeof(FillRule), typeof(PathGeometry), new PropertyMetadata(FillRule.EvenOdd, FillRule_Changed) { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.Never });

        private static void FillRule_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PathGeometry geometry = (PathGeometry)d;
            if (e.NewValue != e.OldValue && geometry.INTERNAL_parentPath != null && geometry.INTERNAL_parentPath._isLoaded)
            {
                geometry.INTERNAL_parentPath.ScheduleRedraw();
            }
        }

        internal override string GetFillRuleAsString()
        {
            return (FillRule == FillRule.Nonzero) ? "nonzero" : "evenodd";
        }

        private void ParseString(string pathAsString)
        {
            //TODO: in this, make a list of the actions to execute on the context, which will be kept. For performance, do not create the Figures while parsing, it should only be made when the user tries to access them.
            int currentIndex = 0;
            int length = pathAsString.Length;
            Point lastAbsolutePosition = new Point();
            Point lastStartAbsolutePosition = new Point();
            bool lastIsCubicBezier = false;
            bool lastIsQuadraticBezier = false;
            Point lastBezierControlPoint = new Point();
            PathFigure currentFigure = null;
            char c = ' ';
            Figures = new PathFigureCollection();

            while (currentIndex < length)
            {
                while (currentIndex < length && ((c = pathAsString[currentIndex]) == ' '))
                {
                    currentIndex++;
                }
                currentIndex++;
                bool relative = char.IsLower(c);

#if !BRIDGE
                switch (char.ToUpperInvariant(c)) //ToUpperInvariant so that we can handle both uppercase and lowercase in the same case in the switch.
#else
                switch (char.ToUpper(c)) //BRIDGETODO : verify this code matchs the code above
#endif
                {
                    case 'F':
                        c = pathAsString[currentIndex];
                        if (c == '0')
                            FillRule = FillRule.EvenOdd;
                        else if (c == '1')
                            FillRule = FillRule.Nonzero;
                        else
                            FillRule = FillRule.EvenOdd;

                        currentIndex++;
                        c = pathAsString[currentIndex];
                        break;
                    case 'M': //move the start of a path figure to a specified position

                        if (currentFigure != null)
                        {
                            Figures.Add(currentFigure);
                        }

                        //we get the point for the move command:
                        Point point = GetPoint(pathAsString, currentIndex, out currentIndex);
                        if (relative)
                        {
                            //we translate the relative coordinates into absolute coordinates:
                            point.X = point.X + lastAbsolutePosition.X;
                            point.Y = point.Y + lastAbsolutePosition.Y;
                        }

                        //we remember this figure's starting position in case it needs to be closed:
                        lastStartAbsolutePosition.X = point.X;
                        lastStartAbsolutePosition.Y = point.Y;

                        //we create the new figure, which will start from the given point, and add it to this.Figures
                        currentFigure = new PathFigure();
                        currentFigure.Segments = new PathSegmentCollection();
                        currentFigure.StartPoint = point;

                        //we remember the last position for the case where the next command is in relative coordinates.
                        lastAbsolutePosition.X = point.X;
                        lastAbsolutePosition.Y = point.Y;

                        //if this is followed by points, we should make lines to these:
                        ReadLines(pathAsString, ref currentIndex, ref lastAbsolutePosition, currentFigure, relative);

                        lastIsCubicBezier = false;
                        lastIsQuadraticBezier = false;


                        break;
                    case 'L': //line to a specified position
                        if (currentFigure == null) //todo: remove all these tests (I assumed a move command was necessary but it apparently isn't)
                        {
                            throw new FormatException("Path badly formatted: a move command is required before any draw command.");
                        }
                        ReadLines(pathAsString, ref currentIndex, ref lastAbsolutePosition, currentFigure, relative);
                        lastIsCubicBezier = false;
                        lastIsQuadraticBezier = false;
                        break;
                    case 'H': //horizontal line to a specified position
                        if (currentFigure == null)
                        {
                            throw new FormatException("Path badly formatted: a move command is required before any draw command.");
                        }
                        List<double> numbersForH = ReadNumbers(pathAsString, ref currentIndex);
                        if (numbersForH.Count == 0)
                        {
                            throw new FormatException("String Badly formatted: you cannot have the H or h draw command followed with no numbers.");
                        }
                        PolyLineSegment polyLineSegmentForH = new PolyLineSegment();
                        polyLineSegmentForH.Points = new PointCollection();
                        double Y = lastAbsolutePosition.Y;
                        foreach (double number in numbersForH)
                        {
                            double newX = number;
                            if (relative)
                            {
                                newX += lastAbsolutePosition.X;
                            }
                            polyLineSegmentForH.Points.Add(new Point(newX, Y));
                            lastAbsolutePosition.X = newX;
                        }
                        currentFigure.Segments.Add(polyLineSegmentForH);

                        lastIsCubicBezier = false;
                        lastIsQuadraticBezier = false;
                        break;
                    case 'V': //vertical line to a specified position
                        if (currentFigure == null)
                        {
                            throw new FormatException("Path badly formatted: a move command is required before any draw command.");
                        }
                        List<double> numbersForV = ReadNumbers(pathAsString, ref currentIndex);
                        if (numbersForV.Count == 0)
                        {
                            throw new FormatException("String Badly formatted: you cannot have the H or h draw command followed with no numbers.");
                        }
                        PolyLineSegment polyLineSegmentForV = new PolyLineSegment();
                        polyLineSegmentForV.Points = new PointCollection();
                        double X = lastAbsolutePosition.X;
                        foreach (double number in numbersForV)
                        {
                            double newY = number;
                            if (relative)
                            {
                                newY += lastAbsolutePosition.Y;
                            }
                            polyLineSegmentForV.Points.Add(new Point(X, newY));
                            lastAbsolutePosition.Y = newY;
                        }
                        currentFigure.Segments.Add(polyLineSegmentForV);
                        lastIsCubicBezier = false;
                        lastIsQuadraticBezier = false;
                        break;
                    case 'C': //cubic bezier curve
                        while (true)
                        {
                            while (currentIndex < length && pathAsString[currentIndex] == ' ')
                            {
                                ++currentIndex;
                            }
                            if (currentIndex < length && !_commandCharacters.Contains(pathAsString[currentIndex]))
                            {
                                Point controlPoint1 = GetPoint(pathAsString, currentIndex, out currentIndex);
                                Point controlPoint2 = GetPoint(pathAsString, currentIndex, out currentIndex);
                                Point endPoint = GetPoint(pathAsString, currentIndex, out currentIndex);
                                if (relative)
                                {
                                    controlPoint1.X += lastAbsolutePosition.X;
                                    controlPoint1.Y += lastAbsolutePosition.Y;
                                    controlPoint2.X += lastAbsolutePosition.X;
                                    controlPoint2.Y += lastAbsolutePosition.Y;
                                    endPoint.X += lastAbsolutePosition.X;
                                    endPoint.Y += lastAbsolutePosition.Y;
                                }
                                BezierSegment bezierSegment = new BezierSegment(); //note: we do not use polyBezierSegment yet because I don't really know how it works.
                                bezierSegment.Point1 = controlPoint1;
                                bezierSegment.Point2 = controlPoint2;
                                bezierSegment.Point3 = endPoint;
                                lastBezierControlPoint = controlPoint2;
                                lastIsCubicBezier = true;
                                currentFigure.Segments.Add(bezierSegment);
                                lastAbsolutePosition.X = endPoint.X;
                                lastAbsolutePosition.Y = endPoint.Y;
                            }
                            else
                            {
                                break;
                            }
                        }
                        lastIsQuadraticBezier = false;
                        break;
                    case 'S': //smooth cubic bezier curve
                        while (true)
                        {
                            while (currentIndex < length && pathAsString[currentIndex] == ' ')
                            {
                                ++currentIndex;
                            }
                            if (currentIndex < length && !_commandCharacters.Contains(pathAsString[currentIndex]))
                            {
                                Point controlPoint1 = new Point();
                                if (!lastIsCubicBezier)
                                {
                                    controlPoint1 = new Point(lastAbsolutePosition.X, lastAbsolutePosition.Y);
                                }
                                else
                                {
                                    Point diffPoint = new Point(lastAbsolutePosition.X - lastBezierControlPoint.X, lastAbsolutePosition.Y - lastBezierControlPoint.Y);
                                    controlPoint1 = new Point(lastAbsolutePosition.X + diffPoint.X, lastAbsolutePosition.Y + diffPoint.Y);
                                }
                                Point controlPoint2 = GetPoint(pathAsString, currentIndex, out currentIndex);
                                Point endPoint = GetPoint(pathAsString, currentIndex, out currentIndex);
                                if (relative)
                                {
                                    controlPoint2.X += lastAbsolutePosition.X;
                                    controlPoint2.Y += lastAbsolutePosition.Y;
                                    endPoint.X += lastAbsolutePosition.X;
                                    endPoint.Y += lastAbsolutePosition.Y;
                                }
                                BezierSegment bezierSegment = new BezierSegment(); //note: we do not use polyBezierSegment yet because I don't really know how it works.
                                bezierSegment.Point1 = controlPoint1;
                                bezierSegment.Point2 = controlPoint2;
                                bezierSegment.Point3 = endPoint;
                                lastBezierControlPoint = controlPoint2;
                                lastIsCubicBezier = true;
                                currentFigure.Segments.Add(bezierSegment);
                                lastAbsolutePosition.X = endPoint.X;
                                lastAbsolutePosition.Y = endPoint.Y;
                            }
                            else
                            {
                                break;
                            }
                        }
                        lastIsQuadraticBezier = false;
                        break;
                    case 'Q': //quadratic bezier
                        while (true)
                        {
                            while (currentIndex < length && pathAsString[currentIndex] == ' ')
                            {
                                ++currentIndex;
                            }
                            if (currentIndex < length && !_commandCharacters.Contains(pathAsString[currentIndex]))
                            {
                                Point controlPoint1 = GetPoint(pathAsString, currentIndex, out currentIndex);

                                Point endPoint = GetPoint(pathAsString, currentIndex, out currentIndex);
                                if (relative)
                                {
                                    controlPoint1.X += lastAbsolutePosition.X;
                                    controlPoint1.Y += lastAbsolutePosition.Y;
                                    endPoint.X += lastAbsolutePosition.X;
                                    endPoint.Y += lastAbsolutePosition.Y;
                                }
                                QuadraticBezierSegment bezierSegment = new QuadraticBezierSegment();
                                bezierSegment.Point1 = controlPoint1;
                                bezierSegment.Point2 = endPoint;
                                currentFigure.Segments.Add(bezierSegment);
                                lastAbsolutePosition.X = endPoint.X;
                                lastAbsolutePosition.Y = endPoint.Y;
                                //we remember the position if the next segment is a smooth quadratic bezier segment.
                                lastBezierControlPoint.X = controlPoint1.X;
                                lastBezierControlPoint.Y = controlPoint1.Y;
                                lastIsCubicBezier = false;
                                lastIsQuadraticBezier = true;
                            }
                            else
                            {
                                break;
                            }
                        }
                        lastIsCubicBezier = false;
                        break;
                    case 'T': //smooth quadratic bezier curve
                        while (true)
                        {
                            while (currentIndex < length && pathAsString[currentIndex] == ' ')
                            {
                                ++currentIndex;
                            }
                            if (currentIndex < length && !_commandCharacters.Contains(pathAsString[currentIndex]))
                            {
                                Point controlPoint1 = new Point();
                                if (!lastIsQuadraticBezier)
                                {
                                    controlPoint1 = new Point(lastAbsolutePosition.X, lastAbsolutePosition.Y);
                                }
                                else
                                {
                                    Point diffPoint = new Point(lastAbsolutePosition.X - lastBezierControlPoint.X, lastAbsolutePosition.Y - lastBezierControlPoint.Y);
                                    controlPoint1 = new Point(lastAbsolutePosition.X + diffPoint.X, lastAbsolutePosition.Y + diffPoint.Y);
                                }
                                Point endPoint = GetPoint(pathAsString, currentIndex, out currentIndex);
                                if (relative)
                                {
                                    endPoint.X += lastAbsolutePosition.X;
                                    endPoint.Y += lastAbsolutePosition.Y;
                                }
                                QuadraticBezierSegment bezierSegment = new QuadraticBezierSegment();
                                bezierSegment.Point1 = controlPoint1;
                                bezierSegment.Point2 = endPoint;
                                currentFigure.Segments.Add(bezierSegment);
                                lastAbsolutePosition.X = endPoint.X;
                                lastAbsolutePosition.Y = endPoint.Y;

                                //we remember the position if the next segment is a smooth quadratic bezier segment.
                                lastBezierControlPoint.X = controlPoint1.X;
                                lastBezierControlPoint.Y = controlPoint1.Y;
                                lastIsCubicBezier = false;
                                lastIsQuadraticBezier = true;
                            }
                            else
                            {
                                break;
                            }
                        }
                        lastIsCubicBezier = false;
                        break;
                    case 'A':
                        //we ignore this one since we cannot handle it yet
                        //todo: remove the following once arcs will be supported:
                        //if (!CSharpXamlForHtml5.Environment.IsRunningInJavaScript)
                        //{
                        //    if (!_messageAboutArcsAlreadyShown)
                        //    {
                        //        //MessageBox.Show("Arcs are not supported yet. It has been replaced with a line.");
                        //        global::System.Diagnostics.Debug.WriteLine("Arcs are not supported yet. The arc has been replaced with a line.");
                        //        _messageAboutArcsAlreadyShown = true;
                        //    }
                        //}

                        while (true)
                        {
                            while (currentIndex < length && pathAsString[currentIndex] == ' ')
                            {
                                ++currentIndex;
                            }
                            if (currentIndex < length && !_commandCharacters.Contains(pathAsString[currentIndex]))
                            {
                                ArcSegment arc = new ArcSegment();

                                double pointForArcX = GetNextNumber(pathAsString, currentIndex, out currentIndex);
                                double pointForArcY = GetNextNumber(pathAsString, currentIndex, out currentIndex);
                                arc.Size = new Size(pointForArcX, pointForArcY);

                                arc.RotationAngle = GetNextNumber(pathAsString, currentIndex, out currentIndex);
                                arc.IsLargeArc = GetNextNumber(pathAsString, currentIndex, out currentIndex) == 1 ? true : false;
                                arc.SweepDirection = GetNextNumber(pathAsString, currentIndex, out currentIndex) == 1 ? SweepDirection.Clockwise : SweepDirection.Counterclockwise;

                                pointForArcX = GetNextNumber(pathAsString, currentIndex, out currentIndex);
                                pointForArcY = GetNextNumber(pathAsString, currentIndex, out currentIndex);
                                Point endPoint = new Point(pointForArcX, pointForArcY);
                                if (relative)
                                {
                                    endPoint.X += lastAbsolutePosition.X;
                                    endPoint.Y += lastAbsolutePosition.Y;
                                }
                                arc.Point = endPoint;
                                lastAbsolutePosition.X = endPoint.X;
                                lastAbsolutePosition.Y = endPoint.Y;
                                ////todo: remove the following lines and add the arc itself to the figure, instead of the replacement line.
                                //LineSegment replacementLine = new LineSegment();
                                //replacementLine.Point = endPoint;


                                //currentFigure.Segments.Add(replacementLine);
                                currentFigure.Segments.Add(arc);
                            }
                            else
                            {
                                break;
                            }
                        }
                        lastIsCubicBezier = false;
                        lastIsQuadraticBezier = false;
                        break;
                    case 'Z':
                        currentFigure.IsClosed = true;
                        lastAbsolutePosition.X = lastStartAbsolutePosition.X;
                        lastAbsolutePosition.Y = lastStartAbsolutePosition.Y;
                        Figures.Add(currentFigure);
                        currentFigure = null; //so that we do not add it again when reaching a Move command for the next figure.
                        lastIsCubicBezier = false;
                        lastIsQuadraticBezier = false;
                        break;
                    default:
                        break;
                }
            }
            if (currentFigure != null)
            {
                Figures.Add(currentFigure);
            }
        }

        private void ReadLines(string pathAsString, ref int currentIndex, ref Point lastAbsolutePosition, PathFigure currentFigure, bool relative)
        {
            PointCollection points = new PointCollection();
            while (true)
            {
                while (currentIndex < pathAsString.Length && pathAsString[currentIndex] == ' ')
                {
                    ++currentIndex;
                }
                if (currentIndex < pathAsString.Length && !_commandCharacters.Contains(pathAsString[currentIndex]))
                {
                    Point point = GetPoint(pathAsString, currentIndex, out currentIndex);
                    if (relative)
                    {
                        point.X = point.X + lastAbsolutePosition.X;
                        point.Y = point.Y + lastAbsolutePosition.Y;
                    }
                    points.Add(point);
                    lastAbsolutePosition.X = point.X;
                    lastAbsolutePosition.Y = point.Y;
                }
                else
                {
                    break;
                }
            }
            if (points.Count > 0)
            {
                PolyLineSegment polyLineSegment = new PolyLineSegment();
                polyLineSegment.Points = new PointCollection();
                polyLineSegment.Points = points;
                currentFigure.Segments.Add(polyLineSegment);
            }
        }

        List<double> ReadNumbers(string stringContainingNumber, ref int index)
        {
            List<double> numbers = new List<double>();
            while (true)
            {
                while (index < stringContainingNumber.Length && stringContainingNumber[index] == ' ')
                {
                    ++index;
                }
                if (index < stringContainingNumber.Length && !_commandCharacters.Contains(stringContainingNumber[index]))
                {
                    double newDouble = GetNextNumber(stringContainingNumber, index, out index);
                    numbers.Add(newDouble);
                }
                else
                {
                    break;
                }
            }
            return numbers;
        }

        /// <summary>
        /// reads and returns the next number in the given string, starting at the given index.
        /// </summary>
        /// <param name="stringContainingNumber">The string in which to read the number.</param>
        /// <param name="startIndex">The index at which we start reading the number (note: the number MUST be the first thing after given index, white spaces are handled)</param>
        /// <param name="stopIndex">The index of the character right after the end of the number.</param>
        /// <returns>The number in the string at the given index as a double.</returns>
        private static double GetNextNumber(string stringContainingNumber, int startIndex, out int stopIndex)
        {

            //Todo: it is possible to have scientific notation for numbers apparently --> make it possible.
            int actualStartIndex = startIndex;
            int exponentStartIndex = -1;
            int numberEndIndex = -1;
            int currentIndex = startIndex;
            //we go past the possible white spaces before the number:
            while (currentIndex < stringContainingNumber.Length && (stringContainingNumber[currentIndex] == ' '))
            {
                ++currentIndex;
            }
            if (currentIndex < stringContainingNumber.Length && (stringContainingNumber[currentIndex] == ','))
            {
                ++currentIndex;
            }
            actualStartIndex = currentIndex; //(got rid of useless spaces and the comma splitting this number and the previous one.)
            int dotIndex = -1; //apparently, when two numbers follow each other, it is authorized to have no space between them if there is no questionning if they are the same number or not (something like 10..5 is authorized and is the same as 10.0 0.5)
            //if we find a character that does not belong to a number, then the string cannot be parsed (?)
            if (currentIndex >= stringContainingNumber.Length || !_numberCharacters.Contains(stringContainingNumber[currentIndex]))
            {
                throw new IndexOutOfRangeException("Invalid path data or non authorized character found.");
            }
            bool firstChar = true;
            //we find the end of the number
            while (currentIndex < stringContainingNumber.Length && _numberCharacters.Contains(stringContainingNumber[currentIndex]))
            {
                char c = stringContainingNumber[currentIndex];
                if (c == '.')
                {
                    if (dotIndex != -1)
                    {
                        if (dotIndex == currentIndex - 1)
                        {
                            numberEndIndex = currentIndex;
                            break;
                        }
                        else
                        {
                            //we have something like 10.4.5 --> we cannot know which numbers these are
                            throw new FormatException("String badly formatted");
                        }
                    }
                    else
                    {
                        dotIndex = currentIndex;
                    }
                }
                else if ((c == '+' || c == '-') && !firstChar)
                {
                    numberEndIndex = currentIndex;
                    break;
                }
                else if (c == 'E' || c == 'e')
                {
                    //Remember the end of the number before the scientific notation:
                    numberEndIndex = currentIndex;
                    //Read the exponent for scientific notation:
                    ++currentIndex;
                    exponentStartIndex = currentIndex;
                    firstChar = true;
                    while (currentIndex < stringContainingNumber.Length && _numberCharacters.Contains(stringContainingNumber[currentIndex]))
                    {
                        if (c == '.')
                        {
                            break;
                        }
                        if ((c == '+' || c == '-') && !firstChar)
                        {
                            break;
                        }
                        if ((c == 'E' || c == 'e') && !firstChar)
                        {
                            break;
                        }
                        ++currentIndex;
                    }
                    break;
                }
                firstChar = false;
                ++currentIndex;
            }
            if (exponentStartIndex == -1)
            {
                string numberAsString = stringContainingNumber.Substring(actualStartIndex, currentIndex - actualStartIndex);
                stopIndex = currentIndex;
                return double.Parse(numberAsString); //todo: add CultureInfo.InvariantCulture to the parameters once it will be handled correctly.
            }
            else //the number is in scientific notation:
            {
                double basicNumber = 1.0;
                if (actualStartIndex != numberEndIndex) //else, we consider 1 as the basic number (the number is as follows: E-06 so we consider it as if it was 1E-06)
                {
                    string numberAsString = stringContainingNumber.Substring(actualStartIndex, numberEndIndex - actualStartIndex);
                    basicNumber = double.Parse(numberAsString); //todo: add CultureInfo.InvariantCulture to the parameters once it will be handled correctly.
                }
                string exponentAsString = stringContainingNumber.Substring(exponentStartIndex, currentIndex - exponentStartIndex);
                double exponent = double.Parse(exponentAsString);
                basicNumber *= Math.Pow(10, exponent);
                stopIndex = currentIndex;
                return basicNumber;
            }
        }

        /// <summary>
        /// Reads the next point's coordinates, starting reading at the given index in the string.
        /// </summary>
        /// <param name="stringContainingPoint">The string in which to read the point's coordinates.</param>
        /// <param name="startIndex">The index at which the point's coordinates start being given.</param>
        /// <param name="stopIndex">The index at which we are in the string after reading the point.</param>
        /// <returns></returns>
        private static Point GetPoint(string stringContainingPoint, int startIndex, out int stopIndex)
        {
            int index;
            double x = GetNextNumber(stringContainingPoint, startIndex, out index);
            while (index < stringContainingPoint.Length && stringContainingPoint[index] == ' ')
            {
                ++index;
            }
            if (index >= stringContainingPoint.Length || stringContainingPoint[index] != ',')
            {
                throw new FormatException("Could not create Path, input string badly formatted");
            }
            ++index;
            double y = GetNextNumber(stringContainingPoint, index, out stopIndex);
            return new Point(x, y);
        }

        internal void UpdateStrokeThickness(double newStrokeThickness)
        {
            _strokeThickness = newStrokeThickness;
        }

        internal override void GetMinMaxXY(ref double minX, ref double maxX, ref double minY, ref double maxY)
        {
            foreach (PathFigure figure in Figures)
            {
                figure.GetMinMaxXY(ref minX, ref maxX, ref minY, ref maxY, _strokeThickness);
            }
        }

        internal override void SetFill(bool newIsFilled)
        {
            foreach (PathFigure figure in Figures)
            {
                figure.IsFilled = newIsFilled;
            }
        }

        /// <summary>
        /// Applies FillStyle, StrokeStyle + Adds the figures to the canvas' context, then calls the Fill method.
        /// </summary>
        internal override void DefineInCanvas(Shapes.Path path, object canvasDomElement, double horizontalMultiplicator, double verticalMultiplicator, double xOffsetToApplyBeforeMultiplication, double yOffsetToApplyBeforeMultiplication, double xOffsetToApplyAfterMultiplication, double yOffsetToApplyAfterMultiplication, Size shapeActualSize)
        {
            //we define the Fillstyle and StrokeStyle:
            //StrokeStyle:

            if (shapeActualSize.Width > 0 && shapeActualSize.Height > 0)
            {
                //this will change the context in the canvas to draw itself.
                foreach (PathFigure figure in Figures)
                {
                    //Add the figure to the canvas:
                    figure.DefineInCanvas(xOffsetToApplyBeforeMultiplication, yOffsetToApplyBeforeMultiplication, xOffsetToApplyAfterMultiplication, yOffsetToApplyAfterMultiplication, horizontalMultiplicator, verticalMultiplicator, canvasDomElement, _strokeThickness, shapeActualSize);
                }
            }
        }

        internal static object INTERNAL_ConvertFromString(string pathAsString)
        {
            PathGeometry geometry = new PathGeometry();
            geometry.ParseString(pathAsString);
            return geometry;
        }
    }
}