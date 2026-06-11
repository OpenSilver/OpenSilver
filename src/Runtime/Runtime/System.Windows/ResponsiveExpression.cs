
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
using System.Windows.Data;

namespace System.Windows;

internal sealed class ResponsiveExpression : Expression
{
    private readonly object _mobile;
    private readonly object _tablet;
    private readonly object _desktop;
    private readonly ResponsiveThreshold? _threshold;
    private readonly FrameworkElement _source;
    private readonly string _elementName;
    private readonly RelativeSource _relativeSource;

    // Used by the change listener to fire invalidation.
    private DependencyObject _targetObject;
    private DependencyProperty _targetProperty;

    // Cached value and a dirty bit.  See GetValue.
    private object _cachedValue;

    // Used to find the value for this expression when it is set on a non-FE.
    // The mentor is the FE that is used to identify the source element.
    private FrameworkElement _mentorCache;

    // The element whose width is measured: a reference element resolved from ElementName/RelativeSource/Source,
    // or the Window when none is specified or the reference element could not be resolved.
    private FrameworkElement _sourceElement;

    private InternalState _state = InternalState.Default;

    public ResponsiveExpression(
        object mobile,
        object tablet,
        object desktop,
        ResponsiveThreshold? threshold,
        FrameworkElement source,
        string elementName,
        RelativeSource relativeSource)
    {
        _mobile = mobile;
        _tablet = tablet;
        _desktop = desktop;
        _threshold = threshold;
        _source = source;
        _elementName = elementName;
        _relativeSource = relativeSource;
    }

    internal override bool CanSetValue(DependencyObject d, DependencyProperty dp) => false;

    internal override object GetValue(DependencyObject d, DependencyProperty dp)
    {
        ArgumentNullException.ThrowIfNull(d);
        ArgumentNullException.ThrowIfNull(dp);

        if (ReadInternalState(InternalState.HasCachedValue))
        {
            return _cachedValue;
        }

        return GetRawValue(d, dp);
    }

    internal override void OnAttach(DependencyObject d, DependencyProperty dp)
    {
        _targetObject = d;
        _targetProperty = dp;

        if (d is not FrameworkElement)
        {
            _targetObject.InheritedContextChanged += OnMentorChanged;
        }
    }

    internal override void OnDetach(DependencyObject d, DependencyProperty dp)
    {
        InvalidateMentorCache();

        if (_targetObject is not FrameworkElement)
        {
            _targetObject.InheritedContextChanged -= OnMentorChanged;
        }

        _targetObject = null;
        _targetProperty = null;
    }

    private object GetRawValue(DependencyObject d, DependencyProperty dp)
    {
        if (!ReadInternalState(InternalState.IsMentorCacheValid))
        {
            _mentorCache = FindMentor(d);
            WriteInternalState(InternalState.IsMentorCacheValid, true);

            if (_mentorCache is not null)
            {
                Debug.Assert(
                    _targetObject == d,
                    "TargetObject that this expression is attached to must be the same as the one on which its value is being queried");

                _mentorCache.AddHandler(FrameworkElement.LoadedEvent, new RoutedEventHandler(OnMentorLoaded), true);
                _mentorCache.AddHandler(FrameworkElement.UnloadedEvent, new RoutedEventHandler(OnMentorUnloaded), true);
            }
        }

        if (!ReadInternalState(InternalState.IsSourceElementCacheValid))
        {
            _sourceElement = ResolveSourceElement();
            WriteInternalState(InternalState.IsSourceElementCacheValid, true);

            _sourceElement?.SizeChanged += OnSourceElementSizeChanged;
        }

        object value = DependencyProperty.UnsetValue;

        if (_sourceElement is not null)
        {
            double width = _sourceElement.RenderSize.Width;
            ResponsiveThreshold threshold = _threshold ?? ResponsiveThreshold.Default;

            if (width >= threshold.Tablet)
            {
                value = _desktop;
            }
            else if (width >= threshold.Mobile)
            {
                value = _tablet;
            }
            else
            {
                value = _mobile;
            }
        }

