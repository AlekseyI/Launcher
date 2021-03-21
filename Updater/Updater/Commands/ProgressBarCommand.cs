using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using CommonClasses.Parsers;
using CommonClasses.Request;
using Updater.Models.Serializers;
using Updater.ViewModels;
using CommonClasses.Helpers;
using Updater.Models;
using CommonClasses.CryptHash;
using CommonClasses.Serializers;
using CommonClasses.Removes;
using CommonClasses;

namespace Updater.Commands
{
    public class ProgressBarCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private DataMainViewModel _dataMainViewModel;

        public ProgressBarCommand(DataMainViewModel dataMainViewModel)
        {
            _dataMainViewModel = dataMainViewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var displayRootRegistry = (Application.Current as App).displayRootRegistry;
            var dataUpdater = new DataUpdaterViewModel();
            RequestData(displayRootRegistry, dataUpdater);
        }

        private async void RequestData(DisplayRootRegistry displayRootRegistry, DataUpdaterViewModel dataUpdater)
        {
            var net = new Net();
            var dataLauncher = new LauncherInfoSerializer();
            var proc = new ProcessHelper();
            
            var settingHelp = new SettingHelper();
            LauncherSettingLastSerializer lastSettingLauncher = null;

        
            try
            {
                lastSettingLauncher = settingHelp.Read<LauncherSettingLastSerializer>(CommonConstant.KeySettings, CommonConstant.FileSettingsLauncher);
                dataLauncher.version = lastSettingLauncher.Info.Version;
            }
            catch (Exception ex)
            {
                if (ex is DirectoryNotFoundException || ex is FileNotFoundException)
                {
                    MessageBox.Show(CommonConstant.NotFoundFileSettings);
                }
                else
                {
                    MessageBox.Show(CommonConstant.ErrorReadOrWriteSettings);
                }
                proc.CloseProcess(Constant.PidLauncher);
                Application.Current.Shutdown();
                return;
            }


           
            var programLauncher = await net.RequestAsync(Constant.UrlUpdaterLauncherCheckVersion, Net.Post, dataLauncher);
           
            if (programLauncher.detail != null)
            {
                MessageBox.Show(ErrorParser.Parse(programLauncher.detail));
                return;
            }

            if (!programLauncher.is_updated)
            {
                var message = MessageBox.Show(Constant.DescriptionMessBoxUpdate, "Обновление", MessageBoxButton.OKCancel);
                if (message == MessageBoxResult.Cancel)
                {
                    return;
                }


                _dataMainViewModel.TimerUpdater.Stop();
           
                displayRootRegistry.ShowPresentation(dataUpdater);
               
                programLauncher = await net.RequestAsync<LauncherInfoSerializer>(
                      Constant.UrlUpdaterLauncherInfo,
                      Net.Post,
                      null,
                      null,
                      null,
                      (s, e) =>
                      {
                          dataUpdater.ProgressValue = (e.BytesReceived * 100 / e.TotalBytesToReceive);

                      });
                if (programLauncher.detail != null)
                {
                    displayRootRegistry.HidePresentation(dataUpdater);
                    MessageBox.Show(ErrorParser.Parse(programLauncher.detail));
                    return;
                }

                var cryptBytesLauncher = Convert.FromBase64String(programLauncher.data);

             
                if (Hash.Sha256Bytes(cryptBytesLauncher) == programLauncher.hash)
                {

                    var crypt = new Crypt(Encoding.UTF8.GetBytes(CommonConstant.Key));
                    crypt.RemoveAndSetIv(ref cryptBytesLauncher);

                    var bytesLauncher = crypt.Decode(cryptBytesLauncher);
       

                    proc.CloseProcess(Constant.PidLauncher);

                
                    var zip = new ArchiveHelper();

                    var bytesLauncherSetting = zip.Extract(bytesLauncher, programLauncher.hash, Path.GetTempPath());

                   

                    settingHelp.RemoveProgram(@".\", false);

                    settingHelp.RemoveDumps<SettingLastSerializer, IRemoveData>(lastSettingLauncher.Info);

                   

                    settingHelp.CopyFilesProgram(Path.GetTempPath() + programLauncher.hash, ".\\");

                    var getLauncherSetting = settingHelp.Read<LauncherSettingSerializer>(bytesLauncherSetting, CommonConstant.KeySettings);

                    

                    settingHelp.RewriteLastToNewSettingsLauncher(getLauncherSetting, lastSettingLauncher, CommonConstant.KeySettings, CommonConstant.FileSettingsLauncher);

                    displayRootRegistry.HidePresentation(dataUpdater);

                 
                    try
                    {
                        proc.StartProcess(getLauncherSetting.Info.StartApp, new string[1] { programLauncher.hash });
                    }
                     catch(System.ComponentModel.Win32Exception)
                    {
                        MessageBox.Show(Constant.NotFoundStartAppLauncher);
                    }

                    Application.Current.Shutdown();
                    return;

                }
                else
                {
                    MessageBox.Show(Constant.NoHashEqual);
                }

               
                displayRootRegistry.HidePresentation(dataUpdater);

                _dataMainViewModel.TimerUpdater.Start();
            }
            
        }

    }
}
