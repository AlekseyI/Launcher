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
    public class UpdateProgramCommand : ICommand
    {

        private CollectionsDataViewModel _mainWindowVeiwModel;

        public UpdateProgramCommand(CollectionsDataViewModel mainWindowVeiwModel)
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
            UpdateProgram();
        }

        private async void UpdateProgram()
        {
            var programInfo = new ProgramInfoSerializer();
            _mainWindowVeiwModel.CheckProgramTimer.Stop();
            _mainWindowVeiwModel.IsEnabledTable = false;
            var net = new Net();

            var setHelp = new SettingHelper();
            SettingSerializer selectProgram = null;
            LauncherSettingSerializer launcherSet = null;
            try
            {
                launcherSet = setHelp.Read<LauncherSettingSerializer>(CommonConstant.KeySettings, CommonConstant.FileSettingsLauncher);

                selectProgram = launcherSet.InfoInstallPrograms?.FirstOrDefault(p => p.Dep == _mainWindowVeiwModel.SelectedRow.Dep);
                if (selectProgram == null)
                {
                    MessageBox.Show(Constant.NotFoundInstallProgram);
                    _mainWindowVeiwModel.IsEnabledTable = true;
                    return;
                }

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

            programInfo.dep = _mainWindowVeiwModel.SelectedRow.Dep;



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

                var bytesLauncher = crypt.Decode(cryptBytesProgram);


                try
                {
                    setHelp.RemoveProgram(selectProgram.Path, true);
                    setHelp.RemoveDumps<SettingSerializer, IRemoveData>(selectProgram);
                }
                catch (DirectoryNotFoundException)
                {
                    MessageBox.Show(Constant.NotFoundFileOrDirectoryWithProgram);
                }
                catch (FileNotFoundException)
                {
                    MessageBox.Show(Constant.NotFoundFileOrDirectoryWithProgram);
                }


                var zip = new ArchiveHelper();

                var bytesProgramSetting = zip.Extract(bytesLauncher, request.hash);

                var getProgramSetting = setHelp.Read<SettingSerializer>(bytesProgramSetting, CommonConstant.KeySettings);

                getProgramSetting.Path = request.hash;


                var instalProgs = launcherSet.InfoInstallPrograms.ToList();
                for (int i = 0; i < instalProgs.Count; i++)
                {
                    if (instalProgs[i].Dep == getProgramSetting.Dep)
                    {
                        instalProgs[i] = getProgramSetting;
                        break;
                    }
                }


                setHelp.Write(launcherSet, CommonConstant.KeySettings, CommonConstant.FileSettingsLauncher);

                _mainWindowVeiwModel.SelectedRow.SetStateAfterUpdate();
                _mainWindowVeiwModel.SelectedRow.Status = StatusHelper.InstallAndUpdated;
                _mainWindowVeiwModel.SelectedRow.Version = getProgramSetting.Version;
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
