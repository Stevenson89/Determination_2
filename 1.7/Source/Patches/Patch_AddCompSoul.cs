using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Determination.Patches
{
    [HarmonyPatch(typeof(PawnGenerator), nameof(PawnGenerator.GeneratePawn), new Type[] { typeof(PawnGenerationRequest) })]
    public class Patch_AddCompSoul
    {
        public static void Postfix(Pawn __result)
        {
            SoulUtilities.AssignPawnSoul(__result);
            __result.TryGetComp<CompSoul>()?.OnPawnGenerated();
        }
    }
}
