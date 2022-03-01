#if MIGRATION
namespace System.Windows.Input.Manipulations
#else
namespace Windows.UI.Xaml.Input.Manipulations
#endif
{
	public sealed class InertiaRotationBehavior2D : InertiaParameters2D
	{
		public float DesiredDeceleration
		{
			get
			{
				return this.desiredDeceleration;
			}
			set
			{
				Validations.CheckFiniteNonNegative(value, "DesiredDeceleration");
				base.ProtectedChangeProperty(() => value == this.desiredDeceleration, delegate
				{
					this.desiredDeceleration = value;
					this.desiredRotation = float.NaN;
				}, "DesiredDeceleration");
			}
		}

		public float DesiredRotation
		{
			get
			{
				return this.desiredRotation;
			}
			set
			{
				Validations.CheckFiniteNonNegative(value, "DesiredRotation");
				base.ProtectedChangeProperty(() => value == this.desiredRotation, delegate
				{
					this.desiredRotation = value;
					this.desiredDeceleration = float.NaN;
				}, "DesiredRotation");
			}
		}

		public float InitialVelocity
		{
			get
			{
				return this.initialVelocity;
			}
			set
			{
				Validations.CheckFinite(value, "InitialVelocity");
				base.ProtectedChangeProperty(() => value == this.initialVelocity, delegate
				{
					this.initialVelocity = value;
				}, "InitialVelocity");
			}
		}

		internal void CheckValid()
		{
			if (!float.IsNaN(this.initialVelocity))
			{
				if (float.IsNaN(this.desiredDeceleration) && float.IsNaN(this.desiredRotation))
				{
					throw Exceptions.InertiaParametersUnspecified2(InertiaRotationBehavior2D.SubpropertyName("DesiredDeceleration"), InertiaRotationBehavior2D.SubpropertyName("DesiredRotation"));
				}
			}
		}

		private static string SubpropertyName(string paramName)
		{
			return "RotationBehavior." + paramName;
		}

		private const string desiredDecelerationName = "DesiredDeceleration";

		private const string desiredRotationName = "DesiredRotation";

		private const string initialVelocityName = "InitialVelocity";

		private float desiredDeceleration = float.NaN;

		private float desiredRotation = float.NaN;

		private float initialVelocity = float.NaN;
	}
}