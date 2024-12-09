using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMapperDto
{
    /// <summary>
    /// 映射到指定属性
    /// </summary>
    /// <param name="name">属性名称</param>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public sealed class MapperTo(string name) : Attribute
    {

    }
    /// <summary>
    /// 映射到指定属性
    /// </summary>
    /// <typeparam name="T">映射类型</typeparam>
    /// <param name="name">属性名称</param>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public sealed class MapperTo<T>(string name) : Attribute
    {

    }
}
