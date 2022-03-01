using System.Diagnostics;
#if MIGRATION
namespace System.Windows.Input.Manipulations
#else
namespace Windows.UI.Xaml.Input.Manipulations
#endif
{
	public class ManipulationDelta2D
	{
		public float TranslationX
		{
			get
			{
				return this.translationX;
			}
		}

		public float TranslationY
		{
			get
			{
				return this.translationY;
			}
		}

		public float Rotation
		{
			get
			{
				return this.rotation;
			}
		}

		public float ScaleX
		{
			get
			{
				return this.scaleX;
			}
		}

		public float ScaleY
		{
			get
			{
				return this.scaleY;
			}
		}

		public float ExpansionX
		{
			get
			{
				return this.expansionX;
			}
		}

		public float ExpansionY
		{
			get
			{
				return this.expansionY;
			}
		}

		internal ManipulationDelta2D(float translationX, float translationY, float rotation, float scaleX, float scaleY, float expansionX, float expansionY)
		{
			Debug.Assert(Validations.IsFinite(translationX), "translationX should be finite");
			Debug.Assert(Validations.IsFinite(translationY), "translationY should be finite");
			Debug.Assert(Validations.IsFinite(rotation), "rotation should be finite");
			Debug.Assert(Validations.IsFiniteNonNegative(scaleX), "scaleX should be finite and not negative");
			Debug.Assert(Validations.IsFiniteNonNegative(scaleY), "scaleY should be finite and not negative");
			Debug.Assert(Validations.IsFinite(expansionX), "expansionX should be finite");
			Debug.Assert(Validations.IsFinite(expansionY), "expansionY should be finite");
			this.translationX = translationX;
			this.translationY = translationY;
			this.rotation = rotation;
			this.scaleX = scaleX;
			this.scaleY = scaleY;
			this.expansionX = expansionX;
			this.expansionY = expansionY;
		}

		private readonly float translationX;

		private readonly float translationY;

		private readonly float rotation;

		private readonly float scaleX;

		private readonly float scaleY;

		private readonly float expansionX;

		private readonly float expansionY;
	}
}