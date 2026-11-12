using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace Determination.Patches
{
    [HarmonyPatch(typeof(DamageWorker_AddInjury), "FinalizeAndAddInjury", [typeof(Pawn), typeof(Hediff_Injury), typeof(DamageInfo), typeof(DamageWorker.DamageResult)])]
    public class NonLethalKarma
    {
        public static void Prefix(Hediff_Injury injury, ref DamageInfo dinfo)
        {
            if (dinfo.Def.GetModExtension<DeterminationModExtension>()?.nonLethal == true)
            {
                injury.destroysBodyParts = false;
            }
        }
    }
}
