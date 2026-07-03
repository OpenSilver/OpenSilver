
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
using OpenSilver;
using OpenSilver.Internal;
using OpenSilver.Internal.Media;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace System.Windows.Media;

/// <summary>
/// Paints an area with a <see cref="UIElement"/>.
/// </summary>
public sealed class VisualBrush : TileBrush
{
    private readonly Bitmap _bitmap;

    /// <summary>
    /// Initializes a new instance of the <see cref="VisualBrush"/> class.
    /// </summary>
    public VisualBrush()
    {
        _bitmap = new(this);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VisualBrush"/> class that contains the specified <see cref="UIElement"/>.
    /// </summary>
    /// <param name="visual">
    /// The contents of the new <see cref="VisualBrush"/>.
    /// </param>
    public VisualBrush(UIElement visual)
        : this()
    {
        Visual = visual;
    }

    ~VisualBrush() => _bitmap.ReleaseResource();

    /// <summary>
    /// Identifies the <see cref="Visual"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty VisualProperty =
        DependencyProperty.Register(
            nameof(Visual),
            typeof(UIElement),
            typeof(VisualBrush),
            new PropertyMetadata(null, OnVisualChanged));

    /// <summary>
    /// Gets or sets the brush's content.
    /// </summary>
    /// <returns>
    /// The brush's content. The default is null.
    /// </returns>
    public UIElement Visual
    {
        get => (UIElement)GetValue(VisualProperty);
        set => SetValueInternal(VisualProperty, value);
    }

    private static void OnVisualChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((VisualBrush)d).Invalidate();
    }

    /// <summary>
    /// Identifies the <see cref="AutoLayoutContent"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AutoLayoutContentProperty =
        DependencyProperty.Register(
            nameof(AutoLayoutContent),
            typeof(bool),
            typeof(VisualBrush),
            new PropertyMetadata(BooleanBoxes.TrueBox));

