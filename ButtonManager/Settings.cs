
using KSP.Localization;
using System.Collections;
using System.Reflection;



namespace ButtonManager
{
    // http://forum.kerbalspaceprogram.com/index.php?/topic/147576-modders-notes-for-ksp-12/#comment-2754813
    // search for "Mod integration into Stock Settings
    // HighLogic.CurrentGame.Parameters.CustomParams<BM>()

    public class BM : GameParameters.CustomParameterNode
    {
        public override string Title { get { return ""; } }
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return Localizer.Format("#LOC_SpaceTuxLib_2"); } }
        public override string DisplaySection { get { return Localizer.Format("#LOC_SpaceTuxLib_2"); } }
        public override int SectionOrder { get { return 1; } }
        public override bool HasPresets { get { return false; } }


        [GameParameters.CustomParameterUI("Debug mode",
            toolTip = "#LOC_SpaceTuxLib_6")]
        public bool debugMode = false;

        public override bool Interactible(MemberInfo member, GameParameters parameters)
        {
            return true;
        }
        public override IList ValidValues(MemberInfo member)
        {
            return null;
        }
    }
}
