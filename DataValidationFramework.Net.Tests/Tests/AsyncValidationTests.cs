using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using DataValidationFramework.Net.Tests.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenSoftware.DataValidationFramework.Net;
using OpenSoftware.DataValidationFrameworkCore;

namespace DataValidationFramework.Net.Tests.Tests
{
    public class AsyncValidator : AsyncValidationRule
    {
        public AsyncValidator()
            : base(InputOutput<A, string>(x => x.Name)
            )
        {
        }

        public ValidationOperation Validate(string name)
        {
            var operation = new ValidationOperation();
            var timer = new System.Timers.Timer {Interval = 100}; // 100 Milliseconds 
            timer.Elapsed += (s, e) =>
            {
                timer.Stop();
                operation.Result = new ValidationResult(name);
            };
            timer.Start();

            return operation;
        }
    }

    [TestClass]
    public class AsyncValidationTests : DataValidationTest
    {
        [TestMethod]
        public void AsyncValidationTest()
        {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);

            var a1 = new A {Name = "a1"};
            var validator = new AsyncValidator();

            var vEngine = new ValidationEngine
            {
                RulesProvider = new SimpleValidationRulesProvider<ValidationResult> {validator}
            };

            vEngine.ValidationResultChanged += (sender, args) =>
                autoResetEvent.Set();

            vEngine.Validate(new List<object> {a1});

            Assert.IsFalse(vEngine.Entity(a1).Errors.Count == 1);

            Assert.IsTrue(autoResetEvent.WaitOne());

            Assert.IsTrue(vEngine.EntitiesInError.Single() == a1);
            Assert.IsTrue(vEngine.Entity(a1).Errors.Count == 1);
        }
    }
}