using CommonClasses;
using CommonClasses.CryptHash;
using CommonClasses.Helpers;
using CommonClasses.Parsers;
using CommonClasses.Removes;
using CommonClasses.Request;
using CommonClasses.Serializers;
using Launcher.Models;
using Launcher.Models.Helpers;
using Launcher.Models.Serializers;
using Launcher.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Updater.Models.Serializers;

namespace Launcher.Commands
{
    public class DownloadInstallProgramCommand : ICommand
    {

        private CollectionsDataViewModel _mainWindowVeiwModel;

        public DownloadInstallProgramCommand(CollectionsDataViewModel mainWindowVeiwModel)
        {
            _mainWindowVeiwModel = mainWindowVeiwModel;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {           
            DownloadInstallProgram();
            
        }

        private async void DownloadInstallProgram()
        {
            _mainWindowVeiwModel.IsEnabledTable = false;
            _mainWindowVeiwModel.CheckProgramTimer.Stop();
            var selectedRow = _mainWindowVeiwModel.SelectedRow;

            var programInfo = new ProgramInfoSerializer();

            var net = new Net();

            var setHelp = new SettingHelper();
            LauncherSettingSerializer launcherSet = null;
            try
            {
                launcherSet = setHelp.Read<LauncherSettingSerializer>(CommonConstant.KeySettings, CommonConstant.FileSettingsLauncher);

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
                Application.Current.Shutdown();
                return;
            }

            programInfo.dep = selectedRow.Dep;

         
            var request = await net.RequestAsync(
                  Constant.UrlGetProgram,
                  Net.Post,
                  programInfo,
                  _mainWindowVeiwModel.Auth,
                  null,
                  (s, e) =>
                  {
                      _mainWindowVeiwModel.ProgressValue = e.BytesReceived * 100 / e.TotalBytesToReceive;
                  });

            if (request.detail != null)
            {
                _mainWindowVeiwModel.CheckProgramTimer.Start();
                MessageBox.Show(ErrorParser.Parse(request.detail));
                _mainWindowVeiwModel.IsEnabledTable = true;
                return;
            }

            var cryptBytesProgram = Convert.FromBase64String(request.data);

        
            if (Hash.Sha256Bytes(cryptBytesProgram) == request.hash)
            {

                var crypt = new Crypt(Encoding.UTF8.GetBytes(CommonConstant.Key));
                crypt.RemoveAndSetIv(ref cryptBytesProgram);

                var bytesProgram = crypt.Decode(cryptBytesProgram);

            
                var zip = new ArchiveHelper();

                var bytesProgramSetting = zip.Extract(bytesProgram, request.hash);         


                var getProgramSetting = setHelp.Read<SettingSerializer>(bytesProgramSetting, CommonConstant.KeySettings);

                getProgramSetting.Path = request.hash;

             
                if (launcherSet.InfoInstallPrograms == null || launcherSet.InfoInstallPrograms?.Count() == 0)
                {
                    launcherSet.InfoInstallPrograms = new List<SettingSerializer>() { getProgramSetting };
                    
                }            
                else 
                {
                    (launcherSet.InfoInstallPrograms as List<SettingSerializer>).Add(getProgramSetting);
                }


                setHelp.Write(launcherSet, CommonConstant.KeySettings, CommonConstant.FileSettingsLauncher);

                _mainWindowVeiwModel.SelectedRow.SetStateAfterDownloadAndInstall();
                _mainWindowVeiwModel.SelectedRow.Status = StatusHelper.InstallAndUpdated;
                _mainWindowVeiwModel.ProgressValue = 0;

            }
            else
            {
                MessageBox.Show(Constant.NoHashEqual);
            }
            _mainWindowVeiwModel.IsEnabledTable = true;
            _mainWindowVeiwModel.CheckProgramTimer.Start();
        }
    }
}
