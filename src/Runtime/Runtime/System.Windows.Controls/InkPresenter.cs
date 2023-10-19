
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
using System.Text;
using System.Windows.Ink;
using System.Windows.Input;
using CSHTML5.Internal;
using OpenSilver.Internal;

namespace System.Windows.Controls
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
            SizeChanged += new SizeChangedEventHandler(OnSizeChanged);
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e) => ResetCanvas(e.NewSize);

        private void ResetCanvas(Size renderSize)
        {
            if (_canvasDom is null) return;

            // 1 - get current size of the canvas
            // 2 - increase the actual size of our canvas
            // 3 - ensure all drawing operations are scaled
            // 4 - scale everything down using CSS
            string sCanvas = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(_canvasDom);
            string width = Math.Ceiling(renderSize.Width).ToInvariantString();
            string height = Math.Ceiling(renderSize.Height).ToInvariantString();
            OpenSilver.Interop.ExecuteJavaScriptFastAsync(
@$"(function(cvs) {{
cvs.width = {width} * window.devicePixelRatio;
cvs.height = {height} * window.devicePixelRatio;
let ctx = cvs.getContext('2d');
ctx.imageSmoothingEnabled = true;
ctx.webkitImageSmoothingEnabled = true;
ctx.mozImageSmoothingEnabled = true;
ctx.msImageSmoothingEnabled = true;
ctx.scale(window.devicePixelRatio, window.devicePixelRatio);
cvs.style.width = {width} + 'px';
cvs.style.height = {height} + 'px';
ctx.strokeStyle = '#222222';
ctx.lineWidth = '4';
ctx.clearRect(0, 0, cvs.width, cvs.height); }})({sCanvas});");
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
            get => (StrokeCollection)GetValue(StrokesProperty);
            set => SetValue(StrokesProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Strokes"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StrokesProperty =
            DependencyProperty.Register(
                nameof(Strokes),
                typeof(StrokeCollection),
                typeof(InkPresenter),
                new PropertyMetadata(
                    new PFCDefaultValueFactory<Stroke>(
                        static () => new StrokeCollection(),
                        static (d, dp) =>
                        {
                            InkPresenter ink = (InkPresenter)d;
                            var collection = new StrokeCollection();
                            collection.CollectionChanged += ink.OnStrokeCollectionChanged;
                            return collection;
                        }),
                    OnStrokesChanged,
                    CoerceStrokes));

        private static void OnStrokesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((InkPresenter)d).HandleStrokesPropertyChanged(e);
        }

        private static object CoerceStrokes(DependencyObject d, object baseValue)
        {
            return baseValue ?? new StrokeCollection();
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
            ResetCanvas(RenderSize);

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

            string sCanvas = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(_canvasDom);
            var sb = new StringBuilder();
            sb.AppendLine($"(function(cvs) {{ const ctx = cvs.getContext('2d');");
            //object context = OpenSilver.Interop.ExecuteJavaScriptAsync(@"$0.getContext('2d')", _canvasDom);
            var firstPoint = stroke.StylusPoints[0];
            //OpenSilver.Interop.ExecuteJavaScriptAsync(@"$0.moveTo($1, $2);", context, firstPoint.X, firstPoint.Y);
            sb.AppendLine($"ctx.moveTo({firstPoint.X.ToInvariantString()}, {firstPoint.Y.ToInvariantString()});");

            for (int i = 1; i < stroke.StylusPoints.Count; i++)
            {
                //OpenSilver.Interop.ExecuteJavaScriptAsync(@"$0.lineTo($1, $2);", context, stroke.StylusPoints[i].X, stroke.StylusPoints[i].Y);
                sb.AppendLine($"ctx.lineTo({stroke.StylusPoints[i].X.ToInvariantString()}, {stroke.StylusPoints[i].Y.ToInvariantString()});");
            }

            //OpenSilver.Interop.ExecuteJavaScriptAsync(@"$0.stroke();", context);
            sb.AppendLine($"ctx.stroke(); }})({sCanvas});");
            OpenSilver.Interop.ExecuteJavaScriptFastAsync(sb.ToString());
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

            string sCanvas = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(_canvasDom);
            OpenSilver.Interop.ExecuteJavaScriptFastAsync(
                $"(function(cvs) {{ const ctx = cvs.getContext('2d'); ctx.moveTo({_lastPos.X.ToInvariantString()}, {_lastPos.Y.ToInvariantString()}); ctx.lineTo({_mousePos.X.ToInvariantString()}, {_mousePos.Y.ToInvariantString()}); ctx.stroke(); }})({sCanvas})");
            //object context = OpenSilver.Interop.ExecuteJavaScriptAsync(@"$0.getContext('2d')", _canvasDom);
            //OpenSilver.Interop.ExecuteJavaScriptAsync(@"$0.moveTo($1, $2); $0.lineTo($3, $4); $0.stroke();", context, _lastPos.X, _lastPos.Y, _mousePos.X, _mousePos.Y);
            _lastPos = _mousePos;
        }
    }
}
