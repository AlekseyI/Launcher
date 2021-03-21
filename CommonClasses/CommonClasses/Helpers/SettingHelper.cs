using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonClasses.Parsers;
using CommonClasses.CryptHash;
using Updater.Models.Serializers;
using CommonClasses.Serializers;

namespace CommonClasses.Helpers
{
    public class SettingHelper
    {

        public void Write<T>(T data, string key, string path) where T : class
        {
            var json = new JsonParser();
            var crypt = new Crypt(Encoding.UTF8.GetBytes(key));
            var res = crypt.Encode(Encoding.UTF8.GetBytes(json.ParseObjToStr(data)));

            crypt.AddToDataIv(ref res);

            using (var file = File.Create(path))
            {
                file.Write(res, 0, res.Length);
            }
        }

        public T Read<T>(string key, string path) where T : class
        {
            var data = File.ReadAllBytes(path);
            var crypt = new Crypt(Encoding.UTF8.GetBytes(key));
            crypt.RemoveAndSetIv(ref data);

            var json = new JsonParser();
            return json.ParseStrToObj<T>(Encoding.UTF8.GetString(crypt.Decode(data)));
        }

        public T Read<T>(byte[] data, string key) where T : class
        {
            var crypt = new Crypt(Encoding.UTF8.GetBytes(key));
            crypt.RemoveAndSetIv(ref data);

            var json = new JsonParser();
            return json.ParseStrToObj<T>(Encoding.UTF8.GetString(crypt.Decode(data)));
        }

        // Удаление лаунчера
        public T RemoveLauncher<T>(string path, string fileSettings, string key) where T : class
        {
            var setHelper = new SettingHelper();
            var files = Directory.GetFiles(path);
            T lastSet = null;
            if (!files.Any((v) => v.Contains(fileSettings)))
            {
                return lastSet;
            }
            foreach (var file in files)
            {
                if (file.Contains(fileSettings))
                {
                    lastSet = Read<T>(path + fileSettings, key);
                }
                else
                {
                    File.Delete(file);
                }
            }
            return lastSet;
        }

        public void RemoveProgram(string path,  bool withDirectory = false)
        {
            if (withDirectory)
            {
                Directory.Delete(path, true);
            }
            else
            {
                 Directory.GetFiles(path).ToList().ForEach(f => File.Delete(f));
            }
        }

        public void CopyFilesProgram(string pathFrom, string pathTo, bool withNestedDirectories=false)
        {
            if (withNestedDirectories)
            {
                Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(pathFrom, pathTo, withNestedDirectories);
            }
            else
            {
                new DirectoryInfo(pathFrom).GetFiles().ToList().ForEach(f => f.CopyTo(f.Name, true));
            }
        }

        public void RemoveProgram(string path, string[] filesNoDeleted=null)
        {
            if (filesNoDeleted != null && filesNoDeleted.Length > 0)
            {
                Directory.GetFiles(path).Where(f => filesNoDeleted.Any(d => !f.Contains(d))).ToList().ForEach(f => File.Delete(f));
            }
            else
            {
                Directory.GetFiles(path).ToList().ForEach(f => File.Delete(f));
            }
                
            
        }

        // Зачистка мусора указанного в массивах путей
        public void RemoveDumps<T, V>(T data) where T : class where V : class
        {
            typeof(T).GetProperties().SkipWhile((v) => !typeof(V).IsAssignableFrom(v.PropertyType)).Select(v => (dynamic)(V)v.GetValue(data)).Where(v => v != null).ToList().ForEach(
                v =>
                {
                    v.Remove();
                });
        }

        public void RewriteLastToNewSettingsLauncher(LauncherSettingSerializer thisSet, LauncherSettingLastSerializer lastSet, string key, string path)
        { 
            if (lastSet.InfoInstallPrograms != null)
            {
                var progs = new List<SettingSerializer>(lastSet.InfoInstallPrograms.Count());
                foreach (var prog in lastSet.InfoInstallPrograms)
                {
                    progs.Add(new SettingSerializer()
                    {
                        Name = prog.Name,
                        Dep = prog.Dep,
                        Version = prog.Version,
                        Path = prog.Path,
                        StartApp = prog.StartApp,
                        PathFiles = prog.PathFiles,
                        PathDirectories = prog.PathDirectories,
                        PathRegistryKeys = prog.PathRegistryKeys
                    });
                }
                thisSet.InfoInstallPrograms = progs;
            }
            
           Write(thisSet, key, path);
        }
    }
}
