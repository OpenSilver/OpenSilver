
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

using CSHTML5.Internal;
using System.Windows.Markup;

#if MIGRATION
namespace System.Windows.Documents
#else
namespace Windows.UI.Xaml.Documents
#endif
{
	/// <summary>
	/// Provides a block-level content element that is used to group content into a paragraph.
	/// </summary>
	[ContentProperty("Inlines")]
	public sealed class Paragraph : Block
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Paragraph"/> class.
		/// </summary>
		public Paragraph()
		{
			this.Inlines = new InlineCollection(this);
		}

		/// <summary>
		/// Gets an <see cref="InlineCollection"/> containing the top-level <see cref="Inline"/>
		/// elements that include the contents of the <see cref="Paragraph"/>.
		/// </summary>
		public InlineCollection Inlines { get; }

		protected internal override void INTERNAL_OnAttachedToVisualTree()
		{
			base.INTERNAL_OnAttachedToVisualTree();
			foreach (var inline in Inlines)
			{
				INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(inline, this);
			}
		}

		internal override string GetContainerText()
		{
			return new INTERNAL_TextContainerParagraph(this).Text;
		}
	}
}
