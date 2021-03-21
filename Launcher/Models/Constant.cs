using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonClasses;

namespace Launcher.Models
{
    public class Constant
    {


        public static int PidUpdater {get; set; }
        public const string NoHashEqual = "Программа повреждена";
        public const string NotFoundProgram = "Исполняемый файл программы не найден";
        public const string DescriptionMessBoxNotFoundSettings = "Файл настроек не найден";
        public const string Ok = "Все норм";
        public const string NotFoundUpdater = "Апдейтор не найден";
        public const string CrashLauncher = "Лаунчер поврежден";
        public const string AlreadyStartLauncher = "Лаунчер уже запущен";
        public const string NotFoundInstallProgram = "Такой программы нет в списке установленных";
        public const string NotFoundFileOrDirectoryWithProgram = "Некоторые файлы или папки не были найдены";


        public static string UrlLogin
        {
            get
            {
                if (string.IsNullOrEmpty(CommonConstant.Server))
                    throw new ArgumentException(nameof(CommonConstant.Server));
                return CommonConstant.Server + CommonConstant.UrlLogin;
            }
        }

       

        public static string UrlCheckVersionPrograms
        {
            get
            {
                if (string.IsNullOrEmpty(CommonConstant.Server))
                    throw new ArgumentException(nameof(CommonConstant.Server));
                return CommonConstant.Server + CommonConstant.UrlCheckVersionPrograms;
            }
        }

        public static string UrlGetProgram
        {
            get
            {
                if (string.IsNullOrEmpty(CommonConstant.Server))
                    throw new ArgumentException(nameof(CommonConstant.Server));
                return CommonConstant.Server + CommonConstant.UrlGetProgram;
            }
        }
    }
}
