using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using OpenSoftware.EntityGraph.Net.Tests.Annotations;

namespace OpenSoftware.EntityGraph.Net.Tests.Model
{
    public class B : INotifyPropertyChanged
    {
        private int _id;
        private A _a;
        private int? _aId;
        private C _c;
        private int? _cId;
        private string _name;

        public B()
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

        public ObservableCollection<A> ASet { get; set; } = new ObservableCollection<A>();

        public C C
        {
            get => _c;
            set
            {
                if (Equals(value, _c)) return;
                _c = value;
                OnPropertyChanged();
            }
        }

        public int? CId
        {
            get => _cId;
            set
            {
                if (value == _cId) return;
                _cId = value;
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