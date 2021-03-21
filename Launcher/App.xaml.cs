using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Launcher.ViewModels;
using Launcher.Views;
using Launcher.Models;
using System.Threading;
using CommonClasses.Helpers;
using Updater.Models.Serializers;
using CommonClasses.Serializers;
using CommonClasses.Removes;
using CommonClasses;
using CommonClasses.CryptHash;
using System.Text;

namespace Launcher
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public DisplayRootRegistry DisplayRootRegistry = new DisplayRootRegistry();
        private CollectionsDataViewModel _mainWindowViewModel;
        private bool instance;

        public App()
        {
            DisplayRootRegistry.RegisterWindowType<CollectionsDataViewModel, MainWindow>();
            DisplayRootRegistry.RegisterWindowType<AuthViewModel, AuthDialogWindow>();

        }

        protected override async void OnStartup(StartupEventArgs e)
        {

            var proc = new ProcessHelper();
            var settingHelp = new SettingHelper();
            if (proc.CheckProcessByName(CommonConstant.NameAppUpdater))
            {
                proc.CloseProcess(CommonConstant.NameAppUpdater);
            }

            var getArgs = Environment.GetCommandLineArgs();
            try
            {
                if (getArgs.Length == 2)
                {
                   
                    settingHelp.RemoveProgram(CommonConstant.PathUpdater, true);                   
                    settingHelp.CopyFilesProgram(Path.GetTempPath() + getArgs[1] + "\\" + CommonConstant.PathUpdater, CommonConstant.PathUpdater, true);                  
                    settingHelp.RemoveProgram(Path.GetTempPath() + getArgs[1], true);
                }

                
            }
            catch (Exception)
            {
            }


            if (!File.Exists(CommonConstant.PathUpdater + CommonConstant.StartAppUpdater))
            {
                MessageBox.Show(Constant.NotFoundUpdater);
                Shutdown();
                return;
            }

            if (!File.Exists(CommonConstant.FileSettingsLauncher))
            {
                MessageBox.Show(Constant.DescriptionMessBoxNotFoundSettings);
                Shutdown();
                return;
            }

            var token = proc.GetPublicKeyTokenFromAssembly(Assembly.GetCallingAssembly());
            if (string.IsNullOrEmpty(token))
            {
                MessageBox.Show(Constant.CrashLauncher);
                Shutdown();
                return;
            }

            var mut = new Mutex(true, token, out instance);

            if (!instance)
            {
                MessageBox.Show(Constant.AlreadyStartLauncher);
                Shutdown();
                return;
            }

            var args = new string[1]
            {
                Process.GetCurrentProcess().Id.ToString()
            };

            var setHelp = new SettingHelper();
            var set = setHelp.Read<LauncherSettingSerializer>(CommonConstant.KeySettings, CommonConstant.FileSettingsLauncher);
           

            Constant.PidUpdater = proc.StartProcess(CommonConstant.PathUpdater + CommonConstant.StartAppUpdater, args);

            base.OnStartup(e);

            _mainWindowViewModel = new CollectionsDataViewModel();

            await DisplayRootRegistry.ShowModalPresentation(_mainWindowViewModel);

            Shutdown();
        }

      
    }
}
