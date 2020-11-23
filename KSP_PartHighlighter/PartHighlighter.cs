using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Highlighting;
using KSP_Log;

namespace KSP_PartHighlighter
{
    /// <summary>
    /// A small class which will highlight specified parts in a flashing pattern. Speed is configurable
    /// </summary>
    public class PartHighlighter : MonoBehaviour
    {
        // Private fields

        private static Log Log = new Log("KSP_PartHighlighter");

        private static PartHighlighter Instance = null;
        private static Dictionary<int, HighlightParts> hPartsLists = null;
        static int highlightCnt = 0;

        internal class HighlightParts
        {
            internal int id = -1;
            internal GameScenes loadedScene;
            internal float interval = 1.0f;
            internal float lastTimechange = Time.realtimeSinceStartup;
            internal bool highlight = false;
            internal bool alwaysOn = false;
            internal bool pause = false;

            internal Color highlightC = XKCDColors.Black;
            internal Color edgeHighlightColor = XKCDColors.Black;
            internal bool highlightActive = false;
            internal int highlightedParts = 0;
            internal List<Part> highlightParts;
        }


        /// <summary>
        /// Creates a new PartHighlighter
        /// </summary>
        /// <returns>Instance of the new PartHighlighter</returns>
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
            if (hPartsLists == null)
                hPartsLists = new Dictionary<int, HighlightParts>();
            if (Log == null)
                Log = new Log("KSP_PartHighlighter");
            Log.SetLevel(Log.LEVEL.DETAIL);
        }

        bool CheckInit()
        {
            if (hPartsLists == null)
            {
                Log.Error("CheckInit, module not initialized");
                Log.Error("Stack: " + Environment.StackTrace);

                return false;
            }
            return true;
        }

        bool CheckInit(int id)
        {
            if (!CheckInit()) return false;
            if (!hPartsLists.ContainsKey(id))
            {
                Log.Error("CheckInit, id not found: " + id);
                Log.Error("Stack: " + Environment.StackTrace);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Create a new Highlight List with the interval and Color specified
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="color"></param>
        /// <returns>HighlightList id</returns>
        public int CreateHighlightList(float interval, Color color)
        {
            int id = CreateHighlightList(interval);
            if (id >= 0)
            {
                UpdateHighlightColors(id, color);
            }
            return id;
        }

        /// <summary>
        /// Create a new HighlightList with the interval specified
        /// </summary>
        /// <param name="interval"></param>
        /// <returns>HighlightList id</returns>
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
                hp.alwaysOn = (interval <= 0);
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

        /// <summary>
        /// Set the flash interval for the specified list
        /// </summary>
        /// <param name="id"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public bool SetFlashInterval(int id, float interval)
        {
            if (!CheckInit(id))
                return false;

            hPartsLists[id].interval = interval;
            hPartsLists[id].alwaysOn = (interval <= 0);
            return true;
        }

        /// <summary>
        /// Pause highlighting on specified list
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pause"></param>
        /// <returns></returns>
        public bool PauseHighlighting(int id, bool pause)
        {
            if (!CheckInit(id))
                return false;

            if (pause)
            {
                HighlightPartsOff(hPartsLists[id]);
            }
            else
                HighlightPartsOn(hPartsLists[id]);
            hPartsLists[id].pause = pause;
            return true;
        }

        /// <summary>
        /// Destroy the specified HighlightList
        /// </summary>
        /// <param name="id"></param>
        /// <returns>True if successful</returns>
        public bool DestroyHighlightList(int id)
        {
            if (!CheckInit(id))
                return false;

            if (hPartsLists.ContainsKey(id))
            {
                hPartsLists.Remove(id);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Sets the highlighting active or inactive for the specified HighlightList
        /// </summary>
        /// <param name="id">HighlightList id</param>
        /// <param name="active"></param>
        /// <returns>True if successful</returns>
        public bool SetHighlighting(int id, bool active)
        {
            if (!CheckInit(id))
                return false;
            hPartsLists[id].highlightActive = active;
            return true;
        }

        /// <summary>
        /// Highlights a single part with the color and edgecolor
        /// </summary>
        /// <param name="highlightC">Main color for part highlight</param>
        /// <param name="edgeHighlightColor">Color for the edge highlight</param>
        /// <param name="p"></param>
        /// <returns>True if successful</returns>
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

        bool ListContainsPart(int id, Part part)
        {
            for (int i = hPartsLists[id].highlightParts.Count - 1; i >= 0; i--)
                if (hPartsLists[id].highlightParts[i].persistentId == part.persistentId)
                    return true;
            return false;
        }
        /// <summary>
        /// Return true if valid id and part is in the partlist
        /// </summary>
        /// <param name="id"></param>
        /// <param name="part"></param>
        /// <returns></returns>
        public bool HighlightListContains(int id, Part part)
        {
            if (!CheckInit(id))
                return false;

            if (hPartsLists.ContainsKey(id))
            {
                if (ListContainsPart(id, part))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Adds a part to to a HighlightList
        /// </summary>
        /// <param name="id">ID of the HighlightList</param>
        /// <param name="part"></param>
        /// <returns></returns>
        public bool AddPartToHighlight(int id, Part part)
        {
            if (!CheckInit(id))
                return false;

            if (hPartsLists.ContainsKey(id))
            {
                if (ListContainsPart(id, part))
                    return false;
                hPartsLists[id].highlightParts.Add(part);

                if (hPartsLists[id].alwaysOn)
                    HighlightPartsOn(hPartsLists[id]);
            }
            return true;
        }

        /// <summary>
        /// Disable highlighting on a single part
        /// </summary>
        /// <param name="id">ID of highlight list</param>
        /// <param name="part"></param>
        /// <returns>True if successful</returns>
        public bool DisablePartHighlighting(int id, Part part)
        {
            if (!CheckInit(id))
                return false;

            if (ListContainsPart(id, part))
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

        /// <summary>
        /// Disable highlighting on a single part
        /// </summary>
        /// <param name="id">ID of highlight list</param>
        /// <param name="part"></param>
        /// <returns>True if successful</returns>
        public bool RemovePartFromList(int id, Part part) => DisablePartHighlighting(id, part);
        
        /// <summary>
        /// Remove all parts from specified list
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool EmptyList(int id)
        {
            if (!CheckInit(id))
                return false;
            for (int i = hPartsLists[id].highlightParts.Count -1; i >= 0; i--)
            {
                if (!RemovePartFromList(id, hPartsLists[id].highlightParts[i]))
                    return false;
            }
            return true;
        }

        void FixedUpdate()
        {
            for (int cnt = hPartsLists.Count - 1; cnt >= 0; cnt--)
            {
                var hp = hPartsLists[cnt];
                if (HighLogic.LoadedScene == hp.loadedScene && !hp.alwaysOn && !hp.pause)
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

        /// <summary>
        /// Change the color for the specified HighlightList
        /// </summary>
        /// <param name="id">HighLishtList id</param>
        /// <param name="newHighlightColor"></param>
        /// <returns></returns>
        public bool UpdateHighlightColors(int id, Color newHighlightColor)
        {
            if (!CheckInit(id))
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
