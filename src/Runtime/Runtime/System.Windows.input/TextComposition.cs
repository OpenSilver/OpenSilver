
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

namespace System.Windows.Input
{
	/// <summary>
	/// Represents a composition related to text input which includes the composition
	/// text itself.
	/// </summary>
	public sealed class TextComposition
	{
        internal static TextComposition Empty { get; } = new TextComposition(string.Empty);

        internal TextComposition(string compositionText)
        {
			CompositionText = compositionText;
		}

		/// <summary>
		/// Gets the composition text for this text composition.
		/// </summary>
		/// <returns>
		/// The composition text for this text composition.
		/// </returns>
		public string CompositionText { get; }
	}
}
