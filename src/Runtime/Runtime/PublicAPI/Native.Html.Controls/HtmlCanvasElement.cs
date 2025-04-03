
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
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CSHTML5.Native.Html.Input;
using OpenSilver.Internal;

namespace CSHTML5.Native.Html.Controls
{
    /// <summary>
    /// Defines an element which can be in a HtmlCanvas. It cannot be instantiated, but
    /// it provides some common properties. Every HtmlCanvas child is an HtmlCanvasElement.
    /// Full documentation is available at:
    /// http://cshtml5.com/links/how-to-use-the-html5-canvas.aspx
    /// </summary>
    public abstract class HtmlCanvasElement
    {
        // FrameworkElement old properties
        public bool IsHitTestVisible;
        public HorizontalAlignment HorizontalAlignment;
        public VerticalAlignment VerticalAlignment;
        public double Opacity;
        public virtual double ActualWidth { get; set; }
        public double ActualHeight;
        public Visibility Visibility;
        public Cursor Cursor;
        public string Name;

        public event EventHandler<HtmlCanvasPointerRoutedEventArgs> PointerPressed;
        public event EventHandler<HtmlCanvasPointerRoutedEventArgs> PointerMoved;
        public event EventHandler<HtmlCanvasPointerRoutedEventArgs> PointerExited;
        public event EventHandler<HtmlCanvasPointerRoutedEventArgs> PointerReleased;
        public event EventHandler<HtmlCanvasPointerRoutedEventArgs> PointerEntered;
        public event EventHandler<HtmlCanvasPointerRoutedEventArgs> RightTapped;

        /// <summary>
        /// X coordinate (relative to its parent)
        /// </summary>
        public double X;
        /// <summary>
        /// Y coordinate (relative to its parent)
        /// </summary>
        public double Y;

        /// <summary>
        /// Common style for this element
        /// </summary>
        public ElementStyle Style;

        private bool _isStyleOverrided;

        // OVERRIDE STYLE PROPERTIES
        private bool? _isVisible;
        private Color? _fillColor;
        private Color? _strokeColor;
        private Color? _shadowColor;
        private double? _shadowBlur;
        private double? _shadowOffsetX;
        private double? _shadowOffsetY;
        private LineCap? _lineCap;
        private LineJoin? _lineJoin;
        private double? _lineWidth;
        private double? _miterLimit;

        // Cache colors as strings for js
        private string _fillStyleStr;
        private string _strokeStyleStr;
        private string _shadowColorStr;

        /// <summary>
        /// Visibility status
        /// </summary>
        public bool IsVisible
        {
            get { return (_isVisible == null) ? Style.IsVisible : (bool)_isVisible; }
            set
            {
                if (_isVisible == null)
                    _isStyleOverrided = true;
                _isVisible = value;
            }
        }

        /// <summary>
        /// Fill color
        /// </summary>
        public Color FillColor
        {
            get { return (_fillColor == null) ? Style.FillColor : (Color)_fillColor; }
            set
            {
                if (_fillColor == null)
                    _isStyleOverrided = true;
                _fillStyleStr = value.ToHtmlString(1);
                _fillColor = value;
            }
        }

        /// <summary>
        /// Stroke color
        /// </summary>
        public Color StrokeColor
        {
            get { return (_strokeColor == null) ? Style.StrokeColor : (Color)_strokeColor; }
            set
            {
                if (_strokeColor == null)
                    _isStyleOverrided = true;
                _strokeStyleStr = value.ToHtmlString(1);
                _strokeColor = value;
            }
        }

        /// <summary>
        /// Shadow color
        /// </summary>
        public Color ShadowColor
        {
            get { return (_shadowColor == null) ? Style.ShadowColor : (Color)_shadowColor; }
            set
            {
                if (_shadowColor == null)
                    _isStyleOverrided = true;
                _shadowColorStr = value.ToHtmlString(1);
                _shadowColor = value;
            }
        }

