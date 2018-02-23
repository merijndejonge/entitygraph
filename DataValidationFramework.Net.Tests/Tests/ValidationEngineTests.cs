using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using DataValidationFramework.Net.Tests.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenSoftware.DataValidationFramework.Net;
using OpenSoftware.DataValidationFrameworkCore;

namespace DataValidationFramework.Net.Tests.Tests
{

    [TestClass]
    public class ValidationEngineTests : DataValidationTest
    {
        public class AValidator : ValidationRule<ValidationResult>
        {
            public AValidator()
                : base(
                    InputOutput<A, string>(a => a.B.Name),
                    InputOutput<A, string>(b => b.B.C.Name)
                )
            {
            }

            public static ValidationResult TestResult { get; set; }

            [ValidateMethod]
            public ValidationResult ValidateMe(string nameOfB, string nameOfC)
            {
                if (nameOfB != nameOfC)
                {
                    TestResult = new ValidationResult("Invalid names");
                    return ValidationResult.Success;
                }

                TestResult = ValidationResult.Success;
                return ValidationResult.Success;
            }
        }

        public class PermutationsOneParameterValidator : ValidationRule<ValidationResult>
        {
            public PermutationsOneParameterValidator() :
                base(InputOutput<A, string>(x => x.Name))
            {
            }

            public ValidationResult Validate(string arg)
            {
                return new ValidationResult(arg);
            }
        }

        public class PermutationsTwoParameterValidator : ValidationRule<ValidationResult>
        {
            public PermutationsTwoParameterValidator() :
                base(
                    InputOutput<A, string>(a => a.Name),
                    InputOutput<A, string>(b => b.LastName)
                )
            {
            }

            public ValidationResult Validate(string arg1, string arg2)
            {
                return new ValidationResult(arg1);
            }
        }

        public class PermutationsTwoParameterNamesValidator : ValidationRule<ValidationResult>
        {
            public PermutationsTwoParameterNamesValidator() :
                base(
                    InputOutput<A, string>(a => a.Name),
                    InputOutput<A, string>(b => b.Name)
                )
            {
            }

            public ValidationResult Validate(string arg1, string arg2)
            {
                return new ValidationResult(arg1 + arg2);
            }
        }

        public class PermutationsThreeParameterNamesValidator : ValidationRule<ValidationResult>
        {
            public PermutationsThreeParameterNamesValidator() :
                base(
                    InputOutput<A, string>(a => a.Name),
                    InputOutput<A, string>(b => b.Name),
                    InputOutput<A, string>(c => c.Name)
                )
            {
            }

            public ValidationResult Validate(string arg1, string arg2, string arg3)
            {
                return new ValidationResult(arg1 + arg2 + arg3);
            }
        }

        public class SignatureValidator : ValidationRule<ValidationResult>
        {
            public SignatureValidator() :
                base(
                    InputOutput<A, string>(x => x.LastName),
                    InputOutput<B, string>(x => x.Name)
                )
            {
            }

            public ValidationResult Validate(string lastName, string name)
            {
                return new ValidationResult("Visited");
            }
        }

        [TestMethod]
        public void AValidatorTest()
        {
            AValidator.TestResult = ValidationResult.Success;
            B.Name = "hello";
            var validator = new ValidationEngine
            {
                RulesProvider = new SimpleValidationRulesProvider<ValidationResult> {new AValidator()}
            };
            validator.Validate(B, nameof(B.Name), new List<object> {A, B, C, D});
            Assert.IsFalse(AValidator.TestResult == ValidationResult.Success);
        }

        [TestMethod]
        public void AValidatorTest2()
        {
            AValidator.TestResult = ValidationResult.Success;
            B.Name = "hello";
            var validator = new ValidationEngine
            {
                RulesProvider = new SimpleValidationRulesProvider<ValidationResult> {new AValidator()}
            };
            validator.Validate(B, nameof(B.Name), new List<object> {A, B, C, D});
            Assert.IsFalse(AValidator.TestResult == ValidationResult.Success);
            C.Name = B.Name;
            validator.Validate(C, nameof(C.Name), new List<object> {A, B, C, D});
            Assert.IsTrue(AValidator.TestResult == ValidationResult.Success);
        }

