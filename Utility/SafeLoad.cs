using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SpaceTuxUtility
{
    public static class ConfigNodeUtils
    {
        //
        // Set of methods to safeload data from ConfigNodes
        //
        public static string SafeLoad(this ConfigNode node, string value, string oldvalue)
        {
            if (!node.HasValue(value))
            {
                //Log.Info("SafeLoad string, node missing value: " + value + ", oldvalue: " + oldvalue);
                return oldvalue;
            }
            return node.GetValue(value);
        }

        public static bool SafeLoad(this ConfigNode node, string value, bool oldvalue)
        {
            if (!node.HasValue(value))
            {
                //Log.Info("SafeLoad bool, node missing value: " + value + ", oldvalue: " + oldvalue);
                return oldvalue;
            }

            try { return bool.Parse(node.GetValue(value)); }
            catch { return oldvalue; }
        }

        public static ushort SafeLoad(this ConfigNode node, string value, ushort oldvalue)
        {
            if (!node.HasValue(value))
            {
                //Log.Info("SafeLoad ushort, node missing value: " + value + ", oldvalue: " + oldvalue);
                return oldvalue;
            }
            try { return ushort.Parse(node.GetValue(value)); }
            catch { return oldvalue; }
        }

        public static int SafeLoad(this ConfigNode node, string value, int oldvalue)
        {
            if (!node.HasValue(value))
            {
                //Log.Info("SafeLoad int, node missing value: " + value + ", oldvalue: " + oldvalue);
                return oldvalue;
            }
            try { return int.Parse(node.GetValue(value)); }
            catch { return oldvalue; }
        }

        public static float SafeLoad(this ConfigNode node, string value, float oldvalue)
        {
            if (!node.HasValue(value))
            {
                //Log.Info("SafeLoad float, node missing value: " + value + ", oldvalue: " + oldvalue);
                return oldvalue;
            }
            try { return float.Parse(node.GetValue(value)); }
            catch { return oldvalue; }
        }

        public static double SafeLoad(this ConfigNode node, string value, double oldvalue)
        {
            if (!node.HasValue(value))
            {
                //Log.Info("SafeLoad double, node missing value: " + value + ", oldvalue: " + oldvalue);
                return oldvalue;
            }
            try { return Double.Parse(node.GetValue(value)); }
            catch { return oldvalue; }
        }

        public static Guid SafeLoad(this ConfigNode node, string value, Guid oldvalue)
        {
            if (!node.HasValue(value))
            {
                //Log.Info("SafeLoad Guid, node missing value: " + value + ", oldvalue: " + oldvalue);
                return oldvalue;
            }
            try { return Guid.Parse(node.GetValue(value)); }
            catch { return oldvalue; }
        }

        public static Vector3 SafeLoad(this ConfigNode node, string value, Vector3 oldvalue)
        {
            if (!node.HasValue(value))
            {
                //Log.Info("SafeLoad bool, node missing value: " + value + ", oldvalue: " + oldvalue);
                return oldvalue;
            }

            try { return ConfigNode.ParseVector3(node.GetValue(value)); }
            catch { return oldvalue; }
        }

        public static Color SafeLoad(this ConfigNode node, string value, Color oldvalue)
        {
            if (!node.HasValue(value))
            {
                //Log.Info("SafeLoad bool, node missing value: " + value + ", oldvalue: " + oldvalue);
                return oldvalue;
            }

            try { return ConfigNode.ParseColor(node.GetValue(value)); }
            catch { return oldvalue; }
        }

        public static KeyCode SafeLoad(this ConfigNode node, string value, KeyCode oldvalue)
        {
            if (!node.HasValue(value))
            {
                //Log.Info("SafeLoad string, node missing value: " + value + ", oldvalue: " + oldvalue);
                return oldvalue;
            }
            tryParseKeyCode(node.GetValue(value), oldvalue, out KeyCode kc);
            return kc;
        }

        private static bool tryParseKeyCode(string value, KeyCode defaultValue, out KeyCode result)
        {
            try
            {
                result = (KeyCode)System.Enum.Parse(typeof(KeyCode), value, true);
                return true;
            }
            catch
            {
                result = defaultValue;
                return false;
            }
        }


    }
}
