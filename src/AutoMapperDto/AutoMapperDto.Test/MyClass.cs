using AutoMapperDto.Other;

namespace AutoMapperDto.Test;

public class MyClass
{
    public string? Name { get; set; }
    public int Age { get; set; }
    public int AAAge { get; set; }
    [Ignore]
    public List<string> List { get; set; }

}


[Mapper<AutoMapperDto.Test.MyClass>]
public partial class MyrecordDto;

[Mapper<MyClass>]
public partial class MyrecordCCC
{
    public int Age { get; set; } 
};
[Mapper<MyClass>]
public partial record Mysds(int Age);

[Mapper<MyClass_2>]
public partial class Test;


