// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------
namespace Microsoft.Expression.Interactivity
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Windows.Interactivity;
    using Interactivity = System.Windows.Interactivity;

#if MIGRATION
    using System.Windows;
    using System.Windows.Data;
#else
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Data;
#endif

    /// <summary>
    /// Helper class for managing binding expressions on dependency objects.
    /// </summary>
    internal static class DataBindingHelper
    {
        private static Dictionary<Type, IList<DependencyProperty>> DependenciesPropertyCache = new Dictionary<Type, IList<DependencyProperty>>();
        /// <summary>
        /// Ensure that all DP on an action with binding expressions are
        /// up to date. DataTrigger fires during data binding phase. Since
        /// actions are children of the trigger, any bindings on the action
        /// may not be up-to-date. This routine is called before the action
        /// is invoked in order to guarantee that all bindings are up-to-date
        /// with the most current data. 
        /// </summary>
        public static void EnsureDataBindingUpToDateOnMembers(DependencyObject dpObject)
        {
            IList<DependencyProperty> dpList = null;

            if (!DependenciesPropertyCache.TryGetValue(dpObject.GetType(), out dpList))
            {
                dpList = new List<DependencyProperty>();
                Type type = dpObject.GetType();

                while (type != null)
                {
                    FieldInfo[] fieldInfos = type.GetFields();

                    foreach (FieldInfo fieldInfo in fieldInfos)
                    {
                        if (fieldInfo.IsPublic &&
                            fieldInfo.FieldType == typeof(DependencyProperty))
                        {
                            DependencyProperty property = fieldInfo.GetValue(null) as DependencyProperty;
                            if (property != null)
                            {
                                dpList.Add(property);
                            }
                        }
                    }

                    type = type.BaseType;
                }
                // Cache the list of DP for performance gain
                DependenciesPropertyCache[dpObject.GetType()] = dpList;
            }

            if (dpList == null)
            {
                return;
            }

            foreach (DependencyProperty property in dpList)
            {
                EnsureBindingUpToDate(dpObject, property);
            }

        }

        /// <summary>
        /// Ensures that all binding expression on actions are up to date
        /// </summary>
        public static void EnsureDataBindingOnActionsUpToDate(TriggerBase<DependencyObject> trigger)
        {
            // Update the bindings on the actions. 
            foreach (Interactivity.TriggerAction action in trigger.Actions)
            {
                DataBindingHelper.EnsureDataBindingUpToDateOnMembers(action);
            }
        }

        /// <summary>
        ///  This helper function ensures that, if a dependency property on a dependency object
        ///  has a binding expression, the binding expression is up-to-date. 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="dp"></param>
        public static void EnsureBindingUpToDate(DependencyObject target, DependencyProperty dp)
        {
            BindingExpression binding = BindingOperations.GetBindingExpression(target, dp);
            if (binding != null)
            {
#if __WPF__
                binding.UpdateTarget();
#else
                target.ClearValue(dp);
                BindingOperations.SetBinding(target, dp, binding.ParentBinding);
#endif
            }
        }

    }
}
