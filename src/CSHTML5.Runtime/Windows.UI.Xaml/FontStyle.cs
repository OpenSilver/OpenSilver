#if MIGRATION
using DotNetForHtml5.Core;
using System.Windows.Markup;

namespace System.Windows
{
    [SupportsDirectContentViaTypeFromStringConverters]
    public partial struct FontStyle
    {
        private int _style;

        static FontStyle()
        {
            TypeFromStringConverters.RegisterConverter(typeof(FontStyle), INTERNAL_ConvertFromString);
        }

        internal FontStyle(int style)
        {
            this._style = style;
        }

        public override bool Equals(object o)
        {
            if (o is FontStyle)
            {
                FontStyle fs = (FontStyle)o;
                return this == fs;
            }
            return false;
        }

        public bool Equals(FontStyle fontStyle)
        {
            return this == fontStyle;
        }

        public override int GetHashCode()
        {
            return this._style;
        }

        public static bool operator ==(FontStyle left, FontStyle right)
        {
            return left._style == right._style;
        }

        public static bool operator !=(FontStyle left, FontStyle right)
        {
            return left._style != right._style;
        }

        public override string ToString()
        {
            switch (this._style)
            {
                case 0:
                    return "Normal";
                case 1:
                    return "Oblique";
                case 2:
                    return "Italic";
                default:
                    return ""; //should not be possible
            }
        }

        internal static object INTERNAL_ConvertFromString(string fontStyleAsString)
        {
            switch ((fontStyleAsString ?? string.Empty).ToLower())
            {
                case "normal":
                    return FontStyles.Normal;
                case "oblique":
                    return FontStyles.Oblique;
                case "italic":
                    return FontStyles.Italic;
                default:
                    throw new Exception(string.Format("Invalid FontStyle: '{0}'", fontStyleAsString));
            }
        }
    }
}
#endif