//-----------------------------------------------------------------------
// <copyright file="DefaultValidatorLocator.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.Validation
{
#pragma warning disable

    using Sirenix.OdinInspector.Editor;
    using Sirenix.OdinInspector.Editor.TypeSearch;
    using Sirenix.OdinInspector.Editor.Validation.Internal;
    using Sirenix.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.Serialization;
    using UnityEngine;

    public class DefaultValidatorLocator : IValidatorLocator
    {
        internal interface IValueValidator_InternalTemporaryHack
        {
            Type ValidatedType { get; }
        }

        public class BrokenAttributeValidator : Validator
        {
            private Type brokenValidatorType;
            private string message;

            public BrokenAttributeValidator(Type brokenValidatorType, string message)
            {
                this.brokenValidatorType = brokenValidatorType;
                this.message = message;
            }

            public override void RunValidation(ref ValidationResult result)
            {
                if (result == null)
                    result = new ValidationResult();

                result.Setup = new ValidationSetup()
                {
                    Validator = this,
                    ParentInstance = this.Property.ParentValues[0],
                    Root = this.Property.SerializationRoot.ValueEntry.WeakValues[0],
                };

                result.ResultType = ValidationResultType.Error;
                result.Message = this.message;
                result.Path = this.Property.Path;
            }
        }

        private static readonly Dictionary<Type, Validator> EmptyPropertyValidatorInstances = new Dictionary<Type, Validator>(FastTypeComparer.Instance);
        private static readonly Dictionary<Type, SceneValidator> EmptySceneValidatorInstances = new Dictionary<Type, SceneValidator>(FastTypeComparer.Instance);
        public static readonly TypeSearchIndex ValidatorSearchIndex;
        public static readonly List<Type> SceneValidators;

        public static readonly DefaultValidatorLocator Instance = new DefaultValidatorLocator();

        private static readonly SpecialInstantiator[] SpecialInstantiators;

        public Func<Type, bool> CustomValidatorFilter;

        private struct SpecialInstantiator
        {
            public Type Type;
            public Func<Type, Validator> InstantiatorFunc;
        }

        static DefaultValidatorLocator()
        {
            ValidatorSearchIndex = new TypeSearchIndex() { MatchedTypeLogName = "validator" };
            SceneValidators = new List<Type>();

            HashSet<Type> specialInstantiators = null;

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var attr in assembly.GetAttributes<RegisterValidatorAttribute>())
                {
                    var type = attr.ValidatorType;
                    object customData = null;

                    if (attr.SpecialInstantiator != null)
                    {
                        if (specialInstantiators == null)
                        {
                            specialInstantiators = new HashSet<Type>(FastTypeComparer.Instance);
                        }

                        specialInstantiators.Add(attr.SpecialInstantiator);
                        customData = attr.SpecialInstantiator;
                    }

                    if (type.InheritsFrom<SceneValidator>())
                    {
                        if (type.IsAbstract)
                        {
                            Debug.LogError("The registered scene validator type " + attr.ValidatorType.GetNiceFullName() + " is abstract, so it cannot be used for validation.");
                            continue;
                        }

                        if (type.IsGenericTypeDefinition)
                        {
                            Debug.LogError("The registered scene validator type " + attr.ValidatorType.GetNiceFullName() + " is a generic type, so it cannot be used for validation; generic matching is not available for scene validators.");
                            continue;
                        }

                        if (type.GetConstructor(Type.EmptyTypes) == null)
                        {
                            Debug.LogError("The registered scene validator type " + attr.ValidatorType.GetNiceFullName() + " has no public parameterless constructor, so it cannot be used for validation.");
                            continue;
                        }

                        SceneValidators.Add(type);
                        continue;
                    }

                    if (!type.InheritsFrom<Validator>())
                    {
                        Debug.LogError("The registered validator type " + attr.ValidatorType.GetNiceFullName() + " is not derived from " + typeof(Validator).GetNiceFullName() + " or " + typeof(SceneValidator).GetNiceFullName());
                        continue;
                    }

                    if (type.ImplementsOpenGenericClass(typeof(AttributeValidator<,>)))
                    {
                        ValidatorSearchIndex.AddIndexedType(new TypeSearchInfo()
                        {
                            MatchType = type,
                            Targets = type.GetArgumentsOfInheritedOpenGenericClass(typeof(AttributeValidator<,>)),
                            TargetCategories = TypeSearchIndex.AttributeValueMatchCategoryArray,
                            Priority = attr.Priority,
                            CustomData = customData,
                        });
                    }
                    else if (type.ImplementsOpenGenericClass(typeof(AttributeValidator<>)))
                    {
                        ValidatorSearchIndex.AddIndexedType(new TypeSearchInfo()
                        {
                            MatchType = type,
                            Targets = type.GetArgumentsOfInheritedOpenGenericClass(typeof(AttributeValidator<>)),
                            TargetCategories= TypeSearchIndex.AttributeMatchCategoryArray,
                            Priority = attr.Priority,
                            CustomData = customData,
                        });
                    }
                    else if (type.ImplementsOpenGenericClass(typeof(ValueValidator<>)))
                    {
                        ValidatorSearchIndex.AddIndexedType(new TypeSearchInfo()
                        {
                            MatchType = type,
                            Targets = type.GetArgumentsOfInheritedOpenGenericClass(typeof(ValueValidator<>)),
                            TargetCategories= TypeSearchIndex.ValueMatchCategoryArray,
                            Priority = attr.Priority,
                            CustomData = customData,
                        });
                    }
                    else
                    {
                        ValidatorSearchIndex.AddIndexedType(new TypeSearchInfo()
                        {
                            MatchType = type,
                            Targets = Type.EmptyTypes,
                            TargetCategories= TypeSearchIndex.EmptyCategoryArray,
                            Priority = attr.Priority,
                            CustomData = customData,
                        });
                    }
                }
            }

            if (specialInstantiators != null)
            {
                SpecialInstantiators = new SpecialInstantiator[specialInstantiators.Count];

                int i = 0;
                foreach (var specialInstantiator in specialInstantiators)
                {
                    var func = FindInstantiateFunc(specialInstantiator);

                    if (func != null)
                    {
                        SpecialInstantiators[i++] = new SpecialInstantiator()
                        {
                            Type = specialInstantiator,
                            InstantiatorFunc = func,
                        };
                    }
                }
            }
        }

        private static Func<Type, Validator> FindInstantiateFunc(Type specialInstantiator)
        {
            var method = specialInstantiator.GetMethod("InstantiateValidator", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { typeof(Type) }, null);

            if (method == null || method.ReturnType != typeof(Validator))
            {
                Debug.LogError("Could not find method with the signature 'static Validator InstantiateValidator(Type type)' on special validator instantiator type '" + specialInstantiator.GetNiceName() + "'.");
                return null;
            }

            return (Func<Type, Validator>)Delegate.CreateDelegate(typeof(Func<Type, Validator>), method);
        }

        public bool PotentiallyHasValidatorsFor(InspectorProperty property)
        {
            var results = GetSearchResults(property);

            for (int i = 0; i < results.Count; i++)
            {
                if (results[i].Length > 0) return true;
            }

            return false;
        }

        protected readonly List<TypeSearchResult> ResultList = new List<TypeSearchResult>();
        protected readonly List<TypeSearchResult[]> SearchResultList = new List<TypeSearchResult[]>();
        protected readonly Dictionary<Type, int> AttributeNumberMap = new Dictionary<Type, int>(FastTypeComparer.Instance);

        Func<Type, bool> IValidatorLocator.CustomValidatorFilter { get => this.CustomValidatorFilter; set => this.CustomValidatorFilter = value; }

        public virtual IList<Validator> GetValidators(InspectorProperty property)
        {
            var results = GetMergedSearchResults(property);
            List<Validator> validators = new List<Validator>(results.Count);

            AttributeNumberMap.Clear();
            var attributes = property.Attributes;

            for (int i = 0; i < results.Count; i++)
            {
                var result = results[i];
                
                if (this.CustomValidatorFilter != null && !this.CustomValidatorFilter(result.MatchedType))
                    continue;

                var emptyInstance = GetEmptyPropertyValidatorInstance(result.MatchedType);

                if (!emptyInstance.CanValidateProperty(property))
                    continue;

                var valueValidator = emptyInstance as IValueValidator_InternalTemporaryHack;

                if (valueValidator != null && property.ValueEntry != null && property.ValueEntry.TypeOfValue != valueValidator.ValidatedType && typeof(Attribute).IsAssignableFrom(property.ValueEntry.TypeOfValue))
                {
                    continue;
                }

                try
                {
                    Validator validator = null;

                    if (result.MatchedInfo.CustomData != null)
                    {
                        var specialInstantiator = result.MatchedInfo.CustomData as Type;

                        if (specialInstantiator != null)
                        {
                            var instantiators = SpecialInstantiators;

                            for (int i2 = 0; i2 < instantiators.Length; i2++)
                            {
                                if (object.ReferenceEquals(instantiators[i2].Type, specialInstantiator))
                                {
                                    validator = instantiators[i2].InstantiatorFunc(result.MatchedType);
                                    break;
                                }
                            }

                            if (validator == null)
                            {
                                // The special instantiator feels we should not have this validator at the moment
                                // This is an okay outcome
                                continue;
                            }
                        }
                        else
                        {
                            Debug.LogError("Unrecognized/invalid custom data on Validator type match info for Validator type '" + result.MatchedInfo.MatchType.GetNiceName() + "'. Skipping validator instance creation.");
                            continue;
                        }
                    }
                    else
                    {
                        validator = (Validator)Activator.CreateInstance(result.MatchedType);
                    }

                    if (validator is IAttributeValidator)
                    {
                        Attribute validatorAttribute = null;
                        Type attrType = result.MatchedTargets[0];

                        if (!attrType.InheritsFrom<Attribute>())
                            throw new NotSupportedException("Please don't manually implement the IAttributeValidator interface on any types; it's a special snowflake.");

                        int number, seen = 0;
                        AttributeNumberMap.TryGetValue(attrType, out number);

                        for (int j = 0; j < attributes.Count; j++)
                        {
                            var attr = attributes[j];

                            if (attr.GetType() == attrType)
                            {
                                if (seen == number)
                                {
                                    validatorAttribute = attr;
                                    break;
                                }
                                else
                                {
                                    seen++;
                                }
                            }
                        }

                        if (validatorAttribute == null)
                            throw new Exception("Could not find the correctly numbered attribute of type '" + attrType.GetNiceFullName() + "' on property " + property.Path + "; found " + seen + " attributes of that type, but needed number " + number + ".");

                        (validator as IAttributeValidator).SetAttributeInstanceAndNumber(validatorAttribute, number);

                        number++;
                        AttributeNumberMap[attrType] = number;
                    }
                    
                    validator.Initialize(property);
                    validators.Add(validator);
                }
                catch (Exception ex)
                {
                    validators.Add(new BrokenAttributeValidator(result.MatchedType, "Creating instance of validator '" + result.MatchedType.GetNiceName() + "' failed with exception: " + ex.ToString()));
                }
            }

            return validators;
        }

        protected List<TypeSearchResult[]> GetSearchResults(InspectorProperty property)
        {
            SearchResultList.Clear();
            SearchResultList.Add(ValidatorSearchIndex.GetMatches(Type.EmptyTypes, TypeSearchIndex.EmptyCategoryArray));

            Type valueType = property.ValueEntry == null ? null : property.ValueEntry.TypeOfValue;

            if (valueType != null)
            {
                SearchResultList.Add(ValidatorSearchIndex.GetMatches(valueType, TargetMatchCategory.Value));
            }

            var attributes = property.Attributes;

            for (int i = 0; i < attributes.Count; i++)
            {
                var attributeType = attributes[i].GetType();

                SearchResultList.Add(ValidatorSearchIndex.GetMatches(attributeType, TargetMatchCategory.Attribute));

                if (valueType != null)
                {
                    SearchResultList.Add(ValidatorSearchIndex.GetMatches(attributeType, valueType, TargetMatchCategory.Attribute, TargetMatchCategory.Value));
                }
            }

            return SearchResultList;
        }

        protected List<TypeSearchResult> GetMergedSearchResults(InspectorProperty property)
        {
            var results = GetSearchResults(property);
            ResultList.Clear();
            TypeSearchIndex.MergeQueryResultsIntoList(results, ResultList);
            return ResultList;
        }

        private static Validator GetEmptyPropertyValidatorInstance(Type type)
        {
            Validator result;
            if (!EmptyPropertyValidatorInstances.TryGetValue(type, out result))
            {
                result = (Validator)FormatterServices.GetUninitializedObject(type);
                EmptyPropertyValidatorInstances[type] = result;
            }
            return result;
        }

        private static SceneValidator GetEmptySceneValidatorInstance(Type type)
        {
            SceneValidator result;
            if (!EmptySceneValidatorInstances.TryGetValue(type, out result))
            {
                result = (SceneValidator)FormatterServices.GetUninitializedObject(type);
                EmptySceneValidatorInstances[type] = result;
            }
            return result;
        }

        public IList<SceneValidator> GetSceneValidators(SceneReference scene)
        {
            List<SceneValidator> result = new List<SceneValidator>();

            foreach (var type in SceneValidators)
            {
                if (GetEmptySceneValidatorInstance(type).CanValidateScene(scene))
                {
                    var validator = (SceneValidator)Activator.CreateInstance(type);
                    validator.Initialize(scene);
                    result.Add(validator);
                }
            }

            return result;
        }
    }
}
#endif