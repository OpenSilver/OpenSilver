using System.Diagnostics;
#if MIGRATION
namespace System.Windows.Input.Manipulations
#else
namespace Windows.UI.Xaml.Input.Manipulations
#endif
{
	public class Manipulation2DDeltaEventArgs : EventArgs
	{
		internal Manipulation2DDeltaEventArgs(float originX, float originY, ManipulationVelocities2D velocities, ManipulationDelta2D delta, ManipulationDelta2D cumulative)
		{
			Debug.Assert(Validations.IsFinite(originX), "originX should be finite");
			Debug.Assert(Validations.IsFinite(originY), "originY should be finite");
			Debug.Assert(velocities != null, "velocities should not be null");
			Debug.Assert(delta != null, "delta should not be null");
			Debug.Assert(cumulative != null, "cumulative should not be null");
			this.originX = originX;
			this.originY = originY;
			this.velocities = velocities;
			this.delta = delta;
			this.cumulative = cumulative;
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

		public ManipulationDelta2D Delta
		{
			get
			{
				return this.delta;
			}
		}

		public ManipulationDelta2D Cumulative
		{
			get
			{
				return this.cumulative;
			}
		}

		private readonly float originX;

		private readonly float originY;

		private readonly ManipulationVelocities2D velocities;

		private readonly ManipulationDelta2D delta;

		private readonly ManipulationDelta2D cumulative;
	}
}