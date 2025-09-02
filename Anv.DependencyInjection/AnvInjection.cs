using Microsoft.Extensions.DependencyInjection;

namespace Anv.DependencyInjection;

/// <summary>
/// Use this interface to access AnvEnv from your code, this allow you to mock it in tests.
/// Recomended when you need mocking.
/// </summary>
public interface IAnvProvider
{
  /// <summary>
  /// Resolves an AnvEnv from the given env name.
  /// </summary>
  AnvEnv Resolve(AnvEnv env);
}

public class AnvProvider : IAnvProvider
{
  public AnvEnv Resolve(AnvEnv env)
    => env;
}

public static class AnvInjection
{
  /// <summary>
  /// Injects the Anv Provider.
  /// </summary>
  public static IServiceCollection AddAnvProvider(this IServiceCollection services)
    => services.AddSingleton<IAnvProvider, AnvProvider>();
}
