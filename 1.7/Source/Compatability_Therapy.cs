using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Determination
{
    //[HarmonyPatchCategory("TherapyCompat")]
    //[HarmonyPatch(AccessTools.TypeByName("ProgressionTherapy.StatPart_MentalStability"))]
    //[HarmonyPatch("TryGetNeed")]

    public class Compatability_Therapy
    {
        public static bool Prefix(ref bool __result, StatRequest req)
        {
            if (req.Thing is Pawn pawn && pawn.Dead)
            {
                __result = false;
                return false;
            }
            return true;
        }
    }
}
