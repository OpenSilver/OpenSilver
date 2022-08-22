
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

using System;
using System.Collections.Specialized;
using CSHTML5.Internal;

#if MIGRATION
using System.Windows.Ink;
using System.Windows.Input;
#else
using Windows.UI.Xaml.Ink;
using Windows.UI.Xaml.Input;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Implements a rectangular surface that displays ink strokes.
    /// </summary>
    public class InkPresenter : Canvas
    {
        private object _canvasDom;
        private Stroke _currentStroke;
        private StylusPoint _lastPos;
        private StylusPoint _mousePos;

        /// <summary>
        /// Initializes a new instance of the <see cref="InkPresenter"/> class.
        /// </summary>
        public InkPresenter()
        {
            Strokes = new StrokeCollection();
            Loaded += InkPresenter_Loaded;
        }

        private void InkPresenter_Loaded(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }

            ResetCanvas();
        }

        private void ResetCanvas()
        {
            if (_canvasDom == null) return;

            // 1 - get current size of the canvas
            // 2 - increase the actual size of our canvas
            // 3 - ensure all drawing operations are scaled
            // 4 - scale everything down using CSS
#if OPENSILVER
            var javascript = "let rect = $0.getBoundingClientRect();" +
                    "$0.width = rect.width * window.devicePixelRatio;" +
                    "$0.height = rect.height * window.devicePixelRatio;" +
                    "let ctx = $0.getContext('2d');" +
                    "ctx.scale(window.devicePixelRatio, window.devicePixelRatio);" +
                    "$0.style.width = rect.width + 'px';" +
                    "$0.style.height = rect.height + 'px';" +
                    "ctx.strokeStyle=$1; ctx.lineWidth=$2;" +
                    "ctx.clearRect(0, 0, $0.width, $0.height);";

            OpenSilver.Interop.ExecuteJavaScriptAsync(javascript, _canvasDom, "#222222", 4); 
#else
            OpenSilver.Interop.ExecuteJavaScriptAsync(@"let rect = $0.getBoundingClientRect();
$0.width = rect.width * window.devicePixelRatio;
$0.height = rect.height * window.devicePixelRatio;
let ctx = $0.getContext('2d');
ctx.scale(window.devicePixelRatio, window.devicePixelRatio);
$0.style.width = rect.width + 'px';
$0.style.height = rect.height + 'px';
ctx.strokeStyle=$1; ctx.lineWidth=$2;
ctx.clearRect(0, 0, $0.width, $0.height);", _canvasDom, "#222222", 4);
#endif
        }

        public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            var div = base.CreateDomElement(parentRef, out domElementWhereToPlaceChildren);
            _canvasDom = INTERNAL_HtmlDomManager.CreateDomElementAndAppendIt("canvas", div, this);
            var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(_canvasDom);
            style.width = "100%";
            style.height = "100%";
            style.position = "absolute";
            style.pointerEvents = "none";
            return div;
        }
        
        /// <summary>
        /// Gets or sets the strokes that the <see cref="InkPresenter"/> displays.
        /// </summary>
        /// <returns>
        /// The collection of ink strokes that are displayed by the <see cref="InkPresenter"/>.
        /// </returns>
        public StrokeCollection Strokes
        {
            get
            {
                var strokes = (StrokeCollection)GetValue(StrokesProperty);
                if (strokes == null)
                {
                    strokes = new StrokeCollection();
                    SetValue(StrokesProperty, strokes);
                }
                return strokes;
            }
            set { SetValue(StrokesProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Strokes"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StrokesProperty =
            DependencyProperty.Register(
                nameof(Strokes),
                typeof(StrokeCollection),
                typeof(InkPresenter),
                new PropertyMetadata(OnStrokePropertyChanged));

        private static void OnStrokePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((InkPresenter)d).HandleStrokesPropertyChanged(e);
        }

        private void HandleStrokesPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            var oldCollection = e.OldValue as StrokeCollection;
            if (oldCollection != null)
            {
                oldCollection.CollectionChanged -= OnStrokeCollectionChanged;               
            }

            var newCollection = e.NewValue as StrokeCollection;
            if (newCollection != null)
            {                
                newCollection.CollectionChanged += OnStrokeCollectionChanged;
            }

            DrawAllStrokes();
        }

        private void DrawAllStrokes()
        {            
            ResetCanvas();

            foreach (var stroke in Strokes)
            {
                DrawStroke(stroke);
            }
        }

        private void DrawStroke(Stroke stroke)
        {
            if (stroke.StylusPoints.Count <= 1)
            {
                return;
            }

            object context = OpenSilver.Interop.ExecuteJavaScriptAsync(@"$0.getContext('2d')", _canvasDom);
            var firstPoint = stroke.StylusPoints[0];
            OpenSilver.Interop.ExecuteJavaScriptAsync(@"$0.moveTo($1, $2);", context, firstPoint.X, firstPoint.Y);

            for (int i = 1; i < stroke.StylusPoints.Count; i++)
            {
                OpenSilver.Interop.ExecuteJavaScriptAsync(@"$0.lineTo($1, $2);", context, stroke.StylusPoints[i].X, stroke.StylusPoints[i].Y);
            }

            OpenSilver.Interop.ExecuteJavaScriptAsync(@"$0.stroke();", context);
        }


        private void OnStrokeCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (_currentStroke != null)
                    {
                        _currentStroke.StylusPoints.CollectionChanged -= OnStylusPointsCollectionChanged;
                    }

                    _currentStroke = e.NewItems[0] as Stroke;

                    DrawStroke(_currentStroke);

                    if (_currentStroke.StylusPoints.Count > 0)
                    {
                        _lastPos = _currentStroke.StylusPoints[_currentStroke.StylusPoints.Count - 1];
                    }

                    _currentStroke.StylusPoints.CollectionChanged += OnStylusPointsCollectionChanged;
                    break;

                default:
                    // in all other cases, redraw canvas
                    if (e.OldItems != null)
                    {
                        foreach (Stroke stroke in e.OldItems)
                        {
                            stroke.StylusPoints.CollectionChanged -= OnStylusPointsCollectionChanged;
                            if (stroke == _currentStroke)
                            {
                                _currentStroke = null;
                            }
                        }
                    }

                    if (e.NewItems != null)
                    {
                        foreach (Stroke stroke in e.NewItems)
                        {
                            stroke.StylusPoints.CollectionChanged += OnStylusPointsCollectionChanged;
                        }
                    }

                    DrawAllStrokes();
                    break;
            }
        }

        private void OnStylusPointsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (_currentStroke.StylusPoints.Count > 1)
                    {
                        _mousePos = _currentStroke.StylusPoints[_currentStroke.StylusPoints.Count - 1];
                        DrawCurrentPoint();
                    }
                    break;
                default:
                    DrawAllStrokes();
                    break;
            }
            
        }

        private void DrawCurrentPoint()
        {
            if (!INTERNAL_VisualTreeManager.IsElementInVisualTree(this) || _currentStroke == null || _currentStroke.StylusPoints.Count <= 1)
            {
                return;
            }

            object context = OpenSilver.Interop.ExecuteJavaScriptAsync(@"$0.getContext('2d')", _canvasDom);
            OpenSilver.Interop.ExecuteJavaScriptAsync(@"$0.moveTo($1, $2); $0.lineTo($3, $4); $0.stroke();", context, _lastPos.X, _lastPos.Y, _mousePos.X, _mousePos.Y);
            _lastPos = _mousePos;
        }
    }
}
