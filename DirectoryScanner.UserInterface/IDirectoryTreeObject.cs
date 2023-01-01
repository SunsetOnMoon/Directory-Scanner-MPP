namespace DirectoryScanner.UserInterface
{
    public interface IDirectoryTreeObject
    {
        public string Name { get; }
        public ulong Size { get; }
        public double Percents { get; }
    }
}
