using System.Collections;
using System.Collections.Generic;

namespace TestApplication
{
    public class TestCategory : ITreeItem, IEnumerable<ITreeItem>
    {
        public string Name { get; private set; }
        private List<ITreeItem> Children { get; set; }

        public TestCategory(string name)
        {
            Name = name;
            Children = new List<ITreeItem>();
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
