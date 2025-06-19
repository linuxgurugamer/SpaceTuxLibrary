#if false
#region NO_LOCALIZATION
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KSP_Log;

namespace ModuleDisableUnattached
{
    class ModuleDisableUnattached : PartModule
    {
        bool modulesDisabled = false;
        List<PartModule> disabledModules;
        Log Log;

        public void Start()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                disabledModules = new List<PartModule>();
                Log = new KSP_Log.Log("ModuleDisableUnattached");
                Log.Info("Start");
            }
        }

        public void FixedUpdate()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (part.parent == null && part.children.Count == 0)
                {
                    if (!modulesDisabled)
                    {
                        modulesDisabled = true;
                        
                        foreach (PartModule m in this.part.Modules)
                        {
                            
                            if (m != this && m.enabled)
                            {
                                Log.Info("Disabling module: " + m.moduleName);
                                
                                if (m is ModuleWheelBase)
                                {
m.enabled = false;
                                m.isEnabled = false;
                                m.moduleIsEnabled = false;
                                    ((ModuleWheelBase)m).DisableSuspension(null);
                                    ((ModuleWheelBase)m).radius = 0.01f;
                                }

                                    disabledModules.Add(m);
                            }
                        }
                    }
                }
                else
                {
                    if (modulesDisabled)
                    {
                        modulesDisabled = false;
                        foreach (var m in disabledModules)
                        {
                            if (m != this)
                            {
                                if (m is ModuleWheelBase)
                                {
                                m.enabled = true;
                                m.isEnabled = true;
                                m.moduleIsEnabled = true;
                               
                                    ((ModuleWheelBase)m).EnableSuspension(null);
                                    ((ModuleWheelBase)m).radius = 0.20f;
                                }
                            }
                        }
                        disabledModules.Clear();
                    }
                }
            }
        }
    }
}

#endregion
#endif
