namespace AutoMapperDto.Test;

public class MyClass
{
    [MapperAs("ceshi")]
    public string? Name { get; set; }
    public int Age { get; set; }
    public string address { get; set; }
    public List<string> List { get; set; }

}

public class MyDto
{
    public string? Name { get; set; }
    public int Age { get; set; }
    public string other { get; set; }
}

public static class Extensions
{
    public static MyDto AsMydto(this MyClass myClass)
    {
        var dot = new MyDto { Age = myClass.Age, Name = myClass.Name };

        return dot;
    }
}