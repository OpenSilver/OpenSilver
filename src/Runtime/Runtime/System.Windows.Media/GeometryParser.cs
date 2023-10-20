
/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

//credits : https://github.com/dotnet/wpf/blob/master/src/Microsoft.DotNet.Wpf/src/PresentationCore/System/Windows/Media/ParsersCommon.cs

using System.Collections.Generic;
using System.Diagnostics;

namespace System.Windows.Media
{
    internal static class GeometryParser
    {
        internal static Geometry ParseGeometry(string pathString/*, IFormatProvider formatProvider*/)
        {
            FillRule fillRule = FillRule.EvenOdd;
            PathStreamGeometryContext context = new PathStreamGeometryContext();
            ParseStringToStreamGeometryContext(context, pathString/*, formatProvider*/, ref fillRule);
            var geometry = context.GetPathGeometry();
            geometry.FillRule = fillRule;
            return geometry;
        }

        private static void ParseStringToStreamGeometryContext(PathStreamGeometryContext geometry, string pathString/*, IFormatProvider formatProvider*/, ref FillRule fillRule)
        {
            // Check to ensure that there's something to parse
            if (pathString != null)
            {
                int curIndex = 0;

                // skip any leading space
                while ((curIndex < pathString.Length) && char.IsWhiteSpace(pathString, curIndex))
                {
                    curIndex++;
                }

                // Is there anything to look at?
                if (curIndex < pathString.Length)
                {
                    // If so, we only care if the first non-WhiteSpace char encountered is 'F'
                    if (pathString[curIndex] == 'F')
                    {
                        curIndex++;

                        // Since we found 'F' the next non-WhiteSpace char must be 0 or 1 - look for it.
                        while ((curIndex < pathString.Length) && Char.IsWhiteSpace(pathString, curIndex))
                        {
                            curIndex++;
                        }

                        // If we ran out of text, this is an error, because 'F' cannot be specified without 0 or 1
                        // Also, if the next token isn't 0 or 1, this too is illegal
                        if ((curIndex == pathString.Length) || ((pathString[curIndex] != '0') && (pathString[curIndex] != '1')))
                        {
                            //throw new FormatException(SR.Get(SRID.Parsers_IllegalToken));
                            throw new FormatException("Parsers Illegal Token");
                        }

                        fillRule = pathString[curIndex] == '0' ? FillRule.EvenOdd : FillRule.Nonzero;

                        // Increment curIndex to point to the next char
                        curIndex++;
                    }
                }

                AbbreviatedGeometryParser parser = new AbbreviatedGeometryParser();

                parser.ParseToGeometryContext(geometry, pathString, curIndex);
            }
        }
    }

    /// <summary>
    /// Parser for XAML abbreviated geometry.
    /// SVG path spec is closely followed http://www.w3.org/TR/SVG11/paths.html
    /// 3/23/2006, new parser for performance (fyuan)
    /// </summary>
    internal sealed partial class AbbreviatedGeometryParser
    {
        private const bool AllowSign = true;
        private const bool AllowComma = true;
        private const bool IsFilled = true;
        private const bool IsClosed = true;
        private const bool IsStroked = true;
        private const bool IsSmoothJoin = true;

        private IFormatProvider _formatProvider;

        private string _pathString;        // Input string to be parsed
        private int _pathLength;
        private int _curIndex;          // Location to read next character from 
        private bool _figureStarted;     // StartFigure is effective

        private Point _lastStart;         // Last figure starting point
        private Point _lastPoint;         // Last point
        private Point _secondLastPoint;   // The point before last point

        private char _token;             // Non whitespace character returned by ReadToken

        private PathStreamGeometryContext _context;

        /// <summary>
        /// Throw unexpected token exception
        /// </summary>
        private void ThrowBadToken()
        {
            //throw new System.FormatException(SR.Get(SRID.Parser_UnexpectedToken, _pathString, _curIndex - 1));
            throw new FormatException(string.Format("Parser Unexpected Token in string \"{0}\" at position {1}.", this._pathString, this._curIndex - 1));
        }

        private bool More()
        {
            return this._curIndex < this._pathLength;
        }

