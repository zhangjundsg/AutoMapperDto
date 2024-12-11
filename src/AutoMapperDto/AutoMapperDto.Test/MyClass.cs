namespace AutoMapperDto.Test;

public class MyClass
{
    public string? Name { get; set; }
    public int Age { get; set; }
    public string? Address { get; set; }

    [MapperAs("AsList")]
    [MapperTo<List<string>>("")]
    [MapperTo("")]
    public List<string>? List { get; set; }

}