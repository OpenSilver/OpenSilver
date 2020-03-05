using System;

#if MIGRATION
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

#if WORKINPROGRESS

namespace Microsoft.Expression.Interactivity.Core
{
	//
	// Summary:
	//     An action that will change the state of a targeted storyboard when invoked.
	[CLSCompliant(false)]
	public partial class ControlStoryboardAction : StoryboardAction
	{
		public static readonly DependencyProperty ControlStoryboardProperty;

		public ControlStoryboardAction()
		{
			
		}

		public ControlStoryboardOption ControlStoryboardOption { get; set; }

		//
		// Summary:
		//     This method is called when some criteria is met and the action should be invoked.
		//     This method will attempt to change the targeted storyboard in a way defined by
		//     the ControlStoryboardOption.
		//
		// Parameters:
		//   parameter:
		protected override void Invoke(object parameter)
		{
			
		}

		protected override void OnStoryboardChanged(DependencyPropertyChangedEventArgs args)
		{
			
		}
	}
}

#endif