
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

namespace System.Windows.Controls;

/// <summary>
/// Defines the functionality required to support a shared-size group that is used by the 
/// <see cref="ColumnDefinitionCollection"/> and <see cref="RowDefinitionCollection"/> classes.
/// This is an abstract class.
/// </summary>
public abstract class DefinitionBase : DependencyObject
{
    internal DefinitionBase(bool isColumnDefinition)
    {
        _isColumnDefinition = isColumnDefinition;
        _parentIndex = -1;
    }

    internal Grid Parent { get; set; }

    /// <summary>
    /// Callback to notify about entering model tree.
    /// </summary>
    internal void OnEnterParentTree() { }

    /// <summary>
    /// Callback to notify about exitting model tree.
    /// </summary>
    internal void OnExitParentTree()
    {
        _offset = 0;
    }

    /// <summary>
    /// Performs action preparing definition to enter layout calculation mode.
    /// </summary>
    internal void OnBeforeLayout(Grid grid)
    {
        //  reset layout state.
        _minSize = 0;
        LayoutWasUpdated = true;
    }

    /// <summary>
    /// Updates min size.
    /// </summary>
    /// <param name="minSize">New size.</param>
    internal void UpdateMinSize(double minSize) => _minSize = Math.Max(_minSize, minSize);

    /// <summary>
    /// Sets min size.
    /// </summary>
    /// <param name="minSize">New size.</param>
    internal void SetMinSize(double minSize) => _minSize = minSize;

    /// <summary>
    /// <see cref="PropertyMetadata.PropertyChangedCallback"/>
    /// </summary>
    /// <remarks>
    /// This method needs to be internal to be accessable from derived classes.
    /// </remarks>
    internal static void OnUserSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        DefinitionBase definition = (DefinitionBase)d;

