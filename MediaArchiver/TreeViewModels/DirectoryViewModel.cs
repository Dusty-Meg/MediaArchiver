﻿using System.Linq;
using MediaArchiver.DataModels;

namespace MediaArchiver.TreeViewModels
{
    public class DirectoryViewModel : TreeViewItemViewModel
    {
        private readonly DirectoryLayout _directoryLayout;

        public DirectoryViewModel(DirectoryLayout directoryLayout, BaseDirectoryViewModel parentModel) : base(parentModel, true)
        {
            _directoryLayout = directoryLayout;
        }

        public DirectoryViewModel(DirectoryLayout directoryLayout, DirectoryViewModel parentModel) : base(parentModel, true)
        {
            _directoryLayout = directoryLayout;
        }

        public string DirectoryName
        {
            get { return _directoryLayout.DirectoryPath; }
        }

        protected override void LoadChildren()
        {
            foreach (DirectoryLayout directoryLayoutFolder in _directoryLayout.Folders)
            {
                base.Children.Add(new DirectoryViewModel(directoryLayoutFolder, this));
            }

            foreach (FileInfo directoryLayoutFile in _directoryLayout.Files)
            {
                Children.Add(new FileViewModel(directoryLayoutFile, this));
            }
        }

        public bool IsDirectoryArchived
        {
            get
            {
                bool directoryChildrenArchived = _directoryLayout.Folders.All(x => x.IsDirectoryArchived);
                bool fileChildrenArchived = _directoryLayout.Files.All(x => x.IsArchived);

                return directoryChildrenArchived && fileChildrenArchived;
            }
        }

        public string FileArchiveButtonText
        {
            get { return IsDirectoryArchived ? "Un-Archive Season" : "Archive Season"; }
        }
    }
}