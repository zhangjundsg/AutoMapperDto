namespace AutoMapperDto.Test;

public class MyClass
{
    public string? Name { get; set; }
    public int Age { get; set; }
    public List<string> List { get; set; }

    public void A()
    {
    }
}

public record Myrecord(int age,string name);

[Mapper<Myrecord>]
public partial record MyrecordDto(int age);


[Mapper<MyClass>]
public sealed partial class MyClassDto
{
}

[Mapper<MyClass>]
public partial class M { }


