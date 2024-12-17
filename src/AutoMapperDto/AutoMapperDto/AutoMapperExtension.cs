using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Text;

namespace AutoMapperDto
{
    public static class AutoMapperExtension
    {
        private static readonly ConcurrentDictionary<(Type Source, Type Target), object> _cache = [];

        public static Func<TSource, TTarget> GetOrCreateMapper<TSource, TTarget>() where TTarget : new()
        {
            var key = (typeof(TSource), typeof(TTarget));

            if (_cache.TryGetValue(key, out var cachedFunc))
            {
                return (Func<TSource, TTarget>)cachedFunc;
            }

            var func = CreateMapper<TSource, TTarget>();

            _cache[key] = func;

            return func;
        }

        private static Func<TSource, TTarget> CreateMapper<TSource, TTarget>() where TTarget : new()
        {
            var sourceParameter = Expression.Parameter(typeof(TSource), "source");
            var targetInstance = Expression.New(typeof(TTarget));

            var bindings = typeof(TTarget).GetProperties()
                .Where(targetProperty => targetProperty.CanWrite)
                .Select(targetProperty =>
                {
                    var sourceProperty = typeof(TSource).GetProperty(targetProperty.Name);
                    if (sourceProperty == null || !sourceProperty.CanRead)
                        return null;

                    var sourceValue = Expression.Property(sourceParameter, sourceProperty);
                    return Expression.Bind(targetProperty, sourceValue);
                })
                .Where(binding => binding != null)
                .ToList();

            var initializer = Expression.MemberInit(targetInstance, bindings);
            var lambda = Expression.Lambda<Func<TSource, TTarget>>(initializer, sourceParameter);

            return lambda.Compile();
        }
    }
}
