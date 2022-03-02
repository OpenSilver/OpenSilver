using System;
using System.Globalization;

#if MIGRATION
namespace System.Windows.Input.Manipulations
#else
namespace Windows.UI.Xaml.Input.Manipulations
#endif
{
	internal struct VectorF
	{
		public VectorF(float x, float y)
		{
			this.x = x;
			this.y = y;
		}

		public float X
		{
			get
			{
				return this.x;
			}
			set
			{
				this.x = value;
			}
		}

		public float Y
		{
			get
			{
				return this.y;
			}
			set
			{
				this.y = value;
			}
		}

		public static explicit operator PointF(VectorF vector)
		{
			return new PointF(vector.x, vector.y);
		}

		public static VectorF operator -(VectorF vector)
		{
			return new VectorF(-vector.x, -vector.y);
		}

		public static bool operator !=(VectorF vector1, VectorF vector2)
		{
			return vector1.x != vector2.x || vector1.y != vector2.y;
		}

		public static bool operator ==(VectorF vector1, VectorF vector2)
		{
			return vector1.x == vector2.x && vector1.y == vector2.y;
		}

		public override bool Equals(object o)
		{
			return o is VectorF && (VectorF)o == this;
		}

		public static VectorF operator +(VectorF vector1, VectorF vector2)
		{
			return new VectorF(vector1.x + vector2.x, vector1.y + vector2.y);
		}

		public static PointF operator +(VectorF vector, PointF point)
		{
			return new PointF(point.X + vector.x, point.Y + vector.y);
		}

		public static VectorF operator -(VectorF vector1, VectorF vector2)
		{
			return new VectorF(vector1.x - vector2.x, vector1.y - vector2.y);
		}

		public static VectorF operator *(float scalar, VectorF vector)
		{
			return new VectorF(vector.x * scalar, vector.y * scalar);
		}

		public static VectorF operator *(VectorF vector, float scalar)
		{
			return new VectorF(vector.x * scalar, vector.y * scalar);
		}

		public static VectorF operator /(VectorF vector, float scalar)
		{
			return new VectorF(vector.x / scalar, vector.y / scalar);
		}

		public static float operator *(VectorF vector1, VectorF vector2)
		{
			return vector1.x * vector2.x + vector1.y * vector2.y;
		}

		public float Length
		{
			get
			{
				return (float)Math.Sqrt((double)this.LengthSquared);
			}
		}

		public float LengthSquared
		{
			get
			{
				return this * this;
			}
		}

		public void Normalize()
		{
			float length = this.Length;
			this.x /= length;
			this.y /= length;
		}

		public static float AngleBetween(VectorF vector1, VectorF vector2)
		{
			vector1.Normalize();
			vector2.Normalize();
			double num = Math.Atan2((double)vector2.y, (double)vector2.x) - Math.Atan2((double)vector1.y, (double)vector1.x);
			if (num > 3.141592653589793)
			{
				num -= 6.283185307179586;
			}
			else if (num < -3.141592653589793)
			{
				num += 6.283185307179586;
			}
			return (float)num;
		}

		public override int GetHashCode()
		{
			return this.x.GetHashCode() ^ this.y.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.CurrentCulture, "(X={0}, Y={1})", new object[]
			{
				this.x,
				this.y
			});
		}

		private float x;

		private float y;
	}
}