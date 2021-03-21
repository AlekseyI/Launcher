using CommonClasses;
using CommonClasses.CryptHash;
using CommonClasses.Helpers;
using Launcher.Models;
using Launcher.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Updater.Models.Serializers;

namespace Launcher.Commands
{
    public class StartProgramCommand : ICommand
    {
        private CollectionsDataViewModel _mainWindowVeiwModel;

        public StartProgramCommand(CollectionsDataViewModel mainWindowVeiwModel)
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
            try
            {
                var launcherSet = setHelp.Read<LauncherSettingSerializer>(CommonConstant.KeySettings, CommonConstant.FileSettingsLauncher);
                var selectProg = launcherSet.InfoInstallPrograms?.FirstOrDefault(p => p.Dep == _mainWindowVeiwModel.SelectedRow.Dep);
                if (selectProg == null)
                {
                    
                    MessageBox.Show(Constant.NotFoundInstallProgram);
                    _mainWindowVeiwModel.CheckProgramTimer.Start();
                    return;
                }

                Thread program = new Thread(() =>
                {
                    using (var proc = new ProcessHelper())
                    {

                        _mainWindowVeiwModel.SelectedRow.DisactivateWithSaveStateButtons();
                        var dep = _mainWindowVeiwModel.SelectedRow.Dep;


                        try
                        {
                            proc.StartProcess(selectProg.Path + "\\" + selectProg.StartApp, null, true);
                        }
                        catch(System.ComponentModel.Win32Exception)
                        {
                            MessageBox.Show(Constant.NotFoundProgram);
                        }
                        

                        var selectRow = _mainWindowVeiwModel.DataTable?.FirstOrDefault(d => d.Dep == dep);
                        if (selectRow != null)
                        {
                            selectRow.RestoringButtons();
                        }
                    }
                });
                program.SetApartmentState(ApartmentState.STA);
                program.Start();

                _mainWindowVeiwModel.StartPrograms.Add(program);

                _mainWindowVeiwModel.CheckProgramTimer.Start();

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
