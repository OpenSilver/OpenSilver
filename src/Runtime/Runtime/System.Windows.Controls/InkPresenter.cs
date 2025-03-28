
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

using System.Collections.Specialized;
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
        private INTERNAL_HtmlDomElementReference _canvasDom;
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

        private void OnSizeChanged(object sender, SizeChangedEventArgs e) => DrawAllStrokes();

        private void ResetCanvas(Size renderSize)
        {
            if (_canvasDom is null) return;

            // 1 - get current size of the canvas
            // 2 - increase the actual size of our canvas
            // 3 - ensure all drawing operations are scaled
            // 4 - scale everything down using CSS
            string sCanvas = OpenSilver.Interop.GetVariableStringForJS(_canvasDom);
            string width = Math.Ceiling(renderSize.Width).ToInvariantString();
            string height = Math.Ceiling(renderSize.Height).ToInvariantString();
            OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                $$"""
                (function(cvs) {
                  const zoom = window.devicePixelRatio;
                  cvs.width = {{width}} * zoom;
                  cvs.height = {{height}} * zoom;
                  const ctx = cvs.getContext('2d');
                  ctx.imageSmoothingEnabled = true;
                  ctx.webkitImageSmoothingEnabled = true;
                  ctx.mozImageSmoothingEnabled = true;
                  ctx.msImageSmoothingEnabled = true;
                  ctx.scale(zoom, zoom);
                  ctx.lineCap = 'round';
                  ctx.lineJoin = 'round';
                })({{sCanvas}})
                """);
        }

        public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            domElementWhereToPlaceChildren = null;
            (var outerDiv, _canvasDom) = INTERNAL_HtmlDomManager.CreateInkPresenterDomElementAndAppendIt(parentRef, this);
            return outerDiv;
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
            set => SetValueInternal(StrokesProperty, value);
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

            foreach (var stroke in Strokes.InternalItems)
            {
                DrawStroke(stroke);
            }
        }

        private void DrawStroke(Stroke stroke)
        {
            var points = stroke.StylusPoints;
            if (points.InternalCount <= 1)
            {
                return;
            }

            string sCanvas = OpenSilver.Interop.GetVariableStringForJS(_canvasDom);
            var sb = StringBuilderCache.Acquire();
            sb.AppendLine("(function(cvs) { const ctx = cvs.getContext('2d');");
            sb.AppendLine($"ctx.strokeStyle = '{stroke.DrawingAttributes.Color.ToHtmlString(1)}';");
            sb.AppendLine($"ctx.lineWidth = '{stroke.DrawingAttributes.Width.ToInvariantString()}';");
            sb.AppendLine("ctx.beginPath();");

            var firstPoint = points[0];
            sb.AppendLine($"ctx.moveTo({firstPoint.X.ToInvariantString()}, {firstPoint.Y.ToInvariantString()});");

            for (int i = 1; i < points.InternalCount; i++)
            {
                sb.AppendLine($"ctx.lineTo({points[i].X.ToInvariantString()}, {points[i].Y.ToInvariantString()});");
            }

            sb.AppendLine($"ctx.stroke(); }})({sCanvas})");
            OpenSilver.Interop.ExecuteJavaScriptVoidAsync(StringBuilderCache.GetStringAndRelease(sb));
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

                    if (_currentStroke.StylusPoints.InternalCount > 0)
                    {
                        _lastPos = _currentStroke.StylusPoints[_currentStroke.StylusPoints.InternalCount - 1];
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
                    if (_currentStroke.StylusPoints.InternalCount > 1)
                    {
                        _mousePos = _currentStroke.StylusPoints[_currentStroke.StylusPoints.InternalCount - 1];
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
            if (!INTERNAL_VisualTreeManager.IsElementInVisualTree(this) || _currentStroke == null || _currentStroke.StylusPoints.InternalCount <= 1)
            {
                return;
            }

            string sCanvas = OpenSilver.Interop.GetVariableStringForJS(_canvasDom);
            OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                $$"""
                (function(cvs) {
                  const ctx = cvs.getContext('2d');
                  ctx.strokeStyle = '{{_currentStroke.DrawingAttributes.Color.ToHtmlString(1)}}';
                  ctx.lineWidth = '{{_currentStroke.DrawingAttributes.Width.ToInvariantString()}}';
                  ctx.beginPath();
                  ctx.moveTo({{_lastPos.X.ToInvariantString()}}, {{_lastPos.Y.ToInvariantString()}});
                  ctx.lineTo({{_mousePos.X.ToInvariantString()}}, {{_mousePos.Y.ToInvariantString()}});
                  ctx.stroke();
                })({{sCanvas}})
                """);

            _lastPos = _mousePos;
        }
    }
}
