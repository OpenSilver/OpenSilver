using System;

#if MIGRATION
namespace System.Windows.Automation
#else
namespace Windows.UI.Xaml.Automation
#endif
{
	//
	// Summary:
	//     Contains values that specify whether a text provider supports selection and,
	//     if so, whether it supports a single, continuous selection or multiple, disjoint
	//     selections.
	[Flags]
	public enum SupportedTextSelection
	{
		//
		// Summary:
		//     Does not support text selections.
		None = 0,
		//
		// Summary:
		//     Supports a single, continuous text selection.
		Single = 1,
		//
		// Summary:
		//     Supports multiple, disjoint text selections.
		Multiple = 2
	}
}
