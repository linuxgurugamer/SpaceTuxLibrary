using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Highlighting;
using KSP_Log;

namespace KSP_PartHighlighter
{
    public class PartHighlighter : MonoBehaviour
    {
        // Private fields

        private static Log Log = new Log("KSP_PartHighlighter");

        private static PartHighlighter Instance = null;
        int highlightCnt = 0;

        internal class HighlightParts
        {
            internal int id = -1;
            internal GameScenes loadedScene;
            internal float interval = 1.0f;
            internal float lastTimechange = Time.realtimeSinceStartup;
            internal bool highlight = false;

            internal Color highlightC = XKCDColors.Black;
            internal Color edgeHighlightColor = XKCDColors.Black;
            internal bool highlightActive = false;
            internal int highlightedParts = 0;
            internal List<Part> highlightParts;
        }

        private Dictionary<int, HighlightParts> hPartsLists = null;

        public static PartHighlighter CreatePartHighlighter()
        {
            if (Instance != null)
                return Instance;
            GameObject go = new GameObject();
            Instance = go.AddComponent<PartHighlighter>();

            Instance.Startup();
            return Instance;
        }

        //
        // Separate non-static initialiation
        //
        internal void Startup()
        {
            hPartsLists = new Dictionary<int, HighlightParts>();
        }

        bool CheckInit()
        {
            if (hPartsLists == null)
            {
                Log.Error("AddPartToHighlight, module not initialized");
                return false;
            }
            return true;
        }

        public int CreateHighlightList(float interval, Color c)
        {
            int id = CreateHighlightList(interval);
            if (id >= 0)
            {
                UpdateHighlightColors(id, c);
            }
            return id;
        }


        public int CreateHighlightList(float interval = 1f)
        {
            if (!CheckInit())
                return -1;
            // surround the creation with a try, just in case of any problems
            try
            {
                HighlightParts hp = new HighlightParts();

                hp.highlightParts = new List<Part>();
                hp.id = highlightCnt++;
                hp.interval = interval;
                hp.loadedScene = HighLogic.LoadedScene;
                hPartsLists.Add(hp.id, hp);
                return hp.id;
            }
            catch (Exception ex)
            {
                Log.Error("CreateHighlightList: " + ex.Message);
                return -1;
            }
        }

        public bool DestroyHighlightList(int id)
        {
            if (!CheckInit())
                return false;

            if (hPartsLists.ContainsKey(id))
            {
                hPartsLists.Remove(id);
                return true;
            }
            return false;
        }

        public bool SetHighlighting(int id, bool active)
        {
            if (!CheckInit())
                return false;
            hPartsLists[id].highlightActive = active;
            return true;
        }

        public bool HighlightSinglePart(Color highlightC, Color edgeHighlightColor, Part p)
        {
            if (!CheckInit())
                return false;

            try
            {
                p.SetHighlightDefault();
                p.SetHighlightType(Part.HighlightType.AlwaysOn);
                p.SetHighlight(true, false);
                p.SetHighlightColor(highlightC);
                p.highlighter.ConstantOn(edgeHighlightColor);
                p.highlighter.SeeThroughOn();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error("HighlightSinglePart: " + ex.Message + " on part");
                return false;
            }
        }

        public bool AddPartToHighlight(int id, Part p)
        {
            if (!CheckInit())
                return false;

            if (hPartsLists.ContainsKey(id))
            {
                if (hPartsLists[id].highlightParts.Contains(p))
                    return false;
                hPartsLists[id].highlightParts.Add(p);
            }
            return true;
        }

        public bool DisablePartHighlighting(int id, Part part)
        {
            if (!CheckInit())
                return false;

            if (hPartsLists[id].highlightParts.Contains(part))
            {
                try
                {
                    part.SetHighlightDefault();
                    part.SetHighlight(false, false);
                    Highlighter highlighter = part.highlighter;
                    part.highlighter.ConstantOff();
                    part.highlighter.SeeThroughOff();
                    hPartsLists[id].highlightParts.Remove(part);
                    return true;
                }
                catch (Exception ex)
                {
                    Log.Error("DisablePartHighlighting: " + ex.Message + " on part, removing part from highlight list");
                    hPartsLists[id].highlightParts.Remove(part);
                    return false;
                }
            }
            return false;
        }

        void FixedUpdate()
        {
            for (int cnt = hPartsLists.Count - 1; cnt >= 0; cnt--)
            {
                var hp = hPartsLists[cnt];
                if (HighLogic.LoadedScene == hp.loadedScene)
                {
                    if (Time.realtimeSinceStartup - hp.lastTimechange >= hp.interval)
                    {
                        hp.highlight = !hp.highlight;
                        hp.lastTimechange = Time.realtimeSinceStartup;
                        if (hp.highlight)
                            HighlightPartsOn(hp);
                        else
                            HighlightPartsOff(hp);
                    }
                }
            }
        }

        public bool UpdateHighlightColors(int id, Color newHighlightColor)
        {
            if (!CheckInit())
                return false;
            hPartsLists[id].highlightActive = true;

            hPartsLists[id].highlightC = newHighlightColor;
            hPartsLists[id].edgeHighlightColor = hPartsLists[id].highlightC;
            return true;
        }

        void HighlightPartsOn(HighlightParts hp)
        {
            if (hp.highlightActive)
            {
                for (int i = hp.highlightParts.Count - 1; i >= 0; i--)
                {
                    Part part = hp.highlightParts[i];

                    try
                    {
                        hp.highlightedParts++;
                        part.SetHighlightColor(hp.highlightC);
                        part.highlighter.ConstantOn(hp.edgeHighlightColor);
                        part.highlighter.SeeThroughOn();
                    }
                    catch (Exception ex)
                    {
                        Log.Error("HighlightPartsOn: " + ex.Message + " on part, removing part from highlight list");
                        hp.highlightParts.Remove(part);
                    }
                }

            }
        }

        void HighlightPartsOff(HighlightParts hp)
        {
            if (hp.highlightedParts > 0)
            {
                for (int i = hp.highlightParts.Count - 1; i >= 0; i--)
                {
                    try
                    {
                        hp.highlightedParts--;
                        hp.highlightParts[i].SetHighlightDefault();
                        hp.highlightParts[i].SetHighlight(false, false);
                        Highlighter highlighter = hp.highlightParts[i].highlighter;
                        hp.highlightParts[i].highlighter.ConstantOff();
                        hp.highlightParts[i].highlighter.SeeThroughOff();
                    }
                    catch (Exception ex)
                    {
                        Log.Error("HighlightPartsOff: " + ex.Message + " on part, removing part from highlight list");
                        hp.highlightParts.Remove(hp.highlightParts[i]);
                    }
                }
                if (hp.highlightedParts < 0)
                {
                    Log.Error("Too many parts unhighlighted");
                    hp.highlightedParts = 0;
                }
            }
        }
    }
}
