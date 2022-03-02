using System;
using System.Diagnostics;
#if MIGRATION
namespace System.Windows.Input.Manipulations
#else
namespace Windows.UI.Xaml.Input.Manipulations
#endif
{
	public class Manipulation2DCompletedEventArgs : EventArgs
	{
		internal Manipulation2DCompletedEventArgs(float originX, float originY, ManipulationVelocities2D velocities, ManipulationDelta2D total)
		{
			Debug.Assert(Validations.IsFinite(originX), "originX should be finite");
			Debug.Assert(Validations.IsFinite(originY), "originY should be finite");
			Debug.Assert(velocities != null, "velocities should not be null");
			Debug.Assert(total != null, "total should not be null");
			this.originX = originX;
			this.originY = originY;
			this.velocities = velocities;
			this.total = total;
		}

		public float OriginX
		{
			get
			{
				return this.originX;
			}
		}

		public float OriginY
		{
			get
			{
				return this.originY;
			}
		}

		public ManipulationVelocities2D Velocities
		{
			get
			{
				return this.velocities;
			}
		}

		public ManipulationDelta2D Total
		{
			get
			{
				return this.total;
			}
		}

		private readonly float originX;

		private readonly float originY;

		private readonly ManipulationVelocities2D velocities;

		private readonly ManipulationDelta2D total;
	}
}