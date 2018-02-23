using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenSoftware.DataValidationFramework.Net;
using OpenSoftware.DataValidationFrameworkCore;
using OpenSoftware.EntityGraph.Net.Tests.Model;

namespace OpenSoftware.EntityGraph.Net.Tests.Tests
{

    [TestClass]
    public class GraphValidatorTests : EntityGraphTest
    {
        public class AValidator : ValidationRule
        {
            public AValidator() :
                base(
                    InputOutput<A, string>(a => a.B.Name),
                    InputOutput<A, string>(a => a.B.C.Name)
                )
            {
            }

            public static bool IsValidated;

            [ValidateMethod]
            public ValidationResult ValidateMe(string nameOfB, string nameOfC)
            {
                IsValidated = true;
                return nameOfB != nameOfC ? new ValidationResult("Invalid names") : ValidationResult.Success;
            }
        }

        public class MultiPropertyValidator : ValidationRule
        {
            public MultiPropertyValidator() :
                base(
                    InputOutput<A, string>(a => a.Name),
                    InputOutput<A, string>(a => a.LastName)
                )
            {
            }

            public ValidationResult Validate(string name, string lastName)
            {
                return name == lastName
                    ? new ValidationResult("Name and LastName cannot be the same", new[] {"dummy", "members"})
                    : ValidationResult.Success;
            }
        }

        public class InputOutputInputOnlyValidator : ValidationRule
        {
            public InputOutputInputOnlyValidator() :
                base(
                    InputOutput<A, string>(a => a.Name),
                    InputOnly<A, string>(a => a.LastName)
                )
            {
            }

            public ValidationResult Validate(string name, string lastName)
            {
                return name == lastName
                    ? new ValidationResult("Name and LastName cannot be the same", new[] {"dummy", "members"})
                    : ValidationResult.Success;
            }
        }

        /// <summary>
        /// Checks if validation framework works correctly when one of the dependency paths
        /// of a valudation rule includes null. In this case the path A.B.C.name includes null,
        /// becasue C equals null.
        /// </summary>
        [TestMethod]
        public void EntityGraphValidatorWithNullElelements()
        {
            var aa = new A();
            var bb = new B {Name = "hi"};
            var cc = new C {Name = "Hello"};

            aa.B = bb;

            var gr = aa.EntityGraph(EntityGraphs.CircularGraphFull);
            gr.Validator = new ValidationEngine
            {
                RulesProvider = new SimpleValidationRulesProvider<ValidationResult> {new AValidator()}
            };

            Assert.IsFalse(gr.Validator.Entity(bb).HasErrors);

            // Now we attach C to the entity graph, which should
            // trigger the validation rule.
            bb.C = cc;
            Assert.IsTrue(gr.Validator.Entity(bb).HasErrors);
        }

        [TestMethod]
        public void AValidatorTest()
        {
            var gr = a.EntityGraph(EntityGraphs.CircularGraphFull);
            gr.Validator = new ValidationEngine
            {
                RulesProvider = new SimpleValidationRulesProvider<ValidationResult> {new AValidator()}
            };

            AValidator.IsValidated = false;
            b.Name = "hello";
            Assert.IsTrue(gr.Validator.Entity(b).HasErrors);
            Assert.IsTrue(AValidator.IsValidated);
        }

        [TestMethod]
        public void AValidatorTest2()
        {
            var gr = a.EntityGraph(EntityGraphs.CircularGraphFull);
            gr.Validator = new ValidationEngine
            {
                RulesProvider = new SimpleValidationRulesProvider<ValidationResult> {new AValidator()}
            };
            AValidator.IsValidated = false;
            b.Name = "hello";
            Assert.IsTrue(gr.Validator.Entity(b).HasErrors);
            Assert.IsTrue(gr.Validator.Entity(c).HasErrors);
            c.Name = b.Name;
            Assert.IsFalse(gr.Validator.Entity(b).HasErrors);
            Assert.IsFalse(gr.Validator.Entity(c).HasErrors);
        }

        [TestMethod]
        public void MultiPropertyValidatorTest()
        {
            a.Name = "John";
            a.LastName = "John";
            var gr = a.EntityGraph(EntityGraphs.CircularGraphFull);
            gr.Validator = new ValidationEngine
            {
                RulesProvider = new SimpleValidationRulesProvider<ValidationResult> {new MultiPropertyValidator()}
            };

            Assert.IsTrue(gr.Validator.Entity(a).HasErrors);
            var validationError = gr.Validator.Entity(a).Errors.Single();
            Assert.IsTrue(validationError.MemberNames.Contains(nameof(a.Name)));
            Assert.IsTrue(validationError.MemberNames.Contains(nameof(a.LastName)));
            Assert.IsTrue(validationError.MemberNames.Count() == 2);
            a.LastName = "Doe";
            Assert.IsFalse(gr.Validator.Entity(a).HasErrors);
        }

        [TestMethod]
        public void InputOutputInputOnlyValidatorTest()
        {
            a.Name = "John";
            a.LastName = "John";
            var gr = a.EntityGraph(EntityGraphs.CircularGraphFull);
            gr.Validator = new ValidationEngine
            {
                RulesProvider =
                    new SimpleValidationRulesProvider<ValidationResult> {new InputOutputInputOnlyValidator()}
            };

            Assert.IsTrue(gr.Validator.Entity(a).HasErrors);
            var validationError = gr.Validator.Entity(a).Errors.Single();
            Assert.IsTrue(validationError.MemberNames.Contains(nameof(a.Name)));
            Assert.IsFalse(validationError.MemberNames.Contains(nameof(a.LastName)));
            Assert.IsTrue(validationError.MemberNames.Count() == 1);
            a.LastName = "Doe";
            Assert.IsFalse(gr.Validator.Entity(a).HasErrors);
        }
    }
}