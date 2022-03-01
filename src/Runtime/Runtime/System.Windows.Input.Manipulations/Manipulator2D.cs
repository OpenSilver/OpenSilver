#if MIGRATION
namespace System.Windows.Input.Manipulations
#else
namespace Windows.UI.Xaml.Input.Manipulations
#endif
{
	public struct Manipulator2D
	{
		public static bool operator ==(Manipulator2D manipulator1, Manipulator2D manipulator2)
		{
			return manipulator1.Id == manipulator2.Id && manipulator1.X == manipulator2.X && manipulator1.Y == manipulator2.Y;
		}

		public static bool operator !=(Manipulator2D manipulator1, Manipulator2D manipulator2)
		{
			return !(manipulator1 == manipulator2);
		}

		public override bool Equals(object obj)
		{
			return obj is Manipulator2D && (Manipulator2D)obj == this;
		}

		public override int GetHashCode()
		{
			return this.id.GetHashCode() ^ this.x.GetHashCode() ^ this.y.GetHashCode();
		}

		public Manipulator2D(int id, float x, float y)
		{
			Validations.CheckFinite(x, "x");
			Validations.CheckFinite(y, "y");
			this.id = id;
			this.x = x;
			this.y = y;
		}

		public int Id
		{
			get
			{
				return this.id;
			}
			set
			{
				this.id = value;
			}
		}

		public float X
		{
			get
			{
				return this.x;
			}
			set
			{
				Validations.CheckFinite(value, "X");
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
				Validations.CheckFinite(value, "Y");
				this.y = value;
			}
		}

		private int id;

		private float x;

		private float y;
	}
}