using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;

namespace UoN.ExpressiveAnnotations.NetCoreSample.Misc
{
    public static class Extensions
    {
        public static IHtmlContent EnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression, bool numericValues = true)
        {
            return EnumDropDownListFor(htmlHelper, expression, numericValues, null);
        }

        public static IHtmlContent EnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression, bool numericValues, object htmlAttributes)
        {
            var modelExplorer = ExpressionMetadataProvider.FromLambdaExpression(expression, htmlHelper.ViewData, htmlHelper.MetadataProvider);
            var metadata = modelExplorer.Metadata;
            var type = Nullable.GetUnderlyingType(metadata.ModelType) ?? metadata.ModelType;
            if (!type.IsEnum)
                throw new ArgumentException
                    ("Given parameter expression has to indicate enum type.", nameof(expression));
            var values = Enum.GetValues(type).Cast<TEnum>();

            var items = values.Select(value => new SelectListItem
            {
                Text = GetEnumDisplayText(value),
                Value = numericValues
                    ? string.Format(CultureInfo.InvariantCulture, $"{Convert.ChangeType(value, value.GetType().GetEnumUnderlyingType())}")
                    : string.Format(CultureInfo.InvariantCulture, $"{value}"),
                Selected = value.Equals(modelExplorer.Model)
            });

            if (modelExplorer.Metadata.IsNullableValueType) // if the enum is nullable, add an empty item to the collection
                items = new[] {new SelectListItem()}.Concat(items);

            return htmlHelper.DropDownListFor(expression, items, htmlAttributes);
        }

        private static string GetEnumDisplayText<TEnum>(TEnum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attrib = field.GetCustomAttributes<DisplayAttribute>().FirstOrDefault();
            return attrib != null ? attrib.GetName() : value.ToString();
        }
    }
}
