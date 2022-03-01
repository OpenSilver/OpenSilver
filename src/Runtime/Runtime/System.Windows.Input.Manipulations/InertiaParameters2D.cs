using System.Diagnostics;

#if MIGRATION
namespace System.Windows.Input.Manipulations
#else
namespace Windows.UI.Xaml.Input.Manipulations
#endif
{
	public abstract class InertiaParameters2D
	{
		internal event Action<InertiaParameters2D, string> Changed;

		internal void ProtectedChangeProperty(Func<bool> isEqual, Action setNewValue, string paramName)
		{
			Debug.Assert(isEqual != null);
			Debug.Assert(setNewValue != null);
			Debug.Assert(paramName != null);
			if (!isEqual.Invoke())
			{
				setNewValue.Invoke();
				if (this.Changed != null)
				{
					this.Changed.Invoke(this, paramName);
				}
			}
		}

		internal InertiaParameters2D()
		{
		}
	}
}