﻿/* https://github.com/uon-nuget/ExpressiveAnnotations
 * Original work Copyright (c) 2014 Jarosław Waliszko
 * Modified work Copyright (c) 2018 The University of Nottingham
 * Licensed MIT: http://opensource.org/licenses/MIT */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using UoN.ExpressiveAnnotations.NetCore.Attributes;
using UoN.ExpressiveAnnotations.NetCore.Caching;

namespace UoN.ExpressiveAnnotations.NetCore.Validators
{
    /// <summary>
    ///     Base class for expressive validators.
    /// </summary>
    /// <typeparam name="T">Any type derived from <see cref="ExpressiveAttribute" /> class.</typeparam>
    public abstract class ExpressiveValidator<T> where T : ExpressiveAttribute
    {
        /// <summary>
        ///     Constructor for expressive model validator.
        /// </summary>
        /// <param name="metadata">The model metadata.</param>
        /// <param name="attribute">The expressive attribute instance.</param>
        /// <exception cref="System.ComponentModel.DataAnnotations.ValidationException"></exception>
        protected ExpressiveValidator(ModelMetadata metadata, T attribute)
        {
            try
            {
                Debug.WriteLine($"[ctor entry] process: {Process.GetCurrentProcess().Id}, thread: {Thread.CurrentThread.ManagedThreadId}");

                var fieldId = $"{metadata.ContainerType.FullName}.{metadata.PropertyName}".ToLowerInvariant();
                AttributeFullId = $"{attribute.TypeId}.{fieldId}".ToLowerInvariant();
                AttributeWeakId = $"{typeof(T).FullName}.{fieldId}".ToLowerInvariant();
                FieldName = metadata.PropertyName;

                var item = ProcessStorage<string, CacheItem>.GetOrAdd(AttributeFullId, _ => // map cache is based on static dictionary, set-up once for entire application instance
                {                                                                           // (by design, no reason to recompile once compiled expressions)
                    Debug.WriteLine($"[cache add] process: {Process.GetCurrentProcess().Id}, thread: {Thread.CurrentThread.ManagedThreadId}");

                    IDictionary<string, Expression> fields = null;
                    attribute.Compile(metadata.ContainerType, parser =>
                    {
                        fields = parser.GetFields();
                        FieldsMap = fields.ToDictionary(x => x.Key, x => Helper.GetCoarseType(x.Value.Type));
                        ConstsMap = parser.GetConsts();
                        EnumsMap = parser.GetEnums();
                        MethodsList = parser.GetMethods();
                    }); // compile the expression associated with attribute (to be cached for subsequent invocations)

                    AssertClientSideCompatibility();

                    ParsersMap = fields
                        .Select(kvp => new
                        {
                            FullName = kvp.Key,
                            ParserAttribute = (kvp.Value as MemberExpression)?.Member.GetAttributes<ValueParserAttribute>().SingleOrDefault()
                        }).Where(x => x.ParserAttribute != null)
                        .ToDictionary(x => x.FullName, x => x.ParserAttribute.ParserName);

                    if (!ParsersMap.ContainsKey(metadata.PropertyName))
                    {
                        var currentField = metadata.ContainerType
                            .GetProperties().Single(p => metadata.PropertyName == p.Name);
                        var valueParser = currentField.GetAttributes<ValueParserAttribute>().SingleOrDefault();
                        if (valueParser != null)
                            ParsersMap.Add(new KeyValuePair<string, string>(metadata.PropertyName, valueParser.ParserName));
                    }

                    return new CacheItem
                    {
                        FieldsMap = FieldsMap,
                        ConstsMap = ConstsMap,
                        EnumsMap = EnumsMap,
                        MethodsList = MethodsList,
                        ParsersMap = ParsersMap
                    };
                });

                FieldsMap = item.FieldsMap;
                ConstsMap = item.ConstsMap;
                EnumsMap = item.EnumsMap;
                MethodsList = item.MethodsList;
                ParsersMap = item.ParsersMap;

                Expression = attribute.Expression;

                IDictionary<string, Guid> errFieldsMap;
                FormattedErrorMessage = attribute.FormatErrorMessage(metadata.GetDisplayName(), attribute.Expression, metadata.ContainerType, out errFieldsMap); // fields names, in contrast to values, do not change in runtime, so will be provided in message (less code in js)
                ErrFieldsMap = errFieldsMap;
            }
            catch (Exception e)
            {
                throw new ValidationException(
                    $"{GetType().Name}: validation applied to {metadata.PropertyName} field failed.", e);
            }
        }

