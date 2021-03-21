using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Updater.Views;
using Updater.Models;
using Updater.ViewModels;
using System.Reflection;
using System.Threading;
using Updater.Models.Serializers;
using CommonClasses.Parsers;
using System.IO;
using Microsoft.Win32;
using CommonClasses.Request;
using System.Net;
using CommonClasses.Helpers;
using CommonClasses.Serializers;
using CommonClasses.Removes;
using CommonClasses;

namespace Updater
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public DisplayRootRegistry displayRootRegistry = new DisplayRootRegistry();
        private DataMainViewModel _mainWindowViewModel;
        private bool _instance;

        public App()
        {
            displayRootRegistry.RegisterWindowType<DataMainViewModel, MainWindow>();
            displayRootRegistry.RegisterWindowType<DataUpdaterViewModel, UpdaterWindow>();

        }


        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                var args = Environment.GetCommandLineArgs();
                Constant.PidLauncher = int.Parse(args[1]);
            }
            catch (Exception)
            {
                MessageBox.Show(Constant.ErrorOnStartUpdater);
                Shutdown();
                return;
            }

            var proc = new ProcessHelper();

            if (!File.Exists(CommonConstant.FileSettingsLauncher))
            {
                MessageBox.Show(CommonConstant.NotFoundFileSettings);
                Shutdown();
                return;
            }

            var token = proc.GetPublicKeyTokenFromAssembly(Assembly.GetCallingAssembly());
            if (string.IsNullOrEmpty(token))
            {
                MessageBox.Show(Constant.CrashUpdater);
                Shutdown();
                return;
            }

            var mut = new Mutex(true, token, out _instance);

            if (!_instance)
            {
                MessageBox.Show(Constant.AlreadyStartUpdater);
                Shutdown();
            }


            base.OnStartup(e);

            _mainWindowViewModel = new DataMainViewModel();

            displayRootRegistry.ShowPresentation(_mainWindowViewModel, false);

        }


    }
}
