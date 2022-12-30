namespace DirectoryScanner.Core
{
    internal class Node
    {
        public string PathToFile { get; }
        public string FileName { get; }
        public ulong FileSize { get; internal set; }
        public List<Node> ChildNodes { get; internal set; }

        public Node(string pathToFile, string fileName)
        {
            PathToFile = pathToFile;
            FileName = fileName;
        }
        public Node(string pathToFile, string fileName, ulong fileSize)
        {
            PathToFile = pathToFile;
            FileName = fileName;
            FileSize = fileSize;
        }
    }
}
