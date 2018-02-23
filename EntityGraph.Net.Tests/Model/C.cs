using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using OpenSoftware.EntityGraph.Net.Tests.Annotations;

namespace OpenSoftware.EntityGraph.Net.Tests.Model
{
    public class C : INotifyPropertyChanged
    {
        private int _id;
        private ObservableCollection<B> _bSet = new ObservableCollection<B>();
        private D _d;
        private int? _dId;
        private string _name;

        public C()
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

        public ObservableCollection<B> BSet
        {
            get => _bSet;
            set
            {
                if (Equals(value, _bSet)) return;
                _bSet = value;
                OnPropertyChanged();
            }
        }

        public D D
        {
            get => _d;
            set
            {
                if (Equals(value, _d)) return;
                _d = value;
                OnPropertyChanged();
            }
        }

        public int? DId
        {
            get => _dId;
            set
            {
                if (value == _dId) return;
                _dId = value;
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