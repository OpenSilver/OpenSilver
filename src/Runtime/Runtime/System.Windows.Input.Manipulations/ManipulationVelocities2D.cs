#if MIGRATION
using System.Diagnostics;

namespace System.Windows.Input.Manipulations
#else
namespace Windows.UI.Xaml.Input.Manipulations
#endif
{
	public class ManipulationVelocities2D
	{
		public float LinearVelocityX
		{
			get
			{
				return this.linearVelocityX.Value;
			}
		}

		public float LinearVelocityY
		{
			get
			{
				return this.linearVelocityY.Value;
			}
		}

		public float AngularVelocity
		{
			get
			{
				return this.angularVelocity.Value;
			}
		}

		public float ExpansionVelocityX
		{
			get
			{
				return this.expansionVelocity.Value;
			}
		}

		public float ExpansionVelocityY
		{
			get
			{
				return this.expansionVelocity.Value;
			}
		}

		internal ManipulationVelocities2D(float linearVelocityX, float linearVelocityY, float angularVelocity, float expansionVelocity)
		{
			Debug.Assert(Validations.IsFinite(linearVelocityX));
			Debug.Assert(Validations.IsFinite(linearVelocityY));
			Debug.Assert(Validations.IsFinite(angularVelocity));
			Debug.Assert(Validations.IsFinite(expansionVelocity));
			this.linearVelocityX = new Lazy<float>(linearVelocityX);
			this.linearVelocityY = new Lazy<float>(linearVelocityY);
			this.angularVelocity = new Lazy<float>(angularVelocity);
			this.expansionVelocity = new Lazy<float>(expansionVelocity);
		}

		internal ManipulationVelocities2D(Func<float> getLinearVelocityX, Func<float> getLinearVelocityY, Func<float> getAngularVelocity, Func<float> getExpansionVelocity)
		{
			Debug.Assert(getLinearVelocityX != null, "getLinearVelocityX should not be null");
			Debug.Assert(getLinearVelocityY != null, "getLinearVelocityX should not be null");
			Debug.Assert(getAngularVelocity != null, "getLinearVelocityX should not be null");
			Debug.Assert(getExpansionVelocity != null, "getLinearVelocityX should not be null");
			this.linearVelocityX = new Lazy<float>(getLinearVelocityX);
			this.linearVelocityY = new Lazy<float>(getLinearVelocityY);
			this.angularVelocity = new Lazy<float>(getAngularVelocity);
			this.expansionVelocity = new Lazy<float>(getExpansionVelocity);
		}

		private readonly Lazy<float> linearVelocityX;

		private readonly Lazy<float> linearVelocityY;

		private readonly Lazy<float> angularVelocity;

		private readonly Lazy<float> expansionVelocity;

		public static readonly ManipulationVelocities2D Zero = new ManipulationVelocities2D(0f, 0f, 0f, 0f);
	}
}