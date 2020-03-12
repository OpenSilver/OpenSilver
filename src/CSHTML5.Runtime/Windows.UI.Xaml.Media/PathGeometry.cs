

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
    public sealed partial class PathGeometry : Geometry
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
            DependencyProperty.Register("Figures", typeof(PathFigureCollection), typeof(PathGeometry), new PropertyMetadata(null, Figures_Changed));

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
            DependencyProperty.Register("FillRule", typeof(FillRule), typeof(PathGeometry), new PropertyMetadata(FillRule.EvenOdd, FillRule_Changed));

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

        internal void UpdateStrokeThickness(double newStrokeThickness)
        {
            _strokeThickness = newStrokeThickness;
        }

        internal protected override void GetMinMaxXY(ref double minX, ref double maxX, ref double minY, ref double maxY)
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
        internal protected override void DefineInCanvas(Shapes.Path path, object canvasDomElement, double horizontalMultiplicator, double verticalMultiplicator, double xOffsetToApplyBeforeMultiplication, double yOffsetToApplyBeforeMultiplication, double xOffsetToApplyAfterMultiplication, double yOffsetToApplyAfterMultiplication, Size shapeActualSize)
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
            return GeometryParser.ParseGeometry(pathAsString);
        }
    }
}