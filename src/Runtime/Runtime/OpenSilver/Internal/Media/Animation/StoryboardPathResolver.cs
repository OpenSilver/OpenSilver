
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
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using OpenSilver.Internal.Data;

namespace OpenSilver.Internal.Media.Animation;

internal static class StoryboardPathResolver
{
    internal static (DependencyObject Target, DependencyProperty Property) Resolve(DependencyObject rootTarget, PropertyPath path)
    {
        if (path.DependencyProperty is not null)
        {
            return (rootTarget, path.DependencyProperty);
        }

        List<SourceValueInfo> parts = path.SVI;
        if (parts.Count == 0)
        {
            throw new InvalidOperationException(string.Format(Strings.Storyboard_PropertyPathMustPointToDependencyProperty, path.Path));
        }

        SourceValueInfo svi;
        DependencyObject target = rootTarget;

        for (int i = 0; i < parts.Count - 1; i++)
        {
            svi = parts[i];

            switch (svi.type)
            {
                case PropertyNodeType.Property:
                    {
                        if (DPFromName(svi.propertyName, svi.typeName, target.GetType()) is not DependencyProperty intermediateProperty)
                        {
                            throw new InvalidOperationException(
                                string.Format(Strings.Storyboard_PropertyPathPropertyNotFound, path.Path));
                        }

                        if (target.GetValue(intermediateProperty) is not DependencyObject intermediateObject)
                        {
                            throw new InvalidOperationException(
                                string.Format(
                                    Strings.Storyboard_PropertyPathDependencyObjectNotFound,
                                    GetAccessorName(svi.propertyName, svi.typeName),
                                    path.Path));
                        }

                        if (i == 0 && intermediateObject is ICloneOnAnimation<DependencyObject> cloneable && !cloneable.IsClone)
                        {
                            intermediateObject = cloneable.Clone();
                            target.SetValueInternal(intermediateProperty, intermediateObject);
                        }

                        target = intermediateObject;
                    }
                    break;

                case PropertyNodeType.Indexer:
                    {
                        if (target is not IList list)
                        {
                            throw new InvalidOperationException(
                                string.Format(Strings.Storyboard_PropertyPathPropertyNotFound, path.Path));
                        }

                        if (!int.TryParse(svi.param, out int index))
                        {
                            throw new InvalidOperationException(string.Format(Strings.PropertyPathIndexWrongType, svi.param));
                        }

                        if (list[index] is not DependencyObject intermediateObject)
                        {
                            throw new InvalidOperationException(
                                string.Format(
                                    Strings.Storyboard_PropertyPathDependencyObjectNotFound,
                                    $"[{svi.param}]",
                                    path.Path));
                        }

                        target = intermediateObject;
                    }
                    break;
            }
        }

        svi = parts[parts.Count - 1];

        if (svi.type == PropertyNodeType.Indexer ||
            DPFromName(svi.propertyName, svi.typeName, target.GetType()) is not DependencyProperty targetProperty)
        {
            throw new InvalidOperationException(
                string.Format(Strings.Storyboard_PropertyPathMustPointToDependencyProperty, path.Path));
        }

        if (target.IsSealed)
        {
            throw new InvalidOperationException(
                string.Format(Strings.Storyboard_PropertyPathSealedCheckFailed, targetProperty.Name, path.Path, target));
        }

        if (!UIElement.IsPropertyAnimatable(target, targetProperty))
        {
            throw new InvalidOperationException(
                string.Format(Strings.Storyboard_PropertyPathIncludesNonAnimatableProperty, path.Path, targetProperty.Name));
        }

        return (target, targetProperty);
    }

    private static string GetAccessorName(string propertyName, string typeName)
    {
        if (string.IsNullOrEmpty(typeName))
        {
            return propertyName;
        }

        return $"({typeName}.{propertyName})";
    }

