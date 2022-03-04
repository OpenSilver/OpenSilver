using System;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    public enum CalendarMode
    {
		/// <summary>The <see cref="System.Windows.Controls.Calendar" /> displays a month at a time.</summary>	
		Month,
		/// <summary>The <see cref="System.Windows.Controls.Calendar" /> displays a year at a time.</summary>		
		Year,
		/// <summary>The <see cref="System.Windows.Controls.Calendar" /> displays a decade at a time.</summary>		
		Decade
	}
}
