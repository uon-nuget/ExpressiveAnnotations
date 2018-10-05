/* https://github.com/MmmBerry/ExpressiveAnnotations
 * Original work Copyright (c) 2014 Jarosław Waliszko
 * Modified work Copyright (c) 2018 University of Nottingham
 * Licensed MIT: http://opensource.org/licenses/MIT */

using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using UoN.ExpressiveAnnotations.NetCore.Attributes;

namespace UoN.ExpressiveAnnotations.NetCore.Validators
{
    /// <summary>
    ///     Model validator for <see cref="AssertThatAttribute" />.
    /// </summary>
    public class AssertThatValidator : ExpressiveValidator<AssertThatAttribute>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="AssertThatValidator" /> class.
        /// </summary>
        /// <param name="metadata">The model metadata.</param>
        /// <param name="attribute">The expressive assertion attribute instance.</param>
        /// <exception cref="System.ComponentModel.DataAnnotations.ValidationException"></exception>
        public AssertThatValidator(ModelMetadata metadata, AssertThatAttribute attribute)
            : base(metadata, attribute)
        {
        }

        /// <summary>
        ///     Attaches the validation rules to the context.
        /// </summary>
        /// <returns>
        ///     void
        /// </returns>
        public void AttachValidationRules(ClientModelValidationContext context, string defaultErrorMessage)
        {
            AttachValidationRules(context, "assertthat", defaultErrorMessage);
        }
    }
}