        /// <summary>
        /// Shadow blur
        /// </summary>
        public double ShadowBlur
        {
            get { return (_shadowBlur == null) ? Style.ShadowBlur : (double)_shadowBlur; }
            set
            {
                if (_shadowBlur == null)
                    _isStyleOverrided = true;
                _shadowBlur = value;
            }
        }

        /// <summary>
        /// Shadow horizontal offset
        /// </summary>
        public double ShadowOffsetX
        {
            get { return (_shadowOffsetX == null) ? Style.ShadowOffsetX : (double)_shadowOffsetX; }
            set
            {
                if (_shadowOffsetX == null)
                    _isStyleOverrided = true;
                _shadowOffsetX = value;
            }
        }

        /// <summary>
        /// Shadow vertical offset
        /// </summary>
        public double ShadowOffsetY
        {
            get { return (_shadowOffsetY == null) ? Style.ShadowOffsetY : (double)_shadowOffsetY; }
            set
            {
                if (_shadowOffsetY == null)
                    _isStyleOverrided = true;
                _shadowOffsetY = value;
            }
        }

        /// <summary>
        /// Style of the end caps for a line
        /// </summary>
        public LineCap LineCap
        {
            get { return (_lineCap == null) ? Style.LineCap : (LineCap)_lineCap; }
            set
            {
                if (_lineCap == null)
                    _isStyleOverrided = true;
                _lineCap = value;
            }
        }
        /// <summary>
        /// Type of corner created, when two lines meet
        /// </summary>
        public LineJoin LineJoin
        {
            get { return (_lineJoin == null) ? Style.LineJoin : (LineJoin)_lineJoin; }
            set
            {
                if (_lineJoin == null)
                    _isStyleOverrided = true;
                _lineJoin = value;
            }
        }

        /// <summary>
        /// Line Width
        /// </summary>
        public double LineWidth
        {
            get { return (_lineWidth == null) ? Style.LineWidth : (double)_lineWidth; }
            set
            {
                if (_lineWidth == null)
                    _isStyleOverrided = true;
                _lineWidth = value;
            }
        }

        /// <summary>
        /// Maximum miter length
        /// </summary>
        public double MiterLimit
        {
            get { return (_miterLimit == null) ? Style.MiterLimit : (double)_miterLimit; }
            set
            {
                if (_miterLimit == null)
                    _isStyleOverrided = true;
                _miterLimit = value;
            }
        }

        /// <summary>
        /// (Deprecated) Access to StrokeColor with Brush instead of Color
        /// </summary>
        [Obsolete]
        public Brush Stroke
        {
            get { return new SolidColorBrush(this.StrokeColor); }
            set
            {
                if (value is SolidColorBrush)
                    this.StrokeColor = ((SolidColorBrush)value).Color;
            }
        }

        /// <summary>
        /// (Deprecated) Access to FillColor with Brush instead of Color
        /// </summary>
        [Obsolete]
        public Brush Background
        {
            get { return new SolidColorBrush(this.FillColor); }
            set
            {
                if (value is SolidColorBrush)
                    this.FillColor = ((SolidColorBrush)value).Color;
            }
        }

        /// <summary>
        /// (Deprecated) Access to FillColor with Brush instead of Color
        /// </summary>
        [Obsolete]
        public Brush Fill
        {
            get { return new SolidColorBrush(this.FillColor); }
            set
            {
                if (value is SolidColorBrush)
                    this.FillColor = ((SolidColorBrush)value).Color;
            }
        }

        /// <summary>
        /// (Deprecated) Exactly the same as LineWidth
        /// </summary>
        public double StrokeThickness
        {
            get { return this.LineWidth; }
            set { this.LineWidth = value; }
        }

        // Constructors that cannot directly be called
        // You can't instantiate a HtmlCanvasElement, only inherited objects
        protected HtmlCanvasElement()
        {
            this.Style = ElementStyle.Default;
            this.IsHitTestVisible = true;
        }