    private static DependencyProperty DPFromName(string propertyName, string typeName, Type ownerType) =>
        GetKnownProperty(propertyName, typeName) ?? DependencyProperty.FromName(propertyName, ownerType);

    private static DependencyProperty GetKnownProperty(string propertyName, string typeName)
    {
        return typeName switch
        {
            nameof(UIElement) => GetKnownUIElementProperty(propertyName),
            nameof(Border) => GetKnownBorderProperty(propertyName),
            nameof(Grid) => GetKnownGridProperty(propertyName),
            nameof(Canvas) => GetKnownCanvasProperty(propertyName),
            nameof(TextBlock) => GetKnownTextBlockProperty(propertyName),
            nameof(TextElement) or nameof(Inline) or nameof(Run) or nameof(Span) => GetKnownTextElementProperty(propertyName),
            nameof(Block) or nameof(Paragraph) => GetKnownBlockProperty(propertyName),
            nameof(LinearGradientBrush) => GetKnownLinearGradientBrushProperty(propertyName),
            nameof(RadialGradientBrush) => GetKnownRadialGradientBrushProperty(propertyName),
            nameof(SolidColorBrush) => GetKnownSolidColorBrushProperty(propertyName),
            nameof(GradientBrush) => GetKnownGradientBrushProperty(propertyName),
            nameof(Brush) => GetKnownBrushProperty(propertyName),
            nameof(GradientStop) => GetKnownGradientStopProperty(propertyName),
            nameof(Rectangle) => GetKnownRectangleProperty(propertyName),
            nameof(Line) => GetKnownLineProperty(propertyName),
            nameof(Shape) or nameof(Path) or nameof(Ellipse) or nameof(Polygon) or nameof(Polyline) => GetKnownShapeProperty(propertyName),
            nameof(TranslateTransform) => GetKnownTranslateTransformProperty(propertyName),
            nameof(ScaleTransform) => GetKnownScaleTransformProperty(propertyName),
            nameof(SkewTransform) => GetKnownSkewTransformProperty(propertyName),
            nameof(RotateTransform) => GetKnownRotateTransformProperty(propertyName),
            nameof(CompositeTransform) => GetKnownCompositeTransformProperty(propertyName),
            nameof(TransformGroup) => GetKnownTransformGroupProperty(propertyName),
            _ => null,
        };
    }

    private static DependencyProperty GetKnownUIElementProperty(string name)
    {
        return name switch
        {
            nameof(UIElement.Opacity) => UIElement.OpacityProperty,
            nameof(UIElement.OpacityMask) => UIElement.OpacityMaskProperty,
            nameof(UIElement.Visibility) => UIElement.VisibilityProperty,
            nameof(UIElement.RenderTransform) => UIElement.RenderTransformProperty,
            _ => null,
        };
    }

    private static DependencyProperty GetKnownBorderProperty(string name)
    {
        return name switch
        {
            nameof(Border.Background) => Border.BackgroundProperty,
            nameof(Border.BorderBrush) => Border.BorderBrushProperty,
            nameof(Border.BorderThickness) => Border.BorderThicknessProperty,
            _ => null,
        };
    }

    private static DependencyProperty GetKnownGridProperty(string name)
    {
        return name switch
        {
            "Row" => Grid.RowProperty,
            "Column" => Grid.ColumnProperty,
            "RowSpan" => Grid.RowSpanProperty,
            "ColumnSpan" => Grid.ColumnSpanProperty,
            _ => null,
        };
    }

    private static DependencyProperty GetKnownCanvasProperty(string name)
    {
        return name switch
        {
            "Left" => Canvas.LeftProperty,
            "Top" => Canvas.TopProperty,
            "ZIndex" => Canvas.ZIndexProperty,
            _ => null,
        };
    }

