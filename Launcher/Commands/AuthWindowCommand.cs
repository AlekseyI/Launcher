using System;
using System.Windows;
using Launcher.ViewModels;
using System.Windows.Input;
using CommonClasses.Serializers;

namespace Launcher.Commands
{
    public class AuthWindowCommand : ICommand
    {

        private CollectionsDataViewModel _mainWindowVeiwModel;

        public AuthWindowCommand(CollectionsDataViewModel mainWindowVeiwModel)
        {
            _mainWindowVeiwModel = mainWindowVeiwModel;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            var displayRootRegistry = (Application.Current as App).DisplayRootRegistry;

            var dialogWindowViewModel = new AuthViewModel();

            await displayRootRegistry.ShowModalPresentation(dialogWindowViewModel);


            if (dialogWindowViewModel.IsAccept)
            {
                _mainWindowVeiwModel.Auth = new AuthSerializer();
                _mainWindowVeiwModel.Auth.username = dialogWindowViewModel.Login;
                _mainWindowVeiwModel.Auth.password = dialogWindowViewModel.Password;
             
                var checkProgram = new CheckUpdateProgramCommand(_mainWindowVeiwModel);
                checkProgram.Execute(null);

                _mainWindowVeiwModel.CheckProgramTimer.Tick += (s, e) =>
                {
                    var checkPrograms = new CheckUpdateProgramCommand(_mainWindowVeiwModel);
                    checkPrograms.Execute(null);

                };

                _mainWindowVeiwModel.CheckProgramTimer.Start();

                _mainWindowVeiwModel.CheckStartProgramTimer.Start();

            }
            else
            {
                Application.Current.Shutdown();
            }

        }

    }
}
