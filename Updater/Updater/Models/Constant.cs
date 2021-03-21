using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonClasses;

namespace Updater.Models
{
    public class Constant
    {

        public static int PidLauncher { get; set; }
        public const string DescriptionMessBoxUpdate = "Обнаружена более новая версия лаунчера, обновиться?";
        public const string NoHashEqual = "Лаунчер был поврежден при скачивании";
        public const string CrashUpdater = "Апдейтор поврежден";
        public const string AlreadyStartUpdater = "Апдейтор уже запущен";
        public const string ErrorOnStartUpdater = "При запуске апдейтора возникла ошибка";
        public const string NotFoundStartAppLauncher = "Не найден исполняемый файл лаунчера";


        public static string UrlUpdaterLauncherCheckVersion
        {
            get
            {
                if (string.IsNullOrEmpty(CommonConstant.Server))
                    throw new ArgumentException(nameof(CommonConstant.Server));
                return CommonConstant.Server + CommonConstant.UrlUpdaterLauncherCheckVersion;
            }
        }

       

        public static string UrlUpdaterLauncherInfo
        {
            get
            {
                if (string.IsNullOrEmpty(CommonConstant.Server))
                    throw new ArgumentException(nameof(CommonConstant.Server));
                return CommonConstant.Server + CommonConstant.UrlUpdaterLauncherInfo;
            }
        }

      


    }
}
