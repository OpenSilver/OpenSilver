
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
using System.Collections.Generic;
using System.Windows.Markup;
using System.Windows.Media.Media3D;

namespace System.Windows.Media;

/// <summary>
/// Represents a composite <see cref="Transform"/> composed of other <see cref="Transform"/> objects.
/// </summary>
[ContentProperty(nameof(Children))]
public sealed class TransformGroup : Transform
{
    private WeakEventToken _weakEventToken;
    private Matrix3D? _matrix3D;

    /// <summary>
    /// Identifies the <see cref="Children"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ChildrenProperty =
        DependencyProperty.Register(
            nameof(Children),
            typeof(TransformCollection),
            typeof(TransformGroup),
            new PropertyMetadata(
                new PFCDefaultValueFactory<Transform>(
                    static () => new TransformCollection(),
                    static (d, dp) =>
                    {
                        TransformGroup tg = (TransformGroup)d;
                        var collection = new TransformCollection();
                        tg.ProvideSelfAsInheritanceContext(collection, null);
                        tg.OnChildrenChanged(null, collection);
                        return collection;
                    }),
                OnChildrenChanged,
                CoerceChildren));

    /// <summary>
    /// Gets or sets the collection of child <see cref="Transform"/> objects.
    /// </summary>
    /// <returns>
    /// The collection of child <see cref="Transform"/> objects. The default is
    /// an empty collection.
    /// </returns>
    public TransformCollection Children
    {
        get => (TransformCollection)GetValue(ChildrenProperty);
        set => SetValueInternal(ChildrenProperty, value);
    }

    private static void OnChildrenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        TransformGroup tg = (TransformGroup)d;
        tg.OnChildrenChanged((TransformCollection)e.OldValue, (TransformCollection)e.NewValue);
        tg.OnTransformChanged();
    }

    private void OnChildrenCollectionChanged(object sender, EventArgs e) => OnTransformChanged();

    private void OnChildrenChanged(TransformCollection oldChildren, TransformCollection newChildren)
    {
        _weakEventToken?.Dispose();
        _weakEventToken = null;

        if (newChildren is not null)
        {
            _weakEventToken = WeakEvent.Subscribe<TransformGroup, TransformCollection, EventArgs>(
                this,
                newChildren,
                static (instance, sender, args) => instance.OnChildrenCollectionChanged(sender, args),
                static (handler, source) => source.Changed -= new EventHandler(handler),
                static (handler, source) => source.Changed += new EventHandler(handler));
        }
    }

    private static object CoerceChildren(DependencyObject d, object baseValue)
    {
        return baseValue ?? new TransformCollection();
    }

    private new void OnTransformChanged()
    {
        _matrix3D = null;
        base.OnTransformChanged();
    }

    /// <summary>
    /// Gets the <see cref="Matrix"/> structure that describes the transformation
    /// represented by this <see cref="TransformGroup"/>.
    /// </summary>
    /// <returns>
    /// A composite of the <see cref="Transform"/> objects in this <see cref="TransformGroup"/>.
    /// </returns>
    public override Matrix Value => Matrix3DUtil.GetAffineApproximation(Matrix3D);

    /// <inheritdoc />
    public override GeneralTransform Inverse
    {
        get
        {
            if (IsAffine)
            {
                return base.Inverse;
            }

            return Matrix3DUtil.TryInvert(Matrix3D, out Matrix3D inverse) ? new Matrix3DTransform(inverse) : null;
        }
    }

    /// <inheritdoc />
    public override bool TryTransform(Point inPoint, out Point outPoint)
    {
        if (IsAffine)
        {
            return base.TryTransform(inPoint, out outPoint);
        }

        return Matrix3DUtil.TryTransform(Matrix3D, inPoint, out outPoint);
    }

    /// <inheritdoc />
    public override Rect TransformBounds(Rect rect)
    {
        if (IsAffine)
        {
            return base.TransformBounds(rect);
        }

        return Matrix3DUtil.TransformBounds(Matrix3D, rect);
    }

    internal override Matrix3D Matrix3D => _matrix3D ??= ComputeMatrix3D();

    internal override bool IsAffine => Matrix3DUtil.IsAffine(Matrix3D);

    private Matrix3D ComputeMatrix3D()
    {
        List<Transform> children = Children.InternalItems;

        if (children.Count == 0)
        {
            return Matrix3D.Identity;
        }

        Matrix3D transform = children[0].Matrix3D;

        for (int i = 1; i < children.Count; i++)
        {
            transform *= children[i].Matrix3D;
        }

        return Matrix3DUtil.Flatten(transform);
    }

    internal override bool IsIdentity
    {
        get
        {
            List<Transform> children = Children.InternalItems;
            if (children.Count == 0)
            {
                return true;
            }

            for (int i = 0; i < children.Count; i++)
            {
                if (!children[i].IsIdentity)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
