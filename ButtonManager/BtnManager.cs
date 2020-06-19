using Smooth.Collections;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using UnityEngine.Events;
using UnityEngine.UI;
using static ButtonManager.Constants;


namespace ButtonManager
{
    // Data from mod for delegate
    /// <summary>
    /// Used to do a descending sort
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    internal class DecendingComparer<TKey> : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            return y.CompareTo(x);
        }
    }

    /// <summary>
    /// List of delegates for a button use in a specific scene
    /// </summary>
    internal class SceneButton
    {
        internal int id;
        internal string buttonObjectName;
        internal GameScenes scene;
        internal UnityAction sceneDelegateMethod;


        //internal int index;
        internal IEnumerator<KeyValuePair<string, ModDelegateDefinition>> listEnumerator;

        internal SortedList<string, ModDelegateDefinition> sortedListRef;

        internal SceneButton(Button btn, UnityAction sceneDelegateMethod, int id)
        {
            this.buttonObjectName = btn.gameObject.name;
            scene = HighLogic.LoadedScene;

            var comparer = System.Collections.Generic.Comparer<string>.Create((x, y) => y.CompareTo(x));

            sortedListRef = new SortedList<string, ModDelegateDefinition>(comparer);
            this.id = id;
            this.sceneDelegateMethod = sceneDelegateMethod;
        }

        internal string UniqueKey { get { return scene.ToString() + ":" + buttonObjectName; } }

        internal static string GetUniqueKey(GameScenes scene, string buttonObjectName)
        {
            return scene.ToString() + ":" + buttonObjectName;
        }
        internal void resortList()
        {
            List<ModDelegateDefinition> tmpList = new List<ModDelegateDefinition>();

            foreach (var b in this.sortedListRef)
                tmpList.Add(b.Value);
            this.sortedListRef.Clear();
            foreach (var b in tmpList)
            {
                this.sortedListRef.Add(b.SortKey, b);
            }
        }
    }


    public class BtnManager
    {
        #region Statics
        static BtnManager instance;
         

        // All active buttons in a scene 
        static internal Dictionary<string, SceneButton> activeSceneButtons = new Dictionary<string, SceneButton>();

        // All buttons ever initialized with this mod
        static internal Dictionary<string, SceneButton> allButtonsRef = new Dictionary<string, SceneButton>();

        // All delegates ever initialized
        static internal Dictionary<string, ModDelegateDefinition> allDelegateRef = new Dictionary<string, ModDelegateDefinition>();

        // All delegates ever saved
        static internal Dictionary<string, ModDelegateDefinition> allSavedDelegateRef = new Dictionary<string, ModDelegateDefinition>();

        static internal SceneButton activeButtonListForScene = null;
        //static internal int activeDelegateCnt;

        static string[] delegateIdToSceneButton = new string[Constants.MAX_BUTTONS];

        //static int id = -1;
        static bool eventInitialized = false;
        static UnityAction launchDelegate;
        #endregion


        /// <summary>
        /// Initialize the GameEvents if not initialized.
        /// </summary>
        void InitEvent()
        {
            if (!eventInitialized)
            {
                GameEvents.onGameSceneLoadRequested.Add(onGameSceneLoadRequested);
            }
            eventInitialized = true;
        }

        /// <summary>
        /// Whenever a game scene load is changed,  clear out all the scene-specific stuff
        /// </summary>
        /// <param name="gs">scene being changed so, not use here</param>
        void onGameSceneLoadRequested(GameScenes gs)
        {
            if (HighLogic.CurrentGame == null)
                return;
            if (HighLogic.CurrentGame.Parameters.CustomParams<BM>().debugMode)
                Log.Detail("onGameSceneLoadRequested, clearing out all data");

            activeSceneButtons.Clear();
        }

        /// <summary>
        /// Initializes the listener for a mod, passing in needed information
        /// </summary>
        /// <param name="button">Game button</param>
        /// <param name="gameDelegateMethod">Method from the game which will be the last delegate called if all previous button delegates continue to call the NextDelegate method</param>
        /// <param name="modName"></param>
        public static void InitializeListener(UnityEngine.UI.Button button, UnityAction gameDelegateMethod, string modName = null)
        {
            if (instance == null)
            {
                instance = new BtnManager();
                instance.InitEvent();
            }
            if (HighLogic.CurrentGame.Parameters.CustomParams<BM>().debugMode && modName != null)
                Log.Detail("InitializeListener: " + modName);

            SceneButton sb = new SceneButton(button, gameDelegateMethod, activeSceneButtons.Count);

            // If already in this scene, just return, will happen if another mod has already initialized it
            if (activeSceneButtons.ContainsKey(sb.UniqueKey)) return;

            activeSceneButtons.Add(sb.UniqueKey, sb);
            if (!allButtonsRef.ContainsKey(sb.UniqueKey))
                allButtonsRef.Add(sb.UniqueKey, sb);

            button.onClick.RemoveAllListeners();

            // Each listener a button gets a unique delegate method
            switch (activeSceneButtons.Count - 1)
            {
                case 0: launchDelegate = new UnityAction(DelegateCall_0); break;
                case 1: launchDelegate = new UnityAction(DelegateCall_1); break;
                case 2: launchDelegate = new UnityAction(DelegateCall_2); break;
                case 3: launchDelegate = new UnityAction(DelegateCall_3); break;
                case 4: launchDelegate = new UnityAction(DelegateCall_4); break;
                case 5: launchDelegate = new UnityAction(DelegateCall_5); break;
                case 6: launchDelegate = new UnityAction(DelegateCall_6); break;
                case 7: launchDelegate = new UnityAction(DelegateCall_7); break;
                case 8: launchDelegate = new UnityAction(DelegateCall_8); break;
                case 9: launchDelegate = new UnityAction(DelegateCall_9); break;
                case 10: launchDelegate = new UnityAction(DelegateCall_10); break;
            }

            button.onClick.AddListener(launchDelegate);
            delegateIdToSceneButton[activeSceneButtons.Count - 1] = sb.UniqueKey;

        }
        /// <summary>
        /// Adds a new listener to a specified button
        /// </summary>
        /// <param name="button">Game button</param>
        /// <param name="delegateMethod">Method to call in the mod when this button is pressed</param>
        /// <param name="modName"></param>
        /// <param name="modDisplayName"></param>
        /// <param name="priority"></param>
        /// <returns>delegateID assigned to this listener</returns>
        public static int AddListener(UnityEngine.UI.Button button, UnityAction delegateMethod, string modName, string modDisplayName, int priority = 5)
        {
            if (HighLogic.CurrentGame.Parameters.CustomParams<BM>().debugMode)
                Log.Detail("AddListener: " + modName);

            string uniqueKey = SceneButton.GetUniqueKey(HighLogic.LoadedScene, button.gameObject.name);
            if (!activeSceneButtons.ContainsKey(uniqueKey))
            {
                Log.Error("Unknown button passed in to AddListener");
                return -1;
            }

            priority = Math.Max(Math.Min(priority, 10), 1);

            var bd = new ModDelegateDefinition(button, activeSceneButtons[uniqueKey].sortedListRef.Count, priority, delegateMethod, modName, modDisplayName);

            if (allSavedDelegateRef.ContainsKey(bd.UniqueKey))
            {
                bd.userPriority = allSavedDelegateRef[bd.UniqueKey].userPriority;
                allSavedDelegateRef.Remove(bd.UniqueKey);
            }
            Log.Info("AddListener 2");

            if (allDelegateRef.ContainsKey(bd.UniqueKey))
            {
                Log.Info("AddListener 2.1");
                var oldBd = allDelegateRef[bd.UniqueKey];
                oldBd.UpdateButton(button); 
                if (oldBd.SortKey != "")
                {
                    string sortKey = oldBd.SortKey;
                    activeSceneButtons[uniqueKey].sortedListRef.Add(oldBd.SortKey, oldBd);
                }
            }
            else
            {
                Log.Info("AddListener 2.2");
                allDelegateRef.Add(bd.UniqueKey, bd);
                string sortKey = bd.SortKey;
                activeSceneButtons[uniqueKey].sortedListRef.Add(bd.SortKey, bd);
            }

            return activeSceneButtons[uniqueKey].id;
        }

        // ##################################### Delegate stuff below ######################################



        /// <summary>
        /// Simple delegate destinations 
        /// </summary>

        #region Delegates
        static void DelegateCall_0() { DelegateCall(0); }
        static void DelegateCall_1() { DelegateCall(1); }
        static void DelegateCall_2() { DelegateCall(2); }
        static void DelegateCall_3() { DelegateCall(3); }
        static void DelegateCall_4() { DelegateCall(4); }
        static void DelegateCall_5() { DelegateCall(5); }
        static void DelegateCall_6() { DelegateCall(6); }
        static void DelegateCall_7() { DelegateCall(7); }
        static void DelegateCall_8() { DelegateCall(8); }
        static void DelegateCall_9() { DelegateCall(9); }
        static void DelegateCall_10() { DelegateCall(10); }
        #endregion

        /// <summary>
        /// The initial call when the button is pressed
        /// </summary>
        /// <param name="currentDelegateId"></param>
        static void DelegateCall(int currentDelegateId)
        {
            if (HighLogic.CurrentGame.Parameters.CustomParams<BM>().debugMode)
                Log.Detail("DelegateCall, currentDelegateId: " + currentDelegateId);

            var scb = activeSceneButtons[delegateIdToSceneButton[currentDelegateId]];

            scb.listEnumerator = scb.sortedListRef.GetEnumerator();
            scb.listEnumerator.MoveNext();

            CommonDelegateCall(scb);
        }
        /// <summary>
        /// Used to invoke the next delegate after the current one is done.  Call this instead of the 
        /// delegate for the button
        /// </summary>
        /// <param name="currentDelegateId">delegateID which was returned from the AddListener method</param>
        /// <param name="modName"></param>
        public static void InvokeNextDelegate(int currentDelegateId, string modName)
        {
            if (HighLogic.CurrentGame.Parameters.CustomParams<BM>().debugMode)
                Log.Detail("InvokeNextDelegate: " + modName + ", currentDelegateId: " + currentDelegateId);

            var scb = activeSceneButtons[delegateIdToSceneButton[currentDelegateId]];
            if (!scb.listEnumerator.MoveNext())
            {
                scb.sceneDelegateMethod();
            }
            else
                CommonDelegateCall(scb);
        }

        /// <summary>
        /// Common code to call a delegate
        /// </summary>
        /// <param name="scb"></param>
        static void CommonDelegateCall(SceneButton scb)
        {
            //ModDelegateDefinition bdd = scb.sortedListRef.Values[scb.index];

            var bdd = scb.listEnumerator.Current.Value;
            if (HighLogic.CurrentGame.Parameters.CustomParams<BM>().debugMode)
                Log.Detail("CommonDelegateCall mod: " + bdd.modDisplayName);

            bdd.delegateMethod();
        }
    }
}
