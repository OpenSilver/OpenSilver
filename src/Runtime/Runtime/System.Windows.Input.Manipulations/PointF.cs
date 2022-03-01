using System.Globalization;

#if MIGRATION
namespace System.Windows.Input.Manipulations
#else
namespace Windows.UI.Xaml.Input.Manipulations
#endif
{
	internal struct PointF
	{
		public PointF(float x, float y)
		{
			this.x = x;
			this.y = y;
		}

		public static explicit operator VectorF(PointF point)
		{
			return new VectorF(point.x, point.y);
		}

		public static bool operator !=(PointF left, PointF right)
		{
			return left.X != right.X || left.Y != right.Y;
		}

		public static bool operator ==(PointF left, PointF right)
		{
			return left.X == right.X && left.Y == right.Y;
		}

		public static PointF operator +(PointF pt, VectorF offset)
		{
			return new PointF(pt.X + offset.X, pt.Y + offset.Y);
		}

		public static VectorF operator -(PointF point1, PointF point2)
		{
			return new VectorF(point1.x - point2.x, point1.y - point2.y);
		}

		public static PointF operator -(PointF point, VectorF vector)
		{
			return new PointF(point.x - vector.X, point.y - vector.Y);
		}

		public float X
		{
			get
			{
				return this.x;
			}
		}

		public float Y
		{
			get
			{
				return this.y;
			}
		}

		public override bool Equals(object obj)
		{
			return obj is PointF && (PointF)obj == this;
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