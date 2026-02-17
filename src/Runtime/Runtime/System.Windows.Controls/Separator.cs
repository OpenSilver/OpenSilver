// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using OpenSilver.Internal;

namespace System.Windows.Controls
{
    /// <summary>
    /// Control that is used to separate items in items controls.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public class Separator : Control
    {
        static Separator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Separator), new PropertyMetadata(typeof(Separator)));
            IsEnabledProperty.OverrideMetadata(typeof(Separator), new PropertyMetadata(BooleanBoxes.FalseBox));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Separator"/> class.
        /// </summary>
        public Separator() { }
    }
}