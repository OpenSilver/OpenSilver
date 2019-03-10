
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
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
    public sealed class EventTrigger : TriggerBase
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
