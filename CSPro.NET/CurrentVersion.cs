
namespace CSPro
{
    public class CurrentVersion
    {
        public const double Version = 6.2;

        public static string VersionString
        {
            get
            {
                return string.Format("CSPro {0:F1}",Version);
            }
        }
    }
}
