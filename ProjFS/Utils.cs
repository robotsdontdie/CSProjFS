using static CSProjFS.Native.ProjectedFSLib;

namespace CSProjFS
{
    public class Utils
    {
        public static int FileNameCompare(string filename1, string filename2)
        {
            return PrjFileNameCompare(filename1, filename2);
        }

        public static bool IsFileNameMatch(string name, string pattern)
        {
            return PrjFileNameMatch(name, pattern);
        }
    }
}
