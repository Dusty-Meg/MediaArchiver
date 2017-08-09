using TreeViewWithViewModelDemo.LoadOnDemand;

namespace MediaArchiver
{
    public class FileViewModel : TreeViewItemViewModel
    {
        private readonly FileInfo _fileInfo;

        public FileViewModel(FileInfo fileInfo, BaseDirectoryViewModel parentModel) : base(parentModel, true)
        {
            _fileInfo = fileInfo;
        }

        public FileViewModel(FileInfo fileInfo, DirectoryViewModel parentModel) : base(parentModel, true)
        {
            _fileInfo = fileInfo;
        }

        public string FilePath
        {
            get { return _fileInfo.Path; }
        }

        public string FileArchived
        {
            get { return _fileInfo.IsArchived ? "Archived" : "Not Archived"; }
        }

        public string FileArchiveButtonText
        {
            get { return _fileInfo.IsArchived ? "Un-Archive File" : "Archive File"; }
        }

        public bool IsArchived
        {
            get { return _fileInfo.IsArchived; }
        }
    }
}