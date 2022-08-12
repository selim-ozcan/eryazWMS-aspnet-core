using eryaz.Debugging;

namespace eryaz
{
    public class eryazConsts
    {
        public const string LocalizationSourceName = "eryaz";

        public const string ConnectionStringName = "Default";

        public const bool MultiTenancyEnabled = true;


        /// <summary>
        /// Default pass phrase for SimpleStringCipher decrypt/encrypt operations
        /// </summary>
        public static readonly string DefaultPassPhrase =
            DebugHelper.IsDebug ? "gsKxGZ012HLL3MI5" : "9bdbab7122394fc983e704b81234b4aa";
    }
}
