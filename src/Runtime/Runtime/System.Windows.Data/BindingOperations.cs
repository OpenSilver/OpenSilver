
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

using System.Collections;
using System.Collections.Generic;
using OpenSilver.Internal;
using OpenSilver.Internal.Data;

namespace System.Windows.Data;

/// <summary>
/// Provides static methods to manipulate bindings, including <see cref="Binding"/>,
/// <see cref="MultiBinding"/> objects.
/// </summary>
public static class BindingOperations
{
    /// <summary>
    /// Creates and associates a new instance of <see cref="BindingExpressionBase"/>
    /// with the specified binding target property.
    /// </summary>
    /// <param name="target">
    /// The binding target of the binding.
    /// </param>
    /// <param name="dp">
    /// The target property of the binding.
    /// </param>
    /// <param name="binding">
    /// The <see cref="BindingBase"/> object that describes the binding.
    /// </param>
    /// <returns>
    /// The instance of <see cref="BindingExpressionBase"/> created for and associated
    /// with the specified property. The <see cref="BindingExpressionBase"/> class is 
    /// the base class of <see cref="BindingExpression"/> and <see cref="MultiBindingExpression"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// The binding parameter cannot be null.
    /// </exception>
    public static BindingExpressionBase SetBinding(DependencyObject target, DependencyProperty dp, BindingBase binding)
    {
        if (target is null)
        {
            throw new ArgumentNullException(nameof(target));
        }
        if (dp is null)
        {
            throw new ArgumentNullException(nameof(dp));
        }
        if (binding is null)
        {
            throw new ArgumentNullException(nameof(binding));
        }

        var expr = binding.CreateBindingExpression(target, dp, null);

        expr.IsInTransfer = true;
        try
        {
            target.SetValue(dp, expr);
        }
        finally
        {
            expr.IsInTransfer = false;
        }

        return expr;
    }

    /// <summary>
    /// Creates and associates a new <see cref="BindingExpression"/> with the specified binding target property.
    /// </summary>
    /// <param name="target">
    /// The binding target of the binding.
    /// </param>
    /// <param name="dp">
    /// The target property of the binding.
    /// </param>
    /// <param name="binding">
    /// The <see cref="Binding"/> object that describes the binding.
    /// </param>
    /// <returns>
    /// The instance of <see cref="BindingExpression"/> created for and associated with the specified property.
    /// </returns>
    public static BindingExpression SetBinding(DependencyObject target, DependencyProperty dp, Binding binding)
        => (BindingExpression)SetBinding(target, dp, (BindingBase)binding);

    /// <summary>
    /// Retrieves the <see cref="BindingExpressionBase"/> object that is set on the specified property.
    /// </summary>
    /// <param name="target">
    /// The object where dp is.
    /// </param>
    /// <param name="dp">
    /// The binding target property from which to retrieve the <see cref="BindingExpressionBase"/> object.
    /// </param>
    /// <returns>
    /// The <see cref="BindingExpressionBase"/> object that is set on the given property or null if no binding 
    /// object has been set.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// The target and dp parameters cannot be null.
    /// </exception>
    public static BindingExpressionBase GetBindingExpressionBase(DependencyObject target, DependencyProperty dp)
    {
        if (target is null)
        {
            throw new ArgumentNullException(nameof(target));
        }

        if (dp is null)
        {
            throw new ArgumentNullException(nameof(dp));
        }

        if (target.GetStorage(dp.GlobalIndex) is Storage storage && storage.Entry.IsExpression)
        {
            return storage.Entry.ModifiedValue.BaseValue as BindingExpressionBase;
        }

        return null;
    }

    /// <summary>
    /// Retrieves the <see cref="BindingBase"/> object that is set on the specified property.
    /// </summary>
    /// <param name="target">
    /// The object where dp is.
    /// </param>
    /// <param name="dp">
    /// The binding target property from which to retrieve the <see cref="BindingBase"/> object.
    /// </param>
    /// <returns>
    /// The <see cref="BindingBase"/> object that is set on the given property or null if no binding object has been set.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// The target and dp parameters cannot be null.
    /// </exception>
    public static BindingBase GetBindingBase(DependencyObject target, DependencyProperty dp)
    {
        if (GetBindingExpressionBase(target, dp) is BindingExpressionBase binding)
        {
            return binding.ParentBindingBase;
        }

        return null;
    }

