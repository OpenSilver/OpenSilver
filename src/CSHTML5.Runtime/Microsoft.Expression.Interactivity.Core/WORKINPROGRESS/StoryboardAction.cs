#if WORKINPROGRESS
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
	public abstract partial class StoryboardAction : TriggerAction<DependencyObject>
	{
		public static readonly DependencyProperty StoryboardProperty;

		protected StoryboardAction()
		{
			
		}

		//
		// Summary:
		//     The targeted Storyboard. This is a dependency property.
		public Storyboard Storyboard { get; set; }

		//
		// Summary:
		//     This method is called when the Storyboard property is changed.
		//
		// Parameters:
		//   args:
		protected virtual void OnStoryboardChanged(DependencyPropertyChangedEventArgs args)
		{
			
		}
	}
}

#endif
#endif