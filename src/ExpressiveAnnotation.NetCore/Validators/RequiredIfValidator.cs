/* https://github.com/jwaliszko/ExpressiveAnnotations
 * Copyright (c) 2014 Jarosław Waliszko
 * Licensed MIT: http://opensource.org/licenses/MIT */

using System;
using System.ComponentModel.DataAnnotations;
using ExpressiveAnnotations.NetCore.Attributes;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ExpressiveAnnotations.NetCore.Validators
{
    /// <summary>
    ///     Model validator for <see cref="RequiredIfAttribute" />.
    /// </summary>
    public class RequiredIfValidator : ExpressiveValidator<RequiredIfAttribute>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RequiredIfValidator" /> class.
        /// </summary>
        /// <param name="metadata">The model metadata.</param>
        /// <param name="attribute">The expressive assertion attribute instance.</param>
        /// <exception cref="System.ComponentModel.DataAnnotations.ValidationException"></exception>
        public RequiredIfValidator(ModelMetadata metadata, RequiredIfAttribute attribute)
            : base(metadata, attribute)
        {
            AllowEmpty = attribute.AllowEmptyStrings;

            try
            {
                var propType = metadata.ModelType;
                if (propType.IsNonNullableValueType())
                    throw new InvalidOperationException(
                        $"{nameof(RequiredIfAttribute)} has no effect when applied to a field of non-nullable value type '{propType.FullName}'. Use nullable '{propType.FullName}?' version instead, or switch to {nameof(AssertThatAttribute)} otherwise.");
            }
            catch (Exception e)
            {
                throw new ValidationException(
                    $"{this.GetType().Name}: validation applied to {metadata.PropertyName} field failed.", e);
            }

        }

        private bool AllowEmpty { get; set; }

        /// <summary>
        ///     Attaches the validation rules to the context.
        /// </summary>
        /// <returns>
        ///     void
        /// </returns>
        public void AttachValidationRules()
        {
            AttachValidationRules("requiredif");

            // TODO: Find an equivalent way in .net core version to attach AllowEmpty
            // rule.ValidationParameters.Add("allowempty", AllowEmpty.ToJson());

        }
    }
}
