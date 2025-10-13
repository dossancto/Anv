namespace Anv;

/// <summary>
/// Represents an environment variable with utility methods for parsing and checking its value.
/// </summary>
public sealed class AnvEnv
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AnvEnv"/> class with the specified name and value.
    /// </summary>
    /// <param name="name">The name of the environment variable.</param>
    /// <param name="val">The value of the environment variable.</param>
    public AnvEnv(string name, string? val)
    {
        Value = val;
        Name = name;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AnvEnv"/> class by loading the value from the environment.
    /// </summary>
    /// <param name="env">The name of the environment variable to load.</param>
    public AnvEnv(string env)
    {
        Value = Environment.GetEnvironmentVariable(env);
        Name = env;
    }

    /// <summary>
    /// Loads an environment variable by name.
    /// </summary>
    /// <param name="envName">The name of the environment variable to load.</param>
    /// <returns>An <see cref="AnvEnv"/> instance representing the environment variable.</returns>
    public static AnvEnv Load(string envName)
      => new(envName);

    /// <summary>
    /// Gets or sets the value of the environment variable.
    /// </summary>
    public string? Value { get; private set; }
    /// <summary>
    /// Gets or sets the name of the environment variable.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Implicitly converts an <see cref="AnvEnv"/> instance to its string value.
    /// </summary>
    /// <param name="val">The <see cref="AnvEnv"/> instance.</param>
    /// <returns>The value of the environment variable.</returns>
    public static implicit operator string?(AnvEnv val) => val.Value;

    /// <summary>
    /// Returns true if the environment variable is defined (not null).
    /// </summary>
    /// <param name="val">The <see cref="AnvEnv"/> instance.</param>
    /// <returns>True if defined; otherwise, false.</returns>
    public static bool operator true(AnvEnv val) => val.IsDefined();
    /// <summary>
    /// Returns true if the environment variable is not defined (null).
    /// </summary>
    /// <param name="val">The <see cref="AnvEnv"/> instance.</param>
    /// <returns>True if not defined; otherwise, false.</returns>
    public static bool operator false(AnvEnv val) => val.IsNotDefined();

    /// <summary>
    /// Returns true if the environment variable is not defined (null).
    /// </summary>
    /// <param name="val">The <see cref="AnvEnv"/> instance.</param>
    /// <returns>True if not defined; otherwise, false.</returns>
    public static bool operator !(AnvEnv val) => val.IsNotDefined();

    /// <summary>
    /// Returns the value if not null; otherwise, throws an <see cref="ArgumentException"/>.
    /// </summary>
    /// <returns>The value of the environment variable.</returns>
    /// <exception cref="ArgumentException">Thrown if the value is null.</exception>
    public string NotNull() => Value ?? throw new ArgumentException($"[{Name}] environment variable not found.");

    /// <summary>
    /// Attempts to parse the value to the specified type.
    /// </summary>
    /// <typeparam name="T">The type to parse to. Must implement <see cref="IParsable{T}"/>.</typeparam>
    /// <returns>The parsed value if successful; otherwise, default.</returns>
    public T? Get<T>() where T : IParsable<T>
      => T.TryParse(NotNull(), default, out var a)
      ? a
      : default;

    /// <summary>
    /// Attempts to parse the value to the specified type, returning a fallback if parsing fails.
    /// </summary>
    /// <typeparam name="T">The type to parse to. Must implement <see cref="IParsable{T}"/>.</typeparam>
    /// <param name="fallback">The fallback value to return if parsing fails.</param>
    /// <returns>The parsed value if successful; otherwise, the fallback value.</returns>
    public T GetDefault<T>(T fallback) where T : IParsable<T>
      => T.TryParse(Value, default, out var a) ? a : fallback;

    /// <summary>
    /// Determines whether the environment variable is not defined (null).
    /// </summary>
    /// <returns>True if the value is null; otherwise, false.</returns>
    public bool IsNotDefined() => Value is null;
    /// <summary>
    /// Determines whether the environment variable is defined (not null).
    /// </summary>
    /// <returns>True if the value is not null; otherwise, false.</returns>
    public bool IsDefined() => Value is not null;
}