        if (definition.InParentLogicalTree)
        {
            Grid parentGrid = definition.Parent;

            if (((GridLength)e.OldValue).GridUnitType != ((GridLength)e.NewValue).GridUnitType)
            {
                parentGrid.Invalidate();
            }
            else
            {
                parentGrid.InvalidateMeasure();
            }
        }
    }

    /// <summary>
    /// <see cref="DependencyProperty.ValidateValueCallback"/>
    /// </summary>
    /// <remarks>
    /// This method needs to be internal to be accessable from derived classes.
    /// </remarks>
    internal static bool IsUserSizePropertyValueValid(object value) => ((GridLength)value).Value >= 0;

    /// <summary>
    /// <see cref="PropertyMetadata.PropertyChangedCallback"/>
    /// </summary>
    /// <remarks>
    /// This method needs to be internal to be accessable from derived classes.
    /// </remarks>
    internal static void OnUserMinSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        DefinitionBase definition = (DefinitionBase)d;

        if (definition.InParentLogicalTree)
        {
            Grid parentGrid = definition.Parent;
            parentGrid.InvalidateMeasure();
        }
    }

    /// <summary>
    /// <see cref="PropertyMetadata.PropertyChangedCallback"/>
    /// </summary>
    /// <remarks>
    /// This method needs to be internal to be accessable from derived classes.
    /// </remarks>
    internal static void OnUserMaxSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        DefinitionBase definition = (DefinitionBase)d;

        if (definition.InParentLogicalTree)
        {
            Grid parentGrid = definition.Parent;
            parentGrid.InvalidateMeasure();
        }
    }

    /// <summary>
    /// Returns <c>true</c> if this definition is a part of shared group.
    /// </summary>
    internal bool IsShared => false;

    /// <summary>
    /// Internal accessor to user size field.
    /// </summary>
    internal GridLength UserSize => UserSizeValueCache;

    /// <summary>
    /// Internal accessor to user min size field.
    /// </summary>
    internal double UserMinSize => UserMinSizeValueCache;

    /// <summary>
    /// Internal accessor to user max size field.
    /// </summary>
    internal double UserMaxSize => UserMaxSizeValueCache;

    /// <summary>
    /// DefinitionBase's index in the parents collection.
    /// </summary>
    internal int Index
    {
        get => _parentIndex;
        set
        {
            Debug.Assert(value >= -1 && _parentIndex != value);
            _parentIndex = value;
        }
    }

    /// <summary>
    /// Layout-time user size type.
    /// </summary>
    internal Grid.LayoutTimeSizeType SizeType
    {
        get => _sizeType;
        set => _sizeType = value;
    }

    /// <summary>
    /// Returns or sets measure size for the definition.
    /// </summary>
    internal double MeasureSize
    {
        get => _measureSize;
        set => _measureSize = value;
    }

    /// <summary>
    /// Returns definition's layout time type sensitive preferred size.
    /// </summary>
    /// <remarks>
    /// Returned value is guaranteed to be true preferred size.
    /// </remarks>
    internal double PreferredSize
    {
        get
        {
            double preferredSize = MinSize;
            if (_sizeType != Grid.LayoutTimeSizeType.Auto && preferredSize < _measureSize)
            {
                preferredSize = _measureSize;
            }
            return preferredSize;
        }
    }

    /// <summary>
    /// Returns or sets size cache for the definition.
    /// </summary>
    internal double SizeCache
    {
        get => _sizeCache;
        set => _sizeCache = value;
    }

    /// <summary>
    /// Returns min size.
    /// </summary>
    internal double MinSize => _minSize;

    /// <summary>
    /// Returns min size, always taking into account shared state.
    /// </summary>
    internal double MinSizeForArrange => _minSize;

    /// <summary>
    /// Returns min size, never taking into account shared state.
    /// </summary>
    internal double RawMinSize => _minSize;

    /// <summary>
    /// Offset.
    /// </summary>
    internal double FinalOffset
    {
        get => _offset;
        set => _offset = value;
    }

    /// <summary>
    /// Internal helper to access up-to-date UserSize property value.
    /// </summary>
    internal GridLength UserSizeValueCache =>
        (GridLength)GetValue(_isColumnDefinition ? ColumnDefinition.WidthProperty : RowDefinition.HeightProperty);

    /// <summary>
    /// Internal helper to access up-to-date UserMinSize property value.
    /// </summary>
    internal double UserMinSizeValueCache =>
        (double)GetValue(_isColumnDefinition ? ColumnDefinition.MinWidthProperty : RowDefinition.MinHeightProperty);

    /// <summary>
    /// Internal helper to access up-to-date UserMaxSize property value.
    /// </summary>
    internal double UserMaxSizeValueCache =>
        (double)GetValue(_isColumnDefinition ? ColumnDefinition.MaxWidthProperty : RowDefinition.MaxHeightProperty);

    /// <summary>
    /// Protected. Returns <c>true</c> if this DefinitionBase instance is in parent's logical tree.
    /// </summary>
    internal bool InParentLogicalTree => _parentIndex != -1;

    /// <summary>
    /// SetFlags is used to set or unset one or multiple
    /// flags on the object.
    /// </summary>
    private void SetFlags(bool value, Flags flags) => _flags = value ? (_flags | flags) : (_flags & (~flags));

    /// <summary>
    /// CheckFlagsAnd returns <c>true</c> if all the flags in the
    /// given bitmask are set on the object.
    /// </summary>
    private bool CheckFlagsAnd(Flags flags) => (_flags & flags) == flags;

    /// <summary>
    /// <see cref="DependencyProperty.ValidateValueCallback"/>
    /// </summary>
    /// <remarks>
    /// Verifies that Shared Size Group Property string
    /// a) not empty.
    /// b) contains only letters, digits and underscore ('_').
    /// c) does not start with a digit.
    /// </remarks>
    private static bool SharedSizeGroupPropertyValueValid(object value)
    {
        //  null is default value
        if (value is null)
        {
            return true;
        }

        string id = (string)value;

        if (id != string.Empty)
        {
            int i = -1;
            while (++i < id.Length)
            {
                bool isDigit = char.IsDigit(id[i]);

                if ((i == 0 && isDigit) || !(isDigit || char.IsLetter(id[i]) || '_' == id[i]))
                {
                    break;
                }
            }

            if (i == id.Length)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Convenience accessor to UseSharedMinimum flag
    /// </summary>
    private bool UseSharedMinimum
    {
        get => CheckFlagsAnd(Flags.UseSharedMinimum);
        set => SetFlags(value, Flags.UseSharedMinimum);
    }

    /// <summary>
    /// Convenience accessor to LayoutWasUpdated flag
    /// </summary>
    private bool LayoutWasUpdated
    {
        get => CheckFlagsAnd(Flags.LayoutWasUpdated);
        set => SetFlags(value, Flags.LayoutWasUpdated);
    }

    private readonly bool _isColumnDefinition;      //  when "true", this is a ColumnDefinition; when "false" this is a RowDefinition (faster than a type check)
    private Flags _flags;                           //  flags reflecting various aspects of internal state
    private int _parentIndex;                       //  this instance's index in parent's children collection

    private Grid.LayoutTimeSizeType _sizeType;      //  layout-time user size type. it may differ from _userSizeValueCache.UnitType when calculating "to-content"

    private double _minSize;                        //  used during measure to accumulate size for "Auto" and "Star" DefinitionBase's
    private double _measureSize;                    //  size, calculated to be the input contstraint size for Child.Measure
    private double _sizeCache;                      //  cache used for various purposes (sorting, caching, etc) during calculations
    private double _offset;                         //  offset of the DefinitionBase from left / top corner (assuming LTR case)

    internal const bool ThisIsColumnDefinition = true;
    internal const bool ThisIsRowDefinition = false;

    [Flags]
    private enum Flags : byte
    {
        //
        //  bool flags
        //
        UseSharedMinimum = 0x00000020,     //  when "1", definition will take into account shared state's minimum
        LayoutWasUpdated = 0x00000040,     //  set to "1" every time the parent grid is measured
    }
}