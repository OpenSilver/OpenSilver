
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

using System;
using System.Windows;
using System.Windows.Documents;

namespace OpenSilver.Internal.Documents;

internal abstract class TextContainer<T> : ITextContainer
    where T : DependencyObject
{
    protected TextContainer(T parent)
    {
        Parent = parent ?? throw new ArgumentNullException(nameof(parent));
    }

    internal T Parent { get; }

    public abstract string Text { get; }

    public virtual void BeginChange() { }

    public virtual void EndChange() { }

    public void OnTextAdded(TextElement textElement) => OnTextAddedOverride(textElement);

    public void OnTextRemoved(TextElement textElement) => OnTextRemovedOverride(textElement);

    protected abstract void OnTextAddedOverride(TextElement textElement);

    protected abstract void OnTextRemovedOverride(TextElement textElement);
}

internal interface ITextContainer
{
    string Text { get; }
    void BeginChange();
    void EndChange();
    void OnTextAdded(TextElement textElement);
    void OnTextRemoved(TextElement textElement);
}