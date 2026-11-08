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
    [HarmonyPatch(typeof(Pawn), nameof(Pawn.MakeCorpse), new Type[] { typeof(Building_Grave), typeof(bool), typeof(float)})]
    public class Patch_CorpseDisintegration
    {
        public static void Postfix(ref Thing __result)
        {
            Corpse corpse = __result as Corpse;
            if (corpse.InnerPawn.genes == null)
                return;
            if (corpse.InnerPawn.genes.GenesListForReading.Any(g => g.def.HasModExtension<DeterminationModExtension>() && g.def.GetModExtension<DeterminationModExtension>().dustOnDeath == true))
            {
                CompDisintegrate disintegrate = corpse.TryGetComp<CompDisintegrate>();
                if (disintegrate != null)
                    disintegrate.active = true;
                SoulUtilities.MakeSoulless(corpse.InnerPawn);
            }
        }
    }
}
