using System.Collections.Concurrent;

namespace DirectoryScanner.Core
{
    public class DirectoryScanner : IDirectoryScanner
    {
        private ConcurrentQueue<Node> _nodeProcQueue;
        private SemaphoreSlim _semaphore;
        private CancellationTokenSource _cancTokenSource;

        public DirectoryTree StartScanning(string pathToFile, ushort maxThreadsCount)
        {
            if (File.Exists(pathToFile))
            {
                FileInfo fileInfo = new FileInfo(pathToFile);
                return new DirectoryTree(new Node(fileInfo.FullName, fileInfo.Name, (ulong)fileInfo.Length));
            }
            if (!Directory.Exists(pathToFile))
                throw new DirectoryNotFoundException($"Directory with this {pathToFile} doesn't exists.");
            if (maxThreadsCount == 0)
                throw new ArgumentException("Count of threads can't be zero.");

            _nodeProcQueue = new ConcurrentQueue<Node>();
            _semaphore = new SemaphoreSlim(maxThreadsCount);
            _cancTokenSource = new CancellationTokenSource();
            CancellationToken token = _cancTokenSource.Token;

            IsScanning = true;

            DirectoryInfo dirInfo = new DirectoryInfo(pathToFile);
            Node rootDir = new Node(dirInfo.FullName, dirInfo.Name);
            ScanDirectory(rootDir, token);
            do
            {
                bool result = _nodeProcQueue.TryDequeue(out var node);
                if (result)
                {
                    try
                    {
                        _semaphore.Wait(token);
                        Task.Run(() =>
                        {
                            ScanDirectory(node, token);
                            _semaphore.Release();
                        }, token);
                    }
                    catch (Exception)
                    {
                    }
                }
            } while ((_semaphore.CurrentCount <= maxThreadsCount || !_nodeProcQueue.IsEmpty) && token.IsCancellationRequested);

            IsScanning = false;
            return new DirectoryTree(rootDir);
        }

        public void StopScanning()
        {
            if (!IsScanning)
                throw new Exception("You don't start directory scanning.");

            IsScanning = false;
            _cancTokenSource.Cancel();
        }

        private void ScanDirectory(Node node, CancellationToken cancellationToken)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(node.PathToFile);
            DirectoryInfo[] subDirsInfo;
            FileInfo[] fileInfos;

            node.ChildNodes = new List<Node>();

            try
            {
                subDirsInfo = dirInfo.GetDirectories();
            }
            catch (Exception)
            {
                return;
            }

            foreach (DirectoryInfo subDirInfo in subDirsInfo)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;
                Node childNode = new Node(subDirInfo.FullName, subDirInfo.Name);
                node.ChildNodes.Add(childNode);
                _nodeProcQueue.Enqueue(childNode);
            }

            fileInfos = dirInfo.GetFiles();
            foreach(FileInfo fileInfo in fileInfos)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;
                if (fileInfo.LinkTarget != null)
                    continue;
                node.ChildNodes.Add(new Node(fileInfo.FullName, fileInfo.Name, (ulong)fileInfo.Length));
            }
        }
        public bool IsScanning { get; private set; }
    }
}
