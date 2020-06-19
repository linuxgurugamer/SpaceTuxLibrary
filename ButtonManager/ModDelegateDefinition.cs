using KSP.IO;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.UI;
using static ButtonManager.Constants;

namespace ButtonManager
{
    /// <summary>
    /// This stores all the data for a specific mod for a specific button
    /// It also containts the methods used to save and load the userPriority
    /// </summary>
    internal class ModDelegateDefinition
    {

        internal int entryOrder;
        internal int priority;
        internal int userPriority;
        internal UnityEngine.UI.Button button;
        internal UnityAction delegateMethod;
        internal string modName;
        internal string modDisplayName;
        internal GameScenes scene;

        internal void UpdateButton(Button button)
        {
            this.button = button;
        }

        /// <summary>
        /// Instantiates new ButtonDelegate, adds to savedbuttons if not there yet
        /// </summary>
        /// <param name="button"></param>
        /// <param name="id"></param>
        /// <param name="priority"></param>
        /// <param name="delegateMethod"></param>
        /// <param name="modName"></param>
        /// <param name="modDisplayName"></param>
        internal ModDelegateDefinition(UnityEngine.UI.Button button, int entryOrder, int priority, UnityAction delegateMethod, string modName, string modDisplayName)
        {
            this.button = button;
            this.entryOrder = entryOrder;
            this.priority = priority;
            userPriority = priority;
            this.delegateMethod = delegateMethod;
            this.modName = modName;
            this.modDisplayName = modDisplayName;
            this.scene = HighLogic.LoadedScene;

            Log.Info("button: " + button.gameObject.name);
        }
        internal ModDelegateDefinition(int userPriority, string modName, GameScenes scene)
        {
            this.userPriority = userPriority;
            this.modName = modName;
            this.scene = HighLogic.LoadedScene;
        }

        /// <summary>
        /// Generates a key used for sorting
        /// </summary>
        internal string SortKey { get { return userPriority.ToString("D2") + ":" + (10 - entryOrder).ToString("D2") + ":" + button.gameObject.name + ":" + modName + ":" + scene.ToString(); } }

        /// <summary>
        /// Generates a unique key for each entry
        /// </summary>
        internal string UniqueKey { get { return scene.ToString() + ":" + button.gameObject.name + ":" + modName; } }

        /// <summary>
        /// Saves the user priority set for all buttons
        /// </summary>
        internal static void SaveUserPriority(Dictionary<string, ModDelegateDefinition> allDelegateRef)
        {
            if (HighLogic.CurrentGame.Parameters.CustomParams<BM>().debugMode)
                Log.Detail("SaveUserPriority");
            ConfigNode node = new ConfigNode(Constants.MODNAME);
            foreach (var a in allDelegateRef.Values)
            {
                ConfigNode buttonNode = new ConfigNode();
                buttonNode.AddValue("userPriority", a.userPriority);
                buttonNode.AddValue("modName", a.modName);
                buttonNode.AddValue("scene", a.scene);
                node.AddNode(buttonNode);
            }
            ConfigNode file = new ConfigNode();
            file.AddNode(Constants.MODNAME, node);
            file.Save(KSPUtil.ApplicationRootPath + Constants.SAVED_PRIORITIES_FILE);
        }

        /// <summary>
        /// Load the saved priorities
        /// </summary>
        /// <returns></returns>
        internal static Dictionary<string, ModDelegateDefinition> LoadUserPriority()
        {
            Log.Info("LoadUserPriority");
            Dictionary<string, ModDelegateDefinition> allSavedDelegateRef = new Dictionary<string, ModDelegateDefinition>();
            if (System.IO.File.Exists(KSPUtil.ApplicationRootPath + Constants.SAVED_PRIORITIES_FILE))
            {
                ConfigNode file = ConfigNode.Load(KSPUtil.ApplicationRootPath + Constants.SAVED_PRIORITIES_FILE);
                ConfigNode node = file.GetNode(Constants.MODNAME);
                foreach (var buttonNode in node.GetNodes())
                {
                    int userPriority = int.Parse(buttonNode.GetValue("userPriority"));
                    string modName = buttonNode.GetValue("modName");

                    GameScenes scene = GameScenes.CREDITS;
                    buttonNode.TryGetEnum("scene", ref scene, GameScenes.CREDITS);

                    ModDelegateDefinition mdd = new ModDelegateDefinition(userPriority, modName, scene);
                    //mdd.scene = scene;

                    {
                        Log.Info("User Priority Loaded: " + mdd.userPriority + ", modName: " + mdd.modName + ", scene: " + mdd.scene.ToString() + ",  UniqueKey: " + mdd.UniqueKey);

                    }
                    allSavedDelegateRef.Add(mdd.UniqueKey, mdd);
                }
            }
            return allSavedDelegateRef;
        }
    }
}
