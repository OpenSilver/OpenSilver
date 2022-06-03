using System;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Specifies values for the different modes of operation of a <see cref="Calendar"/>.
    /// </summary>
    public enum CalendarMode
    {
        /// <summary>The <see cref="Calendar" /> displays a month at a time.</summary>	
        Month,
        /// <summary>The <see cref="Calendar" /> displays a year at a time.</summary>		
        Year,
        /// <summary>The <see cref="Calendar" /> displays a decade at a time.</summary>		
        Decade
    }
}
