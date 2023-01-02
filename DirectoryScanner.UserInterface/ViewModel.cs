using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Forms;
using System.Diagnostics;
using System.Windows;

namespace DirectoryScanner.UserInterface
{
    public class ViewModel : INotifyPropertyChanged
    {
        private const ushort _maxThreadCount = 200;

        private readonly DirectoryScanner.Core.DirectoryScanner _dirScanner = new();
        private string _fileName;
        private string _dirName;
        private DirectoryTree _root;
        private Directory _selectedDirectory;


        private volatile bool _isScanning;
        public bool IsScanning
        {
            get => _isScanning;
            set
            {
                _isScanning = value;
                OnPropertyChanged(nameof(IsScanning));
            }
        }

        public ICommand SetDirectoryCommand { get; }
        public ICommand StartScanningCommand { get; }
        public ICommand StopScanningCommand { get; }
        public ICommand OpenDirectoryCommand { get; }

        public ViewModel()
        {
            SetDirectoryCommand = new Command(_ =>
            {
                using var openFileDialog = new FolderBrowserDialog();
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                    _fileName = openFileDialog.SelectedPath;
                else
                    return;
            }, _ => true);
            StartScanningCommand = new Command(_ =>
            {
                Task.Run(() =>
                {
                    IsScanning = true;
                    var result = _dirScanner.StartScanning(_fileName, _maxThreadCount);
                    IsScanning = false;
                    Root = new DirectoryTree(result.RootDir);

                });
            }, _ => _fileName != null && !IsScanning);
            StopScanningCommand = new Command(_ =>
            {
                _dirScanner.StopScanning();
                IsScanning = false;
            }, _ => IsScanning);
            OpenDirectoryCommand = new Command(_ =>
            {
                DirectoryScanner.Core.Node selected = new DirectoryScanner.Core.Node(_fileName, DirName);
                //SelectedDirectory = new(selected.PathToFile);
                OnDoubleClick(SelectedDirectory.FullPath);
            }, _ => true);
        }


        public DirectoryTree Root
        {
            get => _root;
            private set
            {
                _root = value;
                OnPropertyChanged(nameof(Root));
            }
        }

        public string DirName
        {
            get => _dirName;
            set
            {
                _dirName = value;
                OnPropertyChanged(nameof(DirName));
            }
        }
        public Directory SelectedDirectory
        {
            get => _selectedDirectory;
            set
            {
                _selectedDirectory = value;
                OnPropertyChanged(nameof(SelectedDirectory));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void OnDoubleClick(string fileName)
        {
            Process PrFolder = new Process();
            ProcessStartInfo psi = new ProcessStartInfo();
            //string file = @"C:\Work\5 semester\MPP\TestFolder\test1\test2doc.txt";
            //string file = $@"{selectedFile.PathToFile}\{selectedFile.FileName}";
            psi.CreateNoWindow = true;
            psi.WindowStyle = ProcessWindowStyle.Normal;
            psi.FileName = "explorer";
            psi.Arguments = @"/n, /select, " + fileName;
            PrFolder.StartInfo = psi;
            PrFolder.Start();
        }
    }
}
