using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZPF.XF
{
    public static class FileIO
    {
        public static string GetDataDirectory()
        {
            return FileSystem.AppDataDirectory + @"\"; ;
        }

        public static string CleanPath(string path)
        {
            if (path == null) return null;

            if (!path.ToUpper().StartsWith(GetDataDirectory().ToUpper()) && path[1] != ':')
            {
                path = GetDataDirectory() + @"\" + path;
            };

            path = path.Replace(@"\\", @"\").Replace(@"/", @"\");

            return path;
        }

        public static bool WriteStream(Stream outputStream, string fileName)
        {
            throw new NotImplementedException();
        }
    }
}
