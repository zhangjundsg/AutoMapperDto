using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace AutoMapperDto
{
    public class AutoMapperConfig
    {
        private static readonly Lazy<AutoMapperConfig> _config = new(() => new AutoMapperConfig());
        public static AutoMapperConfig Config => _config.Value;

        private readonly ConcurrentDictionary<Type, Dictionary<string, string>> _mappings = [];
        private AutoMapperConfig() { }

        // 配置映射规则
        public AutoMapperConfig Map<TSource, TDestination>(Expression<Func<TSource, object>> sourceProperty, Expression<Func<TDestination, object>> destinationProperty)
            where TDestination : class
            where TSource : class
        {
            var sourcePropertyName = GetPropertyName(sourceProperty);
            var destinationPropertyName = GetPropertyName(destinationProperty);

            if (string.IsNullOrEmpty(sourcePropertyName) || string.IsNullOrEmpty(destinationPropertyName))
                throw new ArgumentNullException("属性名称为空");

            _mappings.AddOrUpdate(typeof(TSource), _ => new Dictionary<string, string> { { sourcePropertyName!, destinationPropertyName! } },
            (_, mappingDict) =>
            {
                mappingDict[sourcePropertyName!] = destinationPropertyName!;
                return mappingDict;
            });

            return this;
        }

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
