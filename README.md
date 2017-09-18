# Reinterpret.Net

Reinterpret.Net is a .NET library that allows users to take advantage of [C++-style reinterpret casts](http://en.cppreference.com/w/cpp/language/reinterpret_cast). It's built using an extension-method based API meaning integration is simple.

It supports casting from byte array to value-types and some special cases of reference types like string.

## Features
- [ ] **Performance** (library is not yet profiled/optimized)
- [x] **Netstandard1.0**
- [ ] **Netstandard2.0**
- [x] .NETFramework >= 2.0
- [x] Conversion from byte\[\] to primitives (Ex. Int32, Float)
- [ ] Conversion from primitives to byte\[\]
- [x] Conversion from byte\[\] to strings
- [ ] Conversion from strings to byte\[\]
- [x] Conversion from byte\[\] to primitive arrays
- [ ] Conversion from primitive arrays to byte\[\]
- [x] Conversion from custom struct to byte\[\]
- [ ] Conversion from byte\[\] to custom struct

## How to Use

Reinterpret.Net is designed for ease-of-use. It is implemented as an extension method for the byte\[\] Type so reinterpreting is a single method call.

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

## Setup

To compile or open Reinterpret.Net project you'll first need a couple of things:

* Visual Studio 2017

## Builds

NuGet: TODO

Myget: TODO

## Tests

TODO actual tests

|    | Linux Debug | Windows .NET Debug |
|:---|----------------:|------------------:|
|**master**| TODO | [![Build status](https://ci.appveyor.com/api/projects/status/cmwpfv2n91oxq5jn/branch/master?svg=true)](https://ci.appveyor.com/project/HelloKitty/reinterpret-net/branch/master) |
