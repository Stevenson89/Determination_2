using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace Determination.Patches
{
    [HarmonyPatch(typeof(ITab_Pawn_Character), "PawnToShowInfoAbout", MethodType.Getter)]
    public static class DustEqualsCorpse
    {
        public static bool Prefix(ref Pawn? __result)
        {
            if (Find.Selector.SingleSelectedThing is Dust dust)
            {
                __result = dust.InnerPawn;
                return false;
            }
            return true;
        }
    }
}
