#if MIGRATION

using System.Windows.Interactivity;
using System.Windows;
using System.Windows.Media.Animation;

namespace Microsoft.Expression.Interactivity.Core
{
	//
	// Summary:
	//     An abstract class that provides the ability to target a Storyboard.
	//
	// Remarks:
	//     For action authors, this class provides a standard way to target a Storyboard.
	//     Design tools may choose to provide a special editing experience for classes that
	//     inherit from this action, thereby improving the designer experience.
	[OpenSilver.NotImplemented]
	public abstract partial class StoryboardAction : TriggerAction<DependencyObject>
	{
		[OpenSilver.NotImplemented]
		public static readonly DependencyProperty StoryboardProperty =
			DependencyProperty.Register("Storyboard",
										typeof(Storyboard),
										typeof(StoryboardAction),
										null);

		[OpenSilver.NotImplemented]
		protected StoryboardAction()
		{
			
		}

		//
		// Summary:
		//     The targeted Storyboard. This is a dependency property.
		[OpenSilver.NotImplemented]
		public Storyboard Storyboard
		{
			get { return (Storyboard)GetValue(StoryboardProperty); }
			set { SetValue(StoryboardProperty, value); }
		}

		//
		// Summary:
		//     This method is called when the Storyboard property is changed.
		//
		// Parameters:
		//   args:
		[OpenSilver.NotImplemented]
		protected virtual void OnStoryboardChanged(DependencyPropertyChangedEventArgs args)
		{
			
		}
	}
}

#endif
