# AutoMapperDto
使用source generator自动生成实体dto

## 如何使用

使用Visual Studio 2022

### 1 在需要映射的实体class或record上标记`Mapper<T>` `T`为映射源对象

```
public class MyClass
{
    public string? Name { get; set; }
    public int Age { get; set; }
    public List<string> List { get; set; }
}

[Mapper<MyClass>]
public partial class MyClassDto;
```
### 2 在映射时排除某属性，在源对象属性上标记`Ignore`
```
public class MyClass
{
    public string? Name { get; set; }
    public int Age { get; set; }
    [Ignore]
    public List<string> List { get; set; }
}
```
### 3 在映射时指定别名，在源对象属性上标记`MapperAs`
```
public class MyClass
{
    public string? Name { get; set; }
    public int Age { get; set; }
    [MapperAs(nameof(MyClassDto.AsList))]
    public List<string> List { get; set; }
}
```
### 4 使用`As{target}`完成源对象转换为目标对象
```
public class MapperTest
{
    [Mapper<MyClass>]
    public partial class MyClassDto;

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

        Assert.Equal("Test", dto.Name);
        Assert.Equal(12, dto.Age);
        Assert.Equal(3, dto.AsList.Count);

    }
}
```

### 注意

实体需要用`partial`关键字修饰为分部类：

![tips](https://github.com/zhangjundsg/zhangjundsg/blob/main/img/tips.png)
