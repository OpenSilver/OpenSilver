using System;

#if MIGRATION
namespace System.Windows.Interop
#else
namespace Windows.UI.Xaml.Interop
#endif
{
    [OpenSilver.NotImplemented]
	public sealed partial class Settings
	{
		private bool _windowless;
		private bool _enableAutoZoom;
        [OpenSilver.NotImplemented]
		public bool Windowless
		{
			get
			{
				return _windowless;
			}
		}

        [OpenSilver.NotImplemented]
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

        [OpenSilver.NotImplemented]
		public Settings()
		{
			_windowless = false;
			_enableAutoZoom = false;
		}
	}
}
