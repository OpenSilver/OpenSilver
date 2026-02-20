
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

using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;

namespace System.Windows
{
    public abstract partial class UIElement
    {
        /// <summary>
        /// Identifies the <see cref="DoubleTap"/> routed event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static readonly RoutedEvent DoubleTapEvent =
            EventManager.RegisterRoutedEvent(
                nameof(DoubleTap),
                RoutingStrategy.Direct,
                typeof(EventHandler<GestureEventArgs>),
                typeof(UIElement));

        /// <summary>
        /// Occurs when a DoubleTap gesture is committed while over this <see cref="UIElement"/>.
        /// </summary>
        [OpenSilver.NotImplemented]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler<GestureEventArgs> DoubleTap
        {
            add => AddHandler(DoubleTapEvent, value);
            remove => RemoveHandler(DoubleTapEvent, value);
        }

        /// <summary>
        /// Called before the <see cref="DoubleTap"/> event occurs.
        /// </summary>
        /// <param name="e">
        /// Event data for the event.
        /// </param>
        [OpenSilver.NotImplemented]
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected virtual void OnDoubleTap(GestureEventArgs e) { }

        /// <summary>
        /// Identifies the <see cref="ManipulationDelta"/> routed event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static readonly RoutedEvent ManipulationDeltaEvent =
            EventManager.RegisterRoutedEvent(
                nameof(ManipulationDelta),
                RoutingStrategy.Direct,
                typeof(EventHandler<ManipulationDeltaEventArgs>),
                typeof(UIElement));

        /// <summary>
        /// Occurs when the input device changes position during a manipulation.
        /// </summary>
        [OpenSilver.NotImplemented]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler<ManipulationDeltaEventArgs> ManipulationDelta
        {
            add => AddHandler(ManipulationDeltaEvent, value);
            remove => RemoveHandler(ManipulationDeltaEvent, value);
        }

        /// <summary>
        /// Called before the <see cref="ManipulationDelta"/> event occurs.
        /// </summary>
        /// <param name="e">
        /// Event data for the event.
        /// </param>
        [OpenSilver.NotImplemented]
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected virtual void OnManipulationDelta(ManipulationDeltaEventArgs e) { }

        /// <summary>
        /// Identifies the <see cref="ManipulationStarted"/> routed event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static readonly RoutedEvent ManipulationStartedEvent =
            EventManager.RegisterRoutedEvent(
                nameof(ManipulationStarted),
                RoutingStrategy.Direct,
                typeof(EventHandler<ManipulationStartedEventArgs>),
                typeof(UIElement));

        /// <summary>
        /// Occurs when an input device begins a manipulation on the <see cref="UIElement"/>.
        /// </summary>
        [OpenSilver.NotImplemented]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler<ManipulationStartedEventArgs> ManipulationStarted
        {
            add => AddHandler(ManipulationStartedEvent, value);
            remove => RemoveHandler(ManipulationStartedEvent, value);
        }

        /// <summary>
        /// Called before the <see cref="ManipulationStarted"/> event occurs.
        /// </summary>
        /// <param name="e">
        /// Event data for the event.
        /// </param>
        [OpenSilver.NotImplemented]
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected virtual void OnManipulationStarted(ManipulationStartedEventArgs e) { }

        /// <summary>
        /// Identifies the <see cref="ManipulationCompleted"/> routed event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static readonly RoutedEvent ManipulationCompletedEvent =
            EventManager.RegisterRoutedEvent(
                nameof(ManipulationCompleted),
                RoutingStrategy.Direct,
                typeof(EventHandler<ManipulationCompletedEventArgs>),
                typeof(UIElement));

        /// <summary>
        /// Occurs when a manipulation and inertia on the <see cref="UIElement"/> is complete.
        /// </summary>
        [OpenSilver.NotImplemented]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler<ManipulationCompletedEventArgs> ManipulationCompleted
        {
            add => AddHandler(ManipulationCompletedEvent, value);
            remove => RemoveHandler(ManipulationCompletedEvent, value);
        }

