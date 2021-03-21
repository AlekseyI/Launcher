using Launcher.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace Launcher.Models
{
    public class DataProgramInfo : BaseViewModel
    {

        public string Name { get; set; }

        public string NewVersion { get; set; }

        private string _version;

        public string Version
        {
            get => _version;
            set
            {
                _version = value;
                OnPropertyChanged(nameof(Version));
            }
        }

        private string _status;

        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        private bool _isLastButtonStart;

        private bool _isButtonStart;

        public bool IsButtonStart
        {
            get => _isButtonStart;
            set
            {
                _isButtonStart = value;
                OnPropertyChanged(nameof(IsButtonStart));
            }
        }



        private bool _isLastButtonDownloadAndInstall;

        private bool _isButtonDownloadAndInstall;

        public bool IsButtonDownloadAndInstall
        {
            get => _isButtonDownloadAndInstall;
            set
            {
                _isButtonDownloadAndInstall = value;
                OnPropertyChanged(nameof(IsButtonDownloadAndInstall));
            }
        }


        private bool _isLastButtonUpdate;

        private bool _isButtonUpdate;


        public bool IsButtonUpdate
        {
            get => _isButtonUpdate;
            set
            {
                _isButtonUpdate = value;
                OnPropertyChanged(nameof(IsButtonUpdate));
            }
        }

        private bool _isLastButtonRemove;

        private bool _isButtonRemove;

        public bool IsButtonRemove
        {
            get => _isButtonRemove;
            set
            {
                _isButtonRemove = value;
                OnPropertyChanged(nameof(IsButtonRemove));
            }
        }



        public int Dep { get; set; }


        public void DisactivateWithSaveStateButtons()
        {
            _isLastButtonStart = IsButtonStart;
            _isLastButtonDownloadAndInstall = IsButtonDownloadAndInstall;
            _isLastButtonUpdate = IsButtonUpdate;
            _isLastButtonRemove = IsButtonRemove;
            IsButtonStart = IsButtonDownloadAndInstall = IsButtonUpdate = IsButtonRemove = false;
        }

        public void DisactivateButtons()
        {
            IsButtonStart = IsButtonDownloadAndInstall = IsButtonUpdate = IsButtonRemove = false;
        }

        public void RestoringButtons()
        {
            IsButtonStart = _isLastButtonStart;
            IsButtonDownloadAndInstall = _isLastButtonDownloadAndInstall;
            IsButtonUpdate = _isLastButtonUpdate;
            IsButtonRemove = _isLastButtonRemove;
        }

        public void SetStateAfterDownloadAndInstall()
        {
            IsButtonStart = true;
            IsButtonDownloadAndInstall = false;
            IsButtonUpdate = false;
            IsButtonRemove = true;
        }

        public void SetStateAfterRequestNoDownloadAndInstall()
        {
            IsButtonStart = false;
            IsButtonDownloadAndInstall = true;
            IsButtonUpdate = false;
            IsButtonRemove = false;
        }

        public void SetStateAfterStart()
        {
            DisactivateButtons();
        }

        public void SetStateAfterUpdate()
        {
            IsButtonStart = true;
            IsButtonDownloadAndInstall = false;
            IsButtonUpdate = false;
            IsButtonRemove = true;
        }

        public void SetStateAfterRemove()
        {
            IsButtonStart = false;
            IsButtonDownloadAndInstall = true;
            IsButtonUpdate = false;
            IsButtonRemove = false;

        }

        public void SetStateToUpdate()
        {
            IsButtonStart = true;
            IsButtonDownloadAndInstall = false;
            IsButtonUpdate = true;
            IsButtonRemove = true;

        }

    }
}
