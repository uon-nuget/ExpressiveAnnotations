/* https://github.com/jwaliszko/ExpressiveAnnotations
 * Original work Copyright (c) 2014 Jarosław Waliszko
 * Modified work Copyright (c) 2018 The University of Nottingham
 * Licensed MIT: http://opensource.org/licenses/MIT */

using System.Collections.Generic;
using System.Linq.Expressions;
using UoN.ExpressiveAnnotations.NetCore.Analysis;

namespace UoN.ExpressiveAnnotations.NetCore.Functions
{
    /// <summary>
    ///     Functions source.
    /// </summary>
    public interface IFunctionsProvider
    {
        /// <summary>
        ///     Gets functions for the <see cref="Parser" />.
        /// </summary>
        /// <returns>
        ///     Registered functions.
        /// </returns>
        IDictionary<string, IList<LambdaExpression>> GetFunctions();
    }
}