        /// <summary>
        /// Called before the <see cref="ManipulationCompleted"/> event occurs.
        /// </summary>
        /// <param name="e">
        /// Event data for the event.
        /// </param>
        [OpenSilver.NotImplemented]
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected virtual void OnManipulationCompleted(ManipulationCompletedEventArgs e) { }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static readonly RoutedEvent DragEnterEvent =
            EventManager.RegisterRoutedEvent(
                nameof(DragEnter),
                RoutingStrategy.Bubble,
                typeof(DragEventHandler),
                typeof(UIElement));

        [OpenSilver.NotImplemented]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public event DragEventHandler DragEnter
        {
            add => AddHandler(DragEnterEvent, value);
            remove => RemoveHandler(DragEnterEvent, value);
        }

        [OpenSilver.NotImplemented]
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected virtual void OnDragEnter(DragEventArgs e) { }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static readonly RoutedEvent DragLeaveEvent =
            EventManager.RegisterRoutedEvent(
                nameof(DragLeave),
                RoutingStrategy.Bubble,
                typeof(DragEventHandler),
                typeof(UIElement));

        [OpenSilver.NotImplemented]
        [EditorBrowsable(EditorBrowsableState.Never)]
		public event DragEventHandler DragLeave
        {
            add => AddHandler(DragLeaveEvent, value);
            remove => RemoveHandler(DragLeaveEvent, value);
        }

        [OpenSilver.NotImplemented]
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected virtual void OnDragLeave(DragEventArgs e) { }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static readonly RoutedEvent DropEvent =
            EventManager.RegisterRoutedEvent(
                nameof(Drop),
                RoutingStrategy.Bubble,
                typeof(DragEventHandler),
                typeof(UIElement));

        [OpenSilver.NotImplemented]
        [EditorBrowsable(EditorBrowsableState.Never)]
		public event DragEventHandler Drop
        {
            add => AddHandler(DropEvent, value);
            remove => RemoveHandler(DropEvent, value);
        }

        [OpenSilver.NotImplemented]
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected virtual void OnDrop(DragEventArgs e) { }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static readonly RoutedEvent DragOverEvent =
            EventManager.RegisterRoutedEvent(
                nameof(DragOver),
                RoutingStrategy.Bubble,
                typeof(DragEventHandler),
                typeof(UIElement));

        [OpenSilver.NotImplemented]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public event DragEventHandler DragOver
        {
            add => AddHandler(DragOverEvent, value);
            remove => RemoveHandler(DragOverEvent, value);
        }

        [OpenSilver.NotImplemented]
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected virtual void OnDragOver(DragEventArgs e) { }

        /// <summary>
        /// Gets or sets a value that indicates that rendered 
        /// content should be cached when possible.
        /// </summary>
        /// <returns>
        /// A value that indicates that rendered content should be 
        /// cached when possible. If you specify a value of 
        /// <see cref="Media.CacheMode" />, rendering operations from 
        /// <see cref="UIElement.RenderTransform" /> and 
        /// <see cref="UIElement.Opacity" /> execute on the graphics 
        /// processing unit (GPU), if available. The default is null, 
        /// which does not enable a cached composition mode. 
        /// </returns>
        [OpenSilver.NotImplemented]
        public CacheMode CacheMode
        {
            get { return (CacheMode)GetValue(CacheModeProperty); }
            set { SetValueInternal(CacheModeProperty, value); }
        }

        /// <summary>Identifies the <see cref="CacheMode" /> dependency property.</summary>
        /// <returns>The identifier for the <see cref="CacheMode" /> dependency property.</returns>
		[OpenSilver.NotImplemented]
        public static readonly DependencyProperty CacheModeProperty =
            DependencyProperty.Register(nameof(CacheMode), 
                                        typeof(CacheMode), 
                                        typeof(UIElement), 
                                        null);

		[OpenSilver.NotImplemented]
        public Projection Projection
        {
            get { return (Projection)GetValue(ProjectionProperty); }
            set { SetValueInternal(ProjectionProperty, value); }
        }

		[OpenSilver.NotImplemented]
        public static readonly DependencyProperty ProjectionProperty =
            DependencyProperty.Register(nameof(Projection), 
                                        typeof(Projection), 
                                        typeof(UIElement), 
                                        null);
    }
}
