#if MIGRATION
namespace System.Windows.Input.Manipulations
#else
namespace Windows.UI.Xaml.Input.Manipulations
#endif
{
	public abstract class ManipulationParameters2D
	{
		internal ManipulationParameters2D()
		{
		}

		internal abstract void Set(ManipulationProcessor2D processor);
	}
}