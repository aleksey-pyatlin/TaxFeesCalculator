using System.IO;
using FeesCalculator.ConsoleApplication.Utils;

namespace FeesCalculator.ConsoleApplication
{
    class HelperUtils : IHelperUtils
    {
        public string GetPath(string path, string subpath)
        {
            return Path.Combine(path, subpath);
        }
    }
}