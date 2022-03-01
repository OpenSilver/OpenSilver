#if MIGRATION
namespace System.Windows.Input.Manipulations
#else
namespace Windows.UI.Xaml.Input.Manipulations
#endif
{
	public sealed class InertiaExpansionBehavior2D : InertiaParameters2D
	{
		public float InitialRadius
		{
			get
			{
				return this.initialRadius;
			}
			set
			{
				InertiaExpansionBehavior2D.CheckRadius(value, "InitialRadius");
				base.ProtectedChangeProperty(() => value == this.initialRadius, delegate
				{
					this.initialRadius = value;
				}, "InitialRadius");
			}
		}

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
					this.desiredExpansionX = (this.desiredExpansionY = float.NaN);
				}, "DesiredDeceleration");
			}
		}

		public float DesiredExpansionX
		{
			get
			{
				return this.desiredExpansionX;
			}
			set
			{
				Validations.CheckFiniteNonNegative(value, "DesiredExpansionX");
				base.ProtectedChangeProperty(() => value == this.desiredExpansionX, delegate
				{
					this.desiredExpansionX = value;
					this.desiredDeceleration = float.NaN;
				}, "DesiredExpansionX");
			}
		}

		public float DesiredExpansionY
		{
			get
			{
				return this.desiredExpansionY;
			}
			set
			{
				Validations.CheckFiniteNonNegative(value, "DesiredExpansionY");
				base.ProtectedChangeProperty(() => value == this.desiredExpansionY, delegate
				{
					this.desiredExpansionY = value;
					this.desiredDeceleration = float.NaN;
				}, "DesiredExpansionY");
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
				if (this.initialVelocityX != this.initialVelocityY)
				{
					throw Exceptions.OnlyProportionalExpansionSupported(InertiaExpansionBehavior2D.SubpropertyName("InitialVelocityX"), InertiaExpansionBehavior2D.SubpropertyName("InitialVelocityY"));
				}
				if (!float.IsNaN(this.desiredExpansionX) && !float.IsNaN(this.desiredExpansionY) && this.desiredExpansionX != this.desiredExpansionY)
				{
					throw Exceptions.OnlyProportionalExpansionSupported(InertiaExpansionBehavior2D.SubpropertyName("DesiredExpansionX"), InertiaExpansionBehavior2D.SubpropertyName("DesiredExpansionY"));
				}
				if (float.IsNaN(this.desiredDeceleration) && (float.IsNaN(this.desiredExpansionX) || float.IsNaN(this.desiredExpansionY)))
				{
					throw Exceptions.InertiaParametersUnspecified1and2(InertiaExpansionBehavior2D.SubpropertyName("DesiredDeceleration"), InertiaExpansionBehavior2D.SubpropertyName("DesiredExpansionX"), InertiaExpansionBehavior2D.SubpropertyName("DesiredExpansionY"));
				}
			}
		}

		private static string SubpropertyName(string paramName)
		{
			return "ExpansionBehavior." + paramName;
		}

		private static void CheckRadius(float value, string paramName)
		{
			if (value < 1f || double.IsInfinity((double)value) || double.IsNaN((double)value))
			{
				throw Exceptions.IllegialInertiaRadius(paramName, value);
			}
		}

		private const string initialRadiusName = "InitialRadius";

		private const string desiredDecelerationName = "DesiredDeceleration";

		private const string desiredExpansionXName = "DesiredExpansionX";

		private const string desiredExpansionYName = "DesiredExpansionY";

		private const string initialVelocityXName = "InitialVelocityX";

		private const string initialVelocityYName = "InitialVelocityY";

		private float initialRadius = 1f;

		private float desiredDeceleration = float.NaN;

		private float desiredExpansionX = float.NaN;

		private float desiredExpansionY = float.NaN;

		private float initialVelocityX = float.NaN;

		private float initialVelocityY = float.NaN;
	}
}