using System;

//
// This is a replacement for the KSPUtil.ApplicationRootPath, which doesn't rely on
// Unity.  This is safe to call at any time, not only in a Unity method and after KSP has started
//
namespace SpaceTuxUtility
{
    public static class AppRootPath
    {
        public static string Path { get { return AppDomain.CurrentDomain.BaseDirectory; } }
    }
}
