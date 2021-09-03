#if MIGRATION

using System.Windows.Interactivity;
using System.Windows;
using System.Windows.Media.Animation;

namespace Microsoft.Expression.Interactivity.Core
{
	//
	// Summary:
	//     An action that will change a specified property to a specified value when invoked.
	[OpenSilver.NotImplemented]
	public partial class ChangePropertyAction : TargetedTriggerAction<object>
	{
		[OpenSilver.NotImplemented]
		public static readonly DependencyProperty DurationProperty =
			DependencyProperty.Register("Duration", 
										typeof(Duration), 
										typeof(ChangePropertyAction), 
										null);
		[OpenSilver.NotImplemented]
		public static readonly DependencyProperty EaseProperty =
			DependencyProperty.Register("Ease",
										typeof(IEasingFunction),
										typeof(ChangePropertyAction),
										null);
		[OpenSilver.NotImplemented]
		public static readonly DependencyProperty IncrementProperty =
			DependencyProperty.Register("Increment",
										typeof(bool),
										typeof(ChangePropertyAction),
										null);
		[OpenSilver.NotImplemented]
		public static readonly DependencyProperty PropertyNameProperty =
			DependencyProperty.Register("PropertyName",
										typeof(string),
										typeof(ChangePropertyAction),
										null);
		[OpenSilver.NotImplemented]
		public static readonly DependencyProperty ValueProperty =
			DependencyProperty.Register("Value",
										typeof(object),
										typeof(ChangePropertyAction),
										null);

		//
		// Summary:
		//     Initializes a new instance of the Microsoft.Expression.Interactivity.Core.ChangePropertyAction
		//     class.
		[OpenSilver.NotImplemented]
		public ChangePropertyAction()
		{
			
		}

		//
		// Summary:
		//     Gets or sets the duration of the animation that will occur when the ChangePropertyAction
		//     is invoked. This is a dependency property. If the duration is unset, no animation
		//     will be applied.
		[OpenSilver.NotImplemented]
		public Duration Duration
		{
			get { return (Duration)GetValue(DurationProperty); }
			set { SetValue(DurationProperty, value); }
		}
		//
		// Summary:
		//     Gets or sets the easing function to use with the animation when the ChangePropertyAction
		//     is invoked. This is a dependency property.
		[OpenSilver.NotImplemented]
		public IEasingFunction Ease
		{
			get { return (IEasingFunction)GetValue(EaseProperty); }
			set { SetValue(EaseProperty, value); }
		}
		//
		// Summary:
		//     Increment by Value if true; otherwise, set the value directly. If the property
		//     cannot be incremented, it will instead try to set the value directly.
		[OpenSilver.NotImplemented]
		public bool Increment
		{
			get { return (bool)GetValue(IncrementProperty); }
			set { SetValue(IncrementProperty, value); }
		}
		//
		// Summary:
		//     Gets or sets the name of the property to change. This is a dependency property.
		[OpenSilver.NotImplemented]
		public string PropertyName
		{
			get { return (string)GetValue(PropertyNameProperty); }
			set { SetValue(PropertyNameProperty, value); }
		}
		//
		// Summary:
		//     Gets or sets the value to set. This is a dependency property.
		[OpenSilver.NotImplemented]
		public object Value
		{
			get { return (object)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}

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
		[OpenSilver.NotImplemented]
		protected override void Invoke(object parameter)
		{
			
		}
	}
}

#endif
