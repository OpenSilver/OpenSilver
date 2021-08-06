namespace TestApplication
{
    public class Test : ITreeItem
    {
        public string Name { get; }
        public string FileName { get; }

        public Test(string name, string fileName)
        {
            Name = name;
            FileName = fileName;
        }
    }
}
