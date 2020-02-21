#if WORKINPROGRESS
using System;

#if MIGRATION
namespace System.Windows.Interop
#else
namespace Windows.UI.Xaml.Interop
#endif
{
	public sealed partial class Settings
	{
		private bool _windowless;
		private bool _enableAutoZoom;
		public bool Windowless
		{
			get
			{
				return _windowless;
			}
		}

		public bool EnableAutoZoom
		{
			get
			{
				return _enableAutoZoom;
			}

			set
			{
				_enableAutoZoom = value;
			}
		}

		public Settings()
		{
			_windowless = false;
			_enableAutoZoom = false;
		}
	}
}
#endif