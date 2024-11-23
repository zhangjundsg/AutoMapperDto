namespace AutoMapperDto.Test;

public class MyClass
{
    public string? Name { get; set; }
    public int Age { get; set; }
    [Ignore]
    public int AAAge { get; set; }
    public List<string> List { get; set; }

}

[Mapper<MyClass>]
public partial class MyClassDto
{
    public string? Name { get; set; }
}
[Mapper<AutoMapperDto.Other.MyClass_2>]
public partial class MyClassDto2
{
    public string? Name { get; set; }
}
