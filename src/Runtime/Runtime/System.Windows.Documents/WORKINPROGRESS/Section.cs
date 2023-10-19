
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

using System.Windows.Markup;
using CSHTML5.Internal;
using OpenSilver.Internal.Documents;

namespace System.Windows.Documents
{
	/// <summary>
	/// A block-level element used for grouping other <see cref="Block"/>
	/// elements.
	/// </summary>
	[ContentProperty(nameof(Blocks))]
    [OpenSilver.NotImplemented]
	public sealed class Section : Block
	{
		/// <summary>
		/// Gets a <see cref="BlockCollection"/> containing the top-level <see cref="Block"/>
		/// elements that comprise the contents of the <see cref="Section"/>.
		/// This property has no default value.
		/// </summary>
        [OpenSilver.NotImplemented]
		public BlockCollection Blocks { get; private set; }

		/// <summary>
		/// Gets or sets a value that indicates whether a trailing paragraph break should
		/// be inserted after the last paragraph when copying the contents of a root <see cref="Section"/>
		/// element to the clipboard.
		/// </summary>
		/// <returns>
		/// true if a trailing paragraph break should be included; otherwise false.
		/// </returns>
        [OpenSilver.NotImplemented]
		public bool HasTrailingParagraphBreakOnPaste { get; set; }

        public Section()
        {
			Blocks = new BlockCollection(this, false);
        }

		internal override string TagName => "section";
		
		protected internal override void INTERNAL_OnAttachedToVisualTree()
		{
			base.INTERNAL_OnAttachedToVisualTree();
			foreach (var block in Blocks)
			{
				INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(block, this);
			}
		}

		internal override string GetContainerText()
		{
			return new TextContainerSection(this).Text;
		}
	}
}
