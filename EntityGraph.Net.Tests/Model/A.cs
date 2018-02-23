using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using OpenSoftware.EntityGraph.Net.Tests.Annotations;

namespace OpenSoftware.EntityGraph.Net.Tests.Model
{
    public class A : INotifyPropertyChanged
    {
        private int _id;
        private B _b;
        private int? _bId;
        private B _bNotInGraph;
        private int? _bNotInGraphId;
        private ObservableCollection<B> _bSet = new ObservableCollection<B>();
        private ObservableCollection<D> _dSet = new ObservableCollection<D>();
        private string _name;
        private string _lastName;

        public A()
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

        public B B
        {
            get => _b;
            set
            {
                if (Equals(value, _b)) return;
                _b = value;
                OnPropertyChanged();
            }
        }

        public int? BId
        {
            get => _bId;
            set
            {
                if (value == _bId) return;
                _bId = value;
                OnPropertyChanged();
            }
        }

        public B BNotInGraph
        {
            get => _bNotInGraph;
            set
            {
                if (Equals(value, _bNotInGraph)) return;
                _bNotInGraph = value;
                OnPropertyChanged();
            }
        }

        public int? BNotInGraphId
        {
            get => _bNotInGraphId;
            set
            {
                if (value == _bNotInGraphId) return;
                _bNotInGraphId = value;
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

        public ObservableCollection<D> DSet
        {
            get => _dSet;
            set
            {
                if (Equals(value, _dSet)) return;
                _dSet = value;
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

        public string LastName
        {
            get => _lastName;
            set
            {
                if (value == _lastName) return;
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
