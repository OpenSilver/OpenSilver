
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

using OpenSilver.Internal;
using OpenSilver.Internal.Media;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Windows.Media;

/// <summary>
/// Defines a geometric shape, described using a <see cref="StreamGeometryContext"/>.
/// This geometry is light-weight alternative to <see cref="PathGeometry"/>: it does not support 
/// data binding, animation, or modification.
/// </summary>
[TypeConverter(typeof(GeometryConverter))]
public sealed class StreamGeometry : Geometry
{
    private byte[] _data;

    /// <summary>
    /// Initializes a new, empty instance of the <see cref="StreamGeometry"/> class.
    /// </summary>
    public StreamGeometry() { }

    /// <summary>
    /// Identifies the <see cref="FillRule"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty FillRuleProperty =
        DependencyProperty.Register(
            nameof(FillRule),
            typeof(FillRule),
            typeof(StreamGeometry),
            new PropertyMetadata(FillRule.EvenOdd, OnFillRuleChanged),
            ValidateEnums.IsFillRuleValid);

    /// <summary>
    /// Gets or sets a value that determines how the intersecting areas contained in this 
    /// <see cref="StreamGeometry"/> are combined.
    /// </summary>
    /// <returns>
    /// Indicates how the intersecting areas of this <see cref="StreamGeometry"/> are combined. 
    /// The default value is <see cref="FillRule.EvenOdd"/>.
    /// </returns>
    public FillRule FillRule
    {
        get => (FillRule)GetValue(FillRuleProperty);
        set => SetValueInternal(FillRuleProperty, value);
    }

    /// <summary>
    /// Determines whether this <see cref="StreamGeometry"/> describes a geometric shape.
    /// </summary>
    /// <returns>
    /// true if this <see cref="StreamGeometry"/> describes a geometry shape; otherwise, false.
    /// </returns>
    public override bool IsEmpty()
    {
        if (_data is null || _data.Length <= 0)
        {
            return true;
        }

        Debug.Assert(_data is not null && _data.Length >= Unsafe.SizeOf<MIL_PATHGEOMETRY>());

        MIL_PATHGEOMETRY pPathGeometry = MemoryMarshal.Read<MIL_PATHGEOMETRY>(_data);

        return pPathGeometry.FigureCount <= 0;
    }

    /// <summary>
    /// Determines whether this <see cref="StreamGeometry"/> contains a curved segment.
    /// </summary>
    /// <returns>
    /// true if this <see cref="StreamGeometry"/> object has a curved segment; otherwise, false.
    /// </returns>
    public override bool MayHaveCurves()
    {
        if (IsEmpty())
        {
            return false;
        }

        Debug.Assert(_data is not null && _data.Length >= Unsafe.SizeOf<MIL_PATHGEOMETRY>());

        MIL_PATHGEOMETRY pPathGeometryData = MemoryMarshal.Read<MIL_PATHGEOMETRY>(_data);

        return (pPathGeometryData.Flags & MilPathGeometryFlags.HasCurves) != 0;
    }

    /// <summary>
    /// Removes all geometric information from this <see cref="StreamGeometry"/>.
    /// </summary>
    public void Clear() => SetPathData(null);

    /// <summary>
    /// Opens a <see cref="StreamGeometryContext"/> that can be used to describe this <see cref="StreamGeometry"/> 
    /// object's contents.
    /// </summary>
    /// <returns>
    /// A <see cref="StreamGeometryContext"/> that can be used to describe this <see cref="StreamGeometry"/> 
    /// object's contents.
    /// </returns>
    public StreamGeometryContext Open() => new StreamGeometryCallbackContext(this);

    internal override void SerializeData(CapacityStreamGeometryContext context, Matrix transform)
    {
        Matrix matrix = GetCombinedMatrix(transform);
        PathGeometry.ParsePathGeometryData(GetPathGeometryData(), matrix, context);
    }

    /// <summary>
    /// GetPathGeometryData - returns a struct which contains this Geometry represented
    /// as a path geometry's serialized format.
    /// </summary>
    private PathGeometryData GetPathGeometryData()
    {
        if (IsEmpty())
        {
            return GetEmptyPathGeometryData();
        }

        return new PathGeometryData
        {
            FillRule = FillRule,
            SerializedData = _data
        };
    }


    private void SetPathData(byte[] data)
    {
        _data = data;
        RaisePathChanged();
    }

    private void Close(byte[] data) => SetPathData(data);

    private sealed class StreamGeometryCallbackContext : ByteStreamGeometryContext
    {
        private readonly StreamGeometry _owner;

        public StreamGeometryCallbackContext(StreamGeometry owner)
        {
            _owner = owner;
        }

        protected override void CloseCore(byte[] data) => _owner.Close(data);
    }
}