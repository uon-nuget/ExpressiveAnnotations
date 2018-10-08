/* https://github.com/jwaliszko/ExpressiveAnnotations
 * Original work Copyright (c) 2014 Jarosław Waliszko
 * Modified work Copyright (c) 2018 The University of Nottingham
 * Licensed MIT: http://opensource.org/licenses/MIT */

using System;

namespace UoN.ExpressiveAnnotations.NetCore.Attributes
{
    /// <summary>
    ///     Provides a hint for client-side script pointing at parser name, which should be used for DOM field value deserialization.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class ValueParserAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ValueParserAttribute" /> class.
        /// </summary>
        /// <param name="parserName">Name of the parser.</param>
        public ValueParserAttribute(string parserName)
        {
            ParserName = parserName;
        }

        /// <summary>
        ///     Gets the name of the parser.
        /// </summary>
        public string ParserName { get; private set; }
    }
}
