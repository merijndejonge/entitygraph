using System.ComponentModel;
using System.Runtime.CompilerServices;
using DataValidationFramework.Net.Tests.Annotations;

namespace DataValidationFramework.Net.Tests.Model
{
    public class C : INotifyPropertyChanged
    {
        private D _d;
        public D D
        {
            get => _d;
            set
            {
                if (_d == value) return;
                _d = value;
                OnPropertyChanged();
            }
        }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (_name == value) return;
                _name = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}