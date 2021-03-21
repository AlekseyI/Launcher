using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using System.IO;
using System.Threading;

namespace CommonClasses.Helpers
{
    public class ArchiveHelper
    {
        public byte[] Extract(byte[] data, string dirName, string pathToDir = null, string fileSetting = "settings.dat")
        {

            if (dirName == null)
            {
                throw new ArgumentNullException(nameof(dirName));
            }

            string path = pathToDir != null ? pathToDir + dirName : dirName;

            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);

                while (Directory.Exists(path))
                {
                    Thread.Sleep(100);
                }
            }

            Directory.CreateDirectory(path);


            while (!Directory.Exists(path))
            {
                Thread.Sleep(100);
            }

            using (var zip = File.Create(path + "\\" + dirName + ".zip"))
            {
                zip.Write(data, 0, data.Length);
            }

            ZipFile.ExtractToDirectory(path + "\\" + dirName + ".zip", path);
            

            File.Delete(path + "\\" + dirName + ".zip");

            var bytesSetting = File.ReadAllBytes(path + "\\" + fileSetting);

            File.Delete(path + "\\" + fileSetting);

            while (File.Exists(path + "\\" + fileSetting))
            {
                Thread.Sleep(100);
            }

            return bytesSetting;
        }

        public void CreateZipFromPath(string path, string pathZip)
        {
            ZipFile.CreateFromDirectory(path, pathZip + ".zip");
        }
    }
}
