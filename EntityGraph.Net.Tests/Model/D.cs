using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using OpenSoftware.EntityGraph.Net.Tests.Annotations;

namespace OpenSoftware.EntityGraph.Net.Tests.Model
{
    public class D : INotifyPropertyChanged
    {
        private int _id;
        private A _a;
        private int? _aId;
        private ObservableCollection<C> _cSet = new ObservableCollection<C>();
        private string _name;

        public D()
        {
            Id = IdFactory.Assign;
        }

        public int Id
        {
            get => _id;
            set
            {
                if (value == _id) return;
                _id = value;
                OnPropertyChanged();
            }
        }

        public A A
        {
            get => _a;
            set
            {
                if (Equals(value, _a)) return;
                _a = value;
                OnPropertyChanged();
            }
        }

        public int? AId
        {
            get => _aId;
            set
            {
                if (value == _aId) return;
                _aId = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<C> CSet
        {
            get => _cSet;
            set
            {
                if (Equals(value, _cSet)) return;
                _cSet = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                if (value == _name) return;
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