using System.ComponentModel;
using System.Runtime.CompilerServices;
using DataValidationFramework.Net.Tests.Annotations;

namespace DataValidationFramework.Net.Tests.Model
{
    public class A : INotifyPropertyChanged
    {
        private B _b;
        public B B
        {
            get => _b;
            set
            {
                if (_b == value) return;
                _b = value;
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
        private string _lastName;
        public string LastName
        {
            get => _lastName;
            set
            {
                if (_lastName == value) return;
                _lastName = value;
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
