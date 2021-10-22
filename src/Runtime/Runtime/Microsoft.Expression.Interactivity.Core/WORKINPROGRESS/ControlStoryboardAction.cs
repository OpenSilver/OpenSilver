#if MIGRATION

using System;
using System.Windows;

namespace Microsoft.Expression.Interactivity.Core
{
	//
	// Summary:
	//     An action that will change the state of a targeted storyboard when invoked.
	[OpenSilver.NotImplemented]
	public partial class ControlStoryboardAction : StoryboardAction
	{
		[OpenSilver.NotImplemented]
		public static readonly DependencyProperty ControlStoryboardProperty =
			DependencyProperty.Register(nameof(ControlStoryboardOption),
										typeof(ControlStoryboardOption),
										typeof(ControlStoryboardAction),
										null);
		
		[OpenSilver.NotImplemented]
		public ControlStoryboardAction()
		{
			
		}

		[OpenSilver.NotImplemented]
		public ControlStoryboardOption ControlStoryboardOption
		{
			get { return (ControlStoryboardOption)GetValue(ControlStoryboardProperty); }
			set { SetValue(ControlStoryboardProperty, value); }
		}

		//
		// Summary:
		//     This method is called when some criteria is met and the action should be invoked.
		//     This method will attempt to change the targeted storyboard in a way defined by
		//     the ControlStoryboardOption.
		//
		// Parameters:
		//   parameter:
		[OpenSilver.NotImplemented]
		protected override void Invoke(object parameter)
		{
			
		}

		[OpenSilver.NotImplemented]
		protected override void OnStoryboardChanged(DependencyPropertyChangedEventArgs args)
		{
			
		}
	}
}

#endif
