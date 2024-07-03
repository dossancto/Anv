namespace Anv;

public sealed class AnvEnv
{
    public AnvEnv(string name, string? val)
    {
        Value = val;
        Name = name;
    }

    public AnvEnv(string env)
    {
        Value = Environment.GetEnvironmentVariable(env);
        Name = env;
    }

    public static AnvEnv Load(string envName)
      => new(envName);

    public string? Value { get; set; }
    public string Name { get; set; }

    public static implicit operator string?(AnvEnv val) => val.Value;

    public static bool operator true(AnvEnv val) => val.IsDefined();
    public static bool operator false(AnvEnv val) => val.IsNotDefined();

    public static bool operator !(AnvEnv val) => val.IsNotDefined();

    public string NotNull() => Value ?? throw new ArgumentException($"{Name} not found.");

    public T? Get<T>() where T : IParsable<T>
      => T.TryParse(NotNull(), default, out var a)
      ? a
      : default;

    public T GetDefault<T>(T fallback) where T : IParsable<T>
      => T.TryParse(Value, default, out var a) ? a : fallback;

    public bool IsNotDefined() => Value is null;
    public bool IsDefined() => Value is not null;
}
