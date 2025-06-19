// Copied from https://github.com/microsoft/ProjFS-Managed-API and provided under the same license.

using System.Collections.Generic;
using CSProjFS;

namespace SimpleProviderManaged
{
    /// <summary>
    /// Implements IComparer using <see cref="ProjFS.Utils.FileNameCompare(string, string)"/>.
    /// </summary>
    internal class ProjFSSorter : Comparer<string>
    {
        public override int Compare(string x, string y) => Utils.FileNameCompare(x, y);
    }
}
