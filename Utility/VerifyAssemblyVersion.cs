using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEditor;

namespace SpaceTuxUtility
{
    public class VerifyAsmVersion
    {
        /// <summary>
        /// Check the version of the DLL against the expectedVersion
        /// </summary>
        /// <param name="name">Name of DLL to check</param>
        /// <param name="expectedVersion">String containing version to check for, in a a.b.c.d format</param>
        /// <param name="exact">If true, then check for exact match, otherwise check for min version</param>
        /// <returns>True if version found is greater than or equal to the minimum version</returns>
        public static bool CheckVersion(string name, string expectedVersion, bool exact = false)
        {
            System.Version e = ParseVersion(expectedVersion);
            return CheckVersion(name, e, exact);

        }

        /// <summary>
        /// Check the version of the DLL against the mininum required
        /// </summary>
        /// <param name="name">DLL name</param>
        /// <param name="major">Major version number</param>
        /// <param name="minor">Minor version number</param>
        /// <param name="build">Build version number</param>
        /// <param name="revision">Revision version number</param>
        /// <param name="exact">If true, then check for exact match, otherwise check for min version</param>
        /// <returns>True if version found is greater than or equal to the minimum version</returns>
        public static bool CheckVersion(string name, int major, int minor, int build, int revision, bool exact = false)
        {
            System.Version sv = new System.Version(major, minor, build, revision);
            return CheckVersion(name, sv);
        }

        /// <summary>
        /// Check the version of the DLL against the mininum required
        /// </summary>
        /// <param name="name"DLL name></param>
        /// <param name="expectedVersion">Minumum expected version</param>
        /// <param name="exact">If true, then check for exact match, otherwise check for min version</param>
        /// <returns>True if version found is greater than or equal to the minimum version</returns>
        public static bool CheckVersion(string name, System.Version expectedVersion, bool exact = false)
        {
            string version = expectedVersion.Major.ToString()+"." + expectedVersion.Minor.ToString() + "."+expectedVersion.Build.ToString()+"." + expectedVersion.Revision.ToString();
            if (VerifyAssemblyVersion(name, version, out System.Version foundVersion) != null)
            {
                if (exact)
                {
                    return (foundVersion.Major == expectedVersion.Major &&
                            foundVersion.Minor == expectedVersion.Minor &&
                            foundVersion.Build == expectedVersion.Build &&
                            foundVersion.Revision == expectedVersion.Revision);
                }
                else
                {
                    // First check Major
                    if (foundVersion.Major > expectedVersion.Major) return true;
                    if (foundVersion.Major < expectedVersion.Major) return false;

                    // Now Minor
                    if (foundVersion.Minor > expectedVersion.Minor) return true;
                    if (foundVersion.Minor < expectedVersion.Minor) return false;

                    // Now Build
                    if (foundVersion.Build > expectedVersion.Build) return true;
                    if (foundVersion.Build < expectedVersion.Build) return false;

                    // Now Revision
                    if (foundVersion.Revision > expectedVersion.Revision) return true;
                    if (foundVersion.Revision < expectedVersion.Revision) return false;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Verify the loaded assembly meets a minimum version number.
        /// </summary>
        /// <param name="name">Assembly name</param>
        /// <param name="version">Minium version</param>
        /// <returns>The assembly if the version check was successful.  If not, logs and error and returns null.</returns>
        public static Assembly VerifyAssemblyVersion(string name, string version, out System.Version versionFound)
        {

            // Logic courtesy of DMagic
            var assembly = AssemblyLoader.loadedAssemblies.SingleOrDefault(a => a.assembly.GetName().Name == name);
            if (assembly != null)
            {
                string receivedStr;

                // First try the informational version
                var ainfoV = Attribute.GetCustomAttribute(assembly.assembly, typeof(AssemblyInformationalVersionAttribute)) as AssemblyInformationalVersionAttribute;
                if (ainfoV != null)
                {
                    receivedStr = ainfoV.InformationalVersion;
                }
                // If that fails, use the product version
                else
                {
                    receivedStr = FileVersionInfo.GetVersionInfo(assembly.assembly.Location).ProductVersion;
                }

                System.Version expected = ParseVersion(version);
                System.Version received = ParseVersion(receivedStr);
                versionFound = received;

                if (received >= expected)
                {
                    UnityEngine.Debug.Log("Version check for '" + name + "' passed.  Minimum required is " + version + ", version found was " + receivedStr);
                    return assembly.assembly;
                }
                else
                {
                    UnityEngine.Debug.Log("Version check for '" + name + "' failed!  Minimum required is " + version + ", version found was " + receivedStr);
                    return null;
                }
            }
            else
            {
                UnityEngine.Debug.Log("Couldn't find assembly for '" + name + "'!");
                versionFound = null;
                return null;
            }
        }


        private static System.Version ParseVersion(string version)
        {
            Match m = Regex.Match(version, @"^[vV]?(\d+)(.(\d+)(.(\d+)(.(\d+))?)?)?");
            int major = m.Groups[1].Value.Equals("") ? 0 : Convert.ToInt32(m.Groups[1].Value);
            int minor = m.Groups[3].Value.Equals("") ? 0 : Convert.ToInt32(m.Groups[3].Value);
            int build = m.Groups[5].Value.Equals("") ? 0 : Convert.ToInt32(m.Groups[5].Value);
            int revision = m.Groups[7].Value.Equals("") ? 0 : Convert.ToInt32(m.Groups[7].Value);

            return new System.Version(major, minor, build, revision);
        }
    }
}
