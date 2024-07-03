namespace Anv.Tests;

// Usage:
public static class MyEnvs
{
    public static readonly AnvEnv MY_ENV = AnvEnv.Load("MY_ENV");
    public static readonly AnvEnv SOME_INT = AnvEnv.Load("SOME_INT");
    public static readonly AnvEnv USE_LOGS = AnvEnv.Load("USE_LOGS");
    public static readonly AnvEnv SOME_INT_WITH_FALLBACK = AnvEnv.Load("SOME_INT_NOT_DEFINED");

    public static class NESTED
    {
        public static readonly AnvEnv MY_KEY = new("NESTED_MY_KEY");

    }
}

public class UnitTest1
{
    public UnitTest1()
    {
        Environment.SetEnvironmentVariable("MY_ENV", "some-key");

        Environment.SetEnvironmentVariable("SOME_INT", "1");

        Environment.SetEnvironmentVariable("USE_LOGS", "true");

        Environment.SetEnvironmentVariable("NESTED_MY_KEY", "abc");
    }

    [Fact]
    public void Test1()
    {
        MyEnvs.MY_ENV.NotNull();

        Assert.Equal("some-key", MyEnvs.MY_ENV.NotNull());

        Assert.Equal(1, MyEnvs.SOME_INT.Get<int>());
        Assert.Equal(0, MyEnvs.SOME_INT_WITH_FALLBACK.GetDefault(0));

        Assert.Equal(true, MyEnvs.USE_LOGS.GetDefault(false));

        Assert.Equal("abc", MyEnvs.NESTED.MY_KEY);

        if (!MyEnvs.MY_ENV)
        {
            throw new Exception("if not defined fallbacks here");
        }

        if (MyEnvs.SOME_INT_WITH_FALLBACK)
        {
            throw new Exception("This item is not defined, so will no throw error");
        }

        // Envs can be used as string
        string? myEnv = MyEnvs.MY_ENV;

        // NotNull throws exception if the env is not defined
        Assert.Throws<ArgumentException>(() => MyEnvs.SOME_INT_WITH_FALLBACK.NotNull());
    }
}
