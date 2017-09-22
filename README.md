# Reinterpret.Net

Reinterpret.Net supports >=.NETFramework2.0 and .NET Core.

Reinterpret.Net is a .NET library that allows users to take advantage of [C++-style reinterpret casts](http://en.cppreference.com/w/cpp/language/reinterpret_cast) in .NET. It's built using a collection of generic extension methods meaning integration is simple.

It supports casting from bytes to primitives or strings and from primitives and strings to bytes and even supports primitive arrays.

## Features
- [x] **Performance**
- [x] **Netstandard1.1**
- [ ] **Netstandard2.0**
- [x] .NETFramework >= 2.0
- [x] Conversion from byte\[\] to primitives (Ex. Int32, Float)
- [x] Conversion from primitives to byte\[\]
- [x] Conversion from byte\[\] to string
- [x] Conversion from string to byte\[\]
- [x] Conversion from byte\[\] to primitive arrays
- [x] Conversion from primitive arrays to byte\[\]

## How to Use

Reinterpret.Net is designed for ease-of-use. It is implemented as generic extension methods for the byte\[\] Type, primitive arrays and strings so reinterpreting is simple.

Converting from a byte\[\] to an int value (4 bytes)
```csharp
byte[] bytes = GetBytes();
int intValue = bytes.Reinterpret<int>();
```

Converting from a byte\[\] to a string (default unicode encoding)
```csharp
byte[] bytes = GetBytes();
string result = bytes.Reinterpret();
```

Converting from a byte\[\] to a primitive array
```csharp
byte[] bytes = GetBytes();
int[] result = bytes.ReinterpretToArray<int>();
```

The extension method API allows you to perform operations that feel like LINQ
Letting you reinterpret from an int32 array to a UTF16 string if you so choose.
```csharp
int32[] values = GetValues()
string result = values
    .Reinterpret() //to bytes
    .ReinterpretToString(); //to string
```
### High Performance (UNSAFE)

Reinterpret.Net also offers afew select high performance/unsafe extensions that are incredibly performant and require little to no allocations.

These operations are **DANGEROUS** if you're not careful. They will leave the original object in an invalid state and if not handled properly you can modify and invalidate the internal representation of a string. However this is truly in the spirit of reinterpret_cast.

Reinterprets byte\[\] into int\[\] with no allocation instantly
```csharp
byte[] bytes = GetBytes();
int[] result = bytes.ReinterpretWithoutPerserving();
```

Reinterprets byte\[\] into string with no allocation instantly (technically produces a mutable string)
```csharp
byte[] bytes = GetBytes();
string result = bytes.ReinterpretToStringWithoutPreserving();
```

## Performance Comparisions

Performance differs between framework versions. BitConverter in older versions of .NETFramework is slower and in the .NET Core Corefx repo it's implemented in a significantly different way. All these effect the comparisions but Reinterpret.NET is usually still faster, especially the dangerous no allocation verion, with only BlockCopy being competitive due its unmanaged implementation. However for some unknown reason which doesn't appear to be IL related, could be GC or JIT, the performance of reinterpret suffers on the .NET3.5 and .NET2.0 platforms. In fact it fallback to some official .NET libraries in some areas due to the performance being better. I think it can be explored with more advanced profiling/testing tools. However right now .NET2.0/3.5 support was a bonus, not a true goal.

**.NET Core and >=.NETFramework4.0**

![Showcase](https://i.imgur.com/U97V9Vr.png "Perf")
![Showcase](https://i.imgur.com/ktVaxic.png "Perf")
![Showcase](https://i.imgur.com/fTXbDjt.png "Perf")

**.NETFramework2.0 - .NETFramework3.5**

![Showcase](https://i.imgur.com/Cz8AWG1.png "Perf")
![Showcase](https://i.imgur.com/OTk2X6k.png "Perf")
![Showcase](https://i.imgur.com/vLtBBiK.png "Perf")

## Setup

To compile or open Reinterpret.Net project you'll first need a couple of things:

* Visual Studio 2017
* ilasm.exe (For compiling .il files)

## Builds

NuGet: [Reinterpret.Net](https://www.nuget.org/packages/Reinterpret.Net/)

Myget: [![hellokitty MyGet Build Status](https://www.myget.org/BuildSource/Badge/hellokitty?identifier=ae62f610-d20e-43d6-b0de-23563c551b75)](https://www.myget.org/)

## Tests

|    | Linux Debug | Windows .NET Debug |
|:---|----------------:|------------------:|
|**master**| TODO | [![Build status](https://ci.appveyor.com/api/projects/status/cmwpfv2n91oxq5jn/branch/master?svg=true)](https://ci.appveyor.com/project/HelloKitty/reinterpret-net/branch/master) |
