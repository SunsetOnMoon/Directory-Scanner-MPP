namespace DirectoryScanner.Core
{
    public class DirectoryTree
    {
        public Node RootDir { get; }

        public DirectoryTree(Node rootDir)
        {
            RootDir = rootDir;
            GetDirectorySize(rootDir);
        }

        private static void GetDirectorySize(Node node)
        {
            if (node.ChildNodes != null)
            {
                foreach (var childNode in node.ChildNodes)
                {
                    GetDirectorySize(childNode);
                    node.FileSize += childNode.FileSize;
                }
            }
            else
                return;
        }
    }
}
