
/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

using System.Diagnostics;
using System.Windows.Documents;
using System.Windows.Media;

namespace OpenSilver.Internal.Documents;

internal sealed class TextContainerRun : ITextContainer
{
    private readonly Run _run;

    public TextContainerRun(Run run)
    {
        Debug.Assert(run is not null);
        _run = run;
    }

    public string Text => _run.Text;

    public void OnTextContentChanged()
    {
        if (TextContainersHelper.Get(VisualTreeHelper.GetParent(_run)) is ITextContainer parent)
        {
            parent.OnTextContentChanged();
        }
    }
}
