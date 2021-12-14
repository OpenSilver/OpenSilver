namespace TestApplication
{
    public class Test : ITreeItem
    {
        public string Name { get; private set; }
        public string FileName { get; private set; }

        public Test(string name, string fileName)
        {
            Name = name;
            FileName = fileName;
        }
    }
}
