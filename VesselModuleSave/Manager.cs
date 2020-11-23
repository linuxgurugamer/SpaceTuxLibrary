/*
 * VesselModuleSave us authored by Diazo
 * Released under the GPL3 license
 * 
 * Original source:  https://github.com/SirDiazo/VesselModuleSave
 * Forum link: https://forum.kerbalspaceprogram.com/index.php?/topic/114414-104-vesselmodule-class-data-persistence/&tab=comments#comment-2030477
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace VesselModuleSaveFramework
{
    public static class VesselModuleStaticData //store data
    {
        private static Dictionary<string, ConfigNode> VesselModuleConfigNodes; //break out each mod using this framework into it's own confignode for ease of use
        private static List<VesselModule> VesselModulesLoaded; //all our currently loaded vesselmodules, aka all vessels currently loaded in flight

        static VesselModuleStaticData() //initialize our lists on start
        {
            VesselModuleConfigNodes = new Dictionary<string, ConfigNode>();
            VesselModulesLoaded = new List<VesselModule>();
        }

        public static void ClearData() //wipe data, called on game load
        {
            VesselModuleConfigNodes.Clear();
            VesselModulesLoaded.Clear();
        }

        public static void LoadRoutine(ConfigNode node) //load data from .sfs file, runs on GameLoad event
        {
            foreach (ConfigNode modNode in node.nodes)
            {
                if (!VesselModuleConfigNodes.ContainsKey(modNode.name)) //make sure dictionary doesn't already have this mod loaded
                {
                    VesselModuleConfigNodes.Add(modNode.name, modNode);
                }
                else
                {
                    VesselModuleConfigNodes[modNode.name] = modNode; //this should never be hit as it means there was duplicate data in the save file, here for error trap
                    Debug.Log("VesseModuleSave duplicate data found on load");
                }
            }
        }

        public static ConfigNode SaveRoutine() //save data to .sfs, called on GameSave event
        {
            foreach (VesselModuleSave vsm in VesselModulesLoaded)
            {
                vsm.InternalSaveCall();
            }
            ConfigNode vmNode = new ConfigNode("VMSNode");
            foreach (KeyValuePair<string, ConfigNode> data in VesselModuleConfigNodes)
            {
                ConfigNode nodeToAdd = new ConfigNode(data.Key); //name the node correctly so the loading works
                nodeToAdd = data.Value;
                vmNode.AddNode(nodeToAdd);
            }
            return vmNode;
        }

        public static void SaveNodeData(VesselModuleSave vsm, ConfigNode node) //take data from VSM module Save routine and write to static data
        {
            ConfigNode modNode = new ConfigNode(vsm.IDType()); //make our mod node, name is IDType
            if (VesselModuleConfigNodes.ContainsKey(vsm.IDType())) //do we already have an entry for this mod?
            {
                modNode = VesselModuleConfigNodes[vsm.IDType()]; //yes, refer to existing node
            }
            else
            {
                VesselModuleConfigNodes.Add(vsm.IDType(), modNode); //no, add mod to our mod list
            }
            if (modNode.HasNode(vsm.Vessel.id.ToString())) //is this vessel already existant?
            {
                modNode.RemoveNodes(vsm.Vessel.id.ToString()); //remove vessel if it exists, static node has old data in it
            }
            node.name = vsm.Vessel.id.ToString();
            modNode.AddNode(node); //add node to static data, this adds to VesselModuleConfigNodes via reference
        }

        public static void AddVesselModuleToList(VesselModuleSave vsmModule) //add vesselmodule to list of loaded modules, called in VSM.Start
        {
            if (!VesselModulesLoaded.Contains(vsmModule)) //module should only be in the list once so check
            {
                VesselModulesLoaded.Add(vsmModule);
            }
        }

        public static void RemoveVesselModuleFromList(VesselModuleSave vsmModule) //remove vesselmodule from list of loaded modules, called in VSM.destroy
        {
            if (VesselModulesLoaded.Contains(vsmModule)) //module should only be in the list once so check
            {
                VesselModulesLoaded.Remove(vsmModule);
            }
        }

        public static ConfigNode GetSaveNode(VesselModuleSave vsm) //return the correct node for an instance of a VesselModuleSave
        {
            if (VesselModuleConfigNodes.Where(d => d.Key == vsm.IDType()).Count() >= 1) //does this Mod have a node?
            {
                ConfigNode modNode = VesselModuleConfigNodes.Where(d => d.Key == vsm.IDType()).First().Value;
                if (modNode.HasNode(vsm.Vessel.id.ToString())) //does this vessel exist?
                {
                    return modNode.GetNode(vsm.Vessel.id.ToString());
                }
            }
            return new ConfigNode("Empty"); //vessel does not exist
        }

        public static void RefreshVesselData() //internal load called by this mod, required to ensure the race condition does not cause problems, refresh all data in all loaded VesselModules
        {
            foreach (VesselModuleSave vsm in VesselModulesLoaded)
            {
                ConfigNode toLoad = VesselModuleConfigNodes.Where(d => d.Key == vsm.IDType()).First().Value.GetNode(vsm.Vessel.id.ToString());
                if (toLoad != null)
                {
                    vsm.InternalLoadCall(toLoad);
                }
            }
        }

        public static void KillVessel(string vsl) //remove vessel from static data storage on it's death
        {
            foreach (KeyValuePair<string, ConfigNode> data in VesselModuleConfigNodes)
            {
                data.Value.RemoveNodes(vsl);
            }
        }
    }


    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class VesselModuleSaveManager : MonoBehaviour
    {

        public void Start()
        {
            GameEvents.onGameStateSave.Add(GameSaveTrigger);
            GameEvents.onGameStateLoad.Add(GameLoadTrigger);
            GameEvents.onVesselDestroy.Add(VesselDestroyTrigger);
            GameEvents.onVesselRecovered.Add(VesselRecoverTrigger);
            GameObject.DontDestroyOnLoad(this);
        }

        private void VesselDestroyTrigger(Vessel vsl)
        {
            VesselModuleStaticData.KillVessel(vsl.id.ToString());
        }

        private void VesselRecoverTrigger(ProtoVessel vsl, bool b)
        {
            VesselModuleStaticData.KillVessel(vsl.vesselID.ToString());
        }



        private void GameSaveTrigger(ConfigNode node) //called on GameSave event, refresh all data from loaded vessels and save to .sfs
        {
            //need to call save routines here
            if (node.HasNode("VMSNode")) //note that we do not load data at this point, our data storage is static so we know what's in the save file is old, invalid data
            {
                node.RemoveNodes("VMSNode"); //should only ever be on VMSnode in a file, remove all nodes to error trap it
            }
            node.AddNode(VesselModuleStaticData.SaveRoutine());
        }
        private void GameLoadTrigger(ConfigNode node) //load data from .sfs file and then refresh any already loaded vessels, may be no vessels loaded depending on race conditions
        {
            VesselModuleStaticData.ClearData();
            ConfigNode vmNode = new ConfigNode("VMSNode");
            if (node.HasNode("VMSNode"))
            {
                vmNode = node.GetNode("VMSNode");
            }
            VesselModuleStaticData.LoadRoutine(vmNode); //load data into static data module, okay to pass empty node
            VesselModuleStaticData.RefreshVesselData(); //refresh data in any VesselModuleSave that have already loaded somehow

        }


    }


}
