using System.IO;

namespace FeesCalculator.ConsoleApplication.Utils
{
    class HelperUtils : IHelperUtils
    {
        public string GetPath(string path, string subpath)
        {
            return Path.Combine(path, subpath);
        }
    }
}