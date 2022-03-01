using System.Globalization;
#if MIGRATION
namespace System.Windows.Input.Manipulations
#else
namespace Windows.UI.Xaml.Input.Manipulations
#endif
{
	internal struct VectorD
	{
		public VectorD(double x, double y)
		{
			this.x = x;
			this.y = y;
		}

		public double X
		{
			get
			{
				return this.x;
			}
		}

		public double Y
		{
			get
			{
				return this.y;
			}
		}

		public static VectorD operator -(VectorD vector)
		{
			return new VectorD(-vector.x, -vector.y);
		}

		public static bool operator !=(VectorD vector1, VectorD vector2)
		{
			return vector1.x != vector2.x || vector1.y != vector2.y;
		}

		public static bool operator ==(VectorD vector1, VectorD vector2)
		{
			return vector1.x == vector2.x && vector1.y == vector2.y;
		}

		public override bool Equals(object o)
		{
			return o is VectorD && (VectorD)o == this;
		}

		public static VectorD operator +(VectorD vector1, VectorD vector2)
		{
			return new VectorD(vector1.x + vector2.x, vector1.y + vector2.y);
		}

		public static VectorD operator -(VectorD vector1, VectorD vector2)
		{
			return new VectorD(vector1.x - vector2.x, vector1.y - vector2.y);
		}

		public static VectorD operator *(double scalar, VectorD vector)
		{
			return new VectorD(vector.x * scalar, vector.y * scalar);
		}

		public static VectorD operator *(VectorD vector, double scalar)
		{
			return new VectorD(vector.x * scalar, vector.y * scalar);
		}

		public static VectorD operator /(VectorD vector, double scalar)
		{
			return new VectorD(vector.x / scalar, vector.y / scalar);
		}

		public static double operator *(VectorD vector1, VectorD vector2)
		{
			return vector1.x * vector2.x + vector1.y * vector2.y;
		}

		public double Length
		{
			get
			{
				return Math.Sqrt(this.LengthSquared);
			}
		}

		public double LengthSquared
		{
			get
			{
				return this * this;
			}
		}

		public override int GetHashCode()
		{
			return this.x.GetHashCode() ^ this.y.GetHashCode();
		}

		public override string ToString()
		{
			return this.ToString(CultureInfo.CurrentCulture);
		}

		public string ToString(IFormatProvider provider)
		{
			return string.Format(provider, "X={0}, Y={1}", new object[]
			{
				this.x,
				this.y
			});
		}

		private double x;

		private double y;
	}
}