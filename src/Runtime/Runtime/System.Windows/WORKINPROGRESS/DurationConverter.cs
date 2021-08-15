#if WORKINPROGRESS
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
	[OpenSilver.NotImplemented]
	public partial class DurationConverter : TypeConverter
	{
	}
}
#endif