namespace AutoMapperDto.Test;

public class MyClass
{
    public string? Name { get; set; }
    public int Age { get; set; }
    public List<string> List { get; set; }

    public void A()
    {
        var a = new MyClassDto { Age = 10, Name = "张三", List = [] };
        var b = new MyrecordDto { age = 12, name = "里斯" };
    }
}

public record Myrecord(int age,string name);

[Mapper<Myrecord>]
public partial record MyrecordDto();


[Mapper<MyClass>]
public sealed partial class MyClassDto;



