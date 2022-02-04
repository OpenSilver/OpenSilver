#if MIGRATION
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

namespace Microsoft.Expression.Interactivity.Core
{
	/// <summary>
	/// Represents one ternary condition.
	/// </summary>
	public class ComparisonCondition : DependencyObject
	{
		public static readonly DependencyProperty LeftOperandProperty = DependencyProperty.Register("LeftOperand", typeof(object), typeof(ComparisonCondition), new PropertyMetadata(null));
		public static readonly DependencyProperty OperatorProperty = DependencyProperty.Register("Operator", typeof(ComparisonConditionType), typeof(ComparisonCondition), new PropertyMetadata(ComparisonConditionType.Equal));
		public static readonly DependencyProperty RightOperandProperty = DependencyProperty.Register("RightOperand", typeof(object), typeof(ComparisonCondition), new PropertyMetadata(null));

		/// <summary>
		/// Gets or sets the left operand.
		/// </summary>
		public object LeftOperand
		{
			get { return GetValue(LeftOperandProperty); }
			set { SetValue(LeftOperandProperty, value); }
		}
		/// <summary>
		/// Gets or sets the right operand.
		/// </summary>
		public object RightOperand
		{
			get { return GetValue(RightOperandProperty); }
			set { SetValue(RightOperandProperty, value); }
		}
		/// <summary>
		/// Gets or sets the comparison operator. 
		/// </summary>
		public ComparisonConditionType Operator
		{
			get { return (ComparisonConditionType)GetValue(OperatorProperty); }
			set { SetValue(OperatorProperty, value); }
		}

		/// <summary>
		/// Method that evaluates the condition. Note that this method can throw ArgumentException if the operator is
		/// incompatible with the type. For instance, operators LessThan, LessThanOrEqual, GreaterThan, and GreaterThanOrEqual
		/// require both operators to implement IComparable. 
		/// </summary>
		/// <returns>Returns true if the condition has been met; otherwise, returns false.</returns>
		public bool Evaluate()
		{
			return ComparisonLogic.EvaluateImpl(this.LeftOperand, this.Operator, this.RightOperand);
		}
	}
}