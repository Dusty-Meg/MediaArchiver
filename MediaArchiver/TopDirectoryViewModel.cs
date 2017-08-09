using System.Collections.ObjectModel;
using System.Linq;

namespace MediaArchiver
{
    public class TopDirectoryViewModel
    {
        //private readonly BaseDirectoryLayout _directoryLayout;
        private ReadOnlyCollection<BaseDirectoryViewModel> _directories;

        public TopDirectoryViewModel(BaseDirectoryLayout directoryLayout)
        {
            _directories = new ReadOnlyCollection<BaseDirectoryViewModel>((from directory in directoryLayout.Folders select new BaseDirectoryViewModel(directory)).ToList());
        }

        public void UpdateDirectories(BaseDirectoryLayout directoryLayout)
        {
            _directories = new ReadOnlyCollection<BaseDirectoryViewModel>((from directory in directoryLayout.Folders select new BaseDirectoryViewModel(directory)).ToList());
        }

        public ReadOnlyCollection<BaseDirectoryViewModel> Directories
        {
            get { return _directories; }
        }

        public string DirectoryName
        {
            get { return "Base"; }
        }
    }
}