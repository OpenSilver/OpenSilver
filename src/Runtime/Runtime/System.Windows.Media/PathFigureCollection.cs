
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

using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Shapes;
using OpenSilver.Internal;

namespace System.Windows.Media;

/// <summary>
/// Represents a collection of <see cref="PathFigure"/> objects that collectively
/// make up the geometry of a <see cref="PathGeometry"/>.
/// </summary>
public sealed class PathFigureCollection : PresentationFrameworkCollection<PathFigure>
{
    private Geometry _parentGeometry;

    /// <summary>
    /// Initializes a new instance of the <see cref="PathFigureCollection"/> class.
    /// </summary>
    public PathFigureCollection()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PathFigureCollection"/> class that 
    /// can initially contain the specified number of <see cref="PathFigure"/> objects.
    /// </summary>
    /// <param name="capacity">
    /// The initial capacity of this <see cref="PathFigureCollection"/>.
    /// </param>
    public PathFigureCollection(int capacity)
        : base(capacity)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PathFigureCollection"/> class 
    /// that contains the specified <see cref="PathFigure"/> objects.
    /// </summary>
    /// <param name="figures">
    /// The collection of <see cref="PathFigure"/> objects which collectively make 
    /// up the geometry of the <see cref="Path"/>.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// figures is null.
    /// </exception>
    public PathFigureCollection(IEnumerable<PathFigure> figures)
    {
        if (figures is null)
        {
            throw new ArgumentNullException(nameof(figures));
        }

        foreach (PathFigure item in figures)
        {
            Add(item);
        }
    }

    /// <summary>
    /// Returns an instance of <see cref="PathFigureCollection"/> created from a specified string.
    /// </summary>
    /// <param name="source">
    /// The string that is converted to a <see cref="PathFigureCollection"/>.
    /// </param>
    /// <returns>
    /// An instance of <see cref="PathFigureCollection"/> created from source.
    /// </returns>
    public static PathFigureCollection Parse(string source) => Parsers.ParsePathFigureCollection(source, CultureInfo.InvariantCulture);

    internal override void AddOverride(PathFigure figure)
    {
        SetParentGeometry(figure, _parentGeometry);
        AddDependencyObjectInternal(figure);

        OnChanged();
    }

    internal override void RemoveAtOverride(int index)
    {
        SetParentGeometry(GetItemInternal(index), null);
        RemoveAtDependencyObjectInternal(index);

        OnChanged();
    }

    internal override void InsertOverride(int index, PathFigure figure)
    {
        SetParentGeometry(figure, _parentGeometry);
        InsertDependencyObjectInternal(index, figure);

        OnChanged();
    }

    internal override void ClearOverride()
    {
        foreach (PathFigure figure in InternalItems)
        {
            SetParentGeometry(figure, null);
        }

        ClearDependencyObjectInternal();

        OnChanged();
    }

    internal override PathFigure GetItemOverride(int index) => GetItemInternal(index);

    internal override void SetItemOverride(int index, PathFigure figure)
    {
        SetParentGeometry(GetItemInternal(index), null);
        SetParentGeometry(figure, _parentGeometry);
        SetItemDependencyObjectInternal(index, figure);

        OnChanged();
    }

    internal void SetParentGeometry(Geometry geometry)
    {
        if (_parentGeometry == geometry) return;

        _parentGeometry = geometry;
        foreach (var figure in InternalItems)
        {
            figure.SetParentGeometry(geometry);
        }
    }

    private static void SetParentGeometry(PathFigure figure, Geometry geometry)
    {
        Debug.Assert(figure is not null);
        figure.SetParentGeometry(geometry);
    }

    private void OnChanged() => _parentGeometry?.RaisePathChanged();
}
