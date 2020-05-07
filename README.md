# CoreMVVM

A simple, lightweight [MVVM](https://en.wikipedia.org/wiki/Model%E2%80%93view%E2%80%93viewmodel) library for [.NET Standard](https://en.wikipedia.org/wiki/List_of_.NET_libraries_and_frameworks#.NET_Standard) made with [Inversion of Control (IoC)](https://en.wikipedia.org/wiki/Inversion_of_control) in mind, with no external dependencies!

### Details

Under, a short description of some of the features of the library will be described.

#### IOC

The library provides a fairly simple IOC mechanic through the implementation of the interfaces IContainer and ILifetimeScope. However, since these interfaces are public, it's entirly possible to make custom implementations of them that wrap some other IOC service, such as Microsoft's own or another third party service.

#### IStringParser

The IStringParser service provides functionality for parsing resources containing parameters. The service does not define the syntax of the resource strings, and can thus be defined as required in custom implementations.

The default implementation uses a `${resource}` syntax. Consider the following resource string:

```
"${username} has sent you a message"
```

When resolving this string through the IStringParser, a StringTagPair can be provided with a mapping between the value `username` and the wanted value. The returned string will then be formatted correctly, with the parameter replaced by the name of a user.

Furthermore, the default implementation also defines prefixed properties. These have the syntax `${prefix:property}`. The only prefix supported today is the 'res' prefix, which recursively resolves the property as a resource.

The StringParser singleton provides easy access to a singleton IStringParser, aided by a IResourceServiceProvider singleton. Utilizing this class, one does not have to specify resource service at every call.

### But why?

There are countless IoC and MVVM libraries out there, yet I decided to make my own. The main reason was that I wanted to know and understand exactly what sort of black magic takes place behind the IoC pattern.

### Credits

This project was inspired by Thomas Freudenberg's [TinyLittleMVVM](https://github.com/thoemmi/TinyLittleMvvm), which is based on the same idea of a lightweight MVVM library.
