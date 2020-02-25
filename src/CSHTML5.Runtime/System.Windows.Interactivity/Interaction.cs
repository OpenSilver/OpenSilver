
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


        /// <summary>
        /// This property is used as the internal backing store for the public Triggers attached property.
        /// </summary>
        public static readonly DependencyProperty TriggersProperty =
            DependencyProperty.RegisterAttached("Triggers", typeof(TriggerCollection), typeof(Interaction), new PropertyMetadata(null, new PropertyChangedCallback(Interaction.OnTriggersChanged)));

        /// <summary>
        /// Gets the TriggerCollection containing the triggers associated with the specified object.
        /// 
        /// </summary>
        /// <param name="obj">The object from which to retrieve the triggers.</param>
        /// <returns>
        /// A TriggerCollection containing the triggers associated with the specified object.
        /// </returns>
        public static TriggerCollection GetTriggers(DependencyObject obj)
        {
            TriggerCollection triggerCollection = (TriggerCollection)obj.GetValue(Interaction.TriggersProperty);
            if (triggerCollection == null)
            {
                triggerCollection = new TriggerCollection();
                triggerCollection.Attach(obj); //CSHTML5 Added 
                obj.SetValue(Interaction.TriggersProperty, triggerCollection);
            }
            return triggerCollection;
        }

        /// <exception cref="T:System.InvalidOperationException">Cannot host the same TriggerCollection on more than one object at a time.</exception>
        private static void OnTriggersChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            TriggerCollection triggerCollection1 = args.OldValue as TriggerCollection;
            TriggerCollection triggerCollection2 = args.NewValue as TriggerCollection;
            if (triggerCollection1 == triggerCollection2)
            {
                return;
            }
            if (triggerCollection1 != null && triggerCollection1.AssociatedObject != null)
            {
                triggerCollection1.Detach();
            }
            if (triggerCollection2 == null || obj == null)
            {
                return;
            }
            if (triggerCollection2.AssociatedObject != null)
            {
                throw new InvalidOperationException("Cannot Host TriggerCollection Multiple Times");
            }
            FrameworkElement fElement = obj as FrameworkElement;
            if (fElement == null)
            {
                throw new InvalidOperationException("Can only host BehaviorCollection on FrameworkElement");
            }
            triggerCollection2.Attach(fElement);
        }





    }
}
