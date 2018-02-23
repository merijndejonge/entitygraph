using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DataValidationFramework.Net.Tests.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenSoftware.DataValidationFramework.Net;

namespace DataValidationFramework.Net.Tests.Tests
{
    [TestClass]
    public class ValidationAttributeTests : DataValidationTest
    {
        public class MyTestClass : INotifyPropertyChanged
        {
            private int? _intProperty;

            [Mandatory(ErrorMessage = "Int parameter {0} is Missing")]
            public int? IntProperty
            {
                get => _intProperty;
                set
                {
                    _intProperty = value;
                    OnPropertyChanged();
                }
            }

            private string _stringProperty;

            [Mandatory(ErrorMessage = "String parameter {0} is Missing")]
            public string StringProperty
            {
                get => _stringProperty;
                set
                {
                    _stringProperty = value;
                    OnPropertyChanged();
                }
            }

            private string _regExprProperty;

            [Pattern(@"^[a-zA-Z''-'\s]{5,40}$", ErrorMessage = "Regular expression {0} mismatch")]
            public string RegExprProperty
            {
                get => _regExprProperty;
                set
                {
                    _regExprProperty = value;
                    OnPropertyChanged();
                }
            }

            [NoDuplicates(ErrorMessage = "Supplicate elements founhd in {0}")]
            public ObservableCollection<string> Elements { get; } = new ObservableCollection<string>();

            public event PropertyChangedEventHandler PropertyChanged;

            [NotifyPropertyChangedInvocator]
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        [TestMethod]
        public void RequiredAttributeIntTest()
        {
            var entity = new MyTestClass();
            var validator = new EntityValidator(entity);
            entity.IntProperty = null;
            Assert.IsTrue(validator.Entity.HasErrors);
            entity.IntProperty = 0;
            Assert.IsFalse(validator.Entity.HasErrors);
        }

        [TestMethod]
        public void RequiredAttributeStringTest()
        {
            var entity = new MyTestClass();
            var validator = new EntityValidator(entity);
            entity.StringProperty = null;
            Assert.IsTrue(validator.Entity.HasErrors);
            entity.StringProperty = "";
            Assert.IsFalse(validator.Entity.HasErrors);
        }

        [TestMethod]
        public void RegularExpressionAttributeTest()
        {
            var entity = new MyTestClass();
            var validator = new EntityValidator(entity);
            entity.RegExprProperty = "abc";
            Assert.IsTrue(validator.Entity.HasErrors);
            entity.RegExprProperty = "abcdef";
            Assert.IsFalse(validator.Entity.HasErrors);
        }

        [TestMethod]
        public void NoDuplicatesAttributeTest()
        {
            var entity = new MyTestClass();
            var validator = new EntityValidator(entity);

            Assert.IsFalse(validator.Entity.HasErrors);
            entity.Elements.Add("bar");
            Assert.IsFalse(validator.Entity.HasErrors);

            entity.Elements.Add("foo");
            Assert.IsFalse(validator.Entity.HasErrors);

            entity.Elements.Add("bar");
            Assert.IsTrue(validator.Entity.HasErrors);

            entity.Elements.Remove("bar");
            Assert.IsFalse(validator.Entity.HasErrors);
        }
    }
}