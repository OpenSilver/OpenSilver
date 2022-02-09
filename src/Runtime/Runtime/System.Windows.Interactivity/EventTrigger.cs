

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


using CSHTML5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
#if MIGRATION
using System.Windows.Input;
using Microsoft.Windows;
#else
using System.Windows;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
#endif

namespace System.Windows.Interactivity
{
    /// <summary>
    /// A trigger that listens for a specified event on its source and fires when that event is fired.
    /// 
    /// </summary>
    public partial class EventTrigger : EventTriggerBase<object>
    {
        public static readonly DependencyProperty EventNameProperty =
            DependencyProperty.Register("EventName", typeof(string), typeof(EventTrigger), new PropertyMetadata("Loaded", new PropertyChangedCallback(EventTrigger.OnEventNameChanged)));

        /// <summary>
        /// Gets or sets the name of the event to listen for. This is a dependency property.
        /// </summary>
        /// 
        /// <value>
        /// The name of the event.
        /// </value>
        public string EventName
        {
            get
            {
                return (string)this.GetValue(EventTrigger.EventNameProperty);
            }
            set
            {
                this.SetValue(EventTrigger.EventNameProperty, (object)value);
            }
        }

        static EventTrigger()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Windows.Interactivity.EventTrigger"/> class.
        /// </summary>
        public EventTrigger()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Windows.Interactivity.EventTrigger"/> class.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        public EventTrigger(string eventName)
        {
            this.EventName = eventName;
        }

        private static void OnEventNameChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            ((EventTrigger)sender).OnEventNameChanged((string)args.OldValue, (string)args.NewValue);
        }

        protected override string GetEventName()
        {
            return this.EventName;
        }
    }
}