    private static DependencyProperty GetKnownTextBlockProperty(string name)
    {
        return name switch
        {
            nameof(TextBlock.CharacterSpacing) => TextBlock.CharacterSpacingProperty,
            nameof(TextBlock.FontFamily) => TextBlock.FontFamilyProperty,
            nameof(TextBlock.FontSize) => TextBlock.FontSizeProperty,
            nameof(TextBlock.FontStretch) => TextBlock.FontStretchProperty,
            nameof(TextBlock.FontStyle) => TextBlock.FontStyleProperty,
            nameof(TextBlock.FontWeight) => TextBlock.FontWeightProperty,
            nameof(TextBlock.Foreground) => TextBlock.ForegroundProperty,
            nameof(TextBlock.LineHeight) => TextBlock.LineHeightProperty,
            nameof(TextBlock.LineStackingStrategy) => TextBlock.LineStackingStrategyProperty,
            nameof(TextBlock.TextAlignment) => TextBlock.TextAlignmentProperty,
            _ => null,
        };
    }

    private static DependencyProperty GetKnownTextElementProperty(string name)
    {
        return name switch
        {
            nameof(TextElement.CharacterSpacing) => TextElement.CharacterSpacingProperty,
            nameof(TextElement.FontFamily) => TextElement.FontFamilyProperty,
            nameof(TextElement.FontSize) => TextElement.FontSizeProperty,
            nameof(TextElement.FontStretch) => TextElement.FontStretchProperty,
            nameof(TextElement.FontStyle) => TextElement.FontStyleProperty,
            nameof(TextElement.FontWeight) => TextElement.FontWeightProperty,
            nameof(TextElement.Foreground) => TextElement.ForegroundProperty,
            _ => null,
        };
    }

    private static DependencyProperty GetKnownBlockProperty(string name)
    {
        return name switch
        {
            nameof(Block.LineHeight) => Block.LineHeightProperty,
            nameof(Block.LineStackingStrategy) => Block.LineStackingStrategyProperty,
            nameof(Block.TextAlignment) => Block.TextAlignmentProperty,
            _ => GetKnownTextElementProperty(name),
        };
    }

    private static DependencyProperty GetKnownLinearGradientBrushProperty(string name)
    {
        return name switch
        {
            nameof(LinearGradientBrush.StartPoint) => LinearGradientBrush.StartPointProperty,
            nameof(LinearGradientBrush.EndPoint) => LinearGradientBrush.EndPointProperty,
            _ => GetKnownGradientBrushProperty(name),
        };
    }

    private static DependencyProperty GetKnownRadialGradientBrushProperty(string name)
    {
        return name switch
        {
            nameof(RadialGradientBrush.RadiusX) => RadialGradientBrush.RadiusXProperty,
            nameof(RadialGradientBrush.RadiusY) => RadialGradientBrush.RadiusYProperty,
            nameof(RadialGradientBrush.Center) => RadialGradientBrush.CenterProperty,
            _ => GetKnownGradientBrushProperty(name),
        };
    }

    private static DependencyProperty GetKnownGradientBrushProperty(string name)
    {
        return name switch
        {
            nameof(GradientBrush.GradientStops) => GradientBrush.GradientStopsProperty,
            _ => GetKnownBrushProperty(name),
        };
    }

    private static DependencyProperty GetKnownSolidColorBrushProperty(string name)
    {
        return name switch
        {
            nameof(SolidColorBrush.Color) => SolidColorBrush.ColorProperty,
            _ => GetKnownBrushProperty(name),
        };
    }

    private static DependencyProperty GetKnownBrushProperty(string name)
    {
        return name switch
        {
            nameof(Brush.Opacity) => Brush.OpacityProperty,
            _ => null,
        };
    }

    private static DependencyProperty GetKnownRectangleProperty(string name)
    {
        return name switch
        {
            nameof(Rectangle.RadiusX) => Rectangle.RadiusXProperty,
            nameof(Rectangle.RadiusY) => Rectangle.RadiusYProperty,
            _ => GetKnownShapeProperty(name),
        };
    }

