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
    public class CompExtractableSoul : ThingComp
    {
        public LocalTargetInfo extractionTarget = LocalTargetInfo.Invalid;
        public bool ExtractionRequested => extractionTarget.IsValid;
        public CompSoul compSoul
        {
            get
            {
                Corpse corpse = (parent as Corpse);
                if (corpse != null)
                {
                    return corpse.InnerPawn.TryGetComp<CompSoul>();
                }
                return null;
            }
        }

        public bool CanExtract
        {
            get
            {
                if (compSoul != null && compSoul.HasSoul)
                {
                    return true;
                }
                return false;
            }
        }
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_TargetInfo.Look(ref extractionTarget, "extractionTarget");
        }

        private bool Validator(TargetInfo t)
        {
            CompSoulContainer? container = t.Thing?.TryGetComp<CompSoulContainer>();
            if (container != null && !container.HasSoul && !SoulUtilities.IsBossMonster(compSoul.pawn))
            {
                return true;
            }
            Pawn? absorber = t.Thing as Pawn;
            if (absorber != null && !absorber.DeadOrDowned && SoulUtilities.CanAbsorbTargetSoul(absorber, (parent as Corpse).InnerPawn))
            {
                return true;
            }

            return false;
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (!CanExtract) yield break;

            if (CanExtract && !ExtractionRequested)
                yield return new Command_Target
                {
                    defaultLabel = "Extract soul",
                    defaultDesc = "Select a target to absorb this soul. This can be a pawn or a soul container.",
                    icon = ContentFinder<Texture2D>.Get("UI/soulless"),
                    targetingParams = new TargetingParameters
                    {
                        canTargetBuildings = false,
                        canTargetPawns = true,
                        canTargetItems = true,
                        canTargetAnimals = false,
                        canTargetMechs = false,
                        canTargetSelf = false,
                        onlyTargetControlledPawns = true,
                        mapObjectTargetsMustBeAutoAttackable = false,
                        validator = Validator
                    },
                    action = (LocalTargetInfo target) =>
                    {
                        parent.SetForbidden(false);
                        if (!(target.Thing is Pawn))
                            extractionTarget = target;
                        else
                        {
                            Pawn targetedPawn = target.Thing as Pawn;
                            Job job = new Job(DefDatabase<JobDef>.GetNamed("AbsorbSoul"), parent);
                            targetedPawn.jobs.StartJob(job, JobCondition.InterruptForced);
                        }
                    }
                };
            else if (ExtractionRequested)
            {
                yield return new Command_Action
                {
                    defaultLabel = "Cancel Extraction",
                    defaultDesc = "Cancel the extraction process",
                    icon = ContentFinder<Texture2D>.Get("UI/Designators/Cancel"),
                    action = () =>
                    {
                        extractionTarget = LocalTargetInfo.Invalid;
                    }
                };
            }
        }
    }
}
