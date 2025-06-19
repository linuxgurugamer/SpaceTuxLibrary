using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KSPColorPicker
{
    internal class Settings
    {
        public int selectedColorPicker = 0;

        #region NO_LOCALIZATION
        string filePath { get { return SpaceTuxUtility.AppRootPath.Path + "/GameData/SpaceTuxLibrary/PluginData/ColorPicker.cfg"; } }
        const string NODE = "COLOR_PICKER";
        const string VALUE = "selectedColorPicker";
        ConfigNode settings = null;
        #endregion
        public Settings()
        {
            Load();
        }

        internal void Load()
        {
            selectedColorPicker = 0;
            if (System.IO.File.Exists(filePath))
            {
                settings = ConfigNode.Load(filePath);
                if (settings.HasNode(NODE))
                {
                    ConfigNode node = settings.GetNode(NODE);
                    if (node.HasValue(VALUE))
                    {
                        selectedColorPicker = Int16.Parse(node.GetValue(VALUE));
                    }
                }
            }
        }

        internal void Save(int i)
        {
            selectedColorPicker = i;
            ConfigNode settings = new ConfigNode();
            ConfigNode node = new ConfigNode();
            node.AddValue(VALUE, selectedColorPicker);
            settings.AddNode(NODE, node);
            settings.Save(filePath);
        }
    }
}
