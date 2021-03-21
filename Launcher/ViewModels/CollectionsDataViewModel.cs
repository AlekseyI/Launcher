using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using CommonClasses;
using CommonClasses.Helpers;
using CommonClasses.Serializers;
using Launcher.Commands;
using Launcher.Models;
using Updater.Models.Serializers;

namespace Launcher.ViewModels
{
    public class CollectionsDataViewModel : BaseViewModel
    {
        private ObservableCollection<DataProgramInfo> _dataTable;

        public DispatcherTimer CheckProgramTimer { get; set; }

        public DispatcherTimer CheckStartProgramTimer { get; set; }

        public IAuthSerializer Auth { get; set; }

        public List<Thread> StartPrograms { get; set; }

        

        public ObservableCollection<DataProgramInfo> DataTable
        {
            get => _dataTable;
            set
            {
                _dataTable = value;
                OnPropertyChanged(nameof(DataTable));
            }
        }

        public CollectionsDataViewModel()
        {
            StartPrograms = new List<Thread>();
            DataTable = new ObservableCollection<DataProgramInfo>();
            var setHelp = new SettingHelper();
            try
            {
                var set = setHelp.Read<LauncherSettingSerializer>(CommonConstant.KeySettings, CommonConstant.FileSettingsLauncher);
                NameLauncher = set.Info.Name;
                VersionLauncher = set.Info.Version;
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

        
            var timerCheckLauncher = new DispatcherTimer();


            timerCheckLauncher.Tick += (s, e) =>
            {
                try
                {
                    if (Process.GetProcessById(Constant.PidUpdater) == null)
                    {
                        Application.Current.Shutdown();
                    }
                }
                catch (ArgumentException)
                {
                    Application.Current.Shutdown();
                }


            };

            timerCheckLauncher.Interval = CommonConstant.IntervalCheckRunningUpdater;
            timerCheckLauncher.Start();

            CheckProgramTimer = new DispatcherTimer();
            CheckProgramTimer.Interval = CommonConstant.IntervalRequestUpdateProgram;


         
            CheckStartProgramTimer = new DispatcherTimer();


            CheckStartProgramTimer.Tick += (s, e) =>
            {

                if (StartPrograms != null)
                {
                    for (int i = 0; i < StartPrograms.Count; i++)
                    {
                        if (!StartPrograms[i].IsAlive)
                        {
                            StartPrograms[i].Abort();
                            StartPrograms.Remove(StartPrograms[i]);
                        }
                    }    
                }

            };

            CheckStartProgramTimer.Interval = CommonConstant.IntervalCheckExitStartProgram;
        }

        private string _versionLauncher;

        public string VersionLauncher
        {
            get => _versionLauncher;
            set
            {
                _versionLauncher = value;
                OnPropertyChanged(nameof(VersionLauncher));
            }
        }

        private bool _isEnabledTable = true;

        public bool IsEnabledTable
        {
            get => _isEnabledTable;
            set
            {
                _isEnabledTable = value;
                OnPropertyChanged(nameof(IsEnabledTable));
            }
        }

        private string _nameLauncher;

        public string NameLauncher
        {
            get => _nameLauncher;
            set
            {
                _nameLauncher = value;
                OnPropertyChanged(nameof(NameLauncher));
            }
        }

        private ICommand _openAuthDialogWindow;

        public ICommand OpenAuthDialogWindow
        {
            get
            {
                if (_openAuthDialogWindow == null)
                {
                    _openAuthDialogWindow = new AuthWindowCommand(this);
                }
                return _openAuthDialogWindow;
            }
        }


        private DataProgramInfo _selectedRow;

        public DataProgramInfo SelectedRow
        {
            get => _selectedRow;
            set
            {
                _selectedRow = value;
                OnPropertyChanged(nameof(SelectedRow));
            }
        }

        private ICommand _buttonStartCommand;

        public ICommand ButtonStartCommand
        {
            get
            {
                if (_buttonStartCommand == null)
                {

                    _buttonStartCommand = new RelayCommand(
                   param =>
                   {

                       if (SelectedRow != null)
                       {
                           var command = new StartProgramCommand(this);
                           command.Execute(null);
                       }
                   }
               );
                }
                return _buttonStartCommand;
            }

        }

        private ICommand _buttonDownloadAndInstallCommand;

        public ICommand ButtonDownloadAndInstallCommand
        {
            get
            {
                if (_buttonDownloadAndInstallCommand == null)
                {
                    _buttonDownloadAndInstallCommand = new RelayCommand(
                   param =>
                   {
                       if (SelectedRow != null)
                       {
                           SelectedRow.DisactivateButtons();
                           var command = new DownloadInstallProgramCommand(this);
                           command.Execute(null);
                           
                       }
                   }
               );
                }
                return _buttonDownloadAndInstallCommand;
            }

        }


        private ICommand _buttonUpdateCommand;

        public ICommand ButtonUpdateCommand
        {
            get
            {
                if (_buttonUpdateCommand == null)
                {
                    _buttonUpdateCommand = new RelayCommand(
                        param =>
                        {

                            if (SelectedRow != null)
                            {
                                SelectedRow.DisactivateButtons();
                                var command = new UpdateProgramCommand(this);
                                command.Execute(null);

                            }
                        },
                        param => DataTable.Count > 0
                    );
                }
                return _buttonUpdateCommand;
            }
        }


        private ICommand _buttonRemoveCommand;

        public ICommand ButtonRemoveCommand
        {
            get
            {
                if (_buttonRemoveCommand == null)
                {
                    _buttonRemoveCommand = new RelayCommand(
                        param =>
                        {
                            if (SelectedRow != null)
                            {
                                SelectedRow.DisactivateButtons();
                                var command = new RemoveProgramCommand(this);
                                command.Execute(null);
                                
                            }
                        },
                        param => DataTable.Count > 0
                    );
                }
                return _buttonRemoveCommand;
            }
        }

        private long _progressValue;

        public long ProgressValue
        {
            get => _progressValue;
            set
            {
                _progressValue = value;
                OnPropertyChanged(nameof(ProgressValue));
            }
        }
    }
}
