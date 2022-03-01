#if MIGRATION
namespace System.Windows.Input.Manipulations
#else
namespace Windows.UI.Xaml.Input.Manipulations
#endif
{
	public sealed class InertiaTranslationBehavior2D : InertiaParameters2D
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
					this.desiredDisplacement = float.NaN;
				}, "DesiredDeceleration");
			}
		}

		public float DesiredDisplacement
		{
			get
			{
				return this.desiredDisplacement;
			}
			set
			{
				Validations.CheckFiniteNonNegative(value, "DesiredDisplacement");
				base.ProtectedChangeProperty(() => value == this.desiredDisplacement, delegate
				{
					this.desiredDisplacement = value;
					this.desiredDeceleration = float.NaN;
				}, "DesiredDisplacement");
			}
		}

		public float InitialVelocityX
		{
			get
			{
				return this.initialVelocityX;
			}
			set
			{
				Validations.CheckFinite(value, "InitialVelocityX");
				base.ProtectedChangeProperty(() => value == this.initialVelocityX, delegate
				{
					this.initialVelocityX = value;
				}, "InitialVelocityX");
			}
		}

		public float InitialVelocityY
		{
			get
			{
				return this.initialVelocityY;
			}
			set
			{
				Validations.CheckFinite(value, "InitialVelocityY");
				base.ProtectedChangeProperty(() => value == this.initialVelocityY, delegate
				{
					this.initialVelocityY = value;
				}, "InitialVelocityY");
			}
		}

		internal void CheckValid()
		{
			if (!float.IsNaN(this.initialVelocityX) || !float.IsNaN(this.initialVelocityY))
			{
				if (float.IsNaN(this.desiredDeceleration) && float.IsNaN(this.desiredDisplacement))
				{
					throw Exceptions.InertiaParametersUnspecified2(InertiaTranslationBehavior2D.SubpropertyName("DesiredDeceleration"), InertiaTranslationBehavior2D.SubpropertyName("DesiredDisplacement"));
				}
			}
		}

		private static string SubpropertyName(string propertyName)
		{
			return "TranslationBehavior." + propertyName;
		}

		private const string desiredDecelerationName = "DesiredDeceleration";

		private const string desiredDisplacementName = "DesiredDisplacement";

		private const string initialVelocityXName = "InitialVelocityX";

		private const string initialVelocityYName = "InitialVelocityY";

		private float desiredDeceleration = float.NaN;

		private float desiredDisplacement = float.NaN;

		private float initialVelocityX = float.NaN;

		private float initialVelocityY = float.NaN;
	}
}