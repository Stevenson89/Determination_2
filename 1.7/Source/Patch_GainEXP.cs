using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Determination
{
    [HarmonyPatch(typeof(Pawn), "DoKillSideEffects")]
    public class Patch_GainEXP
    {
        public static void AddEXPToInstigator(Pawn instigator, Pawn victim)
        {
            if (instigator.IsAnimal || victim.IsAnimal)
                return;
            int gain = LevelUtilities.GetEXPForKill(victim, instigator);
            CompSoul soul = instigator.TryGetComp<CompSoul>();
            if (soul != null)
                soul.EXP += gain;
        }
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
        {
            var codes = new List<CodeInstruction>(instructions);

            MethodInfo PawnKilledNotifier = AccessTools.Method(typeof(RecordsUtility), nameof(RecordsUtility.Notify_PawnKilled));
            MethodInfo ToInject = AccessTools.Method(typeof(Patch_GainEXP), nameof(AddEXPToInstigator));

            var matcher = new CodeMatcher(instructions, generator);
            matcher
                .MatchEndForward(new CodeMatch(i => i.Calls(PawnKilledNotifier)))
                .ThrowIfInvalid("Determination Mod Error: Transpiler of \"Patch_GainEXP\" failed. Could not find target call to Notify_PawnKilled")
                .Advance(1)
                .Insert(
                    new CodeInstruction(OpCodes.Ldloc_S, (byte)2),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call, ToInject)
                );
            return matcher.InstructionEnumeration();
        }
    }
}
