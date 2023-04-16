using OpenSilver;

namespace System.Windows;

internal sealed partial class TextMeasurementService
{
    private enum JsCall
    {
        [JsCall(@"
document.measureTextBlock({uid},{whiteSpace},{overflowWrap},{strPadding},{strMaxWidth},{emptyVal})")]
        MeasureTextBlock
    }
}