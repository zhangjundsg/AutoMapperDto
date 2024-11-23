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


//生成dto
public partial class MyClassDto
{
    public string? Name { get; set; }
    public int Age { get; set; }
    public List<string> List { get; set; }
}
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

### 注意

实体需要用`partial`关键字修饰为分部类：

![tips](https://github.com/zhangjundsg/zhangjundsg/blob/main/tips.png)
