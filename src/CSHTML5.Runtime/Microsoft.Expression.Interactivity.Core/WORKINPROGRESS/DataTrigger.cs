#if WORKINPROGRESS
#if MIGRATION

using System.Windows;

namespace Microsoft.Expression.Interactivity.Core
{
	//
	// Summary:
	//     Represents a trigger that performs actions when the bound data meets a specified
	//     condition.
	public partial class DataTrigger : PropertyChangedTrigger
	{
		public static readonly DependencyProperty ComparisonProperty =
			DependencyProperty.Register("Comparison",
										typeof(ComparisonConditionType),
										typeof(DataTrigger),
										new PropertyMetadata(ComparisonConditionType.Equal));
		public static readonly DependencyProperty ValueProperty =
			DependencyProperty.Register("Value",
										typeof(object),
										typeof(DataTrigger),
										null);

		public DataTrigger()
		{
			
		}

		//
		// Summary:
		//     Gets or sets the type of comparison to be performed between the specified values.
		//     This is a dependency property.
		public ComparisonConditionType Comparison
		{
			get { return (ComparisonConditionType)GetValue(ComparisonProperty); }
			set { SetValue(ComparisonProperty, value); }
		}
		//
		// Summary:
		//     Gets or sets the value to be compared with the property value of the data object.
		//     This is a dependency property.
		public object Value
		{
			get { return (object)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}

		//
		// Summary:
		//     Called when the binding property has changed. UA_REVIEW:chabiss
		//
		// Parameters:
		//   args:
		//     System.Windows.DependencyPropertyChangedEventArgs argument.
		protected override void EvaluateBindingChange(object args)
		{
			
		}
	}
}

#endif
#endif