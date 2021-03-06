﻿using System.Collections.Generic;
using System.Linq;

namespace MediaArchiver.DataModels
{
    public class DirectoryLayout
    {
        public DirectoryLayout()
        {
            Folders = new List<DirectoryLayout>();
            Files = new List<FileInfo>();
        }

        public FolderInfo FolderInfo { get; set; }

        public IList<DirectoryLayout> Folders { get; set; }

        public IList<FileInfo> Files { get; set; }

        public string DirectoryPath { get; set; }

        public string DirectoryName { get { return DirectoryPath.Split(System.IO.Path.DirectorySeparatorChar).LastOrDefault(); } }

        public bool IsDirectoryArchived
        {
            get
            {
                bool directoryChildrenArchived = Folders.All(x => x.IsDirectoryArchived);
                bool fileChildrenArchived = Files.All(x => x.IsArchived);

                return directoryChildrenArchived && fileChildrenArchived;
            }
        }
    }
}