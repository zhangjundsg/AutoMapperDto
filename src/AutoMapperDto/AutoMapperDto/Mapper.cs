using System;

namespace AutoMapperDto;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class Mapper<T> : Attribute
{

}