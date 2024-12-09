using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMapperDto
{
    /// <summary>
    /// 映射别名
    /// </summary>
    /// <param name="name">别名</param>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class MapperAs(string name) : Attribute
    {
    }
}
