using System.Windows;
using System.Windows.Input;
using Updater.Commands;
using CommonClasses.Request;
using Updater.Models.Serializers;
using CommonClasses.Parsers;
using Updater.Models;
using System.IO;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Reflection;
using System.Diagnostics;
using System.Text;
using CommonClasses;

namespace Updater.ViewModels
{
    public class DataMainViewModel : BaseViewModel
    {

        public DispatcherTimer TimerUpdater { get; private set; }

        public DispatcherTimer TimerCheckLauncher { get; private set; }

        private ICommand _hideShowWindow;

        public ICommand HideShowWindow
        {
            get
            {
                if (_hideShowWindow == null)
                {

                    _hideShowWindow = new RelayCommand(
                        param =>
                        {

                         

                            TimerCheckLauncher = new DispatcherTimer();


                            TimerCheckLauncher.Tick += (s, e) =>
                            {
                                try
                                {
                                    if (Process.GetProcessById(Constant.PidLauncher) == null)
                                    {
                                        Application.Current.Shutdown();
                                    }
                                }
                                catch (ArgumentException)
                                {
                                    Application.Current.Shutdown();
                                }


                            };
                            TimerCheckLauncher.Interval =  CommonConstant.IntervalCheckRunningLauncher;
                            TimerCheckLauncher.Start();

                            TimerUpdater = new DispatcherTimer();

                            var progressBar = new ProgressBarCommand(this);
                            progressBar.Execute(null);

                            TimerUpdater.Tick += (s, e) =>
                            {
                                progressBar = new ProgressBarCommand(this);
                                progressBar.Execute(null);
                            };
                            TimerUpdater.Interval = CommonConstant.IntervalRequestUpdateLauncher;
                            TimerUpdater.Start();
                        }
                            );


                }
                return _hideShowWindow;
            }
        }

    }
}