    private static DependencyProperty GetKnownLineProperty(string name)
    {
        return name switch
        {
            nameof(Line.X1) => Line.X1Property,
            nameof(Line.X2) => Line.X2Property,
            nameof(Line.Y1) => Line.Y1Property,
            nameof(Line.Y2) => Line.Y2Property,
            _ => GetKnownShapeProperty(name),
        };
    }

    private static DependencyProperty GetKnownShapeProperty(string name)
    {
        return name switch
        {
            nameof(Shape.Fill) => Shape.FillProperty,
            nameof(Shape.Stroke) => Shape.StrokeProperty,
            nameof(Shape.StrokeThickness) => Shape.StrokeThicknessProperty,
            _ => null,
        };
    }

    private static DependencyProperty GetKnownTranslateTransformProperty(string name)
    {
        return name switch
        {
            nameof(TranslateTransform.X) => TranslateTransform.XProperty,
            nameof(TranslateTransform.Y) => TranslateTransform.YProperty,
            _ => null,
        };
    }

    private static DependencyProperty GetKnownRotateTransformProperty(string name)
    {
        return name switch
        {
            nameof(RotateTransform.Angle) => RotateTransform.AngleProperty,
            nameof(RotateTransform.CenterX) => RotateTransform.CenterXProperty,
            nameof(RotateTransform.CenterY) => RotateTransform.CenterYProperty,
            _ => null,
        };
    }

    private static DependencyProperty GetKnownScaleTransformProperty(string name)
    {
        return name switch
        {
            nameof(ScaleTransform.ScaleX) => ScaleTransform.ScaleXProperty,
            nameof(ScaleTransform.ScaleY) => ScaleTransform.ScaleYProperty,
            nameof(ScaleTransform.CenterX) => ScaleTransform.CenterXProperty,
            nameof(ScaleTransform.CenterY) => ScaleTransform.CenterYProperty,
            _ => null,
        };
    }

    private static DependencyProperty GetKnownSkewTransformProperty(string name)
    {
        return name switch
        {
            nameof(SkewTransform.AngleX) => SkewTransform.AngleXProperty,
            nameof(SkewTransform.AngleY) => SkewTransform.AngleYProperty,
            nameof(SkewTransform.CenterX) => SkewTransform.CenterXProperty,
            nameof(SkewTransform.CenterY) => SkewTransform.CenterYProperty,
            _ => null,
        };
    }

    private static DependencyProperty GetKnownCompositeTransformProperty(string name)
    {
        return name switch
        {
            nameof(CompositeTransform.TranslateX) => CompositeTransform.TranslateXProperty,
            nameof(CompositeTransform.TranslateY) => CompositeTransform.TranslateYProperty,
            nameof(CompositeTransform.ScaleX) => CompositeTransform.ScaleXProperty,
            nameof(CompositeTransform.ScaleY) => CompositeTransform.ScaleYProperty,
            nameof(CompositeTransform.SkewX) => CompositeTransform.SkewXProperty,
            nameof(CompositeTransform.SkewY) => CompositeTransform.SkewYProperty,
            nameof(CompositeTransform.Rotation) => CompositeTransform.RotationProperty,
            nameof(CompositeTransform.CenterX) => CompositeTransform.CenterXProperty,
            nameof(CompositeTransform.CenterY) => CompositeTransform.CenterYProperty,
            _ => null,
        };
    }

    private static DependencyProperty GetKnownTransformGroupProperty(string name)
    {
        return name switch
        {
            nameof(TransformGroup.Children) => TransformGroup.ChildrenProperty,
            _ => null,
        };
    }

    private static DependencyProperty GetKnownGradientStopProperty(string name)
    {
        return name switch
        {
            nameof(GradientStop.Offset) => GradientStop.OffsetProperty,
            nameof(GradientStop.Color) => GradientStop.ColorProperty,
            _ => null,
        };
    }
}