using System;

namespace AutoMapperDto;

/// <summary>
/// ӳ��attribute
/// </summary>
/// <typeparam name="T">��ӳ��Դ����</typeparam>

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class Mapper<T> : Attribute
{

}