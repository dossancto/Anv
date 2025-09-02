# Anv Dependency Injection

This package is a simple dependency injection package for Anv.

## Installation

You can install this package using the following command:

```bash
dotnet add package Anv.DependencyInjection
```

## Usage

To use this package, you need to add the `AddAnvProvider` extension method to your service collection.

```csharp
using Anv.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAnvProvider();

var app = builder.Build();

app.Run();
```

Then, you can inject the `IAnvProvider` interface into your classes and use it to resolve an `AnvEnv` instance.

```csharp
public class MyClass
{
  private readonly IAnvProvider _anvProvider;

  public MyClass(IAnvProvider anvProvider)
  {
    _anvProvider = anvProvider;
  }

  public void DoSomething()
  {
    var env = _anvProvider.Resolve(AnvEnv.Production);
    // Do something with the env
  }
}
