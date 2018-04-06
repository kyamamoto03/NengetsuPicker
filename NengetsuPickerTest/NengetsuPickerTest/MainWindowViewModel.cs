using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NengetsuPickerTest
{
    public class MainWindowViewModel : ViewModelBase
    {
        string _Nengetsu1;
        public string Nengetsu1
        {
            get { return _Nengetsu1; }
            set
            {
                _Nengetsu1 = value;
                OnPropertyChanged();
            }
        }

        string _Nengetsu2;
        public string Nengetsu2
        {
            get { return _Nengetsu2; }
            set
            {
                _Nengetsu2 = value;
                OnPropertyChanged();
            }
        }

    }

    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var h = PropertyChanged;
            if (h == null) return;
            h(this, new PropertyChangedEventArgs(propertyName));
        }

    }

}
