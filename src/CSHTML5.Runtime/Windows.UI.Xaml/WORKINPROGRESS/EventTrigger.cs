

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

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
#if WORKINPROGRESS
    /// <summary>
    ///   A class that controls a set of actions to activate in response to an event
    /// </summary>
    [ContentProperty("Actions")]
    public sealed partial class EventTrigger : TriggerBase
    {
        private static readonly DependencyProperty ActionsProperty = 
            DependencyProperty.Register("Actions", typeof(TriggerActionCollection), typeof(EventTrigger), new PropertyMetadata(new TriggerActionCollection())
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        private static readonly DependencyProperty RoutedEventProperty = 
            DependencyProperty.Register("RoutedEvent", typeof(RoutedEvent), typeof(EventTrigger), null);

        /// <summary>Gets the collection of <see cref="T:System.Windows.Media.Animation.BeginStoryboard" /> objects that this <see cref="T:System.Windows.EventTrigger" /> maintains.</summary>
        /// <returns>The existing <see cref="T:System.Windows.TriggerActionCollection" />.</returns>
        public TriggerActionCollection Actions
        {
            get { return (TriggerActionCollection)this.GetValue(EventTrigger.ActionsProperty); }
        }

        /// <summary>Gets or sets the name of the event that initiates the trigger.</summary>
        /// <returns>The name or identifier of the event. See Remarks.</returns>
        public RoutedEvent RoutedEvent
        {
            get { return (RoutedEvent)this.GetValue(EventTrigger.RoutedEventProperty); }
            set { this.SetValue(EventTrigger.RoutedEventProperty, (object)value); }
        }
    }
#endif
}
