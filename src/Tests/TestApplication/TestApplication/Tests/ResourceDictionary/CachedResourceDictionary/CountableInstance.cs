using System.Windows.Controls;

namespace TestApplication.Tests.ResourceDictionary.CachedResourceDictionary
{
    public class CountableInstance : UserControl
    {
        public CountableInstance()
        {
            Count++;
        }

        public static int Count { get; private set; }
    }
}
