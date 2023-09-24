using FluentValidation.Internal;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

namespace Application.Configuration.FluentValidation
{
    /// <summary>
    /// Property name resolver for camelCase for Fluent validation
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CamelCasePropertyNameResolver
    {

        public static string ResolvePropertyName(Type type, MemberInfo memberInfo, LambdaExpression expression)
        {
            return ToCamelCase(DefaultPropertyNameResolver(type, memberInfo, expression));
        }

        static string DefaultPropertyNameResolver(Type type, MemberInfo memberInfo, LambdaExpression expression)
        {
            if (expression != null)
            {
                var chain = PropertyChain.FromExpression(expression);
                if (chain.Count > 0) return chain.ToString();
            }

            if (memberInfo != null)
                return memberInfo.Name;

            return null;
        }

        static string ToCamelCase(string text)
        {
            if (string.IsNullOrEmpty(text) || !char.IsUpper(text[0]))
                return text;

            var chars = text.ToCharArray();

            for (var i = 0; i < chars.Length; i++)
            {
                if (i == 1 && !char.IsUpper(chars[i]))
                    break;

                var hasNext = i + 1 < chars.Length;
                if (i > 0 && hasNext && !char.IsUpper(chars[i + 1]))
                    break;

                chars[i] = char.ToLower(chars[i], CultureInfo.InvariantCulture);
            }

            return new string(chars);
        }
    }
}
