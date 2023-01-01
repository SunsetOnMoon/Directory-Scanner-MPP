using System.Collections.ObjectModel;

namespace DirectoryScanner.UserInterface
{
    public class Directory : IDirectoryTreeObject
    {
        public Directory(string name, ulong size, double percents)
        {
            Name = name;
            Size = size;
            Percents = percents;
        }


        public string Name { get; }
        public ulong Size { get; }
        public double Percents { get; }
        public ObservableCollection<IDirectoryTreeObject> Children { get; } = new();
    
    }

    public class File : IDirectoryTreeObject
    {
        public File(string name, ulong size, double percents)
        {
            Name = name;
            Size = size;
            Percents = percents;
        }

        public string Name { get; }
        public ulong Size { get; }
        public double Percents { get; }
    }

    
}
