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
string result = bytes.ReinterpretToString();
```
```csharp
byte[] bytes = GetBytes();
string result = bytes.Reinterpret(Encoding.Unicode);
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

## Setup

To compile or open Reinterpret.Net project you'll first need a couple of things:

* Visual Studio 2017

## Builds

NuGet: TODO

Myget: [![hellokitty MyGet Build Status](https://www.myget.org/BuildSource/Badge/hellokitty?identifier=ae62f610-d20e-43d6-b0de-23563c551b75)](https://www.myget.org/)

## Tests

TODO actual tests

|    | Linux Debug | Windows .NET Debug |
|:---|----------------:|------------------:|
|**master**| TODO | [![Build status](https://ci.appveyor.com/api/projects/status/cmwpfv2n91oxq5jn/branch/master?svg=true)](https://ci.appveyor.com/project/HelloKitty/reinterpret-net/branch/master) |
