using System.Linq;

namespace MediaArchiver
{
    public class FileInfo
    {
        public string Name { get { return Path.Split(System.IO.Path.DirectorySeparatorChar).LastOrDefault(); } }
        public string Path { get; set; }
        public long Size { get; set; }

        public bool IsArchived => Size == 0;
    }
}