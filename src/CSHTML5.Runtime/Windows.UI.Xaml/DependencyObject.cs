

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


using CSHTML5.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if MIGRATION
using System.Windows.Threading;
using System.Windows.Data;
#else
using Windows.UI.Core;
using Windows.UI.Xaml.Data;
#endif

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// Represents an object that participates in the dependency property system. DependencyObject
    /// is the immediate base class of many important UI-related classes, such as
    /// UIElement, Geometry, FrameworkTemplate, Style, and ResourceDictionary.
    /// </summary>
    public partial class DependencyObject
    {
        #region Inheritance Context
        private DependencyObject _inheritanceContext;
        private FrameworkElement _inheritanceContextRoot;
        private bool _inheritanceContextRootDirty;
        private readonly HashSet<DependencyObject> _contextListeners = new HashSet<DependencyObject>();
        internal event EventHandler InheritedContextChanged;

        internal void SetInheritanceContext(DependencyObject context)
        {
            DependencyObject oldContext = this._inheritanceContext;
            this._inheritanceContext = context;
            this._inheritanceContextRootDirty = true;

            // stop listening to old context changes
            if (oldContext != null)
            {
                oldContext.StopListeningToInheritanceContextChanges(this);
            }

            // start listening to context changes
            if (context != null)
            {
                context.ListenToInheritanceContextChanges(this);
            }

            // Notify listeners that inheritance context changed
            this.OnInheritedContextChanged();
        }

        private void OnInheritedContextChanged()
        {
            if (this.InheritedContextChanged != null)
            {
                this.InheritedContextChanged(this, EventArgs.Empty);
            }
            foreach (DependencyObject listener in this._contextListeners)
            {
                listener._inheritanceContextRootDirty = true;
                listener.OnInheritedContextChanged();
            }
        }

        private void ListenToInheritanceContextChanges(DependencyObject listener)
        {
            this._contextListeners.Add(listener);
        }

        private void StopListeningToInheritanceContextChanges(DependencyObject listener)
        {
            bool isListening = this._contextListeners.Contains(listener);
            if (isListening)
            {
                this._contextListeners.Remove(listener);
            }
        }

        internal virtual FrameworkElement GetInheritedContext()
        {
            if (this._inheritanceContextRootDirty)
            {
                for (DependencyObject contextDO = this; contextDO != null; contextDO = contextDO._inheritanceContext)
                {
                    if ((this._inheritanceContextRoot = contextDO as FrameworkElement) != null)
                    {
                        break;
                    }
                }
                this._inheritanceContextRootDirty = false;
            }
            return this._inheritanceContextRoot;
        }
        #endregion

        internal Dictionary<DependencyProperty, INTERNAL_PropertyStorage> INTERNAL_PropertyStorageDictionary { get; } // Contains all the properties that are either not in INTERNAL_AllInheritedProperties or in INTERNAL_UsefulInheritedProperties
        internal Dictionary<DependencyProperty, INTERNAL_PropertyStorage> INTERNAL_AllInheritedProperties { get; } // Here so that when we attach a child, the child gets all the properties that are in there (this allows the inherited properties to go all the way down even for properties that are not contained in the children)
        internal List<DependencyProperty> INTERNAL_PropertiesForWhichToCallPropertyChangedWhenLoadedIntoVisualTree; // When a UI element is added to the Visual Tree, we call "PropertyChanged" on all its set properties so that the control can refresh itself. However, when a property is not set, we don't call PropertyChanged. Unless the property is listed here.


        #region Constructor
        public DependencyObject()
        {
            this.INTERNAL_PropertyStorageDictionary = new Dictionary<DependencyProperty, INTERNAL_PropertyStorage>();
            this.INTERNAL_AllInheritedProperties = new Dictionary<DependencyProperty, INTERNAL_PropertyStorage>();
        }
        #endregion


        /// <summary>
        /// Returns the current effective value of a dependency property from a DependencyObject.
        /// </summary>
        /// <param name="dependencyProperty">
        /// The DependencyProperty identifier of the property for which to retrieve the
        /// value.
        /// </param>
        /// <returns>Returns the current effective value.</returns>
        public object GetValue(DependencyProperty dependencyProperty)
        {
            INTERNAL_PropertyStorage storage;
            if (INTERNAL_PropertyStore.TryGetStorage(this, dependencyProperty, false/*don't create*/, out storage))
            {
                return INTERNAL_PropertyStore.GetEffectiveValue(storage);
            }
            return dependencyProperty.GetTypeMetaData(this.GetType()).DefaultValue;         
        }

        /// <summary>
        /// Sets the local value of a dependency property on a DependencyObject while not overriding a hypothetical Binding (example: when the user writes in a TextBox with a two way Binding on its Text property).
        /// </summary>
        /// <param name="dependencyProperty">The identifier of the dependency property to set.</param>
        /// <param name="value">The new local value.</param>
#if WORKINPROGRESS
        [Obsolete("Use SetCurrentValue")]
#endif
        public void SetLocalValue(DependencyProperty dependencyProperty, object value)
        {
            this.SetCurrentValue(dependencyProperty, value);
        }

        public void SetCurrentValue(DependencyProperty dp, object value)
        {
            INTERNAL_PropertyStorage storage;
            INTERNAL_PropertyStore.TryGetStorage(this, dp, true/*create*/, out storage);
            INTERNAL_PropertyStore.SetValueCommon(storage, value, true);
        }

        public void CoerceCurrentValue(DependencyProperty dependencyProperty, PropertyMetadata propertyMetadata)
        {
            INTERNAL_PropertyStorage storage;
            INTERNAL_PropertyStore.TryGetStorage(this, dependencyProperty, true/*create*/, out storage);
            INTERNAL_PropertyStore.CoerceValueCommon(storage);
        }

        /// <summary>
        /// Returns the local value of a dependency property, if a local value is set.
        /// </summary>
        /// <param name="dependencyProperty">
        /// The DependencyProperty identifier of the property for which to retrieve the
        /// local value.
        /// </param>
        /// <returns>
        /// Returns the local value, or returns the sentinel value UnsetValue if no local
        /// value is set.
        /// </returns>
        public object ReadLocalValue(DependencyProperty dependencyProperty)
        {
            INTERNAL_PropertyStorage storage;
            if (INTERNAL_PropertyStore.TryGetStorage(this, dependencyProperty, false/*don't create*/, out storage))
            {
                if (storage.LocalValue == DependencyProperty.UnsetValue)
                {
                    // In silverlight ReadLocalValue returns a BindingExpression if the value 
                    // is a BindingExpression set from a style's setter and the "real" local
                    // value in unset. (This is not the case in WPF)
                    if (storage.LocalStyleValue is BindingExpression be)
                    {
                        return be;
                    }
                }
                return storage.LocalValue;
            }
            return DependencyProperty.UnsetValue;
        }

        public object GetVisualStateValue(DependencyProperty dependencyProperty) //todo: see if this is actually useful (to get specifically the VisualStateValue) and if so, change the GetValue into a GetVisualStateValue at the "return" line.
        {
            if (dependencyProperty == null)
                throw new ArgumentNullException("No property specified");
            INTERNAL_PropertyStorage storage;
            if (INTERNAL_PropertyStore.TryGetStorage(this, dependencyProperty, false/*don't create*/, out storage))
            {
                return storage.AnimatedValue;
            }
            return dependencyProperty.GetTypeMetaData(this.GetType()).DefaultValue;
        }

        public void SetVisualStateValue(DependencyProperty dependencyProperty, object value)
        {
            if (dependencyProperty == null)
                throw new ArgumentNullException("No property specified");

            INTERNAL_PropertyStorage storage;
            INTERNAL_PropertyStore.TryGetStorage(this, dependencyProperty, true/*create*/, out storage);
            INTERNAL_PropertyStore.SetAnimationValue(storage, value);
        }

        public void SetAnimationValue(DependencyProperty dependencyProperty, object value)
        {
            if (dependencyProperty == null)
                throw new ArgumentNullException("No property specified");

            INTERNAL_PropertyStorage storage;
            INTERNAL_PropertyStore.TryGetStorage(this, dependencyProperty, true/*create*/, out storage);
            INTERNAL_PropertyStore.SetAnimationValue(storage, value);
        }

        public object GetAnimationValue(DependencyProperty dependencyProperty)
        {
            if (dependencyProperty == null)
                throw new ArgumentNullException("No property specified");

            INTERNAL_PropertyStorage storage;
            if (INTERNAL_PropertyStore.TryGetStorage(this, dependencyProperty, false/*don't create*/, out storage))
            {
                return storage.AnimatedValue;
            }
            return dependencyProperty.GetTypeMetaData(this.GetType()).DefaultValue;
        }

        public void SetValue(DependencyProperty dp, object value)
        {
            // Verify the arguments:
            if (dp == null)
                throw new ArgumentNullException("No property specified");

            INTERNAL_PropertyStorage storage;
            INTERNAL_PropertyStore.TryGetStorage(this, dp, true/*create*/, out storage);
            INTERNAL_PropertyStore.SetValueCommon(storage, value, false);
        }

        /// <summary>
        /// Sets the inherited value of a dependency property on a DependencyObject. Do not use this method.
        /// </summary>
        /// <param name="dependencyProperty">The identifier of the dependency property to set.</param>
        /// <param name="value">The new local value.</param>
        /// <param name="recursively">Specifies if the inherited value must be applied to the children of this DependencyObject.</param>
        internal void SetInheritedValue(DependencyProperty dependencyProperty, object value, bool recursively)
        {
            //-----------------------
            // CALL "SET INHERITED VALUE" ON THE STORAGE:
            //-----------------------
            INTERNAL_PropertyStorage storage;
            INTERNAL_PropertyStore.TryGetInheritedPropertyStorage(this, dependencyProperty, true/*create*/, out storage);
            INTERNAL_PropertyStore.SetInheritedValue(storage, value, recursively);
        }

        /// <summary>
        /// Refreshes the value of the given DependencyProperty on this DependencyObject so that it fits the coercion that should be applied on it.
        /// </summary>
        /// <param name="dependencyProperty">The dependencyProperty whose value we want to refresh.</param>
        [Obsolete("Use CoerceValue")]
        public void Coerce(DependencyProperty dependencyProperty)
        {
            this.CoerceValue(dependencyProperty);
        }

        public void CoerceValue(DependencyProperty dp)
        {
            INTERNAL_PropertyStorage storage;
            INTERNAL_PropertyStore.TryGetStorage(this, dp, true/*create*/, out storage);
            INTERNAL_PropertyStore.CoerceValueCommon(storage);
        }

        internal void ResetInheritedProperties()
        {
            foreach (DependencyProperty property in INTERNAL_AllInheritedProperties.Keys)
            {
                INTERNAL_PropertyStorage storage;
                if (INTERNAL_PropertyStorageDictionary.TryGetValue(property, out storage))
                {
                    INTERNAL_PropertyStore.ResetInheritedValue(storage);
                    INTERNAL_PropertyStorageDictionary.Remove(property);
                }
            }
            INTERNAL_AllInheritedProperties.Clear();
        }

        /// <summary>
        /// Gets the CoreDispatcher that this object is associated with.
        /// </summary>
#if MIGRATION
        public Dispatcher Dispatcher
#else
        public CoreDispatcher Dispatcher
#endif
        {
            get
            {
#if MIGRATION
                return Dispatcher.INTERNAL_GetCurrentDispatcher();
#else
                return CoreDispatcher.INTERNAL_GetCurrentDispatcher();
#endif
            }
        }


#region Binding related elements

        internal Binding INTERNAL_GetBinding(DependencyProperty dependencyProperty)
        {
            BindingExpression expr = BindingOperations.GetBindingExpression(this, dependencyProperty); 
            if (expr != null)
            {
                return expr.ParentBinding.Clone();
            }
            return null;
        }

        internal void ApplyBindingExpression(DependencyProperty dp, BindingExpression expression)
        {
            //todo: implement this
            INTERNAL_PropertyStorage storage;
            INTERNAL_PropertyStore.TryGetStorage(this, dp, true/*create*/, out storage);
            INTERNAL_PropertyStore.RefreshBindingExpressionCommon(storage, expression); // Set LocalStyle if Binding is from style.
        }

        internal void INTERNAL_UpdateBindingsSource()
        {
            foreach(INTERNAL_PropertyStorage storage in this.INTERNAL_PropertyStorageDictionary.Select(kp => kp.Value))
            {
                if (storage.IsExpression)
                {
                    ((BindingExpression)storage.LocalValue).OnSourceAvailable();
                }
                else if (storage.IsExpressionFromStyle)
                {
                    ((BindingExpression)storage.LocalStyleValue).OnSourceAvailable();
                }
            }
        }

        /// <exclude/>
        internal protected virtual void INTERNAL_OnAttachedToVisualTree()
        {

        }

        /// <exclude/>
        internal protected virtual void INTERNAL_OnDetachedFromVisualTree()
        {
            // This is particularly useful for elements to clear any references they have to DOM elements. For example, the Grid will use it to set its _tableDiv to null.
        }

#endregion

        //
        // Summary:
        //     Clears the local value of a dependency property.
        //
        // Parameters:
        //   dp:
        //     The System.Windows.DependencyProperty identifier of the property to clear the
        //     value for.
        public void ClearValue(DependencyProperty dp)
        {
            INTERNAL_PropertyStorage storage;
            if (INTERNAL_PropertyStore.TryGetStorage(this, dp, false/*don't create*/, out storage))
            {
                INTERNAL_PropertyStore.ClearValueCommon(storage);
            }
        }

#if WORKINPROGRESS
        public bool CheckAccess()
        {
            return false;
        }
#endif
    }
}
