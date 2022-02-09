

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
            DependencyProperty.RegisterAttached("Behaviors", typeof(BehaviorCollection), typeof(Interaction), new PropertyMetadata(null, new PropertyChangedCallback(OnBehaviorsChanged)));

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
                
                // pymendoza: Do not attach here because OpenSilver internally raises the collection changed event
                //            on SetValue that will in effect attach the dependency object to the collection. Calling Attach to an already attached
                //            collection will throw an InvalidOperationException.

                //col.Attach(obj);

                obj.SetValue(BehaviorsProperty, col);
            }
            return (BehaviorCollection)obj.GetValue(BehaviorsProperty);
        }


        /// <summary>
        /// This property is used as the internal backing store for the public Triggers attached property.
        /// </summary>
        public static readonly DependencyProperty TriggersProperty =
            DependencyProperty.RegisterAttached("Triggers", typeof(TriggerCollection), typeof(Interaction), new PropertyMetadata(null, new PropertyChangedCallback(Interaction.OnTriggersChanged))
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

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

        /// <summary>
        /// A helper function to take the place of FrameworkElement.IsLoaded, as this property is not available in Silverlight.
        /// </summary>
        /// <param name="element">The element of interest.</param>
        /// <returns>True if the element has been loaded; otherwise, False.</returns>
        internal static bool IsElementLoaded(FrameworkElement element)
        {
            return element.IsLoaded;
        }

        /// <exception cref="InvalidOperationException">Cannot host the same BehaviorCollection on more than one object at a time.</exception>
		private static void OnBehaviorsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            BehaviorCollection oldCollection = (BehaviorCollection)args.OldValue;
            BehaviorCollection newCollection = (BehaviorCollection)args.NewValue;

            if (oldCollection != newCollection)
            {
                if (oldCollection != null && ((IAttachedObject)oldCollection).AssociatedObject != null)
                {
                    oldCollection.Detach();
                }

                if (newCollection != null && obj != null)
                {
                    if (((IAttachedObject)newCollection).AssociatedObject != null)
                    {
                        throw new InvalidOperationException("An instance of a Behavior cannot be attached to more than one object at a time.");
                    }

                    newCollection.Attach(obj);
                }
            }
        }
    }
}