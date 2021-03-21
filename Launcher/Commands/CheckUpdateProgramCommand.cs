using CommonClasses;
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
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Updater.Models.Serializers;

namespace Launcher.Commands
{
    public class CheckUpdateProgramCommand : ICommand
    {

        private CollectionsDataViewModel _mainWindowVeiwModel;

        public CheckUpdateProgramCommand(CollectionsDataViewModel mainWindowVeiwModel)
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

            var programsInfo = new ProgramsInfoSerializer();
            var setHelper = new SettingHelper();
            var net = new Net();
            try
            {
                var launcherSet = setHelper.Read<LauncherSettingSerializer>(CommonConstant.KeySettings, CommonConstant.FileSettingsLauncher);

                var request = net.Request(Constant.UrlCheckVersionPrograms, Net.Post, programsInfo, _mainWindowVeiwModel.Auth);
                if (request.detail != null)
                {
                    MessageBox.Show(ErrorParser.Parse(request.detail));
                    return;
                }
                _mainWindowVeiwModel.DataTable.Clear();
                if (launcherSet.InfoInstallPrograms == null || launcherSet.InfoInstallPrograms?.Count() == 0)
                {
                    request.programs_info?.ToList().ForEach(p =>
                    {
                        var prog = new DataProgramInfo { Name = p.name, Version = p.version, NewVersion = p.version, Dep = p.dep };
                        prog.SetStateAfterRequestNoDownloadAndInstall();
                        prog.Status = StatusHelper.NoInstall;
                        _mainWindowVeiwModel.DataTable.Add(prog);
                    });
                }
                else
                {
                    request.programs_info?.ToList().ForEach(p =>
                    {
                        var installProg = launcherSet.InfoInstallPrograms?.FirstOrDefault(ip => ip.Dep == p.dep);
                        if (installProg == null)
                        {
                            var prog = new DataProgramInfo { Name = p.name, Version = p.version, NewVersion = p.version, Dep = p.dep };
                            prog.SetStateAfterRequestNoDownloadAndInstall();
                            prog.Status = StatusHelper.NoInstall;
                            _mainWindowVeiwModel.DataTable.Add(prog);
                        }
                        else
                        {
                            var prog = new DataProgramInfo { Name = p.name, Version = installProg.Version, NewVersion = p.version, Dep = p.dep };
                            
                            if (prog.Version != prog.NewVersion)
                            {
                                prog.SetStateToUpdate();
                                prog.Status = StatusHelper.ThereIsUpdate;
                            }
                            else
                            {
                                prog.SetStateAfterDownloadAndInstall();
                                prog.Status = StatusHelper.InstallAndUpdated;
                            }
                            
                            _mainWindowVeiwModel.DataTable.Add(prog);
                        }
                        
                    });


                }

                if ((launcherSet.InfoInstallPrograms != null || launcherSet.InfoInstallPrograms?.Count() > 0) && request.programs_info != null)
                {
                    var installProgs = launcherSet.InfoInstallPrograms.ToList();
                    for (int i = 0; i < installProgs.Count; i++)
                    {
                        if (!request.programs_info.Any(p => p.dep == installProgs[i].Dep))
                        {
                            try
                            {
                                setHelper.RemoveProgram(installProgs[i].Path, true);
                                setHelper.RemoveDumps<SettingSerializer, IRemoveData>(installProgs[i]);
                            }
                            catch (DirectoryNotFoundException)
                            {
                                
                            }
                            catch (FileNotFoundException)
                            {                              

                            }
                            installProgs.Remove(installProgs[i]);
                        }
                    }
                    launcherSet.InfoInstallPrograms = installProgs;
                    setHelper.Write(launcherSet, CommonConstant.KeySettings, CommonConstant.FileSettingsLauncher);
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
            }
        }
    }
}