        protected HtmlCanvasElement(double X, double Y, ElementStyle Style = null)
        {
            this.IsHitTestVisible = true;
            this.X = X;
            this.Y = Y;
            if (Style == null)
                this.Style = ElementStyle.Default;
            else
                this.Style = Style;
        }

        ToolTip _toolTipContainer;
        UIElement _toolTip;
        public UIElement ToolTip
        {
            get
            {
                return _toolTip;
            }
            set
            {
                if (value != _toolTip)
                {
                    // Unregister previous tooltip if any:
                    if (_toolTip != null)
                    {
                        // Unregister pointer events:
                        this.PointerEntered -= ToolTipOwner_PointerEntered;
                        this.PointerExited -= ToolTipOwner_PointerExited;
                    }

                    // Remember the new tooltip:
                    _toolTip = value;

                    // Register the new tooltip:
                    if (value != null)
                    {
                        _toolTipContainer = ToolTipService.ConvertToToolTip(value);

                        // Register pointer events:
                        this.PointerEntered -= ToolTipOwner_PointerEntered; // Note: we unregister before registering in order to ensure that it is only registered once.
                        this.PointerEntered += ToolTipOwner_PointerEntered;
                        this.PointerExited -= ToolTipOwner_PointerExited; // Note: we unregister before registering in order to ensure that it is only registered once.
                        this.PointerExited += ToolTipOwner_PointerExited;
                    }
                }
            }
        }

        void ToolTipOwner_PointerEntered(object sender, HtmlCanvasPointerRoutedEventArgs e)
        {
            if (_toolTipContainer != null
                && _toolTipContainer.IsOpen == false)
            {
                Point absoluteCoordinates = e.GetPosition(null);
                Point absoluteCoordinatesShiftedToBeBelowThePointer = new Point(absoluteCoordinates.X, absoluteCoordinates.Y + 20);
                ToolTipService.OpenToolTipAt(_toolTipContainer, absoluteCoordinatesShiftedToBeBelowThePointer);
            }
        }

        void ToolTipOwner_PointerExited(object sender, HtmlCanvasPointerRoutedEventArgs e)
        {
            if (_toolTipContainer != null
                && _toolTipContainer.IsOpen == true)
                ToolTipService.CloseToolTip(_toolTipContainer);
        }

        /// <summary>
        /// Abstract draw method
        /// </summary>
        /// <param name="currentDrawingStyle">Draw style used for last element (can allow optimizations, null if unknown)</param>
        /// <param name="jsContext2d">Canvas 2d javascript context</param>
        /// <param name="xParent">X position of the parent element</param>
        /// <param name="yParent">Y position of the parent element</param>
        /// <returns>Draw style used for this element (can be null)</returns>
        public abstract ElementStyle Draw(ElementStyle currentDrawingStyle, object jsContext2d, double xParent = 0, double yParent = 0);

        /// <summary>
        /// Moves the element relatively to its position
        /// </summary>
        /// <param name="deltaX">X movement</param>
        /// <param name="deltaY">Y movement</param>
        public void Move(double deltaX, double deltaY)
        {
            this.X += deltaX;
            this.Y += deltaY;
        }

