/*
 * VesselModuleSave us authored by Diazo
 * Released under the GPL3 license
 * 
 * Original source:  https://github.com/SirDiazo/VesselModuleSave
 * Forum link: https://forum.kerbalspaceprogram.com/index.php?/topic/114414-104-vesselmodule-class-data-persistence/&tab=comments#comment-2030477
 */

using KSP.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VesselModuleSaveFramework
{
    public class VesselModuleSave : VesselModule
    {
        public string IDType() //return our identifier for the MOD using this, not for this instance of VesselModule
        {
            return this.GetType().Namespace + this.GetType().Name; //returns namespace and the inherited module name, should serve as a unique identifer for each mod using this framework
        }

        public virtual void VSMStart()
        {
            //override this to run Start() code
        }

        public virtual ConfigNode VSMSave(ConfigNode node)
        {
            return new ConfigNode(Localizer.Format("#LOC_SpaceTuxLib_18"));
            //override this to run OnSave() code
        }

        public virtual void VSMLoad(ConfigNode node)
        {
            //override this to run OnLoad() code
        }

        public virtual void VSMDestroy()
        {
            //override this for your OnDestroy() method
        }

        protected new void Start()
        {
            base.Start();
            vessel = this.GetComponent<Vessel>();
            VesselModuleStaticData.AddVesselModuleToList(this); //register this module with the VSM manager
            VSMStart(); //run start code in all VSM modules
            InternalLoadCall(VesselModuleStaticData.GetSaveNode(this)); //always want to run load code after Start() to load data
        }

        protected void OnDestroy() //this module is being destoroyed
        {
            VSMDestroy();
            VesselModuleStaticData.RemoveVesselModuleFromList(this);
        }

        public void InternalSaveCall() //internal use only, do not use, background code to Save data
        {
            ConfigNode toSaveA = VesselModuleStaticData.GetSaveNode(this);
            if (toSaveA.name == Localizer.Format("#LOC_SpaceTuxLib_18")) //if vessel not yet in file, returns an Empty config node. Have to apply vessel ID identifier before saving
            {
                toSaveA.name = this.vessel.id.ToString();
            }
            toSaveA = VSMSave(VesselModuleStaticData.GetSaveNode(this)); //call VSMSave here for the actual OnSave call, this is where the data actually gets saved
            VesselModuleStaticData.SaveNodeData(this, toSaveA); //this writes to Static data, no need for return on method
        }
        public void InternalLoadCall(ConfigNode node) //internal use only, do not use, background code to Load data
        {
            VSMLoad(node);
        }


    }
}
