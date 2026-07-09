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

namespace System.Windows;

internal readonly struct SourceItem
{
    internal SourceItem(int startIndex, object source)
    {
        StartIndex = startIndex;
        Source = source;
    }

    internal int StartIndex { get; }

    internal object Source { get; }

    public override bool Equals(object o) => Equals((SourceItem)o);

    public bool Equals(SourceItem sourceItem) => sourceItem.StartIndex == StartIndex && sourceItem.Source == Source;

    public override int GetHashCode() => base.GetHashCode();

    public static bool operator ==(SourceItem sourceItem1, SourceItem sourceItem2) => sourceItem1.Equals(sourceItem2);

    public static bool operator !=(SourceItem sourceItem1, SourceItem sourceItem2) => !sourceItem1.Equals(sourceItem2);
}
