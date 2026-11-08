using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Determination
{
    [HarmonyPatch(typeof(SkillRecord))]
    [HarmonyPatch("Aptitude", MethodType.Getter)]
    public class Patch_Aptitude
    {
        public static void Prefix(ref bool __state, ref int? ___aptitudeCached)
        {
            __state = !___aptitudeCached.HasValue;
        }
        public static void Postfix(bool __state, ref int __result, SkillRecord __instance, Pawn ___pawn, ref int? ___aptitudeCached)
        {
            if (!__state)
                return;
            if (___pawn != null)
            {
                CompSoul comp = ___pawn.TryGetComp<CompSoul>();
                if (comp != null)
                {
                    SoulVariantDef soul = comp.Variant;
                    if (soul == null) return;
                    int num = soul.AptitudeFor(__instance.def);
                    ___aptitudeCached = ___aptitudeCached.HasValue
                        ? new int?(___aptitudeCached.GetValueOrDefault() + num)
                        : new int?(num);
                }
            }
            if (___aptitudeCached.HasValue)
                __result = ___aptitudeCached.Value;
        }
    }
}
