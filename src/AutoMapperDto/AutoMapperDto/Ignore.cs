using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMapperDto;

/// <summary>
/// 忽略映射
/// </summary>

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class Ignore : Attribute
{
}
