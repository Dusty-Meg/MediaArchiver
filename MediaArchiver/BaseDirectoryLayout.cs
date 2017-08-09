using System.Collections.Generic;

namespace MediaArchiver
{
    public class BaseDirectoryLayout
    {
        public BaseDirectoryLayout()
        {
            Folders = new List<DirectoryLayout>();
        }

        public IList<DirectoryLayout> Folders { get; set; }
    }
}