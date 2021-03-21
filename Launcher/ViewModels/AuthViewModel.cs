using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Launcher.Commands;
using CommonClasses.Request;
using CommonClasses.Serializers;
using CommonClasses.Parsers;
using Launcher.Models;

namespace Launcher.ViewModels
{
    public class AuthViewModel : BaseViewModel
    {
        
        private string _login;
        public string Login
        {
            get => _login;
            set
            {
                _login = value;
                OnPropertyChanged(nameof(Login));
            }
        }

        public string Password { get; set; }

        private string _validationErrorText;
        public string ValidationErrorText
        {
            get => _validationErrorText;
            set
            {
                _validationErrorText = value;
                OnPropertyChanged(nameof(ValidationErrorText));
            }
        }

        public bool IsAccept;

        private Visibility _validationError;

        public Visibility ValidationError
        {
            get => _validationError;
            set
            {
                _validationError = value;
                OnPropertyChanged(nameof(ValidationError));
            }
        }


        public AuthViewModel()
        {
            ValidationError = Visibility.Hidden;            
        }

        private ICommand _hideValidationErrorCommand;

        public ICommand HideValidationErrorCommand
        {
            get
            {
                if (_hideValidationErrorCommand == null)
                {
                    _hideValidationErrorCommand = new RelayCommand(
                        param =>
                        {
                            ValidationError = Visibility.Hidden;
                        }
                        );
                }
                return _hideValidationErrorCommand;
            }
        }


        private ICommand _acceptCommand;

        public ICommand AcceptCommand
        {
            get
            {
                if (_acceptCommand == null)
                {
                    _acceptCommand = new RelayCommand(
                        param => 
                        {
                            var net = new Net();
                            var auth = new AuthSerializer();
                            auth.username = Login;
                            Password = (param as PasswordBox).Password;
                            auth.password = Password;                           
                            var authResult = net.Request(Constant.UrlLogin, Net.Post, auth);
                            
                            if (authResult.detail is null)
                            {
                                IsAccept = true;
                                ValidationError = Visibility.Hidden;
                                ValidationErrorText = "";
                                (Application.Current as App).DisplayRootRegistry.HidePresentation(this);
                            }
                            else
                            {
                                IsAccept = false;
                                ValidationError = Visibility.Visible;
                                ValidationErrorText = ErrorParser.Parse(authResult.detail);
                            }
                            
                        });
                       
                }
                return _acceptCommand;
            }
        }



    }
}
