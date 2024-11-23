using System;

namespace AutoMapperDto;

/// <summary>
/// 映射attribute
/// </summary>
/// <typeparam name="T">需映射源类型</typeparam>

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class Mapper<T> : Attribute
{

}