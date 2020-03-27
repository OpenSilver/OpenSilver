#if WORKINPROGRESS

using System.Windows.Markup;

#if MIGRATION
namespace System.Windows.Media.Animation
#else
namespace Windows.UI.Xaml.Media.Animation
#endif
{
	//
	// Summary:
	//     A trigger action that begins a System.Windows.Media.Animation.Storyboard and
	//     distributes its animations to their targeted objects and properties.
	[ContentProperty("Storyboard")]
	public sealed class BeginStoryboard : TriggerAction
	{
		//
		// Summary:
		//     Identifies the System.Windows.Media.Animation.BeginStoryboard.Storyboard dependency
		//     property.
		//
		// Returns:
		//     The identifier for the System.Windows.Media.Animation.BeginStoryboard.Storyboard
		//     dependency property.
		public static readonly DependencyProperty StoryboardProperty;

		//
		// Summary:
		//     Initializes a new instance of the System.Windows.Media.Animation.BeginStoryboard
		//     class.
		public BeginStoryboard()
		{
			
		}

		//
		// Summary:
		//     Gets or sets the System.Windows.Media.Animation.Storyboard that this System.Windows.Media.Animation.BeginStoryboard
		//     starts.
		//
		// Returns:
		//     The System.Windows.Media.Animation.Storyboard that the System.Windows.Media.Animation.BeginStoryboard
		//     starts. The default is null.
		public Storyboard Storyboard { get; set; }
	}
}

#endif