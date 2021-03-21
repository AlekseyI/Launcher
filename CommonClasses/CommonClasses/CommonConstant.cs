using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonClasses
{
    public class CommonConstant
    {
        public static TimeSpan IntervalCheckExitStartProgram = new TimeSpan(0, 0, 5);
        public static TimeSpan IntervalRequestUpdateLauncher = new TimeSpan(0, 10, 0);
        public static TimeSpan IntervalRequestUpdateProgram = new TimeSpan(0, 1, 0);
        public static TimeSpan IntervalCheckRunningLauncher = new TimeSpan(0, 0, 1);
        public static TimeSpan IntervalCheckRunningUpdater = new TimeSpan(0, 0, 1);
        public const string NotFoundFileSettings = "Файл настроек не найден";
        public const string ErrorReadOrWriteSettings = "Ошибка при чтении/записи файла настроек";
        public const string KeySettings = "qo7dkpRKuaJR2?%xVNO32*nL~GD%gnmb";
        public const string Key = "j9RWrG!&%9xWyXkQndvL*x?6:p6W}W!v";
        public const string CommonClassesDll = "CommonClasses.dll";
        public const string NameAppUpdater = "LauncherUpdater";
        public const string StartAppUpdater = NameAppUpdater + ".exe";
        public const string FileSettingsLauncher = "settings.dat";
        public const string PathUpdater = @"Updater\";

        public const string Server = "http://147.135.206.176";

        // ССылки лаунчера
        public const string UrlLogin = "/api/login/";
        public const string UrlCheckVersionPrograms = "/api/check_version_programs/";
        public const string UrlGetProgram = "/api/get_program/";


        // Ссылки апдейтора
        public const string UrlUpdaterLauncherCheckVersion = "/api/check_version_launcher/";
        public const string UrlUpdaterLauncherInfo = "/api/program_launcher/";

    }
}
