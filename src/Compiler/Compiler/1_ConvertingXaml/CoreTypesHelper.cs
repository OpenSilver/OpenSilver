
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
using System.Xml.Linq;

namespace OpenSilver.Compiler;

internal abstract class CoreTypesConverter
{
    private readonly Dictionary<string, Func<XElement, string, string>> _knownCoreTypes;

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

        _knownCoreTypes = new Dictionary<string, Func<XElement, string, string>>(40, StringComparer.OrdinalIgnoreCase)
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
            ["System.Windows.Media.PathGeometry"] = ConvertToPathGeometry,
            ["System.Windows.Media.Matrix"] = ConvertToMatrix,
            ["System.Windows.Media.PointCollection"] = ConvertToPointCollection,
            ["System.Windows.Media.Transform"] = ConvertToTransform,
            ["System.Windows.Media.MatrixTransform"] = ConvertToTransform,
            ["System.Windows.Media.CacheMode"] = ConvertToCacheMode,
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
            ["System.Windows.media.ImageSource"] = ConvertToImageSource,
            ["System.Windows.Vector"] = ConvertToVector,
            ["System.Windows.RoutedEvent"] = ConvertToRoutedEvent,
            ["System.Windows.ResponsiveThreshold"] = ConvertToResponsiveThreshold,
        };
    }

    public abstract string ConvertFromInvariantString(string source, string destinationType);

    public abstract string ConvertToCursor(XElement context, string source);

    public abstract string ConvertToModifierKeys(XElement context, string source);

    public abstract string ConvertToKey(XElement context, string source);

    public abstract string ConvertToMouseAction(XElement context, string source);

    public abstract string ConvertToKeyGesture(XElement context, string source);

    public abstract string ConvertToMouseGesture(XElement context, string source);

    public abstract string ConvertToCommand(XElement context, string source);

    public abstract string ConvertToKeyTime(XElement context, string source);

    public abstract string ConvertToRepeatBehavior(XElement context, string source);

    public abstract string ConvertToKeySpline(XElement context, string source);

    public abstract string ConvertToBrush(XElement context, string source);

    public abstract string ConvertToColor(XElement context, string source);

    public abstract string ConvertToDoubleCollection(XElement context, string source);

    public abstract string ConvertToFontFamily(XElement context, string source);

    public abstract string ConvertToGeometry(XElement context, string source);

    public abstract string ConvertToPathGeometry(XElement context, string source);

    public abstract string ConvertToMatrix(XElement context, string source);

    public abstract string ConvertToPointCollection(XElement context, string source);

    public abstract string ConvertToTransform(XElement context, string source);

    public abstract string ConvertToCacheMode(XElement context, string source);

    public abstract string ConvertToCornerRadius(XElement context, string source);

    public abstract string ConvertToDuration(XElement context, string source);

    public abstract string ConvertToFontWeight(XElement context, string source);

    public abstract string ConvertToGridLength(XElement context, string source);

    public abstract string ConvertToPoint(XElement context, string source);

    public abstract string ConvertToPropertyPath(XElement context, string source);

    public abstract string ConvertToRect(XElement context, string source);

    public abstract string ConvertToSize(XElement context, string source);

    public abstract string ConvertToThickness(XElement context, string source);

    public abstract string ConvertToFontStretch(XElement context, string source);

    public abstract string ConvertToFontStyle(XElement context, string source);

    public abstract string ConvertToTextDecorationCollection(XElement context, string source);

    public abstract string ConvertToImageSource(XElement context, string source);

    public abstract string ConvertToVector(XElement context, string source);

    public abstract string ConvertToRoutedEvent(XElement context, string source);

    public abstract string ConvertToResponsiveThreshold(XElement context, string source);

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

    public string ConvertKnownType(string source, string typeFullName, XElement context)
    {
        if (_knownCoreTypes.TryGetValue(typeFullName, out var converter))
        {
            Debug.Assert(converter is not null);
            return converter(context, source);
        }

        throw new InvalidOperationException($"Cannot find a converter for type '{typeFullName}'");
    }

    public static XamlParseException GetConvertException(string value, string destinationTypeFullName)
    {
        return new XamlParseException($"Cannot convert '{value}' to '{destinationTypeFullName}'.");
    }

    private static bool IsCoreAssemblyOrNull(string assemblyName)
    {
        if (assemblyName is null ||
            assemblyName.Equals(Constants.NAME_OF_CORE_ASSEMBLY_USING_BLAZOR, StringComparison.OrdinalIgnoreCase))
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
