using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KSP_Log;

namespace ButtonManager
{
    internal class Constants
    {
        internal const int MAX_BUTTONS = 30;
        internal const string MODNAME = "ButtonManager";
        internal const string FOLDER = "SpaceTuxLibrary";
        internal const string SAVED_PRIORITIES_FILE = "/GameData/" + FOLDER + "/PluginData/Buttons.cfg";

        internal static Log Log = new Log(Constants.MODNAME);

    }
}
