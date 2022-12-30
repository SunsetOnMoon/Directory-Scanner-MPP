namespace DirectoryScanner.Core
{
    public interface IDirectoryScanner
    {
        DirectoryTree StartScanning(string pathToFile, ushort maxThreadsCount);
        void StopScanning();
        public bool IsScanning { get; }
    }
}
