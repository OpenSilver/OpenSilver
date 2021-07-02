using System.Collections;
using System.Collections.Generic;

namespace TestApplication
{
    public class TestCategory : ITreeItem, IEnumerable<ITreeItem>
    {
        public string Name { get; }
        private List<ITreeItem> Children { get; } = new List<ITreeItem>();

        public TestCategory(string name)
        {
            Name = name;
        }

        public void Add(ITreeItem item)
        {
            Children.Add(item);
        }

        public IEnumerator<ITreeItem> GetEnumerator()
        {
            return Children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Children.GetEnumerator();
        }
    }
}
