using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI
#endif
{
    /// <summary>
    /// Converts a <see cref="T:System.Windows.Media.Color" /> object to and from other types.
    /// </summary>
    public class ColorConverter : TypeConverter
    {
        /// <summary>
        /// Determines whether an object of the specified type can be converted to an instance of <see cref="T:System.Windows.Media.Color" />.
        /// </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="sourceType">The type being evaluated for conversion.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="sourceType" /> is of type <see cref="T:System.String" />; otherwise, <see langword="false" />.</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        /// <summary>
        /// Determines whether an instance of <see cref="T:System.Windows.Media.Color" /> can be converted to the specified type.
        /// </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="destinationType">The type being evaluated for conversion.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="destinationType" /> is of type <see cref="T:System.ComponentModel.Design.Serialization.InstanceDescriptor" />; otherwise, <see langword="false" />.</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(InstanceDescriptor);
        }

        /// <summary>Attempts to convert the specified object to a <see cref="T:System.Windows.Media.Color" />.</summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="culture">Describes the System.Globalization.CultureInfo of the type being converted.</param>
        /// <param name="value">The object being converted.</param>
        /// <returns>The <see cref="T:System.Windows.Media.Color" /> created from converting <paramref name="value" />.</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            object result;

            if (value is null)
            {
                throw GetConvertFromException(value);
            }
            else if (value is string)
            {
                var colorString = value.ToString();

                var trimmedString = colorString.Trim();
                if (!string.IsNullOrEmpty(trimmedString) && (trimmedString[0] == '#'))
                {
                    var tokens = trimmedString.Substring(1);
                    if (tokens.Length == 6) // This is because XAML is tolerant when the user has forgot the alpha channel (eg. #DDDDDD for Gray).
                        tokens = "FF" + tokens;

#if NETSTANDARD
                    if (int.TryParse(tokens, NumberStyles.HexNumber, NumberFormatInfo.CurrentInfo, out var color))
                    {
                        return Color.INTERNAL_ConvertFromInt32(color);
                    }
#else // BRIDGE
                int color;
                if (CSHTML5.Interop.IsRunningInTheSimulator)
                {
                    color = INTERNAL_BridgeWorkarounds.HexToInt_SimulatorOnly(tokens);
                }
                else
                {
                    color = Script.Write<int>("parseInt({0}, 16);", tokens);
                }

                return INTERNAL_ConvertFromInt32(color);
#endif
                }
                else if (trimmedString != null && trimmedString.StartsWith("sc#", StringComparison.Ordinal))
                {
                    var tokens = trimmedString.Substring(3);

                    var separators = new char[1] { ',' };
                    var words = tokens.Split(separators);
                    var values = new float[4];
                    for (int i = 0; i < 3; i++)
                    {
                        values[i] = Convert.ToSingle(words[i]);
                    }
                    if (words.Length == 4)
                    {
                        values[3] = Convert.ToSingle(words[3]);
                        return Color.FromScRgb(values[0], values[1], values[2], values[3]);
                    }
                    else
                    {
                        return Color.FromScRgb(1.0f, values[0], values[1], values[2]);
                    }
                }
                else
                {
                    // Check if the color is a named color
                    if (Enum.TryParse(trimmedString, true, out Colors.INTERNAL_ColorsEnum namedColor))
                    {
                        return Color.INTERNAL_ConvertFromInt32((int)namedColor);
                    }
                }

                throw new Exception(string.Format("Invalid color: {0}", colorString));
            }
            else
            {
                result = base.ConvertFrom(context, culture, value);
            }

            return result;
        }

        /// <summary>Attempts to convert a <see cref="T:System.Windows.Media.Color" /> to a specified type. </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="culture">Describes the System.Globalization.CultureInfo of the type being converted.</param>
        /// <param name="value">The <see cref="T:System.Windows.Media.Color" /> to convert.</param>
        /// <param name="destinationType">The type to convert this <see cref="T:System.Windows.Media.Color" /> to.</param>
        /// <returns>The object created from converting this <see cref="T:System.Windows.Media.Color" />.</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            object result = null;

            if (destinationType != null && value is Color color)
            {
                if (destinationType == typeof(InstanceDescriptor))
                {
                    var mi = typeof(Color).GetMethod("FromArgb", new Type[] { typeof(byte), typeof(byte), typeof(byte), typeof(byte) });
                    result = new InstanceDescriptor(mi, new object[] { color.A, color.R, color.G, color.B });
                }
                else if (destinationType == typeof(string))
                {
                    result = color.ToString(culture);
                }
            }

            if (result is null)
            {
                result = base.ConvertTo(context, culture, value, destinationType);
            }

            return result;
        }
    }
}