    /// <summary>
    /// Returns the <see cref="BindingExpression"/> object associated with the specified binding target 
    /// property on the specified object.
    /// </summary>
    /// <param name="target">
    /// The binding target object where dp is.
    /// </param>
    /// <param name="dp">
    /// The binding target property from which to retrieve the <see cref="BindingExpression"/> object.
    /// </param>
    /// <returns>
    /// The <see cref="BindingExpression"/> object associated with the given property or null if none exists.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// The target and dp parameters cannot be null.
    /// </exception>
    public static BindingExpression GetBindingExpression(DependencyObject target, DependencyProperty dp)
        => GetBindingExpressionBase(target, dp) as BindingExpression;

    /// <summary>
    /// Retrieves the <see cref="Binding"/> object that is set on the specified property.
    /// </summary>
    /// <param name="target">
    /// The object where dp is.
    /// </param>
    /// <param name="dp">
    /// The binding target property from which to retrieve the binding.
    /// </param>
    /// <returns>
    /// The <see cref="Binding"/> object set on the given property or null if no <see cref="Binding"/> object has been set.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// The target and dp parameters cannot be null.
    /// </exception>
    public static Binding GetBinding(DependencyObject target, DependencyProperty dp)
        => GetBindingBase(target, dp) as Binding;

    /// <summary>
    /// Returns the <see cref="MultiBindingExpression"/> object associated with the specified binding 
    /// target property on the specified object.
    /// </summary>
    /// <param name="target">
    /// The binding target object where dp is.
    /// </param>
    /// <param name="dp">
    /// The binding target property from which to retrieve the <see cref="MultiBindingExpression"/> object.
    /// </param>
    /// <returns>
    /// The <see cref="MultiBindingExpression"/> object associated with the given property or null if none 
    /// exists.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// The target and dp parameters cannot be null.
    /// </exception>
    public static MultiBindingExpression GetMultiBindingExpression(DependencyObject target, DependencyProperty dp)
        => GetBindingExpressionBase(target, dp) as MultiBindingExpression;

    /// <summary>
    /// Retrieves the <see cref="MultiBinding"/> object that is set on the specified property.
    /// </summary>
    /// <param name="target">
    /// The object where dp is.
    /// </param>
    /// <param name="dp">
    /// The binding target property from which to retrieve the binding.
    /// </param>
    /// <returns>
    /// The <see cref="MultiBinding"/> object set on the given property or null if no <see cref="MultiBinding"/> object has been set.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// The target and dp parameters cannot be null.
    /// </exception>
    public static MultiBinding GetMultiBinding(DependencyObject target, DependencyProperty dp)
        => GetBindingBase(target, dp) as MultiBinding;

    /// <summary>
    /// Removes all bindings, including bindings of type <see cref="Binding"/> and <see cref="MultiBinding"/>, from
    /// the specified <see cref="DependencyObject"/>.
    /// </summary>
    /// <param name="target">
    /// The object from which to remove bindings.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// If target is null.
    /// </exception>
    public static void ClearAllBindings(DependencyObject target)
    {
        if (target is null)
        {
            throw new ArgumentNullException(nameof(target));
        }

        LocalValueEnumerator lve = target.GetLocalValueEnumerator();

        var batch = new List<DependencyProperty>();

        while (lve.MoveNext())
        {
            LocalValueEntry entry = lve.Current;
            if (IsDataBound(target, entry.Property))
            {
                batch.Add(entry.Property);
            }
        }

        // Clear all properties that are storing BindingExpressions
        for (int i = 0; i < batch.Count; i++)
        {
            target.ClearValue(batch[i]);
        }
    }

    /// <summary>
    /// Removes the binding from a property if there is one.
    /// </summary>
    /// <param name="target">
    /// The object from which to remove the binding.
    /// </param>
    /// <param name="dp">
    /// The dependency property from which to remove the binding.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// The <paramref name="target"/> and <paramref name="dp"/> parameters cannot be null.
    /// </exception>
    public static void ClearBinding(DependencyObject target, DependencyProperty dp)
    {
        if (target is null)
        {
            throw new ArgumentNullException(nameof(target));
        }

        if (dp is null)
        {
            throw new ArgumentNullException(nameof(dp));
        }

        if (IsDataBound(target, dp))
        {
            target.ClearValue(dp);
        }
    }

