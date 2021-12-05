

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

namespace System.Windows.Interactivity
{
    /// <summary>
    /// A trigger that listens for a specified event on its source and fires when that event is fired.
    /// 
    /// </summary>
    [OpenSilver.NotImplemented]
    public partial class EventTrigger
    {
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty SourceNameProperty = 
            DependencyProperty.Register("SourceName", typeof(string), typeof(EventTrigger), null);

        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty SourceObjectProperty = 
            DependencyProperty.Register("SourceObject", typeof(string), typeof(EventTrigger), null);

        [OpenSilver.NotImplemented]
        public string SourceName { get; set; }
        [OpenSilver.NotImplemented]
        public object SourceObject { get; set; }
    }
}
