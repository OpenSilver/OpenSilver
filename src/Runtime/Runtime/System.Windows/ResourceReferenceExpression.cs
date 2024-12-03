
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

using System.Diagnostics;
using OpenSilver.Theming;

namespace System.Windows;

/// <summary>
///     Expression to evaluate a ResourceReference
/// </summary>
internal sealed class ResourceReferenceExpression : Expression
{
    private readonly object _resourceKey; // Name of the resource being referenced by this expression

    // Cached value and a dirty bit.  See GetValue.
    private object _cachedResourceValue;

    // Used to find the value for this expression when it is set on a non-FE.
    // The mentor is the FE that the FindResource method is invoked on.
    private IInternalFrameworkElement _mentorCache;

    // Used by the change listener to fire invalidation.
    private DependencyObject _targetObject;
    private DependencyProperty _targetProperty;

    private InternalState _state = InternalState.Default;

    /// <summary>
    ///     Constructor for ResourceReferenceExpression
    /// </summary>
    /// <param name="resourceKey">
    ///     Name of the resource being referenced
    /// </param>
    public ResourceReferenceExpression(object resourceKey)
    {
        _resourceKey = resourceKey;
    }

    internal override object GetValue(DependencyObject d, DependencyProperty dp)
    {
        if (d is null)
        {
            throw new ArgumentNullException(nameof(d));
        }
        if (dp is null)
        {
            throw new ArgumentNullException(nameof(dp));
        }

        if (ReadInternalState(InternalState.HasCachedResourceValue))
        {
            return _cachedResourceValue;
        }

        return GetRawValue(d, dp);
    }

    internal override bool CanSetValue(DependencyObject d, DependencyProperty dp) => false;

    internal override void OnAttach(DependencyObject d, DependencyProperty dp)
    {
        _targetObject = d;
        _targetProperty = dp;

        if (d is not IInternalFrameworkElement)
        {
            // Listen for the InheritanceContextChanged event on the target node,
            // so that if this context hierarchy changes we can re-evaluate this expression.
            _targetObject.InheritedContextChanged += new EventHandler(InvalidateExpressionValue);
        }
    }

    internal override void OnDetach(DependencyObject d, DependencyProperty dp)
    {
        // Invalidate all the caches
        InvalidateMentorCache();

        if (_targetObject is not IInternalFrameworkElement)
        {
            // Stop listening for the InheritanceContextChanged event on the target node
            _targetObject.InheritedContextChanged -= new EventHandler(InvalidateExpressionValue);
        }

        _targetObject = null;
        _targetProperty = null;
    }

    /// <summary>
    ///     Called to evaluate the ResourceReferenceExpression value
    /// </summary>
    /// <param name="d">DependencyObject being queried</param>
    /// <param name="dp">DependencyProperty</param>
    /// <returns>Computed value. Unset if unavailable.</returns>
    /// <remarks>
    /// This routine has been separated from the above GetValue call because it is
    /// invoked by the ResourceReferenceExpressionConverter during serialization.
    /// </remarks>
    private object GetRawValue(DependencyObject d, DependencyProperty dp)
    {
        // Find the mentor node to invoke FindResource on. For example
        // <Button>
        //   <Button.Background>
        //     <SolidColorBrush Color="{DynamicResource MyColor}" />
        //   </Button.Background>
        // </Button
        // Button is the mentor for the ResourceReference on SolidColorBrush
        if (!ReadInternalState(InternalState.IsMentorCacheValid))
        {
            // Find the mentor by walking up the InheritanceContext
            // links and update the cache
            _mentorCache = FrameworkElement.FindMentor(d);
            WriteInternalState(InternalState.IsMentorCacheValid, true);

            if (_mentorCache is not null)
            {
                Debug.Assert(
                    _targetObject == d,
                    "TargetObject that this expression is attached to must be the same as the one on which its value is being queried");
                
                _mentorCache.ResourcesChanged += new EventHandler(InvalidateExpressionValue);
            }
        }

        object resource;
        if (_mentorCache is IInternalFrameworkElement fe)
        {
            // If there is a mentor do a FindResource call starting at that node
            resource = FindResourceInTree(fe, _resourceKey);
        }
        else
        {
            // If there is no mentor then simply search the App and the Themes for the right resource
            resource = FindResourceFromApp(_resourceKey);
        }

        // Assuming that null means the value doesn't exist in the resources section
        resource ??= DependencyProperty.UnsetValue;

        // Update the cached values with this resource instance
        _cachedResourceValue = resource;
        WriteInternalState(InternalState.HasCachedResourceValue, true);

        // Return the resource
        return resource;
    }

    private static object FindResourceInTree(IInternalFrameworkElement fe, object resourceKey)
    {
        // First, try to find a resource in parents' resources.
        var f = fe;
        while (f != null)
        {
            if (f.HasResources && f.Resources.TryGetResource(resourceKey, out object resource))
            {
                return resource;
            }

            f = (f.Parent ?? f.VisualParent) as FrameworkElement;
        }

        // Then we try to find the resource in the App's Resources
        // if we can't find it in the parents.
        return FindResourceFromApp(resourceKey);
    }

    private static object FindResourceFromApp(object resourceKey)
    {
        if (Application.Current is Application app)
        {
            if (app.HasResources && app.Resources.TryGetResource(resourceKey, out object resource))
            {
                return resource;
            }

            if (app.Theme is Theme theme && theme.TryGetResource(resourceKey, out resource))
            {
                return resource;
            }
        }

        return DependencyProperty.UnsetValue;
    }

    /// <summary>
    ///     This event handler is called to invalidate the cached value held in
    ///     this expression. This is called under the following 3 scenarios
    ///     1. InheritanceContext changes
    ///     2. Logical tree changes
    ///     3. ResourceDictionary changes
    /// </summary>
    private void InvalidateExpressionValue(object sender, EventArgs e)
    {
        if (_targetObject is null)
        {
            return;
        }

        if (e is ResourcesChangedEventArgs args)
        {
            ResourcesChangeInfo info = args.Info;
            if (!info.IsTreeChange)
            {
                // This will happen when
                // 1. Theme changes
                // 2. Entire ResourceDictionary in the ancestry changes
                // 3. Single entry in a ResourceDictionary in the ancestry is changed
                // In all of the above cases it is sufficient to re-evaluate the cache
                // value alone. The mentor relation ships stay the same.
                InvalidateCacheValue();
            }
            else
            {
                // This is the case of a logical tree change and hence we need to
                // re-evaluate both the mentor and the cached value.
                InvalidateMentorCache();
            }
        }
        else
        {
            // There is no information provided by the EventArgs. Hence we
            // pessimistically invalidate both the mentor and the cached value.
            // This code path will execute when the InheritanceContext changes.
            InvalidateMentorCache();
        }

        _targetObject.ApplyExpression(_targetProperty, this);
    }

    /// <summary>
    /// This method is called when the cached value of the resource has
    /// been invalidated.  E.g. after a new Resources property is set somewhere
    /// in the ancestory.
    /// </summary>
    private void InvalidateCacheValue()
    {
        _cachedResourceValue = null;
        WriteInternalState(InternalState.HasCachedResourceValue, false);
    }

    /// <summary>
    ///     This method is called to invalidate all the cached values held in
    ///     this expression. This is called under the following 3 scenarios
    ///     1. InheritanceContext changes
    ///     2. Logical tree changes
    ///     3. ResourceDictionary changes
    ///     This call is more pervasive than the InvalidateCacheValue method
    /// </summary>
    private void InvalidateMentorCache()
    {
        if (ReadInternalState(InternalState.IsMentorCacheValid))
        {
            if (_mentorCache is not null)
            {
                // Your mentor is about to change, make sure you detach handlers for
                // the events that you were listening on the old mentor
                _mentorCache.ResourcesChanged -= new EventHandler(InvalidateExpressionValue);

                // Drop the mentor cache
                _mentorCache = null;
            }

            // Mark the cache invalid
            WriteInternalState(InternalState.IsMentorCacheValid, false);
        }

        // Invalidate the cached value of the expression
        InvalidateCacheValue();
    }

    // Extracts the required flag and returns
    // bool to indicate if it is set or unset
    private bool ReadInternalState(InternalState reqFlag) => (_state & reqFlag) != 0;

    // Sets or Unsets the required flag based on
    // the bool argument
    private void WriteInternalState(InternalState reqFlag, bool set)
    {
        if (set)
        {
            _state |= reqFlag;
        }
        else
        {
            _state &= ~reqFlag;
        }
    }

    /// <summary>
    /// This enum represents the internal state of the RRE.
    /// Additional bools should be coalesced into this enum.
    /// </summary>
    [Flags]
    private enum InternalState : byte
    {
        Default = 0x00,
        HasCachedResourceValue = 0x01,
        IsMentorCacheValid = 0x02,
    }
}
