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
    [HarmonyPatch(typeof(PawnGenerator), nameof(PawnGenerator.GeneratePawn), new Type[] { typeof(PawnGenerationRequest) })]
    public class Patch_AddCompSoul
    {
        public static void Postfix(Pawn __result)
        {
            CompSoul soul = __result.TryGetComp<CompSoul>();
            if (soul == null)
                return;
            if (soul.Variant != null)
                return;
            bool isMonster = SoulUtilities.IsMonster(__result);
            SoulVariantDef variant;
            List<SoulVariantDef> options = new List<SoulVariantDef>();
            if (__result.story?.traits != null)
            {
                foreach (Trait trait in __result.story.traits.allTraits)
                {
                    DeterminationModExtension extension = trait.def.GetModExtension<DeterminationModExtension>();
                    if (extension != null)
                    {
                        if (extension.forcedSoulVariant != null && extension.forcedSoulVariant.type.monster == isMonster)
                        {
                            options.Add(extension.forcedSoulVariant);
                        }
                        else if (extension.forcedSoulColor != null && extension.forcedSoulColor.monster == isMonster)
                        {
                            IEnumerable<SoulVariantDef> candidates = DefDatabase<SoulVariantDef>.AllDefs.Where(d => d.type == extension.forcedSoulColor && d.soulless == false);
                            options.Add(candidates.RandomElement());
                        }
                    }
                }
            }
            if (options.Count != 0)
            {
                variant = options.RandomElement();
            }
            else
                variant = SoulUtilities.GetSoul(isMonster);

            soul.Variant = variant;
            soul.OnPawnGenerated();
        }
    }
}
