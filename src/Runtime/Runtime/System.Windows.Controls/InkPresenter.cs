#if MIGRATION
using CSHTML5.Internal;
using System.Windows.Ink;
using System.Windows.Input;

namespace System.Windows.Controls
{
    /// <summary>
    /// Implements a rectangular surface that displays ink strokes.
    /// </summary>

    public class InkPresenter : Canvas
    {
        private object _canvasDom;

        public InkPresenter()
        {
            Strokes = new StrokeCollection();
            Loaded += InkPresenter_Loaded;
        }

        private void InkPresenter_Loaded(object sender, RoutedEventArgs e)
        {
            ResetCanvas();
        }

        private void ResetCanvas()
        {
            if (_canvasDom == null) return;

            var javascript = @"
                        // get current size of the canvas
                        let rect = $0.getBoundingClientRect();
                        // increase the actual size of our canvas
                        $0.width = rect.width * window.devicePixelRatio;
                        $0.height = rect.height * window.devicePixelRatio;
                        let ctx = $0.getContext('2d');
                        // ensure all drawing operations are scaled
                        ctx.scale(window.devicePixelRatio, window.devicePixelRatio);
                        // scale everything down using CSS
                        $0.style.width = rect.width + 'px';
                        $0.style.height = rect.height + 'px';
                        ctx.strokeStyle=$1; ctx.lineWidth=$2;
                        ctx.clearRect(0, 0, $0.width, $0.height);";
            OpenSilver.Interop.ExecuteJavaScriptAsync(javascript, _canvasDom, "#222222", 4);
        }

        public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            var div = base.CreateDomElement(parentRef, out domElementWhereToPlaceChildren);
            _canvasDom = INTERNAL_HtmlDomManager.CreateDomElementAndAppendIt("canvas", div, this);
            var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(_canvasDom);
            style.width = "100%";
            style.height = "100%";
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
            get { return (StrokeCollection)GetValue(StrokesProperty); }
            set { SetValue(StrokesProperty, value); }
        }

        public static readonly DependencyProperty StrokesProperty =
            DependencyProperty.Register("Strokes", typeof(StrokeCollection), typeof(InkPresenter), new PropertyMetadata(OnStrokePropertyChanged));

        private static void OnStrokePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            InkPresenter presenter = (InkPresenter)d;
            presenter?.HandleStrokesPropertyChanged(e);
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

            if (Strokes != null)
            {
                foreach (var stroke in Strokes)
                {
                    DrawStroke(stroke);
                }
            }            
        }

        private void DrawStroke(Stroke stroke)
        {
            if (stroke == null || stroke.StylusPoints.Count <= 1) return;

            object context = OpenSilver.Interop.ExecuteJavaScriptAsync(@"$0.getContext('2d')", _canvasDom);
            var firstPoint = stroke.StylusPoints[0];
            OpenSilver.Interop.ExecuteJavaScriptAsync(@"$0.moveTo($1, $2);", context, firstPoint.X, firstPoint.Y);

            for (int i = 1; i < stroke.StylusPoints.Count; i++)
            {
                OpenSilver.Interop.ExecuteJavaScriptAsync(@"$0.lineTo($1, $2);", context, stroke.StylusPoints[i].X, stroke.StylusPoints[i].Y);
            }

            OpenSilver.Interop.ExecuteJavaScriptAsync(@"$0.stroke();", context);
        }


        private Stroke CurrentStroke { get; set; }
        private void OnStrokeCollectionChanged(object sender, Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch(e.Action)
            {
                case Collections.Specialized.NotifyCollectionChangedAction.Add:
                    if (CurrentStroke != null)
                    {
                        CurrentStroke.StylusPoints.CollectionChanged -= OnStylusPointsCollectionChanged;
                    }

                    CurrentStroke = e.NewItems[0] as Stroke;


                    if (CurrentStroke == null) return;

                    DrawStroke(CurrentStroke);

                    if (CurrentStroke.StylusPoints.Count > 0)
                    {
                        LastPos = CurrentStroke.StylusPoints[CurrentStroke.StylusPoints.Count - 1];
                    }

                    CurrentStroke.StylusPoints.CollectionChanged += OnStylusPointsCollectionChanged;
                    break;

                default:
                    // in all other cases, redraw canvas
                    if (e.OldItems != null)
                    {
                        foreach (var item in e.OldItems)
                        {
                            if (item is Stroke stroke)
                            {
                                stroke.StylusPoints.CollectionChanged -= OnStylusPointsCollectionChanged;
                                if (stroke == CurrentStroke)
                                {
                                    CurrentStroke = null;
                                }
                            }
                        }
                    }

                    if (e.NewItems != null)
                    {
                        foreach (var item in e.NewItems)
                        {
                            if (item is Stroke stroke)
                            {
                                stroke.StylusPoints.CollectionChanged += OnStylusPointsCollectionChanged;
                            }
                        }
                    }

                    DrawAllStrokes();
                    break;
            }
        }

        private void OnStylusPointsCollectionChanged(object sender, Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch(e.Action)
            {
                case Collections.Specialized.NotifyCollectionChangedAction.Add:
                    if (CurrentStroke.StylusPoints.Count > 1)
                    {
                        MousePos = CurrentStroke.StylusPoints[CurrentStroke.StylusPoints.Count - 1];
                        DrawCurrentPoint();
                    }
                    break;
                default:
                    DrawAllStrokes();
                    break;
            }
            
        }

        private StylusPoint LastPos { get; set; }
        private StylusPoint MousePos { get; set; }

        private void DrawCurrentPoint()
        {
            if (!INTERNAL_VisualTreeManager.IsElementInVisualTree(this) || CurrentStroke == null || CurrentStroke.StylusPoints.Count <= 1)
            {
                return;
            }

            object context = OpenSilver.Interop.ExecuteJavaScriptAsync(@"$0.getContext('2d')", _canvasDom);
            OpenSilver.Interop.ExecuteJavaScriptAsync(@"$0.moveTo($1, $2); $0.lineTo($3, $4); $0.stroke();", context, LastPos.X, LastPos.Y, MousePos.X, MousePos.Y);
            LastPos = MousePos;
        }
    }
}

#endif