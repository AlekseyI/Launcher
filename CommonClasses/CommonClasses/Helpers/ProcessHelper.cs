using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Threading;
using System.Security.Policy;

namespace CommonClasses.Helpers
{
    // пример запуска программы
    // одну и ту же программу запустить две копии не получится, т.к. название доменов name должны отличаться AppDomain.CreateDomain(name,...)
    /*
      using (var proc = new ProcessHelper())
       {
              var res = proc.Open("MVVM_OpenNewWindowMinimalExample.exe");

               if (res != OpenProc.Error.No)
                    {
                                    MessageBox.Show(proc.Description[res]);
                      }
                         }
    */
    public class ProcessHelper : IDisposable
    {

        private bool disposed = false;
        private byte[] _file;
        private Assembly _assembly;
        private AppDomain _domain;
        private AppDomainSetup _domainInfo;


        public enum Error
        {
            No,
            Path,
            Name,
            LoadingFile
        }

        public Dictionary<Error, string> Description { get; private set; }

        public ProcessHelper()
        {
            Description = new Dictionary<Error, string>(4);
            Description.Add(Error.No, "Все норм");
            Description.Add(Error.Path, "Неправильный путь к программе");
            Description.Add(Error.Name, "Неправильное название программы");
            Description.Add(Error.LoadingFile, "Ошибка при запуске программы");
        }

        public void CloseWindowProcess(int pid)
        {
            Process.GetProcessById(pid).CloseMainWindow();
            Thread.Sleep(200);
        }

        public void CloseProcess(int pid)
        {
            Process.GetProcessById(pid).Kill();
            Thread.Sleep(200);
        }

        public int GetPidProcess()
        {
            return Process.GetCurrentProcess().Id;
        }

        public bool CheckProcessByName(string name)
        {
            return Process.GetProcessesByName(name).Length > 0;
        }

        public void CloseWindowProcess(string name)
        {
            Process.GetProcessesByName(name).ToList().ForEach(p =>
            {
                p.CloseMainWindow();
                Thread.Sleep(200);
            });

        }

        public void CloseProcess(string name)
        {
            Process.GetProcessesByName(name).ToList().ForEach(p =>
            {
                p.Kill();
                Thread.Sleep(200);
            });

        }

        public Error Open(string path, bool isWait, bool isOne = true)
        {
            try
            {
                string name = Path.GetFileName(path).Replace(Path.GetExtension(path), "");
                if (Process.GetProcessesByName(name).Length == 0)
                {
                    Process pr = new Process();
                    pr.StartInfo.FileName = path;
                    pr.Start();
                    if (isWait)
                        pr.WaitForExit();
                }
            }
            catch (FileNotFoundException)
            {
                return Error.Path;
            }
            catch (DirectoryNotFoundException)
            {
                return Error.Path;
            }
            return Error.No;
        }

        // Название исполняемого файла должно совпадать с именем сборки
        public Error Open(string path, Func<byte[], byte[]> Transform = null)
        {
            try
            {
                _file = File.ReadAllBytes(path);
                if (Transform != null)
                {
                    _file = Transform(_file);
                }

                _assembly = Assembly.Load(_file);
                var nameDomain = _assembly.GetName().Name;
                _domainInfo = new AppDomainSetup();
                _domainInfo.ApplicationBase = Path.GetDirectoryName(path);
                _domainInfo.ApplicationName = Path.GetFileName(path);
                _domain = AppDomain.CreateDomain(nameDomain, null, _domainInfo);
                _domain.ExecuteAssemblyByName(nameDomain);
                AppDomain.Unload(_domain);

            }
            catch (FileNotFoundException)
            {
                return Error.Path;
            }
            catch (DirectoryNotFoundException)
            {
                return Error.Path;
            }
            catch (FileLoadException)
            {
                return Error.LoadingFile;
            }

            return Error.No;
        }

        public int StartProcess(string path, string[] args = null, bool isWait = false)
        {
            Process pr = new Process();
            pr.StartInfo.FileName = path;
            if (args != null && args.Length > 0)
            {
                pr.StartInfo.Arguments = args.Aggregate((b, n) => " " + b + " " + n).TrimStart(' ');
            }

            pr.Start();
            if (isWait)
            {
                pr.WaitForExit();
            }

            return pr != null ? pr.Id : -1;
        }

        public string GetPublicKeyTokenFromAssembly(Assembly assembly)
        {
            var bytes = assembly.GetName().GetPublicKeyToken();
            if (bytes == null || bytes.Length == 0)
                return "";

            var publicKeyToken = string.Empty;
            for (int i = 0; i < bytes.GetLength(0); i++)
                publicKeyToken += string.Format("{0:x2}", bytes[i]);

            return publicKeyToken;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _domain = null;
                    _assembly = null;
                    _domainInfo = null;
                    _file = null;
                    Description = null;
                }
                disposed = true;
            }
        }

        ~ProcessHelper()
        {
            Dispose(false);
        }

    }
}
