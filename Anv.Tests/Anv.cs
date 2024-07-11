namespace Anv;

public static partial class AppEnv
{
    public static partial class SALVE
    {
        public static readonly AnvEnv TROPA = new("SALVE.TROPA");
        public static partial class MEU_REI
        {
            public static readonly AnvEnv SOMETHING = new("SALVE.MEU_REI.SOMETHING");
            public static readonly AnvEnv DAS_NEVES = new("SALVE.MEU_REI.DAS_NEVES");
        }
    }
    public static readonly AnvEnv AWS_PROFILE = new("AWS_PROFILE");
}
