namespace System.Windows
{
	public enum FontEastAsianLanguage
	{
		/// <summary>No font-specific glyph versions are applied.</summary>
		Normal,
		/// <summary>Replaces default glyphs with the corresponding forms from the Hojo Kanji specification.</summary>
		HojoKanji,
		/// <summary>Replaces default Japanese glyphs with the corresponding forms from the JIS04 specification.</summary>
		Jis04,
		/// <summary>Replaces default Japanese glyphs with the corresponding forms from the JIS78 specification.</summary>
		Jis78,
		/// <summary>Replaces default Japanese glyphs with the corresponding forms from the JIS83 specification.</summary>
		Jis83,
		/// <summary>Replaces default Japanese glyphs with the corresponding forms from the JIS90 specification.</summary>
		Jis90,
		/// <summary>Replaces default glyphs with the corresponding forms from the NLC Kanji specification.</summary>
		NlcKanji,
		/// <summary>Replaces traditional Chinese or Japanese forms with their corresponding simplified forms.</summary>
		Simplified,
		/// <summary>Replaces simplified Chinese or Japanese forms with their corresponding traditional forms.</summary>
		Traditional,
		/// <summary>Replaces simplified Kanji forms with their corresponding traditional forms. This glyph set is explicitly limited to the traditional forms considered proper for use in personal names.</summary>
		TraditionalNames
	}
}
