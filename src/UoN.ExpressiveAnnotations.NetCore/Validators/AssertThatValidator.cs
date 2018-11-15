/* https://github.com/uon-nuget/ExpressiveAnnotations
 * Original work Copyright (c) 2014 Jarosław Waliszko
 * Modified work Copyright (c) 2018 The University of Nottingham
 * Licensed MIT: http://opensource.org/licenses/MIT */

using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Caching.Memory;
using UoN.ExpressiveAnnotations.NetCore.Attributes;
using UoN.ExpressiveAnnotations.NetCore.Caching;

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
        /// <param name="memoryCache">An IMemoryCache instance, scoped to the process.</param>
        /// <param name="requestCache">A RequestCache instance, scoped to the request.</param>
        /// <exception cref="System.ComponentModel.DataAnnotations.ValidationException"></exception>
        public AssertThatValidator(ModelMetadata metadata, AssertThatAttribute attribute, IMemoryCache memoryCache, IMemoryCache requestCache)
            : base(metadata, attribute, memoryCache, requestCache)
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
