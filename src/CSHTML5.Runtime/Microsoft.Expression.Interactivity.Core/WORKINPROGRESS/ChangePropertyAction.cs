#if WORKINPROGRESS

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
	//
	// Summary:
	//     An action that will change a specified property to a specified value when invoked.
	public partial class ChangePropertyAction : TargetedTriggerAction<object>
	{
		public static readonly DependencyProperty DurationProperty;
		public static readonly DependencyProperty EaseProperty;
		public static readonly DependencyProperty IncrementProperty;
		public static readonly DependencyProperty PropertyNameProperty;
		public static readonly DependencyProperty ValueProperty;

		//
		// Summary:
		//     Initializes a new instance of the Microsoft.Expression.Interactivity.Core.ChangePropertyAction
		//     class.
		public ChangePropertyAction()
		{
			
		}

		//
		// Summary:
		//     Gets or sets the duration of the animation that will occur when the ChangePropertyAction
		//     is invoked. This is a dependency property. If the duration is unset, no animation
		//     will be applied.
		public Duration Duration { get; set; }
		//
		// Summary:
		//     Gets or sets the easing function to use with the animation when the ChangePropertyAction
		//     is invoked. This is a dependency property.
		public IEasingFunction Ease { get; set; }
		//
		// Summary:
		//     Increment by Value if true; otherwise, set the value directly. If the property
		//     cannot be incremented, it will instead try to set the value directly.
		public bool Increment { get; set; }
		//
		// Summary:
		//     Gets or sets the name of the property to change. This is a dependency property.
		public string PropertyName { get; set; }
		//
		// Summary:
		//     Gets or sets the value to set. This is a dependency property.
		public object Value { get; set; }

		//
		// Summary:
		//     Invokes the action.
		//
		// Parameters:
		//   parameter:
		//     The parameter of the action. If the action does not require a parameter, then
		//     the parameter may be set to a null reference.
		//
		// Exceptions:
		//   T:System.ArgumentException:
		//     A property with could not be found on the Target.
		//
		//   T:System.ArgumentException:
		//     Could not set to the value specified by .
		protected override void Invoke(object parameter)
		{
			
		}
	}
}

#endif