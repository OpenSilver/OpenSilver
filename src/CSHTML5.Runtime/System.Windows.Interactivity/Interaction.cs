
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if !MIGRATION
using Windows.UI.Xaml;
#endif

namespace System.Windows.Interactivity
{
    /// <summary>
    /// Static class that owns the Triggers and Behaviors attached properties. Handles
    /// propagation of AssociatedObject change notifications.
    /// </summary>
    public static class Interaction
    {
        /// <summary>
        /// This property is used as the internal backing store for the public Behaviors
        /// attached property.
        /// </summary>
        public static readonly DependencyProperty BehaviorsProperty =
            DependencyProperty.RegisterAttached("Behaviors", typeof(BehaviorCollection), typeof(Interaction), new PropertyMetadata(null));

        /// <summary>
        /// Gets the System.Windows.Interactivity.BehaviorCollection associated with
        /// a specified object.
        /// </summary>
        /// <param name="obj">The object from which to retrieve the System.Windows.Interactivity.BehaviorCollection.</param>
        /// <returns>
        /// A System.Windows.Interactivity.BehaviorCollection containing the behaviors
        /// associated with the specified object.
        /// </returns>
        public static BehaviorCollection GetBehaviors(DependencyObject obj)
        {
            if (obj.GetValue(BehaviorsProperty) == null)
            {
                BehaviorCollection col = new BehaviorCollection();
                col.Attach(obj);
                obj.SetValue(BehaviorsProperty, col);
            }
            return (BehaviorCollection)obj.GetValue(BehaviorsProperty);
        }

        





        ///// <summary>
        ///// This property is used as the internal backing store for the public Triggers
        ///// attached property.
        ///// </summary>
        //public static readonly DependencyProperty TriggersProperty =
        //    DependencyProperty.RegisterAttached("Triggers", typeof(TriggerCollection), typeof(Interaction), new PropertyMetadata(null));
        ///// <summary>
        ///// Gets the TriggerCollection containing the triggers associated with the specified
        ///// object.
        ///// </summary>
        ///// <param name="obj">The object from which to retrieve the triggers.</param>
        ///// <returns>
        ///// A TriggerCollection containing the triggers associated with the specified
        ///// object.
        ///// </returns>
        //public static TriggerCollection GetTriggers(DependencyObject obj)
        //{
        //    return (TriggerCollection)obj.GetValue(TriggersProperty);
        //}
        ////public static void SetTriggers(DependencyObject obj, TriggerCollection value)
        ////{
        ////    obj.SetValue(TriggersProperty, value);
        ////}
        




    }
}
