using UnityEngine;
using ToolbarControl_NS;
using ButtonManager;

namespace ButtonManagerMod
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class RegisterToolbar : MonoBehaviour
    {
        void Start()
        {
            ToolbarControl.RegisterMod(PriorityUI.MODID, PriorityUI.MODNAME);

            BtnManager.allSavedDelegateRef = ModDelegateDefinition.LoadUserPriority();
        }
    }
}