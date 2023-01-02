using System.Diagnostics;
using DirectoryScanner.Core;

namespace DirectoryScanner.Tests
{
    public class Tests
    {
        private readonly DirectoryScanner.Core.DirectoryScanner _scanner = new();

        [Test]
        public void InvalidArgumentsTest()
        {
            Assert.Throws<DirectoryNotFoundException>(() => _scanner.StartScanning("invalid", 150),
                "Didn't throw exception about invalid directory name");
            Assert.Throws<ArgumentException>(() => _scanner.StartScanning(@"./", 0),
                "Didn't throw exception about maxThreadCount being 0");
        }

        [Test]
        public void CorrectDirectoryTest()
        {
            var result = _scanner.StartScanning("C:\\Work\\5 semester\\MPP\\TestFolder", 200);
            var root = result.RootDir;
            Assert.That(root.ChildNodes, Is.Not.Null);
            Assert.That(root.ChildNodes!, Has.Count.EqualTo(2));

            int aIndex = 0, fileIndex = 1;
            if (root.ChildNodes![0].FileName != "test1")
            {
                aIndex = 1;
                fileIndex = 0;
            }

            Assert.Multiple(() =>
            {
                Assert.That(root.ChildNodes![fileIndex].FileName, Is.EqualTo("doc.txt"));
                Assert.That(root.ChildNodes![fileIndex].FileSize, Is.EqualTo(6240));
                Assert.That(root.ChildNodes![fileIndex].ChildNodes, Is.Null);
            });

            Assert.That(root.ChildNodes![aIndex].ChildNodes, Is.Not.Null);
            Assert.That(root.ChildNodes![aIndex].ChildNodes, Has.Count.EqualTo(2));
        }

        [Test]
        public void CancellationTest()
        {
            const string dirName = @"C:\";
            const int maxThreadCount = 150;

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var resultNoCancellation = _scanner.StartScanning(dirName, maxThreadCount);
            stopwatch.Stop();
            var executionTimeNoCancellation = stopwatch.ElapsedMilliseconds;

            stopwatch.Reset();
            DirectoryTree resultWithCancellation = null!;
            var task = Task.Run(() =>
            {
                stopwatch.Start();
                resultWithCancellation = _scanner.StartScanning(dirName, maxThreadCount);
                stopwatch.Stop();
            });
            Thread.Sleep(100); // wait for scan to start
            _scanner.StopScanning();
            task.Wait();
            var executionTimeWithCancellation = stopwatch.ElapsedMilliseconds;

            Assert.Multiple(() =>
            {
                Assert.That(executionTimeWithCancellation, Is.LessThan(executionTimeNoCancellation));
                Assert.That(resultNoCancellation.RootDir.FileSize, Is.GreaterThanOrEqualTo(resultWithCancellation.RootDir.FileSize));
            });
        }

        [Test]
        public void FileScanTest()
        {
            var result = _scanner.StartScanning("C:\\Work\\5 semester\\MPP\\TestFolder\\doc.txt", 200);

            Assert.Multiple(() =>
            {
                Assert.That(result.RootDir.ChildNodes, Is.Null);
                Assert.That(result.RootDir.FileSize, Is.EqualTo(6240));
                Assert.That(result.RootDir.FileName, Is.EqualTo("doc.txt"));
            });
        }
    }
}