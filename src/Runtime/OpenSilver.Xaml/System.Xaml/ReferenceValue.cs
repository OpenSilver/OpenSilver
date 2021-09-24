using System;
using System.Collections.Generic;
using System.Text;

namespace System.Xaml
{
	class ReferenceValueInternal
	{
		// static in a generic creates multiple copies
		public static readonly object NullValue = new object();
	}

	/// <summary>
	/// Struct to store a reference type and cache its value (even if null).
	/// </summary>
	struct ReferenceValue<T>
		where T : class
	{
		object _value;

		// automatically translates NullValue into null without an extra test
		public T Value => _value as T;

		public bool HasValue => !ReferenceEquals(_value, null);

		public ReferenceValue(T value)
		{
			_value = value ?? ReferenceValueInternal.NullValue;
		}

		public T Set(T value)
		{
			_value = value;
			if (ReferenceEquals(_value, null))
			{
				_value = ReferenceValueInternal.NullValue;
				return default(T);
			}
			return (T)_value;
		}

		public static implicit operator ReferenceValue<T>(T value)
		{
			return new ReferenceValue<T>(value);
		}

		public static bool operator ==(ReferenceValue<T> left, ReferenceValue<T> right)
		{
			return Equals(left._value, right._value);
		}

		public static bool operator !=(ReferenceValue<T> left, ReferenceValue<T> right)
		{
			return !Equals(left._value, right._value);
		}

		public override bool Equals(object obj)
		{
			return obj is ReferenceValue<T> && this == (ReferenceValue<T>)obj;
		}

		public override int GetHashCode()
		{
			return _value?.GetHashCode() ?? base.GetHashCode();
		}
	}
}
