
/*===================================================================================
* 
*   Copyright (c) Userware (OpenSilver.net, CSHTML5.com)
*      
*   This file is part of both the OpenSilver Compiler (https://opensilver.net), which
*   is licensed under the MIT license (https://opensource.org/licenses/MIT), and the
*   CSHTML5 Compiler (http://cshtml5.com), which is dual-licensed (MIT + commercial).
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using System.Xml.Linq;

namespace OpenSilver.Compiler;

internal abstract class CoreTypesConverter
{
    private readonly Dictionary<string, Func<XObject, string, string>> _knownCoreTypes;

    protected CoreTypesConverter()
    {
        //
        // IMPORTANT: Do not modify this dictionary unless you made changes in the file
        // 'TypeConverterHelper.cs' in the Runtime project. This dictionary and the 
        // dictionary 'TypeConverterHelper.CoreTypeConverters' must stay in sync.
        //
        // ImageSource is the only type to be present in this dictionary and not in the
        // one from the Runtime. This is due to the fact that we need to support the 
        // following XAML syntax : <ImageSource>whatever/you/want</ImageSource> which
        // normally requires the ContentPropertyAttribute to be defined on the type we
        // are trying to instantiate but this is not the case for ImageSource.
        // However, ImageSource has a registered TypeConverter via TypeConverterAttribute,
        // so this is why we do not want to register it in the CoreTypeConverters
        // dictionary, as it would prevent derived types (BitmapSource and BitmapImage)
        // from finding the converter.
        //

        _knownCoreTypes = new Dictionary<string, Func<XObject, string, string>>(45, StringComparer.OrdinalIgnoreCase)
        {
            ["System.Windows.Input.Cursor"] = ConvertToCursor,
            ["System.Windows.Input.ModifierKeys"] = ConvertToModifierKeys,
            ["System.Windows.Input.Key"] = ConvertToKey,
            ["System.Windows.Input.MouseAction"] = ConvertToMouseAction,
            ["System.Windows.Input.KeyGesture"] = ConvertToKeyGesture,
            ["System.Windows.Input.MouseGesture"] = ConvertToMouseGesture,
            ["System.Windows.Input.ICommand"] = ConvertToCommand,
            ["System.Windows.Input.RoutedCommand"] = ConvertToCommand,
            ["System.Windows.Input.RoutedUICommand"] = ConvertToCommand,
            ["System.Windows.Media.Animation.KeyTime"] = ConvertToKeyTime,
            ["System.Windows.Media.Animation.RepeatBehavior"] = ConvertToRepeatBehavior,
            ["System.Windows.Media.Animation.KeySpline"] = ConvertToKeySpline,
            ["System.Windows.Media.Brush"] = ConvertToBrush,
            ["System.Windows.Media.SolidColorBrush"] = ConvertToBrush,
            ["System.Windows.Media.Color"] = ConvertToColor,
            ["System.Windows.Media.DoubleCollection"] = ConvertToDoubleCollection,
            ["System.Windows.Media.FontFamily"] = ConvertToFontFamily,
            ["System.Windows.Media.Geometry"] = ConvertToGeometry,
            ["System.Windows.Media.PathFigureCollection"] = ConvertToPathFigureCollection,
            ["System.Windows.Media.Matrix"] = ConvertToMatrix,
            ["System.Windows.Media.PointCollection"] = ConvertToPointCollection,
            ["System.Windows.Media.Transform"] = ConvertToTransform,
            ["System.Windows.Media.MatrixTransform"] = ConvertToTransform,
            ["System.Windows.Media.CacheMode"] = ConvertToCacheMode,
            ["System.Windows.Media.ImageSource"] = ConvertToImageSource,
            ["System.Windows.Media.Media3D.Matrix3D"] = ConvertToMatrix3D,
            ["System.Windows.Markup.XmlLanguage"] = ConvertToXmlLanguage,
            ["System.Windows.CornerRadius"] = ConvertToCornerRadius,
            ["System.Windows.Duration"] = ConvertToDuration,
            ["System.Windows.FontWeight"] = ConvertToFontWeight,
            ["System.Windows.GridLength"] = ConvertToGridLength,
            ["System.Windows.Point"] = ConvertToPoint,
            ["System.Windows.PropertyPath"] = ConvertToPropertyPath,
            ["System.Windows.Rect"] = ConvertToRect,
            ["System.Windows.Size"] = ConvertToSize,
            ["System.Windows.Thickness"] = ConvertToThickness,
            ["System.Windows.FontStretch"] = ConvertToFontStretch,
            ["System.Windows.FontStyle"] = ConvertToFontStyle,
            ["System.Windows.TextDecorationCollection"] = ConvertToTextDecorationCollection,
            ["System.Windows.Vector"] = ConvertToVector,
            ["System.Windows.RoutedEvent"] = ConvertToRoutedEvent,
            ["System.Windows.ResponsiveThreshold"] = ConvertToResponsiveThreshold,
            ["System.Windows.Controls.RowDefinitionCollection"] = ConvertToRowDefinitionCollection,
            ["System.Windows.Controls.ColumnDefinitionCollection"] = ConvertToColumnDefinitionCollection,
        };
    }

    public abstract string ConvertFromInvariantString(string source, string destinationType);

    public abstract string ConvertToCursor(XObject context, string source);

    public abstract string ConvertToModifierKeys(XObject context, string source);

    public abstract string ConvertToKey(XObject context, string source);

    public abstract string ConvertToMouseAction(XObject context, string source);

    public abstract string ConvertToKeyGesture(XObject context, string source);

    public abstract string ConvertToMouseGesture(XObject context, string source);

    public abstract string ConvertToCommand(XObject context, string source);

    public abstract string ConvertToKeyTime(XObject context, string source);

    public abstract string ConvertToRepeatBehavior(XObject context, string source);

    public abstract string ConvertToKeySpline(XObject context, string source);

    public abstract string ConvertToBrush(XObject context, string source);

    public abstract string ConvertToColor(XObject context, string source);

    public abstract string ConvertToDoubleCollection(XObject context, string source);

    public abstract string ConvertToFontFamily(XObject context, string source);

    public abstract string ConvertToGeometry(XObject context, string source);

    public abstract string ConvertToPathFigureCollection(XObject context, string source);

    public abstract string ConvertToMatrix(XObject context, string source);

    public abstract string ConvertToPointCollection(XObject context, string source);

    public abstract string ConvertToTransform(XObject context, string source);

    public abstract string ConvertToCacheMode(XObject context, string source);

    public abstract string ConvertToMatrix3D(XObject context, string source);

    public abstract string ConvertToXmlLanguage(XObject context, string source);

    public abstract string ConvertToCornerRadius(XObject context, string source);

    public abstract string ConvertToDuration(XObject context, string source);

    public abstract string ConvertToFontWeight(XObject context, string source);

    public abstract string ConvertToGridLength(XObject context, string source);

    public abstract string ConvertToPoint(XObject context, string source);

    public abstract string ConvertToPropertyPath(XObject context, string source);

    public abstract string ConvertToRect(XObject context, string source);

    public abstract string ConvertToSize(XObject context, string source);

    public abstract string ConvertToThickness(XObject context, string source);

    public abstract string ConvertToFontStretch(XObject context, string source);

    public abstract string ConvertToFontStyle(XObject context, string source);

    public abstract string ConvertToTextDecorationCollection(XObject context, string source);

    public abstract string ConvertToImageSource(XObject context, string source);

    public abstract string ConvertToVector(XObject context, string source);

    public abstract string ConvertToRoutedEvent(XObject context, string source);

    public abstract string ConvertToResponsiveThreshold(XObject context, string source);

    public abstract string ConvertToRowDefinitionCollection(XObject context, string source);

    public abstract string ConvertToColumnDefinitionCollection(XObject context, string source);

    public bool IsKnownType(string typeFullName, string assemblyName)
    {
        if (IsCoreAssemblyOrNull(assemblyName))
        {
            return _knownCoreTypes.ContainsKey(typeFullName);
        }

        // Workaround for ICommand, because it is not defined in OpenSilver.dll
        if (IsICommand(typeFullName, assemblyName))
        {
            return true;
        }

        return false;
    }

    public string ConvertKnownType(string source, string typeFullName, XObject context)
    {
        if (_knownCoreTypes.TryGetValue(typeFullName, out var converter))
        {
            Debug.Assert(converter is not null);
            return converter(context, source);
        }

        throw new InvalidOperationException($"Cannot find a converter for type '{typeFullName}'.");
    }

    public static XamlParseException GetConvertException(string value, string destinationTypeFullName, IXmlLineInfo lineInfo)
    {
        return new XamlParseException($"Cannot convert '{value}' to '{destinationTypeFullName}'.", lineInfo);
    }

    internal static XElement GetClosestXElement(XObject xo)
    {
        if (xo is XElement xe)
        {
            return xe;
        }

        if (xo.Parent is XElement parent)
        {
            return parent;
        }

        return null;
    }

    private static bool IsCoreAssemblyOrNull(string assemblyName)
    {
        if (assemblyName is null ||
            assemblyName.Equals(Constants.OPENSILVER_ASSEMBLY_NAME, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return false;
    }

    private static bool IsICommand(string typeFullName, string assemblyName)
    {
        if (assemblyName is null ||
            string.Equals(assemblyName, "System.ObjectModel", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(assemblyName, "netstandard", StringComparison.OrdinalIgnoreCase))
        {
            return string.Equals(typeFullName, "System.Windows.Input.ICommand", StringComparison.OrdinalIgnoreCase);
        }

        return false;
    }
}
