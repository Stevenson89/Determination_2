using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace Determination
{
    [HarmonyPatch(typeof(Corpse), nameof(Corpse.GetGizmos))]
    public class Patch_CorpseGizmos
    {
        static IEnumerable<Gizmo> Postfix(IEnumerable<Gizmo> gizmos, Corpse __instance)
        {
            foreach (Gizmo gizmo in gizmos)
            {
                yield return gizmo;
            }
            Pawn pawn = __instance.InnerPawn;
            CompSoul compSoul = pawn.TryGetComp<CompSoul>();
            // Self revive
            if (compSoul != null && SoulUtilities.CanPawnSelfRevive(pawn) && pawn.IsPlayerControlled)
            {
                yield return new Command_Action()
                {
                    defaultLabel = "Refuse",
                    defaultDesc = "Get up",
                    icon = ContentFinder<Texture2D>.Get("UI/redsoul", true),
                    action = () =>
                    {
                        bool success = ResurrectionUtility.TryResurrect(pawn);
                        if (success == true)
                        {
                            SoulUtilities.AddDeterminationExhaustion(pawn, 0.8f);
                        }
                    }
                };
            }    
        }
        private static void StartAbsorbJob(Pawn absorber, Corpse victim)
        {
            victim.TrySetForbidden(false);
            Job job = JobMaker.MakeJob(Determination_JobDefOf.AbsorbSoul, victim);
            absorber.jobs.TryTakeOrderedJob(job, JobTag.Misc);
        }
    }
}