        [Description("Test that a validation rule is invoked for any matching object.")]
        [TestMethod]
        public void PermutationsOneParameterValidatorTest()
        {
            var a1 = new A {Name = "a1"};
            var a2 = new A {Name = "a2"};
            var validator = new ValidationEngine
            {
                RulesProvider =
                    new SimpleValidationRulesProvider<ValidationResult> {new PermutationsOneParameterValidator()}
            };
            validator.Validate(new List<object> {a1, a2});
            Assert.IsTrue(validator.Entity(a1).HasErrors);
            Assert.IsTrue(validator.Entity(a2).HasErrors);
        }

        [Description(
            "Test that a validation rule is invoked for any matching object. Should't make a difference in case of two parameters with the same name")]
        [TestMethod]
        public void PermutationsTwoParameterValidatorTest()
        {
            var a1 = new A {Name = "a1"};
            var a2 = new A {Name = "a2"};
            var validator = new ValidationEngine
            {
                RulesProvider =
                    new SimpleValidationRulesProvider<ValidationResult> {new PermutationsTwoParameterValidator()}
            };
            validator.Validate(new List<object> {a1, a2});
            Assert.IsTrue(validator.Entity(a1).HasErrors);
            Assert.IsTrue(validator.Entity(a2).HasErrors);
        }

        [Description("Test permutations in case of two different parameter names")]
        [TestMethod]
        public void PermutationsTwoParameterNamesValidatorTest()
        {
            var a1 = new A {Name = "a1"};
            var a2 = new A {Name = "a2"};
            var validator = new ValidationEngine
            {
                RulesProvider =
                    new SimpleValidationRulesProvider<ValidationResult> {new PermutationsTwoParameterNamesValidator()}
            };

            validator.Validate(new List<object> {a1, a2});
            Assert.IsTrue(validator.Entity(a1).Errors.Count == 2);
            Assert.IsTrue(validator.Entity(a1).Errors.Any(e => e.ErrorMessage == "a1a2"));
            Assert.IsTrue(validator.Entity(a1).Errors.Any(e => e.ErrorMessage == "a2a1"));
            Assert.IsTrue(validator.Entity(a2).Errors.Count == 2);
            Assert.IsTrue(validator.Entity(a2).Errors.Any(e => e.ErrorMessage == "a1a2"));
            Assert.IsTrue(validator.Entity(a2).Errors.Any(e => e.ErrorMessage == "a2a1"));
        }

        [Description("Test permutations in case of three different parameter names with two objects")]
        [TestMethod]
        public void PermutationsThreeParameterNamesValidatorTest1()
        {
            var a1 = new A {Name = "a1"};
            var a2 = new A {Name = "a2"};
            var validator = new ValidationEngine
            {
                RulesProvider =
                    new SimpleValidationRulesProvider<ValidationResult> {new PermutationsThreeParameterNamesValidator()}
            };
            validator.Validate(new List<object> {a1, a2});
            Assert.IsFalse(validator.Entity(a1).HasErrors);
            Assert.IsFalse(validator.Entity(a2).HasErrors);
        }

