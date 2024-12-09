namespace AutoMapperDto.Test;

public class MapperTest
{
    [Mapper<MyClass>]
    public partial class MyClassDto;

    [Mapper<MyClass>]
    public partial class MyClassDDDD
    {
        public object MyObj { get; set; }
    }

    [Fact]
    public void Mapper_Test()
    {
        MyClass myClass = new()
        {
            Name = "Test",
            Age = 12,
            address = "china",
            List = ["1", "2", "3"]
        };
        var dto = myClass.AsMyClassDto();

        Assert.Equal("Test", dto.ceshi);
        Assert.Equal(12, dto.Age);
        Assert.Equal(3, dto.List.Count);

    }
    [Fact]
    public void Mapper_Config()
    {
        MyClass myClass = new()
        {
            Name = "Test",
            Age = 12,
            address = "china",
            List = ["1", "2", "3"]
        };
        var a = 1;
    }
}