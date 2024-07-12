# Anv

Anv is a library that provides an easy way to use Environment Variables in C# applications.

Anv adds Helper methods for your environment variables. With better errors when a variable was not found, get default values if not defined and more.

## Usage:

```c#
namespace Anv;

public static class AppEnv
{
    // Define a variable property
    public static readonly AnvEnv MY_ENV = new("MY_ENV");
}
```

So you can easily use this env

```c#
// throws a detailed error message if the Env is not defined
var myEnv = AppEnv.MY_ENV.NotNull();
```

Since your environment variables are defined within classes in your code, your application will throw a compilation error if the environment variable name is misspelled.
