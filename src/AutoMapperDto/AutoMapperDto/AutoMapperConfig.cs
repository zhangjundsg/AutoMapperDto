using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace AutoMapperDto
{
    public static class AutoMapperConfig
    {
        private static readonly Dictionary<string, Dictionary<string, string>> _mappings = new();

        // 配置映射规则
        public static void Map<TSource, TDestination>(Expression<Func<TSource, object>> sourceProperty, Expression<Func<TDestination, object>> destinationProperty)
        {
            var sourcePropertyName = GetPropertyName(sourceProperty);
            var destinationPropertyName = GetPropertyName(destinationProperty);

            if (string.IsNullOrEmpty(sourcePropertyName) || string.IsNullOrEmpty(destinationPropertyName))
                return;

            var sourceType = typeof(TSource).FullName;

            if (!_mappings.ContainsKey(sourceType))
                _mappings[sourceType] = [];

            _mappings[sourceType][sourcePropertyName!] = destinationPropertyName!;
        }

        // 获取映射规则
        public static Dictionary<string, string>? GetMappings(string sourceType) =>
            _mappings.TryGetValue(sourceType, out var dic) ? dic : default;

        /// <summary>
        /// 获取属性名称
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private static string? GetPropertyName(Expression expression)
        {
            if (expression is LambdaExpression lambda)
            {
                if (lambda.Body is MemberExpression member)
                {
                    return member.Member.Name;
                }
            }
            return default;
        }
    }

}
