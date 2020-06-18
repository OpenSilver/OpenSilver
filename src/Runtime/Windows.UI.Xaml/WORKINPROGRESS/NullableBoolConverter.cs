#if WORKINPROGRESS
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
	public partial class NullableBoolConverter : TypeConverter
	{
	}
}
#endif