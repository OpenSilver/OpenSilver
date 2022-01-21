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
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Windows.Interactivity;

#if MIGRATION
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
#else
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI;
#endif

namespace Microsoft.Expression.Interactivity.Core
{
    /// <summary>
	/// An action that will change a specified property to a specified value when invoked.
	/// </summary>
	public class ChangePropertyAction : TargetedTriggerAction<object>
	{
		public static readonly DependencyProperty PropertyNameProperty = DependencyProperty.Register("PropertyName", typeof(string), typeof(ChangePropertyAction), null);
		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(ChangePropertyAction), null);
		public static readonly DependencyProperty DurationProperty = DependencyProperty.Register("Duration", typeof(Duration), typeof(ChangePropertyAction), null);
		public static readonly DependencyProperty IncrementProperty = DependencyProperty.Register("Increment", typeof(bool), typeof(ChangePropertyAction), null);

		/// <summary>
		/// Initializes a new instance of the <see cref="ChangePropertyAction"/> class.
		/// </summary>
		public ChangePropertyAction()
		{
		}

		/// <summary>
		/// Gets or sets the name of the property to change. This is a dependency property.
		/// </summary>
		/// <value>The name of the property to change.</value>
		public string PropertyName
		{
			get { return (string)this.GetValue(PropertyNameProperty); }
			set { this.SetValue(PropertyNameProperty, value); }
		}

		/// <summary>
		/// Gets or sets the value to set. This is a dependency property.
		/// </summary>
		/// <value>The value to set.</value>
		public object Value
		{
			get { return this.GetValue(ValueProperty); }
			set { this.SetValue(ValueProperty, value); }
		}

		/// <summary>
		/// Gets or sets the duration of the animation that will occur when the ChangePropertyAction is invoked.  This is a dependency property.
		/// If the duration is unset, no animation will be applied.
		/// </summary>
		public Duration Duration
		{
			get { return (Duration)this.GetValue(DurationProperty); }
			set { this.SetValue(DurationProperty, value); }
		}

		/// <summary>
		/// Increment by Value if true; otherwise, set the value directly. If the property cannot be incremented, it will instead try to set the value directly.
		/// </summary>
		public bool Increment
		{
			get { return (bool)this.GetValue(IncrementProperty); }
			set { this.SetValue(IncrementProperty, value); }
		}

		protected new object Target
		{
			get
			{
				if (this.TargetObject != null)
				{
					return this.TargetObject;
				}
				else
				{
					var parent = FrameworkElement.FindMentor(this);

                    if (parent != null)
                    {
						return parent;
                    }
				}

				return null;
			}
		}

		/// <summary>
		/// Invokes the action.
		/// </summary>
		/// <param name="parameter">The parameter of the action. If the action does not require a parameter, then the parameter may be set to a null reference.</param>
		/// <exception cref="ArgumentException">A property with <c cref="PropertyName"/> could not be found on the Target.</exception>
		/// <exception cref="ArgumentException">Could not set <c cref="PropertyName"/> to the value specified by <c cref="Value"/>.</exception>
		protected override void Invoke(object parameter)
		{
			if (string.IsNullOrEmpty(this.PropertyName))
			{
				return;
			}

			if (this.Target == null)
			{
				return;
			}

			Type targetType = this.Target.GetType();
			PropertyInfo propertyInfo = targetType.GetProperty(this.PropertyName);
			this.ValidateProperty(propertyInfo);

			object newValue = this.Value;
			TypeConverter converter = TypeConverterHelper.GetTypeConverter(propertyInfo.PropertyType);

			Exception innerException = null;
			try
			{
				if (this.Value != null)
				{
					if (converter != null && converter.CanConvertFrom(this.Value.GetType()))
					{
						newValue = converter.ConvertFrom(context: null, culture: CultureInfo.InvariantCulture, value: this.Value);
					}
					else
					{
						// Try asking the value if it can convert itself to the target property
						converter = TypeConverterHelper.GetTypeConverter(this.Value.GetType());
						if (converter != null && converter.CanConvertTo(propertyInfo.PropertyType))
						{
							newValue = converter.ConvertTo(
								context: null,
								culture: CultureInfo.InvariantCulture,
								value: this.Value,
								destinationType: propertyInfo.PropertyType);
						}
					}
				}

				// If a duration is set, we should animate this value.
				if (this.Duration.HasTimeSpan)
				{
					this.ValidateAnimationPossible(targetType);
					object fromValue = ChangePropertyAction.GetCurrentPropertyValue(this.Target, propertyInfo);
					this.AnimatePropertyChange(propertyInfo, fromValue, newValue);
				}
				else
				{
					if (this.Increment)
					{
						newValue = this.IncrementCurrentValue(propertyInfo);
					}
					propertyInfo.SetValue(this.Target, newValue, new object[0]);
				}
			}
			catch (FormatException e)
			{
				innerException = e;
			}
			catch (ArgumentException e)
			{
				innerException = e;
			}
			catch (MethodAccessException e)
			{
				innerException = e;
			}
			if (innerException != null)
			{
				throw new ArgumentException(string.Format(
					CultureInfo.CurrentCulture,
					"Cannot assign value of type '{0}' to property '{1}' of type '{2}'. The '{1}' property can be assigned only values of type '{2}'.",
					this.Value != null ? this.Value.GetType().Name : "null",
					this.PropertyName,
					propertyInfo.PropertyType.Name),
					innerException);
			}
		}