        // Skip white space, one comma if allowed
        private bool SkipWhiteSpace(bool allowComma)
        {
            bool commaMet = false;

            while (this.More())
            {
                char ch = this._pathString[this._curIndex];

                switch (ch)
                {
                    case ' ':
                    case '\n':
                    case '\r':
                    case '\t': // SVG whitespace
                        break;

                    case ',':
                        if (allowComma)
                        {
                            commaMet = true;
                            allowComma = false; // one comma only
                        }
                        else
                        {
                            this.ThrowBadToken();
                        }
                        break;

                    default:
                        // Avoid calling IsWhiteSpace for ch in (' ' .. 'z']
                        if (((ch > ' ') && (ch <= 'z')) || !char.IsWhiteSpace(ch))
                        {
                            return commaMet;
                        }
                        break;
                }

                this._curIndex++;
            }

            return commaMet;
        }

        /// <summary>
        /// Read the next non whitespace character
        /// </summary>
        /// <returns>True if not end of string</returns>
        private bool ReadToken()
        {
            this.SkipWhiteSpace(!AllowComma);

            // Check for end of string
            if (this.More())
            {
                this._token = this._pathString[this._curIndex++];

                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsNumber(bool allowComma)
        {
            bool commaMet = this.SkipWhiteSpace(allowComma);

            if (this.More())
            {
                this._token = this._pathString[this._curIndex];

                // Valid start of a number
                if ((this._token == '.') || (this._token == '-') || (this._token == '+') || ((this._token >= '0') && (this._token <= '9'))
                    || (this._token == 'I')  // Infinity
                    || (this._token == 'N')) // NaN
                {
                    return true;
                }
            }

            if (commaMet) // Only allowed between numbers
            {
                this.ThrowBadToken();
            }

            return false;
        }

        private void SkipDigits(bool signAllowed)
        {
            // Allow for a sign
            if (signAllowed && this.More() && ((this._pathString[this._curIndex] == '-') || this._pathString[this._curIndex] == '+'))
            {
                this._curIndex++;
            }

            while (this.More() && (this._pathString[this._curIndex] >= '0') && (this._pathString[this._curIndex] <= '9'))
            {
                this._curIndex++;
            }
        }

        /// <summary>
        /// Read a floating point number
        /// </summary>
        /// <returns></returns>
        private double ReadNumber(bool allowComma)
        {
            if (!this.IsNumber(allowComma))
            {
                this.ThrowBadToken();
            }

            bool simple = true;
            int start = this._curIndex;

            //
            // Allow for a sign
            // 
            // There are numbers that cannot be preceded with a sign, for instance, -NaN, but it's
            // fine to ignore that at this point, since the CLR parser will catch this later.
            //
            if (this.More() && ((this._pathString[this._curIndex] == '-') || this._pathString[this._curIndex] == '+'))
            {
                this._curIndex++;
            }

            // Check for Infinity (or -Infinity).
            if (this.More() && (this._pathString[this._curIndex] == 'I'))
            {
                //
                // Don't bother reading the characters, as the CLR parser will
                // do this for us later.
                //
                this._curIndex = Math.Min(this._curIndex + 8, this._pathLength); // "Infinity" has 8 characters
                simple = false;
            }
            // Check for NaN
            else if (this.More() && (this._pathString[this._curIndex] == 'N'))
            {
                //
                // Don't bother reading the characters, as the CLR parser will
                // do this for us later.
                //
                this._curIndex = Math.Min(this._curIndex + 3, this._pathLength); // "NaN" has 3 characters
                simple = false;
            }
            else
            {
                this.SkipDigits(!AllowSign);

                // Optional period, followed by more digits
                if (this.More() && (this._pathString[this._curIndex] == '.'))
                {
                    simple = false;
                    this._curIndex++;
                    this.SkipDigits(!AllowSign);
                }

                // Exponent
                if (this.More() && ((this._pathString[this._curIndex] == 'E') || (this._pathString[this._curIndex] == 'e')))
                {
                    simple = false;
                    this._curIndex++;
                    this.SkipDigits(AllowSign);
                }
            }

            if (simple && (this._curIndex <= (start + 8))) // 32-bit integer
            {
                int sign = 1;

                if (this._pathString[start] == '+')
                {
                    start++;
                }
                else if (this._pathString[start] == '-')
                {
                    start++;
                    sign = -1;
                }

                int value = 0;

                while (start < this._curIndex)
                {
                    value = value * 10 + (this._pathString[start] - '0');
                    start++;
                }

                return value * sign;
            }
            else
            {
                string subString = this._pathString.Substring(start, this._curIndex - start);

                try
                {
                    return Convert.ToDouble(subString, this._formatProvider);
                }
                catch (FormatException except)
                {
                    //throw new FormatException(SR.Get(SRID.Parser_UnexpectedToken, _pathString, start), except);
                    throw new FormatException(string.Format("Parser Unexpected Token in string \"{0}\" at position {1}.", this._pathString, start), except);
                }
            }
        }

        /// <summary>
        /// Read a bool: 1 or 0
        /// </summary>
        /// <returns></returns>
        private bool ReadBool()
        {
            SkipWhiteSpace(AllowComma);

            if (More())
            {
                _token = _pathString[_curIndex++];

                if (_token == '0')
                {
                    return false;
                }
                else if (_token == '1')
                {
                    return true;
                }
            }

            ThrowBadToken();

            return false;
        }

        /// <summary>
        /// Read a relative point
        /// </summary>
        /// <returns></returns>
        private Point ReadPoint(char cmd, bool allowcomma)
        {
            double x = this.ReadNumber(allowcomma);
            double y = this.ReadNumber(AllowComma);

            if (cmd >= 'a') // 'A' < 'a'. lower case for relative
            {
                x += this._lastPoint.X;
                y += this._lastPoint.Y;
            }

            return new Point(x, y);
        }

        /// <summary>
        /// Reflect _secondLastPoint over _lastPoint to get a new point for smooth curve
        /// </summary>
        /// <returns></returns>
        private Point Reflect()
        {
            return new Point(2 * _lastPoint.X - _secondLastPoint.X, 2 * _lastPoint.Y - _secondLastPoint.Y);
        }

        private void EnsureFigure()
        {
            if (!_figureStarted)
            {
                _context.BeginFigure(_lastStart, IsFilled, !IsClosed);
                _figureStarted = true;
            }
        }

        /// <summary>
        /// Parse a PathFigureCollection string
        /// </summary>
        internal void ParseToGeometryContext(PathStreamGeometryContext context, string pathString, int startIndex)
        {
            // [BreakingChange] Dev10 Bug #453199
            // We really should throw an ArgumentNullException here for context and pathString.

            // From original code
            // This is only used in call to Double.Parse
            this._formatProvider = global::System.Globalization.CultureInfo.InvariantCulture;

            this._context = context;
            this._pathString = pathString;
            this._pathLength = pathString.Length;
            this._curIndex = startIndex;

            this._secondLastPoint = new Point(0, 0);
            this._lastPoint = new Point(0, 0);
            this._lastStart = new Point(0, 0);

            this._figureStarted = false;

            bool first = true;

            char last_cmd = ' ';

            while (this.ReadToken()) // Empty path is allowed in XAML
            {
                char cmd = this._token;

                if (first)
                {
                    if ((cmd != 'M') && (cmd != 'm'))  // Path starts with M|m
                    {
                        this.ThrowBadToken();
                    }

                    first = false;
                }

                switch (cmd)
                {
                    case 'm':
                    case 'M':
                        // XAML allows multiple points after M/m
                        this._lastPoint = this.ReadPoint(cmd, !AllowComma);

                        //context.BeginFigure(this._lastPoint, IsFilled, !IsClosed);
                        context.BeginFigure(this._lastPoint, IsFilled, !IsClosed);

                        this._figureStarted = true;
                        this._lastStart = _lastPoint;
                        last_cmd = 'M';

                        while (this.IsNumber(AllowComma))
                        {
                            this._lastPoint = this.ReadPoint(cmd, !AllowComma);

                            context.LineTo(this._lastPoint, IsStroked, !IsSmoothJoin);
                            last_cmd = 'L';
                        }
                        break;

                    case 'l':
                    case 'L':
                    case 'h':
                    case 'H':
                    case 'v':
                    case 'V':
                        this.EnsureFigure();

                        do
                        {
                            switch (cmd)
                            {
                                case 'l': this._lastPoint = this.ReadPoint(cmd, !AllowComma); break;
                                case 'L': this._lastPoint = this.ReadPoint(cmd, !AllowComma); break;
                                case 'h': this._lastPoint.X += this.ReadNumber(!AllowComma); break;
                                case 'H': this._lastPoint.X = this.ReadNumber(!AllowComma); break;
                                case 'v': this._lastPoint.Y += this.ReadNumber(!AllowComma); break;
                                case 'V': this._lastPoint.Y = this.ReadNumber(!AllowComma); break;
                            }

                            context.LineTo(this._lastPoint, IsStroked, !IsSmoothJoin);
                        }
                        while (this.IsNumber(AllowComma));

                        last_cmd = 'L';
                        break;

                    case 'c':
                    case 'C': // cubic Bezier
                    case 's':
                    case 'S': // smooth cublic Bezier
                        this.EnsureFigure();

                        do
                        {
                            Point p;

                            if ((cmd == 's') || (cmd == 'S'))
                            {
                                if (last_cmd == 'C')
                                {
                                    p = this.Reflect();
                                }
                                else
                                {
                                    p = this._lastPoint;
                                }

                                this._secondLastPoint = this.ReadPoint(cmd, !AllowComma);
                            }
                            else
                            {
                                p = this.ReadPoint(cmd, !AllowComma);

                                this._secondLastPoint = this.ReadPoint(cmd, AllowComma);
                            }

                            this._lastPoint = this.ReadPoint(cmd, AllowComma);

                            context.BezierTo(p, this._secondLastPoint, this._lastPoint, IsStroked, !IsSmoothJoin);

                            last_cmd = 'C';
                        }
                        while (this.IsNumber(AllowComma));

                        break;

                    case 'q':
                    case 'Q': // quadratic Bezier
                    case 't':
                    case 'T': // smooth quadratic Bezier
                        this.EnsureFigure();

                        do
                        {
                            if ((cmd == 't') || (cmd == 'T'))
                            {
                                if (last_cmd == 'Q')
                                {
                                    this._secondLastPoint = this.Reflect();
                                }
                                else
                                {
                                    this._secondLastPoint = this._lastPoint;
                                }

                                this._lastPoint = this.ReadPoint(cmd, !AllowComma);
                            }
                            else
                            {
                                this._secondLastPoint = this.ReadPoint(cmd, !AllowComma);
                                this._lastPoint = this.ReadPoint(cmd, AllowComma);
                            }

                            context.QuadraticBezierTo(this._secondLastPoint, this._lastPoint, IsStroked, !IsSmoothJoin);

                            last_cmd = 'Q';
                        }
                        while (this.IsNumber(AllowComma));

                        break;

                    case 'a':
                    case 'A':
                        this.EnsureFigure();

                        do
                        {
                            // A 3,4 5, 0, 0, 6,7
                            double w = this.ReadNumber(!AllowComma);
                            double h = this.ReadNumber(AllowComma);
                            double rotation = this.ReadNumber(AllowComma);
                            bool large = this.ReadBool();
                            bool sweep = this.ReadBool();

                            this._lastPoint = this.ReadPoint(cmd, AllowComma);

                            context.ArcTo(
                                this._lastPoint,
                                new Size(w, h),
                                rotation,
                                large,
                                sweep ? SweepDirection.Clockwise : SweepDirection.Counterclockwise,
                                IsStroked,
                                !IsSmoothJoin);
                        }
                        while (this.IsNumber(AllowComma));

                        last_cmd = 'A';
                        break;

                    case 'z':
                    case 'Z':
                        this.EnsureFigure();
                        context.SetClosedState(IsClosed);

                        this._figureStarted = false;
                        last_cmd = 'Z';

                        this._lastPoint = this._lastStart; // Set reference point to be first point of current figure
                        break;

                    default:
                        this.ThrowBadToken();
                        break;
                }
            }
        }
    }

    internal enum MIL_SEGMENT_TYPE
    {
        MilSegmentNone,
        MilSegmentLine,
        MilSegmentBezier,
        MilSegmentQuadraticBezier,
        MilSegmentArc,
        MilSegmentPolyLine,
        MilSegmentPolyBezier,
        MilSegmentPolyQuadraticBezier,

        MIL_SEGMENT_TYPE_FORCE_DWORD = unchecked((int)0xffffffff)
    };

    /// <summary>
    ///     PathStreamGeometryContext
    /// </summary>
    //internal partial class PathStreamGeometryContext : CapacityStreamGeometryContext
    internal partial class PathStreamGeometryContext
    {
        #region Public Methods
        static PathStreamGeometryContext()
        {
            // We grab the default values for these properties so that we can avoid setting 
            // properties to their default values (as this will require that we reserve 
            // storage for these values).

            //s_defaultFillRule = (FillRule)PathGeometry.FillRuleProperty.GetDefaultValue(typeof(PathGeometry));
            s_defaultFillRule = FillRule.EvenOdd;

            //s_defaultValueForPathFigureIsClosed = (bool)PathFigure.IsClosedProperty.GetDefaultValue(typeof(PathFigure));
            s_defaultValueForPathFigureIsClosed = false;

            //s_defaultValueForPathFigureIsFilled = (bool)PathFigure.IsFilledProperty.GetDefaultValue(typeof(PathFigure));
            s_defaultValueForPathFigureIsFilled = false;

            //s_defaultValueForPathFigureStartPoint = (Point)PathFigure.StartPointProperty.GetDefaultValue(typeof(PathFigure));
            s_defaultValueForPathFigureStartPoint = new Point();

            //todo: IsStroked and IsSmoothJoin are not supported yet
            //// This code assumes that sub-classes of PathSegment don't override the default value for these properties
            //s_defaultValueForPathSegmentIsStroked = (bool)PathSegment.IsStrokedProperty.GetDefaultValue(typeof(PathSegment));
            //s_defaultValueForPathSegmentIsSmoothJoin = (bool)PathSegment.IsSmoothJoinProperty.GetDefaultValue(typeof(PathSegment));

            //s_defaultValueForArcSegmentIsLargeArc = (bool)ArcSegment.IsLargeArcProperty.GetDefaultValue(typeof(ArcSegment));
            s_defaultValueForArcSegmentIsLargeArc = false;

            //s_defaultValueForArcSegmentSweepDirection = (SweepDirection)ArcSegment.SweepDirectionProperty.GetDefaultValue(typeof(ArcSegment));
            s_defaultValueForArcSegmentSweepDirection = SweepDirection.Counterclockwise;

            //s_defaultValueForArcSegmentRotationAngle = (double)ArcSegment.RotationAngleProperty.GetDefaultValue(typeof(ArcSegment));
            s_defaultValueForArcSegmentRotationAngle = 0d;
        }

        internal PathStreamGeometryContext()
        {
            _pathGeometry = new PathGeometry();
        }

        internal PathStreamGeometryContext(FillRule fillRule, Transform transform)
        {
            _pathGeometry = new PathGeometry();

            if (fillRule != s_defaultFillRule)
            {
                _pathGeometry.FillRule = fillRule;
            }

            //todo: PathGeometry.Transform is not supported yet, Transform.Identity is not supported yet.
            //if ((transform != null) && !transform.IsIdentity)
            //{
            //    _pathGeometry.Transform = transform.Clone();
            //}
        }

        //internal override void SetFigureCount(int figureCount)
        internal void SetFigureCount(int figureCount)
        {
            Debug.Assert(_figures == null, "It is illegal to call SetFigureCount multiple times or after BeginFigure.");
            Debug.Assert(figureCount > 0);

            _figures = new PathFigureCollection(figureCount);
            _pathGeometry.Figures = _figures;
        }

        //internal override void SetSegmentCount(int segmentCount)
        internal void SetSegmentCount(int segmentCount)
        {
            Debug.Assert(_figures != null, "It is illegal to call SetSegmentCount before BeginFigure.");
            Debug.Assert(_currentFigure != null, "It is illegal to call SetSegmentCount before BeginFigure.");
            Debug.Assert(_segments == null, "It is illegal to call SetSegmentCount multiple times per BeginFigure or after a *To method.");
            Debug.Assert(segmentCount > 0);

            _segments = new PathSegmentCollection(segmentCount);
            _currentFigure.Segments = _segments;
        }

        /// <summary>
        /// SetClosed - Sets the current closed state of the figure. 
        /// </summary>
        //override internal void SetClosedState(bool isClosed)
        internal void SetClosedState(bool isClosed)
        {
            Debug.Assert(_currentFigure != null);

            if (isClosed != _currentIsClosed)
            {
                _currentFigure.IsClosed = isClosed;
                _currentIsClosed = isClosed;
            }
        }

        /// <summary>
        /// BeginFigure - Start a new figure.
        /// </summary>
        //public override void BeginFigure(Point startPoint, bool isFilled, bool isClosed)
        public void BeginFigure(Point startPoint, bool isFilled, bool isClosed)
        {
            // _currentFigure != null -> _figures != null
            Debug.Assert(_currentFigure == null || _figures != null);

            // Is this the first figure?
            if (_currentFigure == null)
            {
                // If so, have we not yet allocated the collection?
                if (_figures == null)
                {
                    // While we could always just retrieve _pathGeometry.Figures (which would auto-promote)
                    // it's more efficient to create the collection ourselves and set it explicitly.

                    _figures = new PathFigureCollection();
                    _pathGeometry.Figures = _figures;
                }
            }

            FinishSegment();

            // Clear the old reference to the segment collection
            _segments = null;

            _currentFigure = new PathFigure();
            _currentIsClosed = isClosed;

            if (startPoint != s_defaultValueForPathFigureStartPoint)
            {
                _currentFigure.StartPoint = startPoint;
            }

            if (isClosed != s_defaultValueForPathFigureIsClosed)
            {
                _currentFigure.IsClosed = isClosed;
            }

            if (isFilled != s_defaultValueForPathFigureIsFilled)
            {
                _currentFigure.IsFilled = isFilled;
            }

            _figures.Add(_currentFigure);

            _currentSegmentType = MIL_SEGMENT_TYPE.MilSegmentNone;
        }

        /// <summary>
        /// LineTo - append a LineTo to the current figure.
        /// </summary>
        //public override void LineTo(Point point, bool isStroked, bool isSmoothJoin)
        public void LineTo(Point point, bool isStroked, bool isSmoothJoin)
        {
            PrepareToAddPoints(
                        1 /*count*/,
                        isStroked,
                        isSmoothJoin,
                        MIL_SEGMENT_TYPE.MilSegmentPolyLine);

            _currentSegmentPoints.Add(point);
        }

        /// <summary>
        /// QuadraticBezierTo - append a QuadraticBezierTo to the current figure.
        /// </summary>
        //public override void QuadraticBezierTo(Point point1, Point point2, bool isStroked, bool isSmoothJoin)
        public void QuadraticBezierTo(Point point1, Point point2, bool isStroked, bool isSmoothJoin)
        {
            PrepareToAddPoints(
                        2 /*count*/,
                        isStroked,
                        isSmoothJoin,
                        MIL_SEGMENT_TYPE.MilSegmentPolyQuadraticBezier);

            _currentSegmentPoints.Add(point1);
            _currentSegmentPoints.Add(point2);
        }

        /// <summary>
        /// BezierTo - apply a BezierTo to the current figure.
        /// </summary>
        //public override void BezierTo(Point point1, Point point2, Point point3, bool isStroked, bool isSmoothJoin)
        public void BezierTo(Point point1, Point point2, Point point3, bool isStroked, bool isSmoothJoin)
        {
            PrepareToAddPoints(
                        3 /*count*/,
                        isStroked,
                        isSmoothJoin,
                        MIL_SEGMENT_TYPE.MilSegmentPolyBezier);

            _currentSegmentPoints.Add(point1);
            _currentSegmentPoints.Add(point2);
            _currentSegmentPoints.Add(point3);
        }

        /// <summary>
        /// PolyLineTo - append a PolyLineTo to the current figure.
        /// </summary>
        //public override void PolyLineTo(IList<Point> points, bool isStroked, bool isSmoothJoin)
        public void PolyLineTo(IList<Point> points, bool isStroked, bool isSmoothJoin)
        {
            GenericPolyTo(points,
                          isStroked,
                          isSmoothJoin,
                          MIL_SEGMENT_TYPE.MilSegmentPolyLine);
        }

        /// <summary>
        /// PolyQuadraticBezierTo - append a PolyQuadraticBezierTo to the current figure.
        /// </summary>
        //public override void PolyQuadraticBezierTo(IList<Point> points, bool isStroked, bool isSmoothJoin)
        public void PolyQuadraticBezierTo(IList<Point> points, bool isStroked, bool isSmoothJoin)
        {
            GenericPolyTo(points,
                          isStroked,
                          isSmoothJoin,
                          MIL_SEGMENT_TYPE.MilSegmentPolyQuadraticBezier);
        }

        /// <summary>
        /// PolyBezierTo - append a PolyBezierTo to the current figure.
        /// </summary>
        //public override void PolyBezierTo(IList<Point> points, bool isStroked, bool isSmoothJoin)
        public void PolyBezierTo(IList<Point> points, bool isStroked, bool isSmoothJoin)
        {
            GenericPolyTo(points,
                          isStroked,
                          isSmoothJoin,
                          MIL_SEGMENT_TYPE.MilSegmentPolyBezier);
        }

        /// <summary>
        /// ArcTo - append an ArcTo to the current figure.
        /// </summary>
        //public override void ArcTo(Point point, Size size, double rotationAngle, bool isLargeArc, SweepDirection sweepDirection, bool isStroked, bool isSmoothJoin)
        public void ArcTo(Point point, Size size, double rotationAngle, bool isLargeArc, SweepDirection sweepDirection, bool isStroked, bool isSmoothJoin)
        {
            Debug.Assert(_figures != null);
            Debug.Assert(_currentFigure != null);

            FinishSegment();

            // Is this the first segment?
            if (_segments == null)
            {
                // While we could always just retrieve _currentFigure.Segments (which would auto-promote)
                // it's more efficient to create the collection ourselves and set it explicitly.

                _segments = new PathSegmentCollection();
                _currentFigure.Segments = _segments;
            }

            ArcSegment segment = new ArcSegment();

            segment.Point = point;
            segment.Size = size;

            if (isLargeArc != s_defaultValueForArcSegmentIsLargeArc)
            {
                segment.IsLargeArc = isLargeArc;
            }

            if (sweepDirection != s_defaultValueForArcSegmentSweepDirection)
            {
                segment.SweepDirection = sweepDirection;
            }

            if (rotationAngle != s_defaultValueForArcSegmentRotationAngle)
            {
                segment.RotationAngle = rotationAngle;
            }

            //todo: IsStroked and IsSmoothJoin are not supported yet
            //// Handle common PathSegment properties.
            //if (isStroked != s_defaultValueForPathSegmentIsStroked)
            //{
            //    segment.IsStroked = isStroked;
            //}

            //if (isSmoothJoin != s_defaultValueForPathSegmentIsSmoothJoin)
            //{
            //    segment.IsSmoothJoin = isSmoothJoin;
            //}

            _segments.Add(segment);

            _currentSegmentType = MIL_SEGMENT_TYPE.MilSegmentArc;
        }


        /// <summary>
        /// PathStreamGeometryContext is never opened, so it shouldn't be closed.
        /// </summary>
        //public override void Close()
        public void Close()
        {
            Debug.Assert(false);
        }

        #endregion

        /// <summary>
        /// GetPathGeometry - Retrieves the PathGeometry built by this Context.
        /// </summary>
        internal PathGeometry GetPathGeometry()
        {
            FinishSegment();

            Debug.Assert(_currentSegmentPoints == null);

            return _pathGeometry;
        }

        private void GenericPolyTo(IList<Point> points,
                                   bool isStroked,
                                   bool isSmoothJoin,
                                   MIL_SEGMENT_TYPE segmentType)
        {
            Debug.Assert(points != null);

            int count = points.Count;
            PrepareToAddPoints(count, isStroked, isSmoothJoin, segmentType);

            for (int i = 0; i < count; ++i)
            {
                _currentSegmentPoints.Add(points[i]);
            }
        }

        private void PrepareToAddPoints(
                                   int count,
                                   bool isStroked,
                                   bool isSmoothJoin,
                                   MIL_SEGMENT_TYPE segmentType)
        {
            Debug.Assert(_figures != null);
            Debug.Assert(_currentFigure != null);

            Debug.Assert(count != 0);

            if (_currentSegmentType != segmentType ||
                _currentSegmentIsStroked != isStroked ||
                _currentSegmentIsSmoothJoin != isSmoothJoin)
            {
                FinishSegment();

                _currentSegmentType = segmentType;
                _currentSegmentIsStroked = isStroked;
                _currentSegmentIsSmoothJoin = isSmoothJoin;
            }

            if (_currentSegmentPoints == null)
            {
                _currentSegmentPoints = new PointCollection();
            }
        }

        /// <summary>
        /// FinishSegment - called to completed any outstanding Segment which may be present.
        /// </summary>
        private void FinishSegment()
        {
            if (_currentSegmentPoints != null)
            {
                Debug.Assert(_currentFigure != null);

                int count = _currentSegmentPoints.Count;

                Debug.Assert(count > 0);

                // Is this the first segment?
                if (_segments == null)
                {
                    // While we could always just retrieve _currentFigure.Segments (which would auto-promote)
                    // it's more efficient to create the collection ourselves and set it explicitly.

                    _segments = new PathSegmentCollection();
                    _currentFigure.Segments = _segments;
                }

                PathSegment segment;

                switch (_currentSegmentType)
                {
                    case MIL_SEGMENT_TYPE.MilSegmentPolyLine:
                        if (count == 1)
                        {
                            LineSegment lSegment = new LineSegment();
                            lSegment.Point = _currentSegmentPoints[0];
                            segment = lSegment;
                        }
                        else
                        {
                            PolyLineSegment pSegment = new PolyLineSegment();
                            pSegment.Points = _currentSegmentPoints;
                            segment = pSegment;
                        }
                        break;
                    case MIL_SEGMENT_TYPE.MilSegmentPolyBezier:
                        if (count == 3)
                        {
                            BezierSegment bSegment = new BezierSegment();
                            bSegment.Point1 = _currentSegmentPoints[0];
                            bSegment.Point2 = _currentSegmentPoints[1];
                            bSegment.Point3 = _currentSegmentPoints[2];
                            segment = bSegment;
                        }
                        else
                        {
                            Debug.Assert(count % 3 == 0);

                            PolyBezierSegment pSegment = new PolyBezierSegment();
                            pSegment.Points = _currentSegmentPoints;
                            segment = pSegment;
                        }
                        break;
                    case MIL_SEGMENT_TYPE.MilSegmentPolyQuadraticBezier:
                        if (count == 2)
                        {
                            QuadraticBezierSegment qSegment = new QuadraticBezierSegment();
                            qSegment.Point1 = _currentSegmentPoints[0];
                            qSegment.Point2 = _currentSegmentPoints[1];
                            segment = qSegment;
                        }
                        else
                        {
                            Debug.Assert(count % 2 == 0);

                            PolyQuadraticBezierSegment pSegment = new PolyQuadraticBezierSegment();
                            pSegment.Points = _currentSegmentPoints;
                            segment = pSegment;
                        }
                        break;
                    default:
                        segment = null;
                        Debug.Assert(false);
                        break;
                }

                //todo: IsStroked and IsSmoothJoin are not supported yet
                //// Handle common PathSegment properties.
                //if (_currentSegmentIsStroked != s_defaultValueForPathSegmentIsStroked)
                //{
                //    segment.IsStroked = _currentSegmentIsStroked;
                //}

                //if (_currentSegmentIsSmoothJoin != s_defaultValueForPathSegmentIsSmoothJoin)
                //{
                //    segment.IsSmoothJoin = _currentSegmentIsSmoothJoin;
                //}

                _segments.Add(segment);

                _currentSegmentPoints = null;
                _currentSegmentType = MIL_SEGMENT_TYPE.MilSegmentNone;
            }
        }

        #region Private Fields

        private PathGeometry _pathGeometry;
        private PathFigureCollection _figures;
        private PathFigure _currentFigure;
        private PathSegmentCollection _segments;
        private bool _currentIsClosed;

        private MIL_SEGMENT_TYPE _currentSegmentType;
        private PointCollection _currentSegmentPoints;
        private bool _currentSegmentIsStroked;
        private bool _currentSegmentIsSmoothJoin;

        private static FillRule s_defaultFillRule;

        private static bool s_defaultValueForPathFigureIsClosed;
        private static bool s_defaultValueForPathFigureIsFilled;
        private static Point s_defaultValueForPathFigureStartPoint;

        private static bool s_defaultValueForArcSegmentIsLargeArc;
        private static SweepDirection s_defaultValueForArcSegmentSweepDirection;
        private static double s_defaultValueForArcSegmentRotationAngle;

        #endregion
    }
}


