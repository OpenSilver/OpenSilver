#if WORKINPROGRESS
using System;

#if MIGRATION
namespace System.Windows.Interop
#else
namespace Windows.UI.Xaml.Interop
#endif
{
	public partial class SilverlightHost
	{
		private Uri _source;
		private Settings _settings;
		private Content _content;
		public Uri Source
		{
			get
			{
				return _source;
			}
		}

		public Settings Settings
		{
			get
			{
				return _settings;
			}
		}

		public Content Content
		{
			get
			{
				return _content;
			}
		}

		public SilverlightHost()
		{
			_source = null;
			_settings = null;
			_content = null;
		}
	}
}
#endif