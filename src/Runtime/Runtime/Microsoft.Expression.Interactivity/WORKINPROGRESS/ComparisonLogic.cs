// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See THIRD-PARTY-NOTICES file in the project root for full license information.

/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

using System;
using System.ComponentModel;
using System.Globalization;
using Microsoft.Expression.Interactivity.Core;

namespace Microsoft.Expression.Interactivity
{
	internal static class ComparisonLogic
	{
		/// <summary>
		/// This method evaluates operands. 
		/// </summary>
		/// <param name="leftOperand">Left operand from the LeftOperand property.</param>
		/// <param name="operatorType">Operator from Operator property.</param>
		/// <param name="rightOperand">Right operand from the RightOperand property.</param>
		/// <returns>Returns true if the condition is met; otherwise, returns false.</returns>
		internal static bool EvaluateImpl(object leftOperand, ComparisonConditionType operatorType, object rightOperand)
		{
			bool result = false;

			if (leftOperand != null)
			{
				Type type = leftOperand.GetType();

				if (rightOperand != null)
				{
					var typeConverter = TypeConverterHelper.GetTypeConverter(type);
					rightOperand = TypeConverterHelper.DoConversionFrom(typeConverter, rightOperand);
				}
			}

			IComparable leftOperandComparable = leftOperand as IComparable;
			IComparable rightOperandComparable = rightOperand as IComparable;

			if (leftOperandComparable != null && rightOperandComparable != null)
			{
				return EvaluateComparable(leftOperandComparable, operatorType, rightOperandComparable);
			}

			switch (operatorType)
			{
				case ComparisonConditionType.Equal:
					result = object.Equals(leftOperand, rightOperand);
					break;
				case ComparisonConditionType.NotEqual:
					result = !object.Equals(leftOperand, rightOperand);
					break;
				case ComparisonConditionType.LessThan:
				case ComparisonConditionType.LessThanOrEqual:
				case ComparisonConditionType.GreaterThan:
				case ComparisonConditionType.GreaterThanOrEqual:
					if (leftOperandComparable == null && rightOperandComparable == null)
					{
						throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "LeftOperand of type '{1}' and RightOperand of type '{0}' cannot be used with operator '{2}'.", (leftOperand != null) ? leftOperand.GetType().Name : "null", (rightOperand != null) ? rightOperand.GetType().Name : "null", operatorType.ToString()));
					}

					if (leftOperandComparable == null)
					{
						throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "LeftOperand of type '{0}' cannot be used with operator '{1}'.", (leftOperand != null) ? leftOperand.GetType().Name : "null", operatorType.ToString()));
					}

					throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "RightOperand of type '{0}' cannot be used with operator '{1}'.", (rightOperand != null) ? rightOperand.GetType().Name : "null", operatorType.ToString()));
			}

			return result;
		}

		/// <summary>
		/// Evaluates both operands that implement the IComparable interface.
		/// </summary>
		/// <param name="leftOperand">Left operand from the LeftOperand property.</param>
		/// <param name="operatorType">Operator from Operator property.</param>
		/// <param name="rightOperand">Right operand from the RightOperand property.</param>
		/// <returns>Returns true if the condition is met; otherwise, returns false.</returns>
		private static bool EvaluateComparable(IComparable leftOperand, ComparisonConditionType operatorType, IComparable rightOperand)
		{
			object obj = null;

			try
			{
				obj = Convert.ChangeType(rightOperand, leftOperand.GetType(), CultureInfo.CurrentCulture);
			}
			catch (FormatException)
			{
			}
			catch (InvalidCastException)
			{
			}

			if (obj == null)
			{
				return operatorType == ComparisonConditionType.NotEqual;
			}

			int num = leftOperand.CompareTo((IComparable)obj);
			bool result = false;

			switch (operatorType)
			{
				case ComparisonConditionType.Equal:
					result = num == 0;
					break;
				case ComparisonConditionType.GreaterThan:
					result = num > 0;
					break;
				case ComparisonConditionType.GreaterThanOrEqual:
					result = num >= 0;
					break;
				case ComparisonConditionType.LessThan:
					result = num < 0;
					break;
				case ComparisonConditionType.LessThanOrEqual:
					result = num <= 0;
					break;
				case ComparisonConditionType.NotEqual:
					result = num != 0;
					break;
			}

			return result;
		}
	}
}
