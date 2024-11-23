namespace AutoMapperDto.Test;

public class MyClass
{
    public string? Name { get; set; }
    public int Age { get; set; }
    public int AAAge { get; set; }
    [Ignore]
    public List<string> List { get; set; }

}


[Mapper<MyClass>]
public partial class MyrecordDto;



