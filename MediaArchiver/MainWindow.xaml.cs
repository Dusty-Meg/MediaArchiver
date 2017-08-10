using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MediaArchiver.DataModels;
using MediaArchiver.TreeViewModels;
using FileInfo = MediaArchiver.DataModels.FileInfo;

namespace MediaArchiver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string MediaFolder { get; set; }
        private string ArchiveFolder { get; set; }

        private BaseDirectoryLayout BaseDirectoryLayout { get; set; }
        private TopDirectoryViewModel TopDirectoryViewModel { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnLoadFile_Click(object sender, RoutedEventArgs e)
        {
            MediaFolder = txtMediaFolder.Text;
            ArchiveFolder = txtArchiveFolder.Text;

            BaseDirectoryLayout = new BaseDirectoryLayout();

            if (Directory.Exists(MediaFolder))
            {
                foreach (string directories in Directory.GetDirectories(MediaFolder))
                {
                    BaseDirectoryLayout.Folders.Add(LoadDirectories(directories));
                }
            }

            CheckArchiveFolders(BaseDirectoryLayout.Folders);

            LoadTreeView(BaseDirectoryLayout);
        }

        private void CheckArchiveFolders(IList<DirectoryLayout> directories)
        {
            foreach (DirectoryLayout directoryLayout in directories)
            {
                string directoryName = directoryLayout.DirectoryPath.Replace(MediaFolder, string.Empty).TrimStart('\\');
                string archiceDirectory = System.IO.Path.Combine(ArchiveFolder, directoryName);

                if (!Directory.Exists(archiceDirectory))
                {
                    Directory.CreateDirectory(archiceDirectory);
                }

                CheckArchiveFolders(directoryLayout.Folders);
            }

        }

        private void btnArchiveFile_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            FileViewModel fileViewModel = button?.DataContext as FileViewModel;

            CheckFileViewModel(fileViewModel);
        }

        private void btnArchiveSeason_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            DirectoryViewModel directoryViewModel = button?.DataContext as DirectoryViewModel;

            CheckDirectoryViewModel(directoryViewModel);
        }

        private void btnArchiveSeries_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            BaseDirectoryViewModel baseDirectoryLayout = button?.DataContext as BaseDirectoryViewModel;

            CheckBaseDirectoryViewModel(baseDirectoryLayout);
        }

        private void CheckBaseDirectoryViewModel(BaseDirectoryViewModel directoryViewModel)
        {
            if (directoryViewModel != null)
            {
                CheckChildren(directoryViewModel.Children, directoryViewModel.IsDirectoryArchived);
            }
        }

        private void CheckDirectoryViewModel(DirectoryViewModel directoryViewModel, bool? archive = null)
        {
            if (directoryViewModel != null)
            {
                CheckChildren(directoryViewModel.Children, archive ?? directoryViewModel.IsDirectoryArchived);
            }
        }

        private void CheckChildren(ObservableCollection<TreeViewItemViewModel> children, bool archive)
        {
            foreach (TreeViewItemViewModel treeViewItemViewModel in children)
            {
                FileViewModel fileViewModels = treeViewItemViewModel as FileViewModel;

                if (fileViewModels != null)
                {
                    CheckFileViewModel(fileViewModels, archive);

                    continue;
                }

                DirectoryViewModel directoryViewModels = treeViewItemViewModel as DirectoryViewModel;

                if (directoryViewModels != null)
                {
                    CheckDirectoryViewModel(directoryViewModels, archive);

                    continue;
                }
            }
        }

        private void CheckFileViewModel(FileViewModel fileViewModel, bool? archive = null)
        {
            if (fileViewModel != null)
            {
                if (archive == null)
                { 
                    archive = fileViewModel.IsArchived;
                }

                if (fileViewModel.IsArchived && archive.Value)
                {
                    UnArchiveFile(fileViewModel.FilePath);
                }
                else if (!fileViewModel.IsArchived && !archive.Value)
                {
                    ArchiveFile(fileViewModel.FilePath);
                }
            }
        }

        private void ArchiveFile(string filePath)
        {
            string fileName = filePath.Replace(MediaFolder, string.Empty).TrimStart('\\');
            string archiveFile = System.IO.Path.Combine(ArchiveFolder, fileName);

            File.Move(filePath, archiveFile);
            using ( File.Create(filePath));

            string[] folders = fileName.Split(System.IO.Path.DirectorySeparatorChar);

            int length = folders.Length;

            DirectoryLayout tempDir = null;

            for (int i = 0; i < length; i++)
            {
                if (i == length - 1 && tempDir != null)
                {
                    FileInfo tempFile = tempDir.Files.FirstOrDefault(x => x.Name == folders[i]);

                    tempFile.Size = 0;
                }

                if (tempDir == null)
                {
                    tempDir = BaseDirectoryLayout.Folders.FirstOrDefault(x => x.DirectoryName == folders[i]);
                }
                else
                {
                    tempDir = tempDir.Folders.FirstOrDefault(x => x.DirectoryName == folders[i]);
                }
            }

            TopDirectoryViewModel.UpdateDirectories(BaseDirectoryLayout);
            tvDirectories.DataContext = TopDirectoryViewModel;

            tvDirectories.Items.Refresh();
            tvDirectories.UpdateLayout();
        }

        private void UnArchiveFile(string filePath)
        {
            string fileName = filePath.Replace(MediaFolder, string.Empty).TrimStart('\\');
            string archiveFile = System.IO.Path.Combine(ArchiveFolder, fileName);

            File.Delete(filePath);
            File.Move(archiveFile, filePath);

            string[] folders = fileName.Split(System.IO.Path.DirectorySeparatorChar);

            int length = folders.Length;

            DirectoryLayout tempDir = null;

            for (int i = 0; i < length; i++)
            {
                if (i == length - 1 && tempDir != null)
                {
                    FileInfo tempFile = tempDir.Files.FirstOrDefault(x => x.Name == folders[i]);

                    tempFile.Size = 9;
                }

                if (tempDir == null)
                {
                    tempDir = BaseDirectoryLayout.Folders.FirstOrDefault(x => x.DirectoryName == folders[i]);
                }
                else
                {
                    tempDir = tempDir.Folders.FirstOrDefault(x => x.DirectoryName == folders[i]);
                }
            }

            TopDirectoryViewModel.UpdateDirectories(BaseDirectoryLayout);
            tvDirectories.DataContext = TopDirectoryViewModel;

            tvDirectories.Items.Refresh();
            tvDirectories.UpdateLayout();
        }

        private void LoadTreeView(BaseDirectoryLayout baseDirectoryLayout)
        {
            TopDirectoryViewModel = new TopDirectoryViewModel(baseDirectoryLayout);

            tvDirectories.DataContext = TopDirectoryViewModel;
        }

        private DirectoryLayout LoadDirectories(string directory)
        {
            DirectoryLayout directoryLayout = new DirectoryLayout
            {
                DirectoryPath = directory
            };

            if (Directory.Exists(directory))
            {
                if (Directory.GetDirectories(directory).Length != 0)
                {
                    foreach (string directories in Directory.GetDirectories(directory))
                    {
                        directoryLayout.Folders.Add(LoadDirectories(directories));
                    }
                }
                else
                {
                    foreach (string file in Directory.GetFiles(directory))
                    {
                        FileInfo fileInfo = new FileInfo
                        {
                            Path = file,
                            Size = new System.IO.FileInfo(file).Length
                        };

                        directoryLayout.Files.Add(fileInfo);
                    }
                }
            }

            return directoryLayout;
        }

        private void cmdLoadMediaFolder_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK || result == System.Windows.Forms.DialogResult.Yes)
                {
                    txtMediaFolder.Text = dialog.SelectedPath;
                }
            }
        }

        private void cmdLoadArchiveFolder_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK || result == System.Windows.Forms.DialogResult.Yes)
                {
                    txtArchiveFolder.Text = dialog.SelectedPath;
                }
            }
        }
    }
}