		private void AnimatePropertyChange(PropertyInfo propertyInfo, object fromValue, object newValue)
		{
			Storyboard sb = new Storyboard();
			Timeline timeline;
			if (typeof(double).IsAssignableFrom(propertyInfo.PropertyType))
			{
				timeline = this.CreateDoubleAnimation((double)fromValue, (double)newValue);
			}
			else if (typeof(Color).IsAssignableFrom(propertyInfo.PropertyType))
			{
				timeline = this.CreateColorAnimation((Color)fromValue, (Color)newValue);
			}
			else if (typeof(Point).IsAssignableFrom(propertyInfo.PropertyType))
			{
				timeline = this.CreatePointAnimation((Point)fromValue, (Point)newValue);
			}
			else
			{
				timeline = this.CreateKeyFrameAnimation(fromValue, newValue);
			}

			timeline.Duration = this.Duration;
			sb.Children.Add(timeline);

			Storyboard.SetTarget(sb, (DependencyObject)this.Target);

			Storyboard.SetTargetProperty(sb, new PropertyPath(propertyInfo.Name));

			sb.Completed += (o, e) =>
			{
				propertyInfo.SetValue(this.Target, newValue, new object[0]);
			};

			sb.FillBehavior = FillBehavior.Stop;

			sb.Begin();
		}

		private static object GetCurrentPropertyValue(object target, PropertyInfo propertyInfo)
		{
			FrameworkElement targetElement = target as FrameworkElement;
			Type targetType = target.GetType();
			object fromValue = propertyInfo.GetValue(target, null);

			if (targetElement != null &&
				(propertyInfo.Name == "Width" || propertyInfo.Name == "Height") &&
				Double.IsNaN((double)fromValue))
			{
				if (propertyInfo.Name == "Width")
				{
					fromValue = targetElement.ActualWidth;
				}
				else
				{
					fromValue = targetElement.ActualHeight;
				}
			}

			return fromValue;
		}

