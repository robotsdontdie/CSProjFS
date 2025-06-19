﻿// Copied from https://github.com/microsoft/ProjFS-Managed-API and provided under the same license.

using System;

namespace SimpleProviderManaged
{
    public static class EnvironmentHelper
    {
        public static bool IsFullSymlinkSupportAvailable()
        {
            // Using registry instead of OSVersion due to https://docs.microsoft.com/en-us/windows/win32/api/sysinfoapi/nf-sysinfoapi-getversionexa?redirectedfrom=MSDN.
            // This code can be replaced with Environment.OSVersion on .NET Core 5 and higher.
            int build = Convert.ToInt32(Microsoft.Win32.Registry.GetValue(
                @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion",
                "CurrentBuild",
                0));

            if (build >= 19041)
            {
                return true;
            }

            return false;
        }
    }
}
