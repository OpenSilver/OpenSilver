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
    [OpenSilver.NotImplemented]
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
        [OpenSilver.NotImplemented]
		public static readonly DependencyProperty StoryboardProperty =
			DependencyProperty.Register("Storyboard",
										typeof(Storyboard),
										typeof(BeginStoryboard),
										null);

		//
		// Summary:
		//     Initializes a new instance of the System.Windows.Media.Animation.BeginStoryboard
		//     class.
        [OpenSilver.NotImplemented]
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
        [OpenSilver.NotImplemented]
		public Storyboard Storyboard
		{
			get { return (Storyboard)GetValue(StoryboardProperty); }
			set { SetValue(StoryboardProperty, value); }
		}
	}
}