		private void ValidateAnimationPossible(Type targetType)
		{
			if (this.Increment)
			{
				throw new InvalidOperationException("The Increment property cannot be set to True if the Duration property is set.");
			}
			if (!typeof(DependencyObject).IsAssignableFrom(targetType))
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
					"Cannot animate a property change on a type '{0}' Target. Property changes can only be animated on types derived from DependencyObject.",
					targetType.Name));
			}
		}

		private Timeline CreateKeyFrameAnimation(object newValue, object fromValue)
		{
			ObjectAnimationUsingKeyFrames objectAnimation = new ObjectAnimationUsingKeyFrames();
			DiscreteObjectKeyFrame k1 = new DiscreteObjectKeyFrame()
			{
				KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0)),
				Value = fromValue,
			};
			DiscreteObjectKeyFrame k2 = new DiscreteObjectKeyFrame()
			{
				KeyTime = KeyTime.FromTimeSpan(this.Duration.TimeSpan),
				Value = newValue,
			};

			objectAnimation.KeyFrames.Add(k1);
			objectAnimation.KeyFrames.Add(k2);

			return objectAnimation;
		}

		private Timeline CreatePointAnimation(Point fromValue, Point newValue)
		{
			return new PointAnimation()
			{
				From = (Point)fromValue,
				To = (Point)newValue,
			};
		}

		private Timeline CreateColorAnimation(Color fromValue, Color newValue)
		{
			return new ColorAnimation()
			{
				From = fromValue,
				To = newValue,
			};
		}

		private Timeline CreateDoubleAnimation(double fromValue, double newValue)
		{
			return new DoubleAnimation()
			{
				From = fromValue,
				To = newValue,
			};
		}

		private void ValidateProperty(PropertyInfo propertyInfo)
		{
			if (propertyInfo == null)
			{
				throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
															"Cannot find a property named '{0}' on type '{1}'.",
															this.PropertyName,
															this.Target.GetType().Name));
			}

			if (!propertyInfo.CanWrite)
			{
				throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
															"Property '{0}' defined by type '{1}' does not expose a set method and therefore cannot be modified.",
															this.PropertyName,
															this.Target.GetType().Name));
			}
		}

		private object IncrementCurrentValue(PropertyInfo propertyInfo)
		{
			if (!propertyInfo.CanRead)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
					"The '{0}' property cannot be incremented because its value cannot be read.",
					propertyInfo.Name));
			}

			object currentValue = propertyInfo.GetValue(this.Target, null);
			object returnValue = currentValue;

			Type propertyType = propertyInfo.PropertyType;
			TypeConverter converter = TypeConverterHelper.GetTypeConverter(propertyInfo.PropertyType);
			object value = this.Value;

			if (value == null || currentValue == null)
			{
				// we can't increment by null, so we'll attempt to set it instead
				// likewise, we can't increment, null by x, so we'll just set value instead
				return value;
			}

			if (converter.CanConvertFrom(value.GetType()))
			{
				value = TypeConverterHelper.DoConversionFrom(converter, value);
			}

			if (typeof(double).IsAssignableFrom(propertyType))
			{
				returnValue = (double)currentValue + (double)value;
			}
			else if (typeof(int).IsAssignableFrom(propertyType))
			{
				returnValue = (int)currentValue + (int)value;
			}
			else if (typeof(float).IsAssignableFrom(propertyType))
			{
				returnValue = (float)currentValue + (float)value;
			}
			else if (typeof(string).IsAssignableFrom(propertyType))
			{
				returnValue = (string)currentValue + (string)value;
			}
			else
			{
				returnValue = TryAddition(currentValue, value);
			}
			return returnValue;
		}

		private static object TryAddition(object currentValue, object value)
		{
			object returnValue = null;
			Type valueType = value.GetType();
			Type additiveType = currentValue.GetType();

			MethodInfo uniqueAdditionOperation = null;
			object convertedValue = value;

			foreach (MethodInfo additionOperation in additiveType.GetMethods())
			{
				if (string.Compare(additionOperation.Name, "op_Addition", StringComparison.Ordinal) != 0)
				{
					continue;
				}

				ParameterInfo[] parameters = additionOperation.GetParameters();

				Debug.Assert(parameters.Length == 2, "op_Addition is expected to have 2 parameters");

				Type secondParameterType = parameters[1].ParameterType;
				if (!parameters[0].ParameterType.IsAssignableFrom(additiveType))
				{
					continue;
				}
				else if (!secondParameterType.IsAssignableFrom(valueType))
				{
					TypeConverter additionConverter = TypeConverterHelper.GetTypeConverter(secondParameterType);
					if (additionConverter.CanConvertFrom(valueType))
					{
						convertedValue = TypeConverterHelper.DoConversionFrom(additionConverter, value);
					}
					else
					{
						continue;
					}
				}

				if (uniqueAdditionOperation != null)
				{
					throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
						"More than one potential addition operator was found on type '{0}'.",
						additiveType.Name));
				}
				uniqueAdditionOperation = additionOperation;
			}

			if (uniqueAdditionOperation != null)
			{
				returnValue = uniqueAdditionOperation.Invoke(null, new object[] { currentValue, convertedValue });
			}
			else
			{
				// we couldn't figure out how to add, so pack it up and just set value
				returnValue = value;
			}

			return returnValue;
		}
	}	
}