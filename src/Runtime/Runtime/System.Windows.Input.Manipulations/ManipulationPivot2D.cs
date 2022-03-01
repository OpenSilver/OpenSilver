using System.Diagnostics;

#if MIGRATION
namespace System.Windows.Input.Manipulations
#else
namespace Windows.UI.Xaml.Input.Manipulations
#endif
{
	public sealed class ManipulationPivot2D : ManipulationParameters2D
	{
		public float X
		{
			get
			{
				return this.x;
			}
			set
			{
				Validations.CheckFiniteOrNaN(value, "X");
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
				Validations.CheckFiniteOrNaN(value, "Y");
				this.y = value;
			}
		}

		public float Radius
		{
			get
			{
				return this.radius;
			}
			set
			{
				ManipulationPivot2D.CheckPivotRadius(value, "Radius");
				this.radius = value;
			}
		}

		internal bool HasPosition
		{
			get
			{
				return !float.IsNaN(this.x) && !float.IsNaN(this.y);
			}
		}

		internal override void Set(ManipulationProcessor2D processor)
		{
			Debug.Assert(processor != null);
			processor.Pivot = this;
		}

		private static void CheckPivotRadius(float value, string paramName)
		{
			if (!float.IsNaN(value) && (float.IsInfinity(value) || value < 1f))
			{
				throw Exceptions.IllegalPivotRadius(paramName, value);
			}
		}

		private float x = float.NaN;

		private float y = float.NaN;

		private float radius = float.NaN;
	}
}