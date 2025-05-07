using dotenv.net;

namespace DexChen.UI.Utils
{
    public static class EnvLoader
    {
        public static void Load() => DotEnv.Load(options: new DotEnvOptions(
                envFilePaths: [".env"],
                ignoreExceptions: false
            ));
    }
}