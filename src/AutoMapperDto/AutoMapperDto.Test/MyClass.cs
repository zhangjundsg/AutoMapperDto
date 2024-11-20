namespace AutoMapperDto.Test;

public class MyClass
{
    public string? Name { get; set; }
    public int Age { get; set; }
}

[Mapper<MyClass>]
public sealed partial class MyClassDto;