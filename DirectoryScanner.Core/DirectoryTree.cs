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
            if (node != null)
            {
                foreach (Node childNode in node.ChildNodes)
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
