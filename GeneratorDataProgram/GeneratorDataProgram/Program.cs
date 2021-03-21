using CommonClasses;
using CommonClasses.CryptHash;
using CommonClasses.Helpers;
using CommonClasses.Removes;
using CommonClasses.Serializers;
using GeneratorDataProgram.Helpers;
using GeneratorDataProgram.Models;
using System;
using System.IO;
using System.Text;
using System.Linq;
using Updater.Models.Serializers;
using System.Threading;
using System.Text.RegularExpressions;

namespace GeneratorDataProgram
{
    class Program
    {

        private enum ProgramId
        {
            Error = 0,
            Launcher = 1,
            Program = 2
        }

        static void Main(string[] args)
        {

            var programId = (int)ProgramId.Error;
            string nameZip = null;
            Console.WriteLine("Программа создает настройки для программы");
            do
            {
                Console.Write("Для какой программы генерируются настройки(1-Лаунчер,2-Программа): ");
                int.TryParse(Console.ReadLine(), out programId);
                if (programId != (int)ProgramId.Launcher && programId != (int)ProgramId.Program)
                {
                    Console.WriteLine("Ошибка, введите число 1 или 2");
                }
            }
            while (programId != (int)ProgramId.Launcher && programId != (int)ProgramId.Program);

            if (programId == (int)ProgramId.Launcher)
            {
                var setHelp = new SettingHelper();
                var parserPath = new ParserPathHelper();
                var setLauncher = new LauncherSettingSerializer();
                setLauncher.Info = new SettingSerializer();

                Console.WriteLine("Создается файл настроек для Лаунчера");
                Console.Write("Name = ");
                setLauncher.Info.Name = Console.ReadLine();
                Console.Write("Version = ");
                setLauncher.Info.Version = Console.ReadLine();
                Console.Write("StartApp = ");
                setLauncher.Info.StartApp = Console.ReadLine();
                var isValid = false;
                do
                {
                    try
                    {
                        Console.Write("DepId(0-Лаунчер) = ");
                        setLauncher.Info.Dep = int.Parse(Console.ReadLine());
                        isValid = true;
                    }
                    catch (ArgumentNullException)
                    {
                        isValid = false;
                    }
                    catch (FormatException)
                    {
                        isValid = false;
                    }
                    catch (OverflowException)
                    {
                        isValid = false;
                    }
                    if (!isValid)
                    {
                        Console.WriteLine("Введите целое число");
                    }
                }
                while (!isValid);

                Console.WriteLine("Следующие данные вводить в формате {\"Paths\": [\"path\", ...]}, при этом экранирование вводить вручную\nЕсли нечего вводить то просто Enter");

                do
                {
                    isValid = false;
                    Console.Write("PathFiles = ");
                    try
                    {
                        setLauncher.Info.PathFiles = parserPath.ParseToObj<Files>(Console.ReadLine());
                        isValid = true;
                    }
                    catch (ArgumentException)
                    {
                        isValid = false;
                    }
                    if (!isValid)
                    {
                        Console.WriteLine("Неверный формат введеных данных");
                    }
                }
                while (!isValid);

                do
                {
                    isValid = false;
                    Console.Write("PathDirectories = ");
                    try
                    {
                        setLauncher.Info.PathDirectories = parserPath.ParseToObj<Directories>(Console.ReadLine());
                        isValid = true;
                    }
                    catch (ArgumentException)
                    {
                        isValid = false;
                    }
                    if (!isValid)
                    {
                        Console.WriteLine("Неверный формат введеных данных");
                    }
                }
                while (!isValid);


                do
                {
                    isValid = false;
                    Console.Write("PathRegistries = ");
                    try
                    {
                        setLauncher.Info.PathRegistryKeys = parserPath.ParseToObj<Registries>(Console.ReadLine());
                        isValid = true;
                    }
                    catch (ArgumentException)
                    {
                        isValid = false;
                    }
                    if (!isValid)
                    {
                        Console.WriteLine("Неверный формат введеных данных");
                    }
                }
                while (!isValid);

                setHelp.Write(setLauncher, CommonConstant.KeySettings, Constant.Path + "\\" + CommonConstant.FileSettingsLauncher);

                nameZip = Regex.Replace(setLauncher.Info.StartApp, @"[.]\w+", "");

            }
            else
            {

                var setHelp = new SettingHelper();
                var parserPath = new ParserPathHelper();
                var setProgram = new SettingSerializer();

                Console.WriteLine("Создается файл настроек для Программы");
                Console.Write("Name = ");
                setProgram.Name = Console.ReadLine();
                Console.Write("Version = ");
                setProgram.Version = Console.ReadLine();
                Console.Write("StartApp = ");
                setProgram.StartApp = Console.ReadLine();
                var isValid = false;
                do
                {
                    try
                    {
                        Console.Write("DepId(0-Лаунчер) = ");
                        setProgram.Dep = int.Parse(Console.ReadLine());
                        isValid = true;
                    }
                    catch (ArgumentNullException)
                    {
                        isValid = false;
                    }
                    catch (FormatException)
                    {
                        isValid = false;
                    }
                    catch (OverflowException)
                    {
                        isValid = false;
                    }
                    if (!isValid)
                    {
                        Console.WriteLine("Введите целое число");
                    }
                }
                while (!isValid);

                Console.WriteLine("Следующие данные вводить в формате {\"Paths\": [\"path\", ...]}, при этом экранирование вводить вручную\nЕсли нечего вводить то просто Enter");
                do
                {
                    isValid = false;
                    Console.Write("PathFiles = ");
                    try
                    {
                        setProgram.PathFiles = parserPath.ParseToObj<Files>(Console.ReadLine());
                        isValid = true;
                    }
                    catch (ArgumentException)
                    {
                        isValid = false;
                    }
                    if (!isValid)
                    {
                        Console.WriteLine("Неверный формат введеных данных");
                    }
                }
                while (!isValid);

                do
                {
                    isValid = false;
                    Console.Write("PathDirectories = ");
                    try
                    {
                        setProgram.PathDirectories = parserPath.ParseToObj<Directories>(Console.ReadLine());
                        isValid = true;
                    }
                    catch (ArgumentException)
                    {
                        isValid = false;
                    }
                    if (!isValid)
                    {
                        Console.WriteLine("Неверный формат введеных данных");
                    }
                }
                while (!isValid);


                do
                {
                    isValid = false;
                    Console.Write("PathRegistries = ");
                    try
                    {
                        setProgram.PathRegistryKeys = parserPath.ParseToObj<Registries>(Console.ReadLine());
                        isValid = true;
                    }
                    catch (ArgumentException)
                    {
                        isValid = false;
                    }
                    if (!isValid)
                    {
                        Console.WriteLine("Неверный формат введеных данных");
                    }
                }
                while (!isValid);

                setHelp.Write(setProgram, CommonConstant.KeySettings, Constant.Path + "\\" + CommonConstant.FileSettingsLauncher);

                nameZip = Regex.Replace(setProgram.StartApp, @"[.]\w+", "");
            }

            var onlyFileSetting = -1;
           
            do
            {
                Console.Write("Создать только файл настроек?(Да-1,Нет-2): ");
                int.TryParse(Console.ReadLine(), out onlyFileSetting);
                if (onlyFileSetting != 1 && onlyFileSetting != 2)
                {
                    Console.WriteLine("Ошибка, введите число 1 или 2");
                }
            }
            while (onlyFileSetting != 1 && onlyFileSetting != 2);

            if (onlyFileSetting == 1)
            {
                return;
            }

            var zip = new ArchiveHelper();
            zip.CreateZipFromPath(Constant.Path, nameZip);

            var bytesSZip = File.ReadAllBytes(nameZip + ".zip");

            var crypt1 = new Crypt(Encoding.UTF8.GetBytes(CommonConstant.Key));
            var res1 = crypt1.Encode(bytesSZip);

            crypt1.AddToDataIv(ref res1);

            

            using (var file = File.Create(nameZip + ".zip"))
            {
                file.Write(res1, 0, res1.Length);
            }

            Console.WriteLine("Архив успешно создан");

            Directory.Delete(Constant.Path, true);

            while(Directory.Exists(Constant.Path))
            {
                Thread.Sleep(100);
            }

            Directory.CreateDirectory(Constant.Path);

            while (!Directory.Exists(Constant.Path))
            {
                Thread.Sleep(100);
            }

            File.Move(nameZip + ".zip", Constant.Path + "\\" + nameZip + ".zip");

            while (!File.Exists(Constant.Path + "\\" + nameZip + ".zip"))
            {
                Thread.Sleep(100);
            }
        }
    }
}
