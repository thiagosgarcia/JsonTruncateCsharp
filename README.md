# JsonTruncateCsharp
C# json truncate functionality for limiting json Detph without trowing and exception

![Nuget](https://img.shields.io/nuget/v/JsonTruncate?label=JsonTruncate)

## Usage
There's an Extension method `SerializeObject` in `TruncateExtensions` class. 

Example:
```c#
object _obj = new
{
    Id = 1,
    Reference = new
    {
        Id = 2,
        Reference = new
        {
            Id = 3,
            Reference = new
            {
                Id = 4
            }
        }
    }
};

var result = _obj.SerializeObject(2); //This should limit json to 2 nested objects

Assert.Equal(@"{""Id"":1,""Reference"":{""Id"":2,""Reference"":{}}}", result);
```

In this case, truncated property `Reference`, will apper with value `{}` in truncated object.
