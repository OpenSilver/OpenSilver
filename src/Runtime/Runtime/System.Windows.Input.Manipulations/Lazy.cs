using System.Diagnostics;

#if MIGRATION
namespace System.Windows.Input.Manipulations
#else
namespace Windows.UI.Xaml.Input.Manipulations
#endif
{
	internal class Lazy<T>
	{
		public Lazy(Func<T> getValue)
		{
			Debug.Assert(getValue != null);
			this.getValue = getValue;
			this.gotValue = false;
		}

		public Lazy(T value)
		{
			this.value = value;
			this.gotValue = true;
		}

		public T Value
		{
			get
			{
				if (!this.gotValue)
				{
					this.value = this.getValue.Invoke();
					this.getValue = null;
					this.gotValue = true;
				}
				return this.value;
			}
		}

		private Func<T> getValue;

		private T value;

		private bool gotValue;
	}
}