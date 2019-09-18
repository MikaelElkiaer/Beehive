using System.Runtime.InteropServices;

namespace Beehive.Utils
{
    public class OSUtils
    {
        public static OSPlatform GetOperationSystem()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return OSPlatform.Linux;
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return OSPlatform.Windows;
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return OSPlatform.OSX;
            else
                return OSPlatform.Linux;
        }
    }
}