    /// <summary>
    /// Returns a value that indicates whether the specified property is currently data-bound.
    /// </summary>
    /// <param name="target">
    /// The object where dp is.
    /// </param>
    /// <param name="dp">
    /// The dependency property to check.
    /// </param>
    /// <returns>
    /// true if the specified property is data-bound; otherwise, false.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="target"/> or <paramref name="dp"/> is null.
    /// </exception>
    public static bool IsDataBound(DependencyObject target, DependencyProperty dp)
    {
        if (target is null)
        {
            throw new ArgumentNullException(nameof(target));
        }

        if (dp is null)
        {
            throw new ArgumentNullException(nameof(dp));
        }

        return target.GetStorage(dp.GlobalIndex) is Storage storage &&
               storage.Entry.IsExpression &&
               storage.Entry.ModifiedValue.BaseValue is BindingExpressionBase;
    }

    /// <summary>
    /// Enables a <see cref="CollectionView"/> object to participate in synchronized access to a collection 
    /// used on multiple threads by using a mechanism other than a simple lock.
    /// </summary>
    /// <param name="collection">
    /// The collection that needs synchronized access.
    /// </param>
    /// <param name="context">
    /// An object that is passed to the callback.
    /// </param>
    /// <param name="synchronizationCallback">
    /// The callback that is invoked whenever access to the collection is required. You can use it to ensure 
    /// that the collection is accessed by one thread at a time.
    /// </param>
    public static void EnableCollectionSynchronization(IEnumerable collection, object context, CollectionSynchronizationCallback synchronizationCallback)
    {
        if (collection is null)
        {
            throw new ArgumentNullException(nameof(collection));
        }

        if (synchronizationCallback is null)
        {
            throw new ArgumentNullException(nameof(synchronizationCallback));
        }

        ViewManager.Current.RegisterCollectionSynchronizationCallback(
            collection, context, synchronizationCallback);
    }

    /// <summary>
    /// Enables a <see cref="CollectionView"/> object to participate in synchronized access to a collection 
    /// used on multiple threads by using a simple locking mechanism.
    /// </summary>
    /// <param name="collection">
    /// The collection that needs synchronized access.
    /// </param>
    /// <param name="lockObject">
    /// The object to lock when accessing the collection.
    /// </param>
    public static void EnableCollectionSynchronization(IEnumerable collection, object lockObject)
    {
        if (collection is null)
        {
            throw new ArgumentNullException(nameof(collection));
        }

        if (lockObject is null)
        {
            throw new ArgumentNullException(nameof(lockObject));
        }

        ViewManager.Current.RegisterCollectionSynchronizationCallback(
            collection, lockObject, null);
    }

    /// <summary>
    /// Remove the synchronization registered for the specified collection.
    /// </summary>
    /// <param name="collection">
    /// The collection to remove synchronized access from.
    /// </param>
    public static void DisableCollectionSynchronization(IEnumerable collection)
    {
        if (collection is null)
        {
            throw new ArgumentNullException(nameof(collection));
        }

        ViewManager.Current.RegisterCollectionSynchronizationCallback(
            collection, null, null);
    }

    /// <summary>
    /// Provides access to a collection by using the synchronization mechanism that the application specified 
    /// when it called EnableCollectionSynchronization.
    /// </summary>
    /// <param name="collection">
    /// The collection to access.
    /// </param>
    /// <param name="accessMethod">
    /// The action to perform on the collection.
    /// </param>
    /// <param name="writeAccess">
    /// true if accessMethod will write to the collection; otherwise, false.
    /// </param>
    public static void AccessCollection(IEnumerable collection, Action accessMethod, bool writeAccess)
    {
        ViewManager vm = ViewManager.Current ??
            throw new InvalidOperationException(string.Format(Strings.AccessCollectionAfterShutDown, collection));

        vm.AccessCollection(collection, accessMethod, writeAccess);
    }

    /// <summary>
    /// Occurs when the data-binding system notices a collection.
    /// </summary>
    public static event EventHandler<CollectionRegisteringEventArgs> CollectionRegistering;

    /// <summary>
    /// Occurs when the data-binding system notices a collection view.
    /// </summary>
    public static event EventHandler<CollectionViewRegisteringEventArgs> CollectionViewRegistering;

    // Raise the CollectionRegistering event
    internal static void OnCollectionRegistering(IEnumerable collection, object parent) =>
        CollectionRegistering?.Invoke(null, new CollectionRegisteringEventArgs(collection, parent));

    // Raise the CollectionViewRegistering event
    internal static void OnCollectionViewRegistering(CollectionView view) =>
        CollectionViewRegistering?.Invoke(null, new CollectionViewRegisteringEventArgs(view));
}
