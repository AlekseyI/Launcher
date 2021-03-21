using CommonClasses;
using CommonClasses.Helpers;
using CommonClasses.Removes;
using CommonClasses.Serializers;
using Launcher.Models;
using Launcher.Models.Helpers;
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
    public class RemoveProgramCommand : ICommand
    {

        private CollectionsDataViewModel _mainWindowVeiwModel;

        public RemoveProgramCommand(CollectionsDataViewModel mainWindowVeiwModel)
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

            var setHelp = new SettingHelper();
            _mainWindowVeiwModel.CheckProgramTimer.Stop();
            _mainWindowVeiwModel.IsEnabledTable = false;
            try
            {
                var launcherSet = setHelp.Read<LauncherSettingSerializer>(CommonConstant.KeySettings, CommonConstant.FileSettingsLauncher);
                var selectProgram = launcherSet.InfoInstallPrograms?.FirstOrDefault(p => p.Dep == _mainWindowVeiwModel.SelectedRow.Dep);
                if (selectProgram == null)
                {
                    MessageBox.Show(Constant.NotFoundInstallProgram);
                    _mainWindowVeiwModel.IsEnabledTable = true;
                    return;
                }
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

                (launcherSet.InfoInstallPrograms as List<SettingSerializer>).Remove(selectProgram);
                if (launcherSet.InfoInstallPrograms.Count() == 0)
                    launcherSet.InfoInstallPrograms = null;

                setHelp.Write(launcherSet, CommonConstant.KeySettings, CommonConstant.FileSettingsLauncher);

                _mainWindowVeiwModel.SelectedRow.SetStateAfterRemove();
                _mainWindowVeiwModel.SelectedRow.Status = StatusHelper.NoInstall;
                _mainWindowVeiwModel.CheckProgramTimer.Start();
                _mainWindowVeiwModel.IsEnabledTable = true;
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
        }
    }
}
