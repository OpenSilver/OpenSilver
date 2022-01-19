

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
using System.Windows.Markup;
using System.Windows.Interactivity;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    ///   A class that controls a set of actions to activate in response to an event
    /// </summary>
    [ContentProperty("Actions")]
    public sealed partial class EventTrigger : TriggerBase
    {
        private static readonly DependencyProperty RoutedEventProperty = 
            DependencyProperty.Register("RoutedEvent", typeof(RoutedEvent), typeof(EventTrigger), null);

        public EventTrigger() : base(typeof(EventTrigger))
        {
        }

        /// <summary>Gets or sets the name of the event that initiates the trigger.</summary>
        /// <returns>The name or identifier of the event. See Remarks.</returns>
		[OpenSilver.NotImplemented]
        public RoutedEvent RoutedEvent
        {
            get { return (RoutedEvent)this.GetValue(EventTrigger.RoutedEventProperty); }
            set { this.SetValue(EventTrigger.RoutedEventProperty, (object)value); }
        }
    }
}