        /// <summary>
        ///     Gets the expression.
        /// </summary>
        protected string Expression { get; private set; }

        /// <summary>
        ///     Gets the formatted error message.
        /// </summary>
        protected string FormattedErrorMessage { get; private set; }

        /// <summary>
        ///     Gets fields names and corresponding guid identifiers obfuscating such fields in error message string.
        /// </summary>
        protected IDictionary<string, Guid> ErrFieldsMap { get; private set; }

        /// <summary>
        ///     Gets names and coarse types of properties extracted from specified expression within given context.
        /// </summary>
        protected IDictionary<string, string> FieldsMap { get; private set; }

        /// <summary>
        ///     Gets properties names and parsers registered for them via <see cref="ValueParserAttribute" />.
        /// </summary>
        protected IDictionary<string, string> ParsersMap { get; private set; }

        /// <summary>
        ///     Gets names and values of constants extracted from specified expression within given context.
        /// </summary>
        protected IDictionary<string, object> ConstsMap { get; private set; }

        /// <summary>
        ///     Gets names and values of enums extracted from specified expression within given context.
        /// </summary>
        protected IDictionary<string, object> EnumsMap { get; private set; }

        /// <summary>
        ///     Gets names of methods extracted from specified expression within given context.
        /// </summary>
        protected IEnumerable<string> MethodsList { get; private set; }

        /// <summary>
        ///     Gets attribute strong identifier - attribute type identifier concatenated with annotated field identifier.
        /// </summary>
        private string AttributeFullId { get; set; }

        /// <summary>
        ///     Gets attribute partial identifier - attribute type name concatenated with annotated field identifier.
        /// </summary>
        private string AttributeWeakId { get; set; }

        /// <summary>
        ///     Gets name of the annotated field.
        /// </summary>
        private string FieldName { get; set; }

        /// <summary>
        ///     Attaches client validation rule to the context.
        /// </summary>
        /// <param name="context">The Client Model Validation Context.</param>
        /// <param name="type">The validation type.</param>
        /// <param name="defaultErrorMessage">The default Error Message.</param>
        /// <returns>
        ///     void
        /// </returns>
        /// <exception cref="System.ComponentModel.DataAnnotations.ValidationException"></exception>
        protected void AttachValidationRules(ClientModelValidationContext context, string type, string defaultErrorMessage)
        {
            var errorMessage = !string.IsNullOrEmpty(FormattedErrorMessage) ? FormattedErrorMessage : defaultErrorMessage;
            var uniqueValidationType = ProvideUniqueValidationType(type);

            try
            {
                MergeAttribute(context.Attributes, "data-val", "true");
                MergeAttribute(context.Attributes, $"data-val-{uniqueValidationType}", errorMessage);
                MergeAttribute(context.Attributes, $"data-val-{uniqueValidationType}-expression", Expression.ToJson());
                Debug.Assert(FieldsMap != null);
                if (FieldsMap.Any())
                    MergeAttribute(context.Attributes, $"data-val-{uniqueValidationType}-fieldsmap", FieldsMap.ToJson());
                Debug.Assert(ConstsMap != null);
                if (ConstsMap.Any())
                    MergeAttribute(context.Attributes, $"data-val-{uniqueValidationType}-constsmap", ConstsMap.ToJson());
                Debug.Assert(EnumsMap != null);
                if (EnumsMap.Any())
                    MergeAttribute(context.Attributes, $"data-val-{uniqueValidationType}-enumsmap", EnumsMap.ToJson());
                Debug.Assert(MethodsList != null);
                if (MethodsList.Any())
                    MergeAttribute(context.Attributes, $"data-val-{uniqueValidationType}-methodslist", MethodsList.ToJson());
                Debug.Assert(ParsersMap != null);
                if (ParsersMap.Any())
                    MergeAttribute(context.Attributes, $"data-val-{uniqueValidationType}-parsersmap", ParsersMap.ToJson());
                Debug.Assert(ErrFieldsMap != null);
                if (ErrFieldsMap.Any())
                    MergeAttribute(context.Attributes, $"data-val-{uniqueValidationType}-errfieldsmap", ErrFieldsMap.ToJson());
            }
            catch (Exception e)
            {
                throw new ValidationException(
                    $"{GetType().Name}: collecting of client validation rules for {FieldName} field failed.", e);
            }
        }

