using KSP.Localization;
using KSP.UI.Screens;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

using ClickThroughFix;
using ToolbarControl_NS;
using static ButtonManager.Constants;

namespace ButtonManager
{

    [KSPAddon(KSPAddon.Startup.AllGameScenes, true)]
    public class PriorityUI : MonoBehaviour
    {
        const int WIDTH = 400;
        const int HEIGHT = 300;

        static Texture2D upArrow = null;
        static Texture2D downArrow = null;
        internal GUIContent upContent;
        internal GUIContent downContent;

        void Start()
        {
            DontDestroyOnLoad(this);
            _windowId = 21398734;
            _windowPosition = new Rect((Screen.width - WIDTH) / 2, (Screen.height - HEIGHT) / 2, WIDTH, HEIGHT);

            if (upArrow == null)
            {
                upArrow = new Texture2D(2, 2);
                if (ToolbarControl.LoadImageFromFile(ref upArrow, KSPUtil.ApplicationRootPath + "GameData/" + FOLDER + "/PluginData/Textures/up"))
                    upContent = new GUIContent("", upArrow, "");
                else
                    upContent = new GUIContent("^", null, "");
            }
            if (downArrow == null)
            {
                downArrow = new Texture2D(2, 2);
                if (ToolbarControl.LoadImageFromFile(ref downArrow, KSPUtil.ApplicationRootPath + "GameData/" + FOLDER + "/PluginData/Textures/down"))
                    downContent = new GUIContent("", downArrow, "");
                else
                    downContent = new GUIContent(Localizer.Format("#LOC_SpaceTuxLib_1"), null, "");
            }
            #region NO_LOCALIZATION
            InvokeRepeating("CheckButtons", 0f, 1f);
            #endregion
            StartCoroutine(CheckButtons());
        }

        ToolbarControl toolbarControl = null;
        bool isVisible = false;

        internal const string MODID = "ButtonPriority_NS";
        internal const string MODNAME = "Button Priority";
        private Rect _windowPosition;
        private int _windowId;
        const int butW = 19;

        
        #region NO_LOCALIZATION
        private void SetupToolbar()
        {
            toolbarControl = gameObject.AddComponent<ToolbarControl>();
            toolbarControl.AddToAllToolbars(Toggle, Toggle,
                ApplicationLauncher.AppScenes.SPACECENTER |
                ApplicationLauncher.AppScenes.FLIGHT |
                ApplicationLauncher.AppScenes.MAPVIEW |
                ApplicationLauncher.AppScenes.VAB |
                ApplicationLauncher.AppScenes.SPH |
                ApplicationLauncher.AppScenes.TRACKSTATION,
                MODID,
                "ButtonManagerBtn",
                FOLDER + "/PluginData/Textures/buttonManager-38",
                FOLDER + "/PluginData/Textures/buttonManager-24",
                MODNAME
            );
        }
        #endregion

        void Toggle()
        {
            isVisible = !isVisible;
            if (isVisible) // && !DescendingComparer<ButtonDelegate>.useUserPriorityForSort)
            {
                selectedButtonMods = null;
            }
        }

        IEnumerator CheckButtons()
        {
            while (true)
            {
                bool multipleDelegates = false;
                foreach (ButtonManager.SceneButton b in BtnManager.activeSceneButtons.Values)
                {
                    if (b.sortedListRef.Count > 1)
                    {
                        if (toolbarControl == null)
                            SetupToolbar();
                        multipleDelegates = true;
                        break;
                    }
                }

                if ( !multipleDelegates &&  toolbarControl != null)
                {
                    toolbarControl.OnDestroy();
                    Destroy(toolbarControl);
                }

                yield return new WaitForSeconds(1f);
            }
        }
    


        static GUIStyle buttonStyleUp, buttonStyleDown;
        static bool styleInitted = false;

        void OnGUI()
        {
            // need to hide if nthing
            if (isVisible)
            {
                if (!styleInitted)
                {
                    buttonStyleUp = new GUIStyle(GUI.skin.label);
                    buttonStyleUp.padding = new RectOffset(0, 0, 0, 0);
                    //buttonStyleUp.margin = new RectOffset(0, 0, 0, 0);
                    buttonStyleUp.margin.top = GUI.skin.button.margin.top;
                    //buttonStyleUp.border = new RectOffset(0, 0, 0, 0);

                    buttonStyleDown = new GUIStyle(GUI.skin.label);
                    buttonStyleDown.padding = new RectOffset(0, 0, 0, 0);
                    buttonStyleDown.margin = new RectOffset(0, 0, 0, 0);
                    buttonStyleDown.margin.left = buttonStyleUp.margin.left;
                    //buttonStyleDown.border = new RectOffset(0, 0, 0, 0);
                }
                GUI.skin = HighLogic.Skin;

                _windowPosition = ClickThruBlocker.GUILayoutWindow(_windowId, _windowPosition, Display, Localizer.Format("#LOC_SpaceTuxLib_2"));

            }
        }

        Vector2 btnScrollPos, modScrollPos;
        internal ButtonManager.SceneButton selectedButtonMods = null;
        bool resortNeeded = true;
        void Display(int id)
        {
            if (BtnManager.activeSceneButtons.Count == 1)
            {
                selectedButtonMods = BtnManager.activeSceneButtons.Values.First();
            }
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();

            GUILayout.Label(Localizer.Format("#LOC_SpaceTuxLib_3"));

            btnScrollPos = GUILayout.BeginScrollView(btnScrollPos);
            foreach (ButtonManager.SceneButton b in BtnManager.activeSceneButtons.Values)
            {
                var s = b.buttonObjectName;
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(s))
                {
                    selectedButtonMods = b;
                    resortNeeded = true;
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();

            GUILayout.EndVertical();

            if (selectedButtonMods != null && resortNeeded)
            {
                selectedButtonMods.resortList();
                ModDelegateDefinition.SaveUserPriority(BtnManager.allSavedDelegateRef);
                resortNeeded = false;
            }

            GUILayout.BeginVertical();
            if (selectedButtonMods != null)
            {
                modScrollPos = GUILayout.BeginScrollView(modScrollPos);
                foreach (var m in selectedButtonMods.sortedListRef)
                {
                    ModDelegateDefinition bdd = null; // =  BtnManager.activeSceneButtons[br.uniqueKey];
                    bdd = m.Value;
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(bdd.modDisplayName + ":" + bdd.userPriority);
                        GUILayout.BeginVertical();
                        if (GUILayout.Button(upArrow, buttonStyleUp, GUILayout.Width(butW)))
                        {
                            bdd.userPriority++;
                            resortNeeded = true;
                        }
                        if (GUILayout.Button(downArrow, buttonStyleDown, GUILayout.Width(butW)))
                        {
                            bdd.userPriority--;
                            resortNeeded = true;
                        }
                        GUILayout.EndVertical();
                        GUILayout.EndHorizontal();
                    }
                }
                GUILayout.EndScrollView();
            }
            else
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(Localizer.Format("#LOC_SpaceTuxLib_4"));
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(Localizer.Format("#LOC_SpaceTuxLib_5")))
            {
                isVisible = false;
            }
            GUILayout.EndHorizontal();
            GUI.DragWindow();
        }
    }
}
