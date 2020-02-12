
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



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
        private static readonly DependencyProperty ActionsProperty = DependencyProperty.Register("Actions", typeof(TriggerActionCollection), typeof(EventTrigger), new PropertyMetadata(new TriggerActionCollection()));
        private static readonly DependencyProperty RoutedEventProperty = DependencyProperty.Register("RoutedEvent", typeof(RoutedEvent), typeof(EventTrigger), null);

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
