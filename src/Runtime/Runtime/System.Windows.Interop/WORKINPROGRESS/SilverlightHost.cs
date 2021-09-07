using System;

#if MIGRATION
namespace System.Windows.Interop
#else
namespace Windows.UI.Xaml.Interop
#endif
{
    [OpenSilver.NotImplemented]
	public partial class SilverlightHost
	{
		private Uri _source;
		private Settings _settings;
		private Content _content;
        [OpenSilver.NotImplemented]
		public Uri Source
		{
			get
			{
				return _source;
			}
		}

        [OpenSilver.NotImplemented]
		public Settings Settings
		{
			get
			{
				return _settings;
			}
		}

        [OpenSilver.NotImplemented]
		public Content Content
		{
			get
			{
				return _content;
			}
		}

        [OpenSilver.NotImplemented]
		public SilverlightHost()
		{
			_source = null;
			_settings = null;
			_content = null;
		}
	}
}
