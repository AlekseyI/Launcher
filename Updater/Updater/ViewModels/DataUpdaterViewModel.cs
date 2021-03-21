using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Updater.ViewModels
{
    public class DataUpdaterViewModel : BaseViewModel
    {
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