        [Description("Test permutations in case of three different parameter names with three objects")]
        [TestMethod]
        public void PermutationsThreeParameterNamesValidatorTest2()
        {
            var a1 = new A {Name = "a1"};
            var a2 = new A {Name = "a2"};
            var a3 = new A {Name = "a3"};
            var validator = new ValidationEngine
            {
                RulesProvider =
                    new SimpleValidationRulesProvider<ValidationResult> {new PermutationsThreeParameterNamesValidator()}
            };
            validator.Validate(new List<object> {a1, a2, a3});
            Assert.IsTrue(validator.Entity(a1).Errors.Count == 6);
            Assert.IsTrue(validator.Entity(a1).Errors.Any(e => e.ErrorMessage == "a1a2a3"));
            Assert.IsTrue(validator.Entity(a1).Errors.Any(e => e.ErrorMessage == "a1a3a2"));
            Assert.IsTrue(validator.Entity(a1).Errors.Any(e => e.ErrorMessage == "a2a1a3"));
            Assert.IsTrue(validator.Entity(a1).Errors.Any(e => e.ErrorMessage == "a2a3a1"));
            Assert.IsTrue(validator.Entity(a1).Errors.Any(e => e.ErrorMessage == "a3a1a2"));
            Assert.IsTrue(validator.Entity(a1).Errors.Any(e => e.ErrorMessage == "a3a2a1"));

            Assert.IsTrue(validator.Entity(a2).Errors.Count == 6);
            Assert.IsTrue(validator.Entity(a3).Errors.Count == 6);
        }

        [TestMethod]
        public void SingleEntityValidation1()
        {
            var validator = new ValidationEngine
            {
                RulesProvider = new SimpleValidationRulesProvider<ValidationResult> {new AValidator()}
            };
            AValidator.TestResult = ValidationResult.Success;
            validator.Validate(A, nameof(A.Name));
            Assert.IsTrue(AValidator.TestResult == ValidationResult.Success);
        }

        [TestMethod]
        public void SingleEntityValidation2()
        {
            var a = new A();
            var validator = new ValidationEngine
            {
                RulesProvider =
                    new SimpleValidationRulesProvider<ValidationResult> {new PermutationsOneParameterValidator()}
            };
            AValidator.TestResult = ValidationResult.Success;
            validator.Validate(a, nameof(a.Name));
            Assert.IsTrue(validator.Entity(a).HasErrors);
        }

        [TestMethod]
        public void SignatureMatchTest()
        {
            var a = new A {B = new B()};
            var validator = new ValidationEngine
            {
                RulesProvider = new SimpleValidationRulesProvider<ValidationResult>
                {
                    new SignatureValidator()
                }
            };
            validator.Validate(a, nameof(a.Name));
            Assert.IsFalse(validator.Entity(a).HasErrors);
        }

        /// <summary>
        /// Test that ValidationEngine does not prevent entities to get garbage collected
        /// </summary>
        [TestMethod]
        public void WeakReferenceTest()
        {
            var engine = new ValidationEngine();
            var type = engine.GetType();
            const BindingFlags bindingFlags = BindingFlags.FlattenHierarchy |
                                              BindingFlags.GetField |
                                              BindingFlags.NonPublic |
                                              BindingFlags.Instance;
            Assert.IsNotNull(type.BaseType);
            var entitiesField = type.BaseType.GetField("_entities", bindingFlags);
            Assert.IsNotNull(entitiesField);

            var entities =
                (Dictionary<WeakReference, IEntityErrorInfo<ValidationResult>>) entitiesField.GetValue(engine);
            Assert.IsNotNull(entities);

            var x = new A();
            var errorInfo = engine.Entity(x);

            Assert.IsNotNull(errorInfo);
            Assert.IsTrue(entities.Count == 1);
            Assert.IsTrue(entities.Keys.ToArray()[0].IsAlive);
            Assert.IsTrue(entities.Keys.ToArray()[0].Target.Equals(x));

            // ReSharper disable once RedundantAssignment
            x = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Assert.IsTrue(entities.Count == 1);
            Assert.IsFalse(entities.Keys.ToArray()[0].IsAlive);
            Assert.IsNull(entities.Keys.ToArray()[0].Target);

            x = new A();
            errorInfo = engine.Entity(x);
            Assert.IsNotNull(errorInfo);
            Assert.IsTrue(entities.Count == 1);
        }
    }
}