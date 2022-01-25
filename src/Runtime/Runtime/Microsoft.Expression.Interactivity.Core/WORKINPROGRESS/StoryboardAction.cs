
using System.Windows.Interactivity;

#if MIGRATION
using System.Windows;
using System.Windows.Media.Animation;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;
#endif

namespace Microsoft.Expression.Interactivity.Core
{
	/// <summary>
	/// An abstract class that provides the ability to target a Storyboard.
	/// </summary>
	/// <remarks>
	/// For action authors, this class provides a standard way to target a Storyboard. Design tools may choose to provide a 
	/// special editing experience for classes that inherit from this action, thereby improving the designer experience.
	/// </remarks>
	public abstract class StoryboardAction : TriggerAction<DependencyObject>
	{
		public static readonly DependencyProperty StoryboardProperty = DependencyProperty.Register("Storyboard", typeof(Storyboard), typeof(StoryboardAction), new PropertyMetadata(OnStoryboardChanged));

		/// <summary>
		/// The targeted Storyboard. This is a dependency property.
		/// </summary>
		public Storyboard Storyboard
		{
			get { return (Storyboard)this.GetValue(StoryboardProperty); }
			set { this.SetValue(StoryboardProperty, value); }
		}

		private static void OnStoryboardChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
		{
			StoryboardAction storyboardAction = sender as StoryboardAction;
			if (storyboardAction != null)
			{
				storyboardAction.OnStoryboardChanged(args);
			}
		}

		/// <summary>
		/// This method is called when the Storyboard property is changed.
		/// </summary>
		/// <param name="args"></param>
		protected virtual void OnStoryboardChanged(DependencyPropertyChangedEventArgs args)
		{
		}
	}
}