    /// <summary>
    /// Gets or sets a value that specifies whether this <see cref="VisualBrush"/> will run layout its <see cref="Visual"/>.
    /// </summary>
    /// <returns>
    /// true if this Brush should run layout on its <see cref="Visual"/>; otherwise, false. The default is true.
    /// </returns>
    public bool AutoLayoutContent
    {
        get => (bool)GetValue(AutoLayoutContentProperty);
        set => SetValueInternal(AutoLayoutContentProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="PixelRatio"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PixelRatioProperty =
        DependencyProperty.Register(
            nameof(PixelRatio),
            typeof(int),
            typeof(VisualBrush),
            new PropertyMetadata(1, OnPixelRatioChanged),
            IsValidPixelRatio);

    /// <summary>
    /// Gets or sets the pixel ratio used when rasterizing the <see cref="Visual"/> into a snapshot image.
    /// </summary>
    /// <returns>
    /// A value greater than or equal to 1. Higher values increase the snapshot resolution and sharpness.
    /// The default value is 1.
    /// </returns>
    public int PixelRatio
    {
        get => (int)GetValue(PixelRatioProperty);
        set => SetValueInternal(PixelRatioProperty, value);
    }

    private static void OnPixelRatioChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((VisualBrush)d).Invalidate();
    }

    private static bool IsValidPixelRatio(object value) => ((int)value) >= 1;

    /// <summary>
    /// Regenerate the brush.
    /// </summary>
    public void Invalidate()
    {
        _bitmap.SetDirty();
        RaiseChanged();
    }

    internal override ImageSource GetImageSource() => _bitmap;

    internal override ISvgBrush GetSvgElement(Shape shape) => new VisualBrushSvgPattern(shape, this);

    private sealed class VisualBrushSvgPattern : SvgPattern
    {
        public VisualBrushSvgPattern(Shape shape, VisualBrush visualBrush)
            : base(shape, visualBrush)
        {
        }

        protected override int PixelRatio => Unsafe.As<VisualBrush>(TileBrush).PixelRatio;
    }

    private sealed class Bitmap : ImageSource
    {
        private readonly VisualBrush _visualBrush;
        private readonly object _lock = new();
        private string _cachedUrl = string.Empty;
        private bool _isDirty = false;
        private TaskCompletionSource<string> _pending;

        public Bitmap(VisualBrush visualBrush)
        {
            _visualBrush = visualBrush;
        }

        public void SetDirty()
        {
            lock (_lock)
            {
                _isDirty = true;
                _pending = null;
            }
        }

        internal void ReleaseResource()
        {
            lock (_lock)
            {
                RevokeURL(_cachedUrl);
                _isDirty = true;
                _cachedUrl = null;
                _pending = null;
            }
        }

        internal override ValueTask<string> GetDataStringAsync(UIElement parent)
        {
            string url;
            TaskCompletionSource<string> tcsToComplete = null;

            lock (_lock)
            {
                if (_isDirty && _pending is null)
                {
                    TaskCompletionSource<string> tcs = TryRequestSnapshot();

                    if (tcs is null)
                    {
                        RevokeURL(_cachedUrl);
                        _cachedUrl = string.Empty;
                        _isDirty = false;
                    }
                    else
                    {
                        _pending = tcsToComplete = tcs;
                    }
                }

                url = _cachedUrl;
            }

            if (tcsToComplete is not null)
            {
                _ = ApplySnapshotAsync(tcsToComplete);
            }

            return new ValueTask<string>(url);
        }

        private TaskCompletionSource<string> TryRequestSnapshot()
        {
            if (_visualBrush.Visual is not UIElement element)
            {
                return null;
            }

            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(element))
            {
                return RequestSnapshot(element);
            }

            if (VisualTreeHelper.GetParent(element) is null && _visualBrush.AutoLayoutContent)
            {
                if (!Renderer.Render(element))
                {
                    return null;
                }

                UIElement.PropagateResumeLayout(null, element);
                element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                element.Arrange(new Rect(element.DesiredSize));

                var snapshot = RequestSnapshot(element);

                Renderer.Clear(element);
                UIElement.PropagateSuspendLayout(element);

                return snapshot;
            }

            return null;
        }

        private async Task ApplySnapshotAsync(TaskCompletionSource<string> tcs)
        {
            string url = await tcs.Task;

            string urlToRevoke = null;
            bool raiseChanged = false;

            lock (_lock)
            {
                if (ReferenceEquals(_pending, tcs))
                {
                    _pending = null;
                    urlToRevoke = _cachedUrl;
                    _cachedUrl = url;
                    _isDirty = false;
                    raiseChanged = true;
                }
                else
                {
                    urlToRevoke = url;
                }
            }

            RevokeURL(urlToRevoke);

            if (raiseChanged)
            {
                _visualBrush.RaiseChanged();
            }
        }

        private TaskCompletionSource<string> RequestSnapshot(UIElement element)
        {
            var tcs = new TaskCompletionSource<string>(TaskContinuationOptions.RunContinuationsAsynchronously);

            string pixelRatio = _visualBrush.PixelRatio.ToInvariantString();

            var callback = OpenSilver.Interop.GetVariableStringForJS(
                JavaScriptCallbackHelper.CreateSelfDisposedJavaScriptCallback<string>(tcs.SetResult));

            OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                $"osjs.visualBrush.create('{element.OuterDiv.Uid}', {pixelRatio}, {callback})");

            return tcs;
        }

        private static void RevokeURL(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                OpenSilver.Interop.ExecuteJavaScriptVoidAsync($"osjs.visualBrush.release('{url}')");
            }
        }

        private static class Renderer
        {
            private static HtmlElementReference? _renderArea;
            private static Window _window;

            public static bool Render(UIElement element)
            {
                if (TryGetRenderArea(out HtmlElementReference renderArea))
                {
                    INTERNAL_VisualTreeManager.InternalAttachVisual(element, _window, renderArea);
                    element.UpdateIsRenderableCache();
                    element.UpdateIsVisibleCache();
                    return true;
                }

                return false;
            }

            public static void Clear(UIElement element)
            {
                if (INTERNAL_VisualTreeManager.IsElementInVisualTree(element))
                {
                    INTERNAL_VisualTreeManager.InternalDetachVisual(element);
                }
            }

            private static bool TryGetRenderArea(out HtmlElementReference renderArea)
            {
                if (_renderArea.HasValue)
                {
                    renderArea = _renderArea.Value;
                    return true;
                }

                if (Application.Current is not Application app || app.MainWindow is not Window window)
                {
                    renderArea = default;
                    return false;
                }

                string id = INTERNAL_HtmlDomManager.NewId();

                if (!OpenSilver.Interop.ExecuteJavaScriptBoolean($"osjs.visualBrush.createRenderArea('{id}', '{window.OuterDiv.Uid}')"))
                {
                    renderArea = default;
                    return false;
                }

                _renderArea = renderArea = new HtmlElementReference(id);
                _window = window;

                return true;
            }
        }
    }
}
