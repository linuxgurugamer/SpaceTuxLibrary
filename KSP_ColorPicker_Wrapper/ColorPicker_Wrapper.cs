using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection;
using System.Security.Cryptography;

using UnityEngine;
using KSPColorPicker;

#if false

namespace KSP_ColorPicker_Wrapper
{
    public class ColorPicker_Wrapper
    {
        /// <summary>
        /// Whether the Toolbar Plugin is available.
        /// </summary>
        public static bool ColorPickerAvailable        {            get            {                if (colorPickerAvailable == null)                {                    colorPickerAvailable = HasMod("KSP_ColorPicker");                }                return (bool)colorPickerAvailable;            }        }

        private static bool HasMod(string modIdent)
        {
            foreach (AssemblyLoader.LoadedAssembly a in AssemblyLoader.loadedAssemblies)
            {
                if (modIdent == a.name)
                    return true;
                //return a.assembly.GetName().Version.ToString();

            }
            return false;
        }

        private static bool? colorPickerAvailable = null;


        public static KSP_ColorPicker CreateColorPicker(Color initialColor, string texturePath, bool activePingsNeeded = true)
        {
            if (colorPickerAvailable == true)
                return KSP_ColorPicker._CreateColorPicker(initialColor, false, texturePath, 0, 0, activePingsNeeded, true);
            return null;
        }
        public static KSP_ColorPicker CreateColorPicker(Color initialColor, string texturePath = null, bool activePingsNeeded = true, bool destroyOnClose = true)
        {
            if (colorPickerAvailable == true)
                return KSP_ColorPicker._CreateColorPicker(initialColor, false, texturePath, 0, 0, activePingsNeeded, destroyOnClose);
            return null;
        }

        public static KSP_ColorPicker CreateColorPicker(Color initialColor, bool useDefinedPosition = false, string texturePath = null,
           int pLeft = 0, int pTop = 0, bool activePingsNeeded = true, bool destroyOnClose = true)
        {
            if (colorPickerAvailable == true)
                return KSP_ColorPicker._CreateColorPicker(initialColor, useDefinedPosition, texturePath, pLeft, pTop, activePingsNeeded, destroyOnClose);
            return null;
        }


 
        public void PingTime()
        {
            if (colorPickerAvailable == true)
            {
                KSP_ColorPicker.colorPickerInstance._PingTime();
            }
        }


        Color SelectedColor;
        KSP_ColorPicker colorPickerInstance;
        bool showPicker;
        bool success;
    }
}
#endif