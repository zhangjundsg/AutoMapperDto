# AutoMapperDto
使用source generator自动生成实体dto

## 如何使用

使用Visual Studio 2022

### 1 在需要映射的实体class或record上标记`Mapper<T>` `T`为映射源对象

```
[Mapper<MyClass>]
public sealed partial class MyClassDto;
```
### 注意

实体需要用`partial`关键字修饰为分部类,没关键字IDE会给出提示：

![tips]("https://github.com/zhangjundsg/zhangjundsg/blob/main/tips.png")
