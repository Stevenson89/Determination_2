using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using Verse;

namespace Determination.Patches
{
    public class DustPatches
    {
        [HarmonyPatch(typeof(Corpse), "IngestibleNow", MethodType.Getter)]
        [HarmonyPostfix]
        public static void Postfix_Ingestible(Corpse __instance, ref bool __result)
        {
            if (__instance is Dust)
                __result = false;
        }
        public static IEnumerable<CodeInstruction> Transpiler_TryResurrectWithSideEffects(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var codeMatcher = new CodeMatcher(instructions, generator);

            codeMatcher.MatchStartForward(new CodeMatch(OpCodes.Brfalse_S));
            Label? label = (Label)codeMatcher.Operand;

            codeMatcher.MatchStartForward(
                new CodeMatch(OpCodes.Ldloc_1),
                CodeMatch.Calls(() => default(Corpse).GetComp<CompRottable>())
            )
                .ThrowIfInvalid("Determination DustPatches Could not find call to corpse.GetComp")
                .Advance(2)
                .InsertAndAdvance(new CodeInstruction(OpCodes.Dup))
                .InsertAndAdvance(new CodeInstruction(OpCodes.Brfalse_S, label));

            return codeMatcher.Instructions();
        }
    }
}
