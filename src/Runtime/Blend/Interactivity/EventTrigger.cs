// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------
namespace System.Windows.Interactivity
{
    using System.Diagnostics.CodeAnalysis;

#if MIGRATION
    using System.Windows;
#else
    using global::Windows.UI.Xaml;
#endif

    /// <summary>
    /// A trigger that listens for a specified event on its source and fires when that event is fired.
    /// </summary>
    public class EventTrigger : EventTriggerBase<object>
    {
        public static readonly DependencyProperty EventNameProperty = DependencyProperty.Register("EventName",
                                                                                                    typeof(string),
                                                                                                    typeof(EventTrigger),
#if __WPF__
                                                                                                    new FrameworkPropertyMetadata(
#else
                                                                                                    new PropertyMetadata(
#endif
                                                                                                        "Loaded",
                                                                                                        new PropertyChangedCallback(OnEventNameChanged)));

        /// <summary>
        /// Initializes a new instance of the <see cref="EventTrigger"/> class.
        /// </summary>
        public EventTrigger()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventTrigger"/> class.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        public EventTrigger(string eventName)
        {
            this.EventName = eventName;
        }

        /// <summary>
        /// Gets or sets the name of the event to listen for. This is a dependency property.
        /// </summary>
        /// <value>The name of the event.</value>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        public string EventName
        {
            get { return (string)this.GetValue(EventNameProperty); }
            set { this.SetValue(EventNameProperty, value); }
        }

        protected override string GetEventName()
        {
            return this.EventName;
        }

        private static void OnEventNameChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            ((EventTrigger)sender).OnEventNameChanged((string)args.OldValue, (string)args.NewValue);
        }
    }
}