        protected bool MergeAttribute(IDictionary<string, string> attributes, string key, string value)
        {
            if (attributes.ContainsKey(key))
            {
                return false;
            }

            attributes.Add(key, value);
            return true;
        }


        /// <summary>
        ///     Provides unique validation type within current annotated field range, when multiple annotations are used (required for client-side).
        /// </summary>
        /// <param name="baseName">Base name.</param>
        /// <returns>
        ///     Unique validation type within current request.
        /// </returns>
        private string ProvideUniqueValidationType(string baseName)
        {
            return $"{baseName}{AllocateSuffix()}";
        }

        private string AllocateSuffix()
        {
            var count = RequestStorage.Get<int>(AttributeWeakId);
            count++;
            AssertAttribsQuantityAllowed(count);
            RequestStorage.Set(AttributeWeakId, count);
            return count == 1 ? string.Empty : char.ConvertFromUtf32(95 + count); // single lowercase letter from latin alphabet or an empty string
        }

        private void ResetSuffixAllocation()
        {
            RequestStorage.Remove(AttributeWeakId);
        }

        private void AssertClientSideCompatibility() // verify client-side compatibility of current expression
        {
            AssertNoNamingCollisionsAtCorrespondingSegments();
            AssertNoRestrictedMetaIdentifierUsed();
        }

        private void AssertNoNamingCollisionsAtCorrespondingSegments()
        {
            string name;
            int level;
            var prefix = "Naming collisions cannot be accepted by client-side";
            var collision = FieldsMap.Keys.SegmentsCollide(ConstsMap.Keys, out name, out level)
                            || FieldsMap.Keys.SegmentsCollide(EnumsMap.Keys, out name, out level)
                            || ConstsMap.Keys.SegmentsCollide(EnumsMap.Keys, out name, out level); // combination (3 2) => 3!/(2!1!) = 3
            if (collision)
                throw new InvalidOperationException(
                    $"{prefix} - {name} part at level {level} is ambiguous.");

            // instead of extending the checks above to combination (4 2), check for collisions with methods is done separately to provide more accurate messages:

            var fields = FieldsMap.Keys.Select(x => x.Split('.').First());
            name = MethodsList.Intersect(fields).FirstOrDefault();
            if (name != null)
                throw new InvalidOperationException(
                    $"{prefix} - method {name}(...) is colliding with {FieldsMap.Keys.First(x => x.StartsWith(name))} field identifier.");

            var consts = ConstsMap.Keys.Select(x => x.Split('.').First());
            name = MethodsList.Intersect(consts).FirstOrDefault();
            if (name != null)
                throw new InvalidOperationException(
                    $"{prefix} - method {name}(...) is colliding with {ConstsMap.Keys.First(x => x.StartsWith(name))} const identifier.");

            var enums = EnumsMap.Keys.Select(x => x.Split('.').First());
            name = MethodsList.Intersect(enums).FirstOrDefault();
            if (name != null)
                throw new InvalidOperationException(
                    $"{prefix} - method {name}(...) is colliding with {EnumsMap.Keys.First(x => x.StartsWith(name))} enum identifier.");
        }

        private void AssertNoRestrictedMetaIdentifierUsed()
        {
            const string meta = "__meta__";
            if (FieldsMap.Keys.Select(x => x.Split('.').First()).Contains(meta)
                || EnumsMap.Keys.Select(x => x.Split('.').First()).Contains(meta)
                || ConstsMap.Keys.Select(x => x.Split('.').First()).Contains(meta)
                || MethodsList.Contains(meta))
                throw new InvalidOperationException(
                    $"{meta} identifier is restricted for internal client-side purposes, please use different name.");
        }

        private void AssertAttribsQuantityAllowed(int count)
        {
            const int max = 27;
            if (count > max)
                throw new InvalidOperationException(
                    $"No more than {max} unique attributes of the same type can be applied for a single field or property.");
        }
    }
}
