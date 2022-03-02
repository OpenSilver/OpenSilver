using System;
using System.Diagnostics;
#if MIGRATION
namespace System.Windows.Input.Manipulations
#else
namespace Windows.UI.Xaml.Input.Manipulations
#endif
{
	public class Manipulation2DStartedEventArgs : EventArgs
	{
		internal Manipulation2DStartedEventArgs(float originX, float originY)
		{
			Debug.Assert(Validations.IsFinite(originX));
			Debug.Assert(Validations.IsFinite(originY));
			this.originX = originX;
			this.originY = originY;
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

		private readonly float originX;

		private readonly float originY;
	}
}