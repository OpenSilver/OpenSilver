namespace System.Windows
{
    public enum FontCapitals
    {
        /// <summary>
        /// Capital letters render normally.
        /// </summary>
        Normal,
        /// <summary>
        /// Both capital and lowercase letters are replaced with a glyph form of an uppercase letter with the same approximate height.
        /// </summary>
        AllSmallCaps,
        /// <summary>
        /// Lowercase letters are replaced with a glyph form of an uppercase letter with the same approximate height.
        /// </summary>
        SmallCaps,
        /// <summary>
        /// Lowercase letters are replaced with a glyph form of an uppercase letter with the same approximate height. Petite capitals are smaller than small capitals.
        /// </summary>
        AllPetiteCaps,
        /// <summary>
        /// Lowercase letters are replaced with a glyph form of an uppercase letter with the same approximate height. Petite capitals are smaller than small capitals.
        /// </summary>
        PetiteCaps,
        /// <summary>Capital letters display in unicase. Unicase fonts render both upper and lowercase letters in a mixture of upper and lowercase glyphs determined by the type designer.</summary>
        Unicase,
        /// <summary>
        /// Glyph forms are substituted with a typographic form specifically designed for titles.
        /// </summary>
        Titling,
    }
}
