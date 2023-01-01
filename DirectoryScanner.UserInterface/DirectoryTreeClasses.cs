﻿using System.Collections.ObjectModel;

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

    public class DirectoryTree
    {
        public DirectoryTree(DirectoryScanner.Core.Node root)
        {
            Root = new ObservableCollection<IDirectoryTreeObject> { TransformDirToNode(root, root.FileSize) };
        }
        public ObservableCollection<IDirectoryTreeObject> Root { get; }

        private static IDirectoryTreeObject TransformDirToNode(DirectoryScanner.Core.Node node, ulong parentSize)
        {
            IDirectoryTreeObject newNode;
            double percents = (double)node.FileSize / parentSize * 100;
            percents = double.IsNaN(percents) ? 0 : percents;
            if (node.ChildNodes == null)
             newNode = new File(node.FileName, (ulong)node.FileSize, percents);
            else
            {
                newNode = new Directory(node.FileName, (ulong)node.FileSize, percents);
                foreach (var childNode in node.ChildNodes)
                    ((Directory)newNode).Children.Add(TransformDirToNode(childNode, node.FileSize));
            }

            return newNode;
        }
    }
}