        /// <summary>
        /// Moves the element to an absolute position (still relative to its parent)
        /// </summary>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        public void MoveTo(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Apply the element's drawing style (if needed)
        /// </summary>
        /// <param name="oldStyle">Previous style used to draw</param>
        /// <param name="jsContext2d">Canvas 2d javascript context</param>
        /// <returns>Current applied style (can be null)</returns>
        public ElementStyle ApplyStyle(ElementStyle oldStyle, object jsContext2d)
        {
            if (this._isStyleOverrided)
            {
                string context2d = OpenSilver.Interop.GetVariableStringForJS(jsContext2d);

                OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                    $"""
                    {context2d}.fillStyle = '{(_fillColor != null ? _fillStyleStr : Style.fillColorStr)}';
                    {context2d}.strokeStyle = '{(_strokeColor != null ? _strokeStyleStr : Style.strokeColorStr)}';
                    {context2d}.shadowColor = '{(_shadowColor != null ? _shadowColorStr : Style.shadowColorStr)}';
                    {context2d}.shadowBlur = {ShadowBlur.ToInvariantString()};
                    {context2d}.shadowOffsetX = {ShadowOffsetX.ToInvariantString()};
                    {context2d}.shadowOffsetY = {ShadowOffsetY.ToInvariantString()};
                    {context2d}.lineCap = '{ElementStyle.LineCapToHtmlString(LineCap)}';
                    {context2d}.lineJoin = '{ElementStyle.LineJoinToHtmlString(LineJoin)}';
                    {context2d}.lineWidth = {LineWidth.ToInvariantString()};
                    {context2d}.miterLimit = {MiterLimit.ToInvariantString()};
                    """);

                return null;
            }
            else if (this.Style != oldStyle)
            {
                this.Style.Apply(jsContext2d);
                return this.Style;
            }
            else
            {
                return oldStyle;
            }
        }

        public abstract bool IsPointed(double x, double y);

        public virtual void OnPointerMoved(HtmlCanvasPointerRoutedEventArgs e)
        {
            if (PointerMoved != null)
            {
                PointerMoved(this, e);
            }
        }

        public virtual void OnPointerPressed(HtmlCanvasPointerRoutedEventArgs e)
        {
            if (PointerPressed != null)
            {
                PointerPressed(this, e);
            }
        }

        public virtual void OnRightTapped(HtmlCanvasPointerRoutedEventArgs e)
        {
            if (RightTapped != null)
            {
                RightTapped(this, e);
            }
        }

        public virtual void OnPointerReleased(HtmlCanvasPointerRoutedEventArgs e)
        {
            if (PointerReleased != null)
            {
                PointerReleased(this, e);
            }
        }

        public virtual void OnPointerEntered(HtmlCanvasPointerRoutedEventArgs e)
        {
            if (PointerEntered != null)
            {
                PointerEntered(this, e);
            }
        }

        public virtual void OnPointerExited(HtmlCanvasPointerRoutedEventArgs e)
        {
            if (PointerExited != null)
            {
                PointerExited(this, e);
            }
        }

        ContextMenu _contextMenu;
        public ContextMenu ContextMenu
        {
            get
            {
                return _contextMenu;
            }
            set
            {
                if (_contextMenu != null)
                {
                    UnregisterContextMenu(this, _contextMenu);
                }
                _contextMenu = value;
                if (_contextMenu != null)
                {
                    RegisterContextMenu(this, _contextMenu);
                }
            }
        }

        /// <summary>
        /// Occurs when any context menu on the element is opened.
        /// </summary>
        public event ContextMenuEventHandler ContextMenuOpening;

        private static void RegisterContextMenu(HtmlCanvasElement htmlCanvas, ContextMenu menu)
        {
            htmlCanvas.RightTapped += new EventHandler<HtmlCanvasPointerRoutedEventArgs>(OnRightTappedHandler);
        }

        private static void UnregisterContextMenu(HtmlCanvasElement htmlCanvas, ContextMenu menu)
        {
            htmlCanvas.RightTapped -= new EventHandler<HtmlCanvasPointerRoutedEventArgs>(OnRightTappedHandler);
        }

        private static void OnRightTappedHandler(object sender, HtmlCanvasPointerRoutedEventArgs e)
        {
            HtmlCanvasElement htmlCanvas = (HtmlCanvasElement)sender;
            ContextMenu contextMenu = htmlCanvas.ContextMenu;
            if (contextMenu != null && !contextMenu.IsOpen)
            {
                Point pointerPosition = e.GetPosition(null);
                htmlCanvas.ContextMenuOpening?.Invoke(
                    htmlCanvas,
                    new ContextMenuEventArgs(pointerPosition.X, pointerPosition.Y));

                contextMenu.IsOpen = true;
            }
        }
    }
}
