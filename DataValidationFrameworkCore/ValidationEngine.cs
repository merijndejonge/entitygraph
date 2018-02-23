using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace OpenSoftware.DataValidationFrameworkCore
{
    /// <summary>
    /// Class that implements cross-entity validation.
    /// 
    /// There are two alternative ways fro cross-entity validation
    /// - Validation for all objects and validation rules. In this variant, the engine computes all possible
    ///   bindings for a given set of entities of type TEntity and a collection of validation rules provided by 
    ///   the given IValidationRulesProvider. For each binding it invokes the corresponding validation rule.
    /// - Validation given an object and a property name. In this variant, the permutations of entity bindings 
    ///   are restricted to include the given object. The set of validation rules is restricted to those rules
    ///   that have the given property in their signature.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public abstract class ValidationEngine<TEntity, TResult> : IValidationEngine<TEntity, TResult>
        where TEntity : class
        where TResult : class
    {
        private IValidationRulesProvider<TResult> _rulesProvider;
        private string[] _observedProperties;
        private Dictionary<RuleBinding<TResult>, TResult> _validationResults;

        readonly Dictionary<WeakReference, IEntityErrorInfo<TResult>> _entities =
            new Dictionary<WeakReference, IEntityErrorInfo<TResult>>();

        public IEntityErrorInfo<TResult> Entity(TEntity entity)
        {
            CleanDeadEntries();
            var errorInfoKey = _entities.Keys.SingleOrDefault(x => x.Target == entity);
            if (errorInfoKey != null) return _entities[errorInfoKey];
            errorInfoKey = new WeakReference(entity);
            _entities.Add(errorInfoKey, new EntityErrorInfo<TResult>());
            return _entities[errorInfoKey];
        }

        /// <summary>
        /// Removes weak references from the dictionary of error infos for which the entity has been garbage collected
        /// </summary>
        private void CleanDeadEntries()
        {
            foreach (var key in _entities.Keys.Where(x => x.IsAlive == false).ToArray())
            {
                _entities.Remove(key);
            }
        }

        /// <summary>
        /// Gets or sets the validation rules provider of this instance.
        /// </summary>
        public IValidationRulesProvider<TResult> RulesProvider
        {
            get => _rulesProvider;
            set
            {
                if (_rulesProvider != value)
                {
                    ValidationRules = null;
                    _observedProperties = null;
                }

                _rulesProvider = value;
                if (value != null)
                {
                    ValidationRules = _rulesProvider.ValidationRules;
                    _observedProperties = ValidationRules.SelectMany(
                        rule => rule.Signature.Select(dep => dep.TargetProperty.Name)).ToArray();
                }
                else
                {
                    ValidationRules = null;
                }

                ValidationRuleSetChanged?.Invoke(ValidationRules,
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Method that invokes all matching validation rules for the given object and 
        /// property name.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        public void Validate(TEntity obj, string propertyName)
        {
            Validate(obj, propertyName, new List<TEntity> {obj});
        }

        /// <inheritdoc />
        /// <summary>
        /// Method that invokes all matching validation rules for the given object
        /// </summary>
        /// <param name="obj"></param>
        public void Validate(TEntity obj)
        {
            Validate(new List<TEntity> {obj});
        }

        /// <inheritdoc />
        /// <summary>
        /// Method that invokes all matching validation rules for all possible bindings given
        /// a collection of objects, an object 'obj' that should be present in any bindings, and a 
        /// (changed) property with name 'propertyName' that should be part in any signature.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <param name="objects"></param>
        public void Validate(object obj, string propertyName, IEnumerable<TEntity> objects)
        {
            if (Skip(propertyName))
            {
                return;
            }

            var type = obj.GetType();
            var rules = GetRulesByTypeAndPropertyName(type, propertyName);
            ValidateRules(rules, objects, obj);
        }

        /// <summary>
        /// Method that invokes all matching validation rules for all possible bindings given
        /// a collection of objects and an object that must be present in any selcted signature.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="objects"></param>
        public void Validate(object obj, IEnumerable<TEntity> objects)
        {
            var type = obj.GetType();
            var rules = GetRulesByType(type);
            ValidateRules(rules, objects, obj);
        }

        /// <inheritdoc />
        /// <summary>
        /// Method that invokes all matching validation rules for all possible bindings given a 
        /// collection of entities.
        /// </summary>
        /// <param name="objects"></param>
        public void Validate(IEnumerable<TEntity> objects)
        {
            ValidateRules(ValidationRules, objects, null);
        }

        /// <inheritdoc />
        /// <summary>
        /// Frees allocated resources for this validation engine.
        /// </summary>
        public void Dispose()
        {
            RulesProvider = null;
        }

        /// <inheritdoc />
        /// <summary>
        /// Returns the collection of entities that have validation errors.
        /// </summary>
        public IEnumerable<TEntity> EntitiesInError
        {
            get
            {
                return ValidationResults.Keys.SelectMany(rb => rb.DependencyBindings.Select(db => db.TargetOwnerObject))
                    .OfType<TEntity>();
            }
        }

        /// <summary>
        /// Event handler that is called when the parameterObjectBindings of any of the validation rules changes.
        /// </summary>
        public event EventHandler<ValidationResultChangedEventArgs<TEntity, TResult>> ValidationResultChanged;

        /// <inheritdoc />
        /// <summary>
        /// Occurs when the collection of validation rules changes. 
        /// Only the NotifyCollectionChangedAction.Reset action is supported
        /// </summary>
        public event NotifyCollectionChangedEventHandler ValidationRuleSetChanged;

        /// <inheritdoc />
        /// <summary>
        /// Occurs when the collection of entities in error changes. 
        /// Only the NotifyCollectionChangedAction.Reset action is supported
        /// </summary>
        public event NotifyCollectionChangedEventHandler EntitiesInErrorChanged;

        /// <summary>
        /// Method that checks if the entity has a validation error for the given set of members.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="membersInError"></param>
        /// <param name="validationResult"></param>
        /// <returns></returns>
        protected abstract bool HasValidationResult(TEntity entity, string[] membersInError, TResult validationResult);

        /// <summary>
        /// Method that clears the validation result of the given entity, for the given members.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="membersInError"></param>
        /// <param name="validationResult"></param>
        protected abstract void
            ClearValidationResult(TEntity entity, string[] membersInError, TResult validationResult);

        /// <summary>
        /// Method that sets a validation error for the given memebrs of the given entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="membersInError"></param>
        /// <param name="validationResult"></param>
        protected abstract void SetValidationResult(TEntity entity, string[] membersInError, TResult validationResult);

        /// <summary>
        /// Method that checks if the given validation result indicates a successful validation.
        /// </summary>
        /// <param name="validationResult"></param>
        /// <returns></returns>
        protected abstract bool IsValidationSuccess(TResult validationResult);

        /// <summary>
        ///  Returns an IEnumerable of validation rules that involve the given type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private IEnumerable<ValidationRule<TResult>> GetRulesByType(Type type)
        {
            return from rule in ValidationRules
                where rule.Signature.Any(dep =>
                    dep.TargetPropertyOwnerType.IsAssignableFrom(type))
                select rule;
        }

        /// <summary>
        /// Returns an IEnumerable of validation rules that involve the given property name of the given type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private IEnumerable<ValidationRule<TResult>> GetRulesByTypeAndPropertyName(Type type, string propertyName)
        {
            return from rule in ValidationRules
                where rule.Signature.Any(dep =>
                    dep.TargetPropertyOwnerType.IsAssignableFrom(type) &&
                    dep.TargetProperty.Name == propertyName)
                select rule;
        }

        /// <summary>
        /// Gets or sets the collection of validation rules for this validation engine.
        /// </summary>
        private IEnumerable<ValidationRule<TResult>> ValidationRules
        {
            get
            {
                if (_validationRules != null) return _validationRules;
                _validationRules = new List<ValidationRule<TResult>>();
                return _validationRules;
            }
            set
            {
                if (!Equals(_validationRules, value))
                {
                    _validationRules = value;
                }
            }
        }

        private IEnumerable<ValidationRule<TResult>> _validationRules;

        /// <summary>
        /// This method implements a quick to check if validation should continue,
        /// by looking up if a property plays a role in a validation rule.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private bool Skip(string propertyName)
        {
            return _observedProperties == null || _observedProperties.Contains(propertyName) == false;
        }

        /// <summary>
        /// Given a validation rule with dependency expressions and a collection of objects
        /// return a list of tuples (represented as lists) with all permutations of matching objects.
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="objects"></param>
        /// <returns></returns>
        private static IEnumerable<IEnumerable<ParameterObjectBinding>> GetRuleArgumentObjectBindings(
            ValidationRule<TResult> rule, IEnumerable<TEntity> objects)
        {
            var bindings = new List<IEnumerable<ParameterObjectBinding>>();

            var dependencyParameters = rule.GetValidationRuleDependencyParameters();

            var parameterObjectBindings =
                (from obj in objects
                    from parameter in dependencyParameters
                    where parameter.ParameterObjectType.IsInstanceOfType(obj)
                    select new ParameterObjectBinding
                    {
                        ParameterName = parameter.ParameterName,
                        ParameterObjectType = parameter.ParameterObjectType,
                        ParameterObject = obj
                    })
                .GroupBy(x => x.ParameterName);
            foreach (var group in parameterObjectBindings.ToList())
            {
                bindings.Add(group.ToList());
            }

            return GetPermutations(bindings);
        }

        /// <summary>
        /// Given a validation rule and a collection of tuples of parameter to object bindings (represented as list),
        /// create corresponding RuleBindings.
        /// We create a separate rule binding for each parameter to object binding tuple.
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="objectBindings"></param>
        /// <returns></returns>
        private static IEnumerable<RuleBinding<TResult>> GetRuleBindings(ValidationRule<TResult> rule,
            IEnumerable<IEnumerable<ParameterObjectBinding>> objectBindings)
        {
            var result = new List<RuleBinding<TResult>>();
            foreach (var objectBinding in objectBindings)
            {
                var bindings = (from dependency in rule.Signature
                    select new ValidationRuleDependencyBinding
                    {
                        ValidationRuleDependency = dependency,
                        ParameterObjectBinding =
                            objectBinding.Single(b => b.ParameterName == dependency.ParameterExpression.Name)
                    }).Distinct(new ValidationRuleDependencyBindingComparer());
                var validationRuleDependencyBindings =
                    bindings as ValidationRuleDependencyBinding[] ?? bindings.ToArray();
                if (validationRuleDependencyBindings.Length != rule.Signature.Count())
                {
                    continue;
                }

                result.Add(new RuleBinding<TResult>
                {
                    DependencyBindings = validationRuleDependencyBindings.ToArray(),
                    ValidationRule = rule
                });
            }

            return result;
        }

        /// <summary>
        /// Synthesizes all possible permutations for each collection in the provided list.
        /// That is, for a the collection
        /// { {a,b}, {c,d} }
        /// The following permutation is calculated:
        /// { {a, c}, {a, d}, {b, c}, {c, d} }
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        private static List<IEnumerable<T>> GetPermutations<T>(List<IEnumerable<T>> list)
        {
            if (!list.Any())
            {
                return list;
            }

            return GetPermutations(list.First(), list.Skip(1).ToList());
        }

        /// <summary>
        /// This method does the actual work for computating the collection of permutations.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="head"></param>
        /// <param name="tail"></param>
        /// <returns></returns>
        private static List<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> head, List<IEnumerable<T>> tail)
        {
            if (tail.Count == 0)
            {
                var newList = new List<IEnumerable<T>>();
                foreach (var e in head)
                {
                    newList.Add(new List<T> {e});
                }

                return newList;
            }

            var newhead = tail.First();
            var result = GetPermutations(newhead, tail.Skip(1).ToList());
            var list = new List<IEnumerable<T>>();
            foreach (var e in head)
            {
                foreach (var le in result)
                {
                    var tmp = le.ToList();
                    tmp.Add(e);
                    list.Add(tmp);
                }
            }

            return list;
        }

        /// <summary>
        /// Filter rules for which the collection of dependency bindings does not contain obj
        /// and rules for which any TargetOwnerObject of an dependency binings is null.
        /// </summary>
        /// <param name="bindings"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static IEnumerable<RuleBinding<TResult>> FilterRuleBindings(IEnumerable<RuleBinding<TResult>> bindings,
            object obj)
        {
            return (from binding in bindings
                where
                    binding.DependencyBindings.All(b => b.TargetOwnerObject != null)
                    &&
                    (obj == null || binding.DependencyBindings.Any(b => b.TargetOwnerObject == obj))
                select binding).ToList();
        }

        /// <summary>
        /// This is the actual validation method that invokes a collection of validation rules for a collection of validation
        /// rule bindings.
        /// For each rule, this amounts to:
        /// 1) Synthesizing the bindings of objects from the provided objects to parameters of validation rule dependencies 
        ///    defined in the signature of the validation rule.
        /// 2) Synthesizing all possible bindings for the signature of the validation rule
        /// 3) Filtering this collection to remove invalid bindings
        /// 4) For each resulting binding, invoke the validaton rule.
        /// </summary>
        /// <param name="rules"></param>
        /// <param name="objects"></param>
        /// <param name="obj"></param>
        private void ValidateRules(IEnumerable<ValidationRule<TResult>> rules, IEnumerable<TEntity> objects, object obj)
        {
            if (rules == null)
            {
                return;
            }

            objects = objects.ToArray();
            foreach (var rule in rules)
            {
                var objectBindings = GetRuleArgumentObjectBindings(rule, objects);
                var ruleBindings = GetRuleBindings(rule, objectBindings);
                var filteredBindings = FilterRuleBindings(ruleBindings, obj);
                foreach (var binding in filteredBindings)
                {
                    if (rule is AsyncValidationRule<TResult> asyncRule)
                    {
                        var validationOperation = asyncRule.EvaluateAsync(binding);
                        validationOperation.Completed += (s, a) =>
                            ProcessValidationResult(binding, validationOperation.Result);
                    }
                    else
                    {
                        var result = rule.Evaluate(binding);
                        ProcessValidationResult(binding, result);
                    }
                }
            }
        }

        /// <summary>
        /// Callback method that is called when the result of a validation rule has changed.
        /// </summary>
        private void ProcessValidationResult(RuleBinding<TResult> ruleBinding, TResult validationResult)
        {
            var bindingGroups = (from depBinding in ruleBinding.DependencyBindings
                where
                    depBinding.ValidationRuleDependency.InputOnly == false &&
                    depBinding.TargetOwnerObject is TEntity
                group depBinding by depBinding.TargetOwnerObject as TEntity).ToArray();

            TResult oldValidationResult = null;
            if (ValidationResults.ContainsKey(ruleBinding))
            {
                oldValidationResult = ValidationResults[ruleBinding];
            }

            foreach (var bindingGroup in bindingGroups)
            {
                var entity = bindingGroup.Key;
                var membersInError = bindingGroup
                    .Select(binding => binding.ValidationRuleDependency.TargetProperty.Name).Distinct().ToArray();
                if (HasValidationResult(entity, membersInError, oldValidationResult))
                    ClearValidationResult(entity, membersInError, oldValidationResult);
                if (HasValidationResult(entity, membersInError, validationResult) == false)
                    SetValidationResult(entity, membersInError, validationResult);
            }

            if (IsValidationSuccess(validationResult) && oldValidationResult != null)
            {
                ValidationResults.Remove(ruleBinding);
                EntitiesInErrorChanged?.Invoke(EntitiesInError,
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
            else
            {
                ValidationResults[ruleBinding] = validationResult;
                EntitiesInErrorChanged?.Invoke(EntitiesInError,
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }

            ValidationResultChanged?.Invoke(this,
                new ValidationResultChangedEventArgs<TEntity, TResult>(validationResult,
                    bindingGroups.Select(g => g.Key)));
        }

        /// <summary>
        /// Gets a dictionary of validation rule bindings and the corresponding validation results.
        /// </summary>
        private Dictionary<RuleBinding<TResult>, TResult> ValidationResults
        {
            get
            {
                if (_validationResults != null) return _validationResults;
                _validationResults = new Dictionary<RuleBinding<TResult>, TResult>();
                return _validationResults;
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Returns the collection of registered validation rules.
        /// </summary>
        IEnumerable IValidationEngine<TEntity, TResult>.ValidationRules => ValidationRules;

        /// <inheritdoc />
        /// <summary>
        /// This class tests for equality between ValidationRuleDependencyBinding objects.
        /// Two ValidationRuleDependencyBinding objects are equal if the target properties 
        /// of their ValidationRuleDependencies are euqals and if their TargetOwnerObjects are equal.
        /// This means that both dependencies correspond to the same property of the same owning object.
        /// </summary>
        private class ValidationRuleDependencyBindingComparer : IEqualityComparer<ValidationRuleDependencyBinding>
        {
            public bool Equals(ValidationRuleDependencyBinding x, ValidationRuleDependencyBinding y)
            {
                if (
                    x.ValidationRuleDependency.TargetProperty == y.ValidationRuleDependency.TargetProperty &&
                    x.TargetOwnerObject == y.TargetOwnerObject)
                {
                    return true;
                }

                return false;
            }

            public int GetHashCode(ValidationRuleDependencyBinding db)
            {
                var hashCode = 0;
                hashCode ^= db.ValidationRuleDependency.TargetProperty.GetHashCode();
                if (db.TargetOwnerObject != null)
                    hashCode ^= db.TargetOwnerObject.GetHashCode();
                return hashCode;
            }
        }
    }
}