        _cachedValue = value;
        WriteInternalState(InternalState.HasCachedValue, true);

        return value;
    }

    private FrameworkElement ResolveSourceElement()
    {
        if (_mentorCache is null)
        {
            return null;
        }

        FrameworkElement sourceElement = null;

        if (_source is not null)
        {
            sourceElement = _source;
        }
        else if (_elementName is not null)
        {
            sourceElement = BindingExpression.FindName(_mentorCache, _elementName) as FrameworkElement;
        }
        else if (_relativeSource is not null)
        {
            sourceElement = _relativeSource.Mode switch
            {
                RelativeSourceMode.Self => _mentorCache,
                RelativeSourceMode.TemplatedParent => _mentorCache.TemplatedParent as FrameworkElement,
                RelativeSourceMode.FindAncestor => BindingExpression.FindAncestorOftype(_mentorCache, _relativeSource.AncestorType, _relativeSource.AncestorLevel) as FrameworkElement,
                _ => null,
            };
        }

        sourceElement ??= _mentorCache.ParentWindow;

        return sourceElement;
    }

    private void InvalidateMentorCache()
    {
        if (ReadInternalState(InternalState.IsMentorCacheValid))
        {
            if (_mentorCache is not null)
            {
                _mentorCache.RemoveHandler(FrameworkElement.LoadedEvent, new RoutedEventHandler(OnMentorLoaded));
                _mentorCache.RemoveHandler(FrameworkElement.UnloadedEvent, new RoutedEventHandler(OnMentorUnloaded));

                _mentorCache = null;
            }

            WriteInternalState(InternalState.IsMentorCacheValid, false);
        }

        InvalidateSourceElementCache();
    }

    private void InvalidateSourceElementCache()
    {
        if (ReadInternalState(InternalState.IsSourceElementCacheValid))
        {
            _sourceElement?.SizeChanged -= OnSourceElementSizeChanged;
            _sourceElement = null;

            WriteInternalState(InternalState.IsSourceElementCacheValid, false);
        }

        InvalidateCacheValue();
    }

    private void InvalidateCacheValue()
    {
        _cachedValue = null;
        WriteInternalState(InternalState.HasCachedValue, false);
    }

    private void OnSourceElementSizeChanged(object sender, SizeChangedEventArgs e)
    {
        InvalidateCacheValue();

        _targetObject.ApplyExpression(_targetProperty, this);
    }

    private void OnMentorLoaded(object sender, RoutedEventArgs e)
    {
        InvalidateSourceElementCache();

        _targetObject.ApplyExpression(_targetProperty, this);
    }

    private void OnMentorUnloaded(object sender, RoutedEventArgs e)
    {
        InvalidateSourceElementCache();
    }

    private void OnMentorChanged(object sender, EventArgs e)
    {
        if (_targetObject is null)
        {
            return;
        }

        InvalidateMentorCache();

        _targetObject.ApplyExpression(_targetProperty, this);
    }

    private static FrameworkElement FindMentor(DependencyObject d)
    {
        // Find the nearest FE InheritanceContext
        while (d is not null)
        {
            if (d is FrameworkElement fe)
            {
                return fe;
            }
            else
            {
                d = d.InheritanceContext;
            }
        }

        return null;
    }

    // Extracts the required flag and returns bool to indicate if it is set or unset
    private bool ReadInternalState(InternalState reqFlag) => (_state & reqFlag) != 0;

    // Sets or Unsets the required flag based on the bool argument
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
    /// This enum represents the internal state of the ResponsiveExtension.
    /// Additional bools should be coalesced into this enum.
    /// </summary>
    [Flags]
    private enum InternalState : byte
    {
        Default = 0x00,
        HasCachedValue = 0x01,
        IsMentorCacheValid = 0x02,
        IsSourceElementCacheValid = 0x04,
    }
}
