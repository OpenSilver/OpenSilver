
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

using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using OpenSilver.Internal;

namespace System.Windows.Input;

/// <summary>
/// Converts a <see cref="ModifierKeys"/> object to and from other types.
/// </summary>
public class ModifierKeysConverter : TypeConverter
{
    /// <summary>
    /// Determines whether an object of the specified type can be converted to an instance of <see cref="ModifierKeys"/>,
    /// using the specified context.
    /// </summary>
    /// <param name="context">
    /// A format context that provides information about the environment from which this converter is being invoked.
    /// </param>
    /// <param name="sourceType">
    /// The type being evaluated for conversion.
    /// </param>
    /// <returns>
    /// true if sourceType is type System.String; otherwise, false.
    /// </returns>
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        // We can only handle string
        return sourceType == typeof(string);
    }

    /// <summary>
    /// Determines whether an instance of <see cref="ModifierKeys"/> can be converted to the specified type, using 
    /// the specified context.
    /// </summary>
    /// <param name="context">
    /// A format context that provides information about the environment from which this converter is being invoked.
    /// </param>
    /// <param name="destinationType">
    /// The type being evaluated for conversion.
    /// </param>
    /// <returns>
    /// true if destinationType is type <see cref="string"/>; otherwise, false.
    /// </returns>
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
        // We can convert to a string
        if (destinationType != typeof(string))
        {
            return false;
        }

        // When invoked by the serialization engine we can convert to string only for known type
        if (context is null || context.Instance is not ModifierKeys modifiers)
        {
            return false;
        }

        return IsDefinedModifierKeys(modifiers);
    }

    /// <summary>
    /// Attempts to convert the specified object to a <see cref="ModifierKeys"/>, using the specified context.
    /// </summary>
    /// <param name="context">
    /// A format context that provides information about the environment from which this converter is being invoked.
    /// </param>
    /// <param name="culture">
    /// Culture specific information.
    /// </param>
    /// <param name="source">
    /// The object to convert.
    /// </param>
    /// <returns>
    /// The converted object.
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// source cannot be converted.
    /// </exception>
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object source)
    {
        if (source is not string stringSource)
        {
            throw GetConvertFromException(source);
        }

        return FromString(context, culture, stringSource);
    }

    internal static ModifierKeys FromString(ITypeDescriptorContext context, CultureInfo culture, string stringSource)
    {
        string modifiersToken = stringSource.Trim();

        // Empty token means there were no modifiers, exit early
        if (modifiersToken.Length == 0 || modifiersToken.Equals("None", StringComparison.OrdinalIgnoreCase))
        {
            return ModifierKeys.None;
        }

        // Silverlight uses the default enum converter. To remain compatible, in case there is no + in the source string, we
        // use the default converter to support the comma based syntax ("Alt,Control,Windows" instead of "Alt+Control+Windows"
        // for instance).
        if (modifiersToken.IndexOfAny(_separator) == -1)
        {
            if (TryParseModifier(modifiersToken.AsSpan(), out ModifierKeys modifier))
            {
                return modifier;
            }

            return (ModifierKeys)SLConverter.ConvertFrom(context, culture, modifiersToken);
        }

        ModifierKeys modifiers = ModifierKeys.None;

        foreach (string token in modifiersToken.Split(_separator))
        {
            ReadOnlySpan<char> modifier = token.AsSpan().Trim();

            // This would be a case where we have a token like "Ctrl + " for example,
            // which itself is invalid but we choose to support this malformed behaviour.
            if (modifier.IsEmpty)
            {
                break;
            }

            if (!TryParseModifier(modifier, out ModifierKeys key))
            {
                throw new NotSupportedException(string.Format(Strings.Unsupported_Modifier, modifier.ToString()));
            }

            modifiers |= key;
        }

        return modifiers;
    }

    private static bool TryParseModifier(ReadOnlySpan<char> source, out ModifierKeys modifier)
    {
        modifier = source switch
        {
            _ when source.Equals("Ctrl".AsSpan(), StringComparison.OrdinalIgnoreCase) => ModifierKeys.Control,
            _ when source.Equals("Control".AsSpan(), StringComparison.OrdinalIgnoreCase) => ModifierKeys.Control,
            _ when source.Equals("Win".AsSpan(), StringComparison.OrdinalIgnoreCase) => ModifierKeys.Windows,
            _ when source.Equals("Windows".AsSpan(), StringComparison.OrdinalIgnoreCase) => ModifierKeys.Windows,
            _ when source.Equals("Apple".AsSpan(), StringComparison.OrdinalIgnoreCase) => ModifierKeys.Apple,
            _ when source.Equals("Alt".AsSpan(), StringComparison.OrdinalIgnoreCase) => ModifierKeys.Alt,
            _ when source.Equals("Shift".AsSpan(), StringComparison.OrdinalIgnoreCase) => ModifierKeys.Shift,
            _ => ModifierKeys.None,
        };

        return modifier != ModifierKeys.None;
    }

    /// <summary>
    /// Attempts to convert a <see cref="ModifierKeys"/> to the specified type, using the specified context.
    /// </summary>
    /// <param name="context">
    /// A format context that provides information about the environment from which this converter is being invoked.
    /// </param>
    /// <param name="culture">
    /// Culture specific information.
    /// </param>
    /// <param name="value">
    /// The object to convert.
    /// </param>
    /// <param name="destinationType">
    /// The type to convert the object to.
    /// </param>
    /// <returns>
    /// The converted object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// destinationType is null.
    /// </exception>
    /// <exception cref="InvalidEnumArgumentException">
    /// value does not map to a valid <see cref="ModifierKeys"/>.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// value cannot be converted.
    /// </exception>
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        if (destinationType is null)
        {
            throw new ArgumentNullException(nameof(destinationType));
        }

        // We can only convert to string
        if (destinationType != typeof(string))
        {
            throw GetConvertToException(value, destinationType);
        }

        // Check whether value falls within defined set
        ModifierKeys modifiers = (ModifierKeys)value;
        if (!IsDefinedModifierKeys(modifiers))
        {
            throw new InvalidEnumArgumentException(nameof(value), (int)modifiers, typeof(ModifierKeys));
        }

        return ToString(modifiers);
    }

    internal static string ToString(ModifierKeys modifiers)
    {
        // This is a fast path for when only a single modifier (or none) is set, which is a very common scenario.
        // Therefore we want a fast path with an allocation free return, taking advantage of interned strings.
        return modifiers switch
        {
            ModifierKeys.None => string.Empty,
            ModifierKeys.Control => "Ctrl",
            ModifierKeys.Alt => "Alt",
            ModifierKeys.Shift => "Shift",
            ModifierKeys.Windows => "Windows",
            // Since we were not able to match a single modifier alone, there must be multiple modifiers involved.
            _ => ConvertMultipleModifiers(modifiers),
        };
    }

    private static string ConvertMultipleModifiers(ModifierKeys modifiers)
    {
        // Ctrl+Alt+Windows+Shift is the maximum char length, though the composition of such value is improbable
        Span<char> modifierSpan = stackalloc char[22];
        int totalLength = 0;

        if (modifiers.HasFlag(ModifierKeys.Control))
        {
            AppendWithDelimiter("Ctrl", ref totalLength, ref modifierSpan);
        }

        if (modifiers.HasFlag(ModifierKeys.Alt))
        {
            AppendWithDelimiter("Alt", ref totalLength, ref modifierSpan);
        }

        if (modifiers.HasFlag(ModifierKeys.Windows))
        {
            AppendWithDelimiter("Windows", ref totalLength, ref modifierSpan);
        }

        if (modifiers.HasFlag(ModifierKeys.Shift))
        {
            AppendWithDelimiter("Shift", ref totalLength, ref modifierSpan);
        }

        //Helper function to concatenate modifiers
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void AppendWithDelimiter(string literal, ref int totalLength, ref Span<char> modifierSpan)
        {
            // If this is not the first modifier in the span, we prepend a delimiter (e.g. Ctrl -> Ctrl+Alt)
            if (totalLength > 0)
            {
                "+".AsSpan().CopyTo(modifierSpan.Slice(totalLength));
                totalLength++;
            }

            literal.AsSpan().CopyTo(modifierSpan.Slice(totalLength));
            totalLength += literal.Length;
        }

        return modifierSpan.Slice(0, totalLength).ToString();
    }

    /// <summary>
    /// Determines whether the specified value is a valid <see cref="ModifierKeys"/> value.
    /// </summary>
    /// <param name="modifierKeys">
    /// The value to check for validity.
    /// </param>
    /// <returns>
    /// true if input is a valid <see cref="ModifierKeys"/> value; otherwise, false.
    /// </returns>
    public static bool IsDefinedModifierKeys(ModifierKeys modifierKeys)
    {
        return modifierKeys == ModifierKeys.None || (((int)modifierKeys & ~(int)ModifiersAllBitsSet) == 0);
    }

    private static EnumConverter SLConverter => _slConverter ??= new EnumConverter(typeof(ModifierKeys));

    private static EnumConverter _slConverter;
    private static readonly char[] _separator = ['+'];

    /// <summary>
    /// Specifies all bits of the <see cref="ModifierKeys"/> enum set.
    /// </summary>
    private const ModifierKeys ModifiersAllBitsSet = ModifierKeys.Windows | ModifierKeys.Shift | ModifierKeys.Alt | ModifierKeys.Control;
